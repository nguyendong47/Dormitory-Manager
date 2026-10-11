using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Desktop;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử luồng E2E tích hợp Headless toàn diện cho phân hệ quản lý khách ra vào (Visitor)
/// và điểm danh sinh viên nội trú ban đêm theo phòng (Attendance Tracking)
/// </summary>
public class VisitorAndAttendanceE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    /// <summary>
    /// Kịch bản E2E 1: Đăng ký khách vào KTX (Check-in) -> Hiển thị trên danh sách & KPI cập nhật
    /// -> Chọn khách -> Xác nhận khách rời khỏi KTX (Check-out) -> Trạng thái cập nhật chuẩn xác
    /// </summary>
    [AvaloniaFact]
    public async Task Should_CheckIn_And_CheckOut_Visitor_Successfully()
    {
        // 1. Khởi tạo ViewModel danh sách khách từ TestFixture
        var vm = _fixture.CreateVisitorListViewModel();
        await vm.LoadVisitorsCommand.ExecuteAsync(null);

        var initialVisitorCount = vm.Visitors.Count;
        var initialActiveCount = vm.ActiveVisitorsCount;

        // 2. Thiết lập Handler giả lập hộp thoại Check-in khách đến thăm
        const string visitorName = "Nguyễn Văn Khách E2E";
        const string identityCard = "001204009988";
        const string phoneNumber = "0987654321";
        const string noteText = "Thăm người nhà";

        vm.ShowCheckInDialogHandler = async dialogVm =>
        {
            await dialogVm.LoadRoomsCommand.ExecuteAsync(null);

            dialogVm.VisitorName = visitorName;
            dialogVm.IdentificationNumber = identityCard;
            dialogVm.PhoneNumber = phoneNumber;
            dialogVm.Note = noteText;

            // Chọn phòng A101
            dialogVm.SelectedRoom = dialogVm.Rooms.FirstOrDefault(r => r.RoomNumber == "A101");
            dialogVm.SelectedRoom.Should().NotBeNull("Phòng A101 phải tồn tại trong dữ liệu seed mẫu");

            // Nạp danh sách sinh viên phòng A101 và chọn sinh viên đầu tiên
            var studentService = _fixture.GetService<IStudentService>();
            var students = await studentService.GetAllStudentsAsync(roomId: dialogVm.SelectedRoom!.Id);
            dialogVm.StudentsInRoom.Clear();
            foreach (var student in students)
            {
                dialogVm.StudentsInRoom.Add(student);
            }
            dialogVm.SelectedStudent = dialogVm.StudentsInRoom.FirstOrDefault();
            dialogVm.SelectedStudent.Should().NotBeNull("Phòng A101 phải có sinh viên cư trú");

            // Xác nhận đăng ký check-in
            await dialogVm.ConfirmCheckInCommand.ExecuteAsync(null);
            return true;
        };

        // 3. Thực thi lệnh Check-in
        await vm.CheckInCommand.ExecuteAsync(null);

        // 4. Kiểm tra danh sách hiển thị và thẻ KPI
        vm.Visitors.Count.Should().Be(initialVisitorCount + 1, "Số lượng khách phải tăng thêm 1 sau khi check-in");
        vm.Visitors.Should().Contain(v => v.VisitorName == visitorName, "Khách vừa tạo phải xuất hiện trong danh sách");

        var createdVisitor = vm.Visitors.First(v => v.VisitorName == visitorName);
        createdVisitor.Status.Should().Be(VisitorStatus.InDormitory, "Khách mới check-in phải có trạng thái Trong KTX");
        createdVisitor.IdentificationNumber.Should().Be(identityCard);
        createdVisitor.PhoneNumber.Should().Be(phoneNumber);
        createdVisitor.RoomNumber.Should().Be("A101");
        createdVisitor.Note.Should().Be(noteText);
        createdVisitor.CheckOutTime.Should().BeNull("Khách đang trong KTX chưa có giờ check-out");

        vm.ActiveVisitorsCount.Should().Be(initialActiveCount + 1, "Chỉ số khách đang trong KTX trên thẻ KPI phải tăng 1");

        // 5. Chọn khách vừa tạo làm SelectedVisitor và kiểm tra điều kiện Check-out
        vm.SelectedVisitor = createdVisitor;
        vm.CanCheckOut.Should().BeTrue("Khách đang trong KTX phải kích hoạt được nút Check-out");

        // 6. Thực thi lệnh Check-out (DialogService tự động xác nhận nhờ ConfirmResult = true)
        _fixture.DialogService.ConfirmResult = true;
        await vm.CheckOutCommand.ExecuteAsync(null);

        // 7. Xác minh trạng thái sau khi Check-out
        var checkedOutVisitor = vm.Visitors.First(v => v.Id == createdVisitor.Id);
        checkedOutVisitor.Status.Should().Be(VisitorStatus.CheckedOut, "Trạng thái phải chuyển sang Đã rời KTX");
        checkedOutVisitor.CheckOutTime.Should().NotBeNull("Thời gian check-out phải được ghi nhận");

        vm.SelectedVisitor = checkedOutVisitor;
        vm.CanCheckOut.Should().BeFalse("Khách đã check-out không thể tiếp tục check-out");

        // 8. Xác minh tính toàn vẹn dữ liệu trong cơ sở dữ liệu SQLite
        var dbRecord = await _fixture.Context.VisitorLogs.FindAsync(createdVisitor.Id);
        dbRecord.Should().NotBeNull();
        dbRecord!.Status.Should().Be(VisitorStatus.CheckedOut);
        dbRecord.CheckOutTime.Should().NotBeNull();
    }

    /// <summary>
    /// Kịch bản E2E 2: Phát hiện khách lưu trú quá giờ quy định (sau 22:00)
    /// -> Quét tự động chuyển trạng thái thành Overstayed -> Cập nhật KPI -> Lọc danh sách chỉ khách quá giờ
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Detect_And_Highlight_Overstayed_Visitors()
    {
        // 1. Tạo trực tiếp bản ghi khách vào từ hôm qua chưa check-out trong cơ sở dữ liệu
        var room = await _fixture.Context.Rooms.FirstAsync(r => r.RoomNumber == "A101");
        var student = await _fixture.Context.Students.FirstAsync(s => s.CurrentRoomId == room.Id);

        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var overstayedLog = new VisitorLog
        {
            VisitorName = "Phạm Khách Quá Giờ E2E",
            IdentificationNumber = "034203112233",
            PhoneNumber = "0901234888",
            RoomId = room.Id,
            StudentId = student.Id,
            CheckInTime = yesterday.AddHours(14), // Vào từ 14:00 hôm qua
            Status = VisitorStatus.InDormitory,
            Note = "Khách vào ngày hôm trước chưa rời khỏi KTX"
        };
        _fixture.Context.VisitorLogs.Add(overstayedLog);
        await _fixture.Context.SaveChangesAsync();

        // 2. Mở VisitorListViewModel và tải danh sách khách
        var vm = _fixture.CreateVisitorListViewModel();
        await vm.LoadVisitorsCommand.ExecuteAsync(null);

        vm.Visitors.Should().Contain(v => v.Id == overstayedLog.Id);

        // 3. Thực thi lệnh quét khách quá giờ giới nghiêm (MarkOverstayedCommand)
        await vm.MarkOverstayedCommand.ExecuteAsync(null);

        // 4. Kiểm tra trạng thái của bản ghi chuyển sang Overstayed trong DB và giao diện
        var dbLog = await _fixture.Context.VisitorLogs.FindAsync(overstayedLog.Id);
        dbLog.Should().NotBeNull();
        dbLog!.Status.Should().Be(VisitorStatus.Overstayed, "Bản ghi phải được tự động chuyển sang trạng thái Quá giờ");

        vm.OverstayedCount.Should().BeGreaterThan(0, "Thẻ KPI số khách quá giờ phải lớn hơn 0");

        // 5. Thử nghiệm bộ lọc OnlyOverstayed
        vm.OnlyOverstayed = true;
        await vm.LoadVisitorsCommand.ExecuteAsync(null);

        vm.Visitors.Should().NotBeEmpty("Danh sách lọc khách quá giờ không được rỗng");
        vm.Visitors.Should().OnlyContain(v => v.Status == VisitorStatus.Overstayed,
            "Khi bật bộ lọc OnlyOverstayed, tất cả dòng dữ liệu phải có trạng thái Quá giờ");
        vm.Visitors.Should().Contain(v => v.Id == overstayedLog.Id);

        // 6. Tắt bộ lọc và làm mới
        vm.OnlyOverstayed = false;
        await vm.LoadVisitorsCommand.ExecuteAsync(null);
        vm.Visitors.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Kịch bản E2E 3: Điểm danh sinh viên nội trú ban đêm theo phòng
    /// -> Lưu kết quả theo lô vào DB -> Tải lại kiểm tra tính toàn vẹn và thẻ KPI
    /// -> Cập nhật lại trong cùng ngày (upsert mượt mà, không trùng lặp)
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Record_Night_Attendance_And_Prevent_Duplicate_Date()
    {
        // 1. Khởi tạo ViewModel điểm danh phòng KTX
        var vm = _fixture.CreateAttendanceTrackingViewModel();
        var today = DateTime.Today;
        vm.SelectedDate = today;

        // 2. Tải danh sách phòng và chọn phòng A101
        await vm.LoadRoomsCommand.ExecuteAsync(null);
        var roomA101 = vm.Rooms.FirstOrDefault(r => r.RoomNumber == "A101");
        roomA101.Should().NotBeNull("Phòng A101 phải có trong danh sách phòng");

        vm.SelectedRoom = roomA101;
        await vm.LoadAttendanceCommand.ExecuteAsync(null);

        // Đảm bảo danh sách sinh viên được nạp lên đầy đủ
        vm.Students.Count.Should().BeGreaterThanOrEqualTo(2, "Phòng A101 phải có ít nhất 2 sinh viên được nạp");
        vm.CanSave.Should().BeTrue("Khi có phòng và danh sách sinh viên, CanSave phải là true");

        // 3. Phân bổ trạng thái điểm danh cho từng sinh viên
        var student1 = vm.Students[0];
        var student2 = vm.Students[1];

        student1.Status = AttendanceStatus.Present;
        student1.Reason = null;

        student2.Status = AttendanceStatus.AbsentWithoutPermission;
        student2.Reason = "Đi chơi không báo";

        // Cập nhật thống kê KPI trước khi lưu
        vm.UpdateKpiStats();
        vm.PresentCount.Should().Be(1);
        vm.AbsentWithoutPermissionCount.Should().Be(1);

        // 4. Thực thi lệnh lưu kết quả điểm danh theo lô
        await vm.SaveAttendanceCommand.ExecuteAsync(null);

        // 5. Kiểm tra dữ liệu được lưu thành công vào cơ sở dữ liệu SQLite
        var savedRecords = await _fixture.Context.AttendanceRecords
            .Where(a => a.RoomId == roomA101!.Id && a.AttendanceDate == today.Date)
            .ToListAsync();

        savedRecords.Should().HaveCount(vm.Students.Count, "Số bản ghi điểm danh trong DB phải bằng số sinh viên của phòng");

        var dbRecord1 = savedRecords.First(r => r.StudentId == student1.StudentId);
        dbRecord1.Status.Should().Be(AttendanceStatus.Present);

        var dbRecord2 = savedRecords.First(r => r.StudentId == student2.StudentId);
        dbRecord2.Status.Should().Be(AttendanceStatus.AbsentWithoutPermission);
        dbRecord2.Reason.Should().Be("Đi chơi không báo");

        // 6. Tải lại dữ liệu ngày hôm đó và xác nhận các trạng thái hiển thị đúng trên giao diện
        await vm.LoadAttendanceCommand.ExecuteAsync(null);

        vm.Students.First(s => s.StudentId == student1.StudentId).Status.Should().Be(AttendanceStatus.Present);
        var reloadedStudent2 = vm.Students.First(s => s.StudentId == student2.StudentId);
        reloadedStudent2.Status.Should().Be(AttendanceStatus.AbsentWithoutPermission);
        reloadedStudent2.Reason.Should().Be("Đi chơi không báo");

        // Kiểm tra số liệu thẻ KPI
        vm.TotalStudentsCount.Should().Be(vm.Students.Count);
        vm.PresentCount.Should().Be(1);
        vm.AbsentWithoutPermissionCount.Should().Be(1);
        vm.AbsentWithPermissionCount.Should().Be(0);
        vm.LateCount.Should().Be(0);

        // 7. Kiểm tra tính năng cập nhật (upsert) trong cùng ngày: thay đổi trạng thái của student2 thành có phép
        reloadedStudent2.Status = AttendanceStatus.AbsentWithPermission;
        reloadedStudent2.Reason = "Gia đình có việc gấp xin phép về quê";

        await vm.SaveAttendanceCommand.ExecuteAsync(null);

        // Xác minh không có lỗi unique constraint và số lượng bản ghi không bị nhân đôi
        var updatedRecords = await _fixture.Context.AttendanceRecords
            .Where(a => a.RoomId == roomA101!.Id && a.AttendanceDate == today.Date)
            .ToListAsync();

        updatedRecords.Should().HaveCount(vm.Students.Count, "Upsert trong cùng ngày không được tạo thêm bản ghi trùng lặp");

        var updatedRecord2 = updatedRecords.First(r => r.StudentId == student2.StudentId);
        updatedRecord2.Status.Should().Be(AttendanceStatus.AbsentWithPermission);
        updatedRecord2.Reason.Should().Be("Gia đình có việc gấp xin phép về quê");

        // Kiểm tra KPI cập nhật đúng
        vm.PresentCount.Should().Be(1);
        vm.AbsentWithoutPermissionCount.Should().Be(0);
        vm.AbsentWithPermissionCount.Should().Be(1);

        // 8. Kiểm tra tính năng thao tác nhanh: Đánh dấu tất cả có mặt
        vm.MarkAllPresentCommand.Execute(null);
        vm.PresentCount.Should().Be(vm.Students.Count);
        vm.AbsentWithPermissionCount.Should().Be(0);

        await vm.SaveAttendanceCommand.ExecuteAsync(null);

        var allPresentRecords = await _fixture.Context.AttendanceRecords
            .Where(a => a.RoomId == roomA101!.Id && a.AttendanceDate == today.Date)
            .ToListAsync();
        allPresentRecords.Should().OnlyContain(r => r.Status == AttendanceStatus.Present);
    }

    /// <summary>
    /// Kịch bản E2E 4: Điều hướng chuyển đổi mượt mà giữa màn hình Khách ra vào (Visitors)
    /// và màn hình Điểm danh KTX (Attendance) trên MainWindow Sidebar
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Navigate_Between_Visitor_And_Attendance_Screens()
    {
        // 1. Khởi tạo MainWindow ViewModel và giao diện Avalonia Headless
        var vm = _fixture.CreateMainWindowViewModel();
        var mainWindow = new MainWindow(vm);
        mainWindow.Show();

        // 2. Đăng nhập với tài khoản Quản trị viên
        vm.LoginVm.Username = "admin";
        vm.LoginVm.Password = "Admin@123456";
        await vm.LoginVm.LoginCommand.ExecuteAsync(null);

        vm.IsLoggedIn.Should().BeTrue();
        vm.CurrentView.Should().BeOfType<DashboardViewModel>();
        vm.ActiveMenu.Should().Be("Dashboard");

        // 3. Điều hướng tới màn hình Khách ra vào (Visitors)
        vm.NavigateToVisitorsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<VisitorListViewModel>("CurrentView phải chuyển sang VisitorListViewModel");
        vm.ActiveMenu.Should().Be("Visitors");

        // 4. Điều hướng tới màn hình Điểm danh KTX (Attendance)
        vm.NavigateToAttendanceCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<AttendanceTrackingViewModel>("CurrentView phải chuyển sang AttendanceTrackingViewModel");
        vm.ActiveMenu.Should().Be("Attendance");

        // 5. Điều hướng quay lại Dashboard
        vm.NavigateToDashboardCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<DashboardViewModel>();
        vm.ActiveMenu.Should().Be("Dashboard");

        // 6. Điều hướng tới màn hình phòng ở và quay lại an toàn
        vm.NavigateToRoomsCommand.Execute(null);
        vm.CurrentView.Should().BeOfType<RoomListViewModel>();
        vm.ActiveMenu.Should().Be("Rooms");

        mainWindow.Close();
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
