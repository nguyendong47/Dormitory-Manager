using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Bộ kiểm thử đơn vị toàn diện cho AttendanceTrackingViewModel - Điểm danh sinh viên theo phòng
/// </summary>
public class AttendanceTrackingViewModelTests
{
    private readonly IAttendanceService _attendanceService;
    private readonly IRoomService _roomService;
    private readonly IStudentService _studentService;
    private readonly IDialogService _dialogService;

    private readonly List<RoomDto> _sampleRooms;
    private readonly List<StudentDto> _sampleStudentsRoom1;
    private readonly List<StudentDto> _sampleStudentsRoom2;
    private readonly List<AttendanceRecordDto> _sampleRecordsRoom1;

    public AttendanceTrackingViewModelTests()
    {
        _attendanceService = Substitute.For<IAttendanceService>();
        _roomService = Substitute.For<IRoomService>();
        _studentService = Substitute.For<IStudentService>();
        _dialogService = Substitute.For<IDialogService>();

        _sampleRooms = new List<RoomDto>
        {
            new() { Id = 101, RoomNumber = "101", Building = "A", Capacity = 4, CurrentOccupancy = 3 },
            new() { Id = 102, RoomNumber = "102", Building = "A", Capacity = 4, CurrentOccupancy = 2 }
        };

        _sampleStudentsRoom1 = new List<StudentDto>
        {
            new() { Id = 1, StudentCode = "SV001", FullName = "Nguyen Van A", CurrentRoomId = 101 },
            new() { Id = 2, StudentCode = "SV002", FullName = "Tran Thi B", CurrentRoomId = 101 },
            new() { Id = 3, StudentCode = "SV003", FullName = "Le Van C", CurrentRoomId = 101 }
        };

        _sampleStudentsRoom2 = new List<StudentDto>
        {
            new() { Id = 4, StudentCode = "SV004", FullName = "Pham Van D", CurrentRoomId = 102 },
            new() { Id = 5, StudentCode = "SV005", FullName = "Hoang Thi E", CurrentRoomId = 102 }
        };

        _sampleRecordsRoom1 = new List<AttendanceRecordDto>
        {
            new()
            {
                Id = 1,
                StudentId = 1,
                StudentName = "Nguyen Van A",
                StudentCode = "SV001",
                RoomId = 101,
                RoomNumber = "101",
                Status = AttendanceStatus.Late,
                Reason = "Về muộn 15 phút do làm thêm",
                AttendanceDate = DateTime.Today
            }
        };

        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromResult(_sampleRooms));

        _studentService.GetAllStudentsAsync(Arg.Any<string?>(), 101)
            .Returns(Task.FromResult(_sampleStudentsRoom1));

        _studentService.GetAllStudentsAsync(Arg.Any<string?>(), 102)
            .Returns(Task.FromResult(_sampleStudentsRoom2));

        _attendanceService.GetRoomAttendanceAsync(101, Arg.Any<DateTime>(), default)
            .Returns(Task.FromResult<IReadOnlyList<AttendanceRecordDto>>(_sampleRecordsRoom1));

        _attendanceService.GetRoomAttendanceAsync(102, Arg.Any<DateTime>(), default)
            .Returns(Task.FromResult<IReadOnlyList<AttendanceRecordDto>>(new List<AttendanceRecordDto>()));
    }

    private AttendanceTrackingViewModel CreateViewModel()
    {
        return new AttendanceTrackingViewModel(
            _attendanceService,
            _roomService,
            _studentService,
            _dialogService);
    }

    #region 1. Constructor Tests

    [Fact]
    public void Constructor_WhenAttendanceServiceIsNull_ThrowsArgumentNullException()
    {
        var act = () => new AttendanceTrackingViewModel(null!, _roomService, _studentService, _dialogService);
        act.Should().Throw<ArgumentNullException>().WithParameterName("attendanceService");
    }

    [Fact]
    public void Constructor_WhenRoomServiceIsNull_ThrowsArgumentNullException()
    {
        var act = () => new AttendanceTrackingViewModel(_attendanceService, null!, _studentService, _dialogService);
        act.Should().Throw<ArgumentNullException>().WithParameterName("roomService");
    }

    [Fact]
    public void Constructor_WhenStudentServiceIsNull_ThrowsArgumentNullException()
    {
        var act = () => new AttendanceTrackingViewModel(_attendanceService, _roomService, null!, _dialogService);
        act.Should().Throw<ArgumentNullException>().WithParameterName("studentService");
    }

    [Fact]
    public void Constructor_WhenDialogServiceIsNull_ThrowsArgumentNullException()
    {
        var act = () => new AttendanceTrackingViewModel(_attendanceService, _roomService, _studentService, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("dialogService");
    }

    #endregion

    #region 2. Initialization and Loading Rooms

    [Fact]
    public async Task Initialize_Should_LoadRooms_And_SelectFirstRoom_And_LoadAttendance()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.Rooms.Should().HaveCount(2);
        vm.SelectedRoom.Should().NotBeNull();
        vm.SelectedRoom!.Id.Should().Be(101);
        vm.SelectedRoom.RoomNumber.Should().Be("101");

        vm.Students.Should().HaveCount(3);
        vm.TotalStudentsCount.Should().Be(3);
        vm.SelectedDate.Should().Be(DateTime.Today);
        vm.RecordedBy.Should().Be("Quản lý KTX");
        vm.CanSave.Should().BeTrue();
    }

    [Fact]
    public async Task Initialize_WhenNoRoomsAvailable_Should_SetSelectedRoomNull_And_EmptyStudents()
    {
        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromResult(new List<RoomDto>()));

        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.Rooms.Should().BeEmpty();
        vm.SelectedRoom.Should().BeNull();
        vm.Students.Should().BeEmpty();
        vm.TotalStudentsCount.Should().Be(0);
        vm.CanSave.Should().BeFalse();
        vm.StatusMessage.Should().Contain("Không có phòng nào");
    }

    [Fact]
    public async Task LoadRoomsAsync_WhenServiceThrowsException_Should_ShowErrorDialog()
    {
        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .ThrowsAsync(new InvalidOperationException("Lỗi kết nối cơ sở dữ liệu"));

        var vm = CreateViewModel();
        _dialogService.ClearReceivedCalls();

        await vm.LoadRoomsAsync();

        vm.StatusMessage.Should().Contain("Lỗi tải danh sách phòng");
        await _dialogService.Received(1).ShowMessageAsync(
            "Lỗi",
            Arg.Is<string>(m => m.Contains("Lỗi kết nối cơ sở dữ liệu")));
    }

    #endregion

    #region 3. Room and Date Switching & Attendance Loading

    [Fact]
    public async Task SelectedRoom_WhenChanged_Should_LoadStudents_And_ExistingAttendanceRecords()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        // Đổi sang phòng 102
        vm.SelectedRoom = _sampleRooms[1];
        await vm.LoadAttendanceAsync();

        vm.SelectedRoom.Id.Should().Be(102);
        vm.Students.Should().HaveCount(2);
        vm.Students[0].StudentCode.Should().Be("SV004");
        vm.Students[1].StudentCode.Should().Be("SV005");
        vm.TotalStudentsCount.Should().Be(2);
    }

    [Fact]
    public async Task SelectedRoom_WhenChangedToNull_Should_ClearStudents_And_UpdateKpi()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.SelectedRoom = null;
        await vm.LoadAttendanceAsync();

        vm.Students.Should().BeEmpty();
        vm.TotalStudentsCount.Should().Be(0);
        vm.CanSave.Should().BeFalse();
    }

    [Fact]
    public async Task SelectedDate_WhenChanged_Should_ReloadAttendanceForNewDate()
    {
        var targetDate = DateTime.Today.AddDays(-1);
        _attendanceService.GetRoomAttendanceAsync(101, targetDate, default)
            .Returns(Task.FromResult<IReadOnlyList<AttendanceRecordDto>>(new List<AttendanceRecordDto>()));

        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.SelectedDate = targetDate;
        await vm.LoadAttendanceAsync();

        await _attendanceService.Received().GetRoomAttendanceAsync(101, targetDate, default);
        vm.Students.Should().HaveCount(3);
        // Khi không có bản ghi cho ngày hôm qua, tất cả mặc định Present
        vm.PresentCount.Should().Be(3);
    }

    [Fact]
    public void SelectedDateNullable_WhenSet_Should_UpdateSelectedDate()
    {
        var vm = CreateViewModel();
        var newDate = new DateTime(2026, 11, 20);

        vm.SelectedDateNullable = newDate;

        vm.SelectedDate.Should().Be(newDate);
        vm.SelectedDateNullable.Should().Be(newDate);
    }

    [Fact]
    public async Task LoadAttendanceAsync_WhenStudentRecordDoesNotExist_Should_DefaultToPresent()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        // Student 2 & 3 không có bản ghi sẵn trong _sampleRecordsRoom1
        var student2 = vm.Students.First(s => s.StudentId == 2);
        var student3 = vm.Students.First(s => s.StudentId == 3);

        student2.Status.Should().Be(AttendanceStatus.Present);
        student2.Reason.Should().BeNull();
        student3.Status.Should().Be(AttendanceStatus.Present);
        student3.Reason.Should().BeNull();
    }

    [Fact]
    public async Task LoadAttendanceAsync_WhenStudentRecordExists_Should_MapStatusAndReason()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        // Student 1 có bản ghi sẵn là Late và Reason
        var student1 = vm.Students.First(s => s.StudentId == 1);
        student1.Status.Should().Be(AttendanceStatus.Late);
        student1.Reason.Should().Be("Về muộn 15 phút do làm thêm");
        student1.StatusText.Should().Be("Về muộn");
    }

    [Fact]
    public async Task LoadAttendanceAsync_WhenServiceThrowsException_Should_ShowErrorDialog()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        _studentService.GetAllStudentsAsync(Arg.Any<string?>(), 101)
            .ThrowsAsync(new Exception("Lỗi mạng"));

        _dialogService.ClearReceivedCalls();

        await vm.LoadAttendanceAsync();

        vm.StatusMessage.Should().Contain("Lỗi tải dữ liệu điểm danh");
        await _dialogService.Received(1).ShowMessageAsync(
            "Lỗi",
            Arg.Is<string>(m => m.Contains("Lỗi mạng")));
    }

    #endregion

    #region 4. Quick Action Commands (MarkAll)

    [Fact]
    public async Task MarkAllPresentCommand_Should_SetAllStudentsToPresent_And_UpdateKpi()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        // Ban đầu student 1 đang là Late
        vm.LateCount.Should().Be(1);

        vm.MarkAllPresent();

        vm.Students.Should().OnlyContain(s => s.Status == AttendanceStatus.Present);
        vm.PresentCount.Should().Be(3);
        vm.LateCount.Should().Be(0);
        vm.AbsentWithoutPermissionCount.Should().Be(0);
        vm.AbsentWithPermissionCount.Should().Be(0);
    }

    [Fact]
    public async Task MarkAllAbsentCommand_Should_SetAllStudentsToAbsentWithoutPermission_And_UpdateKpi()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.MarkAllAbsent();

        vm.Students.Should().OnlyContain(s => s.Status == AttendanceStatus.AbsentWithoutPermission);
        vm.AbsentWithoutPermissionCount.Should().Be(3);
        vm.PresentCount.Should().Be(0);
        vm.LateCount.Should().Be(0);
    }

    [Fact]
    public async Task MarkAllLateCommand_Should_SetAllStudentsToLate_And_UpdateKpi()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.MarkAllLate();

        vm.Students.Should().OnlyContain(s => s.Status == AttendanceStatus.Late);
        vm.LateCount.Should().Be(3);
        vm.PresentCount.Should().Be(0);
        vm.AbsentWithoutPermissionCount.Should().Be(0);
    }

    #endregion

    #region 5. KPI Stats & Item Status Changes

    [Fact]
    public async Task StudentItem_WhenStatusChanges_Should_TriggerParentKpiUpdate()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        // Ban đầu: SV1 = Late (1), SV2 = Present (1), SV3 = Present (1)
        vm.TotalStudentsCount.Should().Be(3);
        vm.PresentCount.Should().Be(2);
        vm.LateCount.Should().Be(1);
        vm.AbsentWithPermissionCount.Should().Be(0);

        // Đổi SV2 thành Vắng có phép
        vm.Students[1].Status = AttendanceStatus.AbsentWithPermission;

        vm.PresentCount.Should().Be(1);
        vm.AbsentWithPermissionCount.Should().Be(1);
        vm.LateCount.Should().Be(1);

        // Đổi SV3 thành Vắng không phép
        vm.Students[2].Status = AttendanceStatus.AbsentWithoutPermission;

        vm.PresentCount.Should().Be(0);
        vm.AbsentWithoutPermissionCount.Should().Be(1);
        vm.AbsentWithPermissionCount.Should().Be(1);
        vm.LateCount.Should().Be(1);
    }

    [Fact]
    public async Task StudentItem_WhenSelectedStatusOptionChanges_Should_UpdateStatus_And_TriggerParentKpiUpdate()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        var absentOption = AttendanceTrackingViewModel.StatusOptions.First(o => o.Value == AttendanceStatus.AbsentWithoutPermission);
        vm.Students[0].SelectedStatusOption = absentOption;

        vm.Students[0].Status.Should().Be(AttendanceStatus.AbsentWithoutPermission);
        vm.AbsentWithoutPermissionCount.Should().Be(1);
    }

    [Fact]
    public void StudentItem_BadgeProperties_Should_ReturnCorrectColorsAndText()
    {
        var item = new StudentAttendanceItemViewModel
        {
            StudentId = 1,
            StudentName = "Nguyen Van A",
            StudentCode = "SV001",
            RoomId = 101,
            RoomNumber = "101"
        };

        // Present
        item.Status = AttendanceStatus.Present;
        item.StatusText.Should().Be("Có mặt");
        item.StatusBadgeColor.Should().Be("#107C41");
        item.StatusBadgeBackground.Should().Be("#DFF6DD");

        // AbsentWithoutPermission
        item.Status = AttendanceStatus.AbsentWithoutPermission;
        item.StatusText.Should().Be("Vắng không phép");
        item.StatusBadgeColor.Should().Be("#A80000");
        item.StatusBadgeBackground.Should().Be("#FDE7E9");

        // AbsentWithPermission
        item.Status = AttendanceStatus.AbsentWithPermission;
        item.StatusText.Should().Be("Vắng có phép");
        item.StatusBadgeColor.Should().Be("#0078D4");
        item.StatusBadgeBackground.Should().Be("#DEECF9");

        // Late
        item.Status = AttendanceStatus.Late;
        item.StatusText.Should().Be("Về muộn");
        item.StatusBadgeColor.Should().Be("#C67D00");
        item.StatusBadgeBackground.Should().Be("#FFF4CE");
    }

    [Fact]
    public void StatusOptions_Should_ContainAllFourStatuses()
    {
        AttendanceTrackingViewModel.StatusOptions.Should().HaveCount(4);
        AttendanceTrackingViewModel.StatusOptions.Select(o => o.Value)
            .Should().BeEquivalentTo(new[]
            {
                AttendanceStatus.Present,
                AttendanceStatus.AbsentWithoutPermission,
                AttendanceStatus.AbsentWithPermission,
                AttendanceStatus.Late
            });
    }

    #endregion

    #region 6. CanSave Logic

    [Fact]
    public async Task CanSave_WhenRoomNotNullAndStudentsNotEmpty_ShouldBeTrue()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.CanSave.Should().BeTrue();
    }

    [Fact]
    public async Task CanSave_WhenRoomIsNull_ShouldBeFalse()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.SelectedRoom = null;

        vm.CanSave.Should().BeFalse();
    }

    [Fact]
    public async Task CanSave_WhenStudentsIsEmpty_ShouldBeFalse()
    {
        _studentService.GetAllStudentsAsync(Arg.Any<string?>(), 101)
            .Returns(Task.FromResult(new List<StudentDto>()));

        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.Students.Should().BeEmpty();
        vm.CanSave.Should().BeFalse();
    }

    #endregion

    #region 7. SaveAttendanceCommand

    [Fact]
    public async Task SaveAttendanceCommand_WhenSelectedRoomNullOrStudentsEmpty_Should_ShowWarningDialog_And_NotCallService()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        vm.SelectedRoom = null;

        await vm.SaveAttendanceAsync();

        await _attendanceService.DidNotReceive()
            .RecordBatchRoomAttendanceAsync(Arg.Any<BatchRoomAttendanceDto>(), Arg.Any<CancellationToken>());

        await _dialogService.Received(1)
            .ShowMessageAsync("Thông báo", Arg.Is<string>(m => m.Contains("Vui lòng chọn phòng")));
    }

    [Fact]
    public async Task SaveAttendanceCommand_WhenValid_Should_CallRecordBatchRoomAttendanceAsync_WithCorrectDto_And_ShowSuccessDialog()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        BatchRoomAttendanceDto? capturedDto = null;
        _attendanceService.RecordBatchRoomAttendanceAsync(
            Arg.Do<BatchRoomAttendanceDto>(dto => capturedDto = dto),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<AttendanceRecordDto>>(new List<AttendanceRecordDto>()));

        vm.RecordedBy = "Giám thị KTX";
        vm.Students[1].Status = AttendanceStatus.AbsentWithoutPermission;
        vm.Students[1].Reason = "Đi chơi chưa về";

        await vm.SaveAttendanceAsync();

        capturedDto.Should().NotBeNull();
        capturedDto!.RoomId.Should().Be(101);
        capturedDto.AttendanceDate.Should().Be(vm.SelectedDate);
        capturedDto.RecordedBy.Should().Be("Giám thị KTX");
        capturedDto.StudentAttendances.Should().HaveCount(3);

        var student2Attendance = capturedDto.StudentAttendances.First(s => s.StudentId == 2);
        student2Attendance.Status.Should().Be(AttendanceStatus.AbsentWithoutPermission);
        student2Attendance.Reason.Should().Be("Đi chơi chưa về");

        await _dialogService.Received(1)
            .ShowMessageAsync("Thành công", Arg.Is<string>(m => m.Contains("thành công")));
    }

    [Fact]
    public async Task SaveAttendanceCommand_WhenServiceThrowsException_Should_ShowErrorDialog()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        _attendanceService.RecordBatchRoomAttendanceAsync(
            Arg.Any<BatchRoomAttendanceDto>(),
            Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Lỗi lưu cơ sở dữ liệu"));

        await vm.SaveAttendanceAsync();

        vm.StatusMessage.Should().Contain("Lỗi lưu điểm danh");
        await _dialogService.Received(1)
            .ShowMessageAsync("Lỗi", Arg.Is<string>(m => m.Contains("Lỗi lưu cơ sở dữ liệu")));
    }

    #endregion

    #region 8. RefreshCommand

    [Fact]
    public async Task RefreshCommand_Should_ReloadRooms_And_Attendance()
    {
        var vm = CreateViewModel();
        await vm.LoadRoomsAsync();

        _roomService.ClearReceivedCalls();

        await vm.RefreshAsync();

        await _roomService.Received(1).GetAllRoomsAsync();
        vm.Rooms.Should().HaveCount(2);
        vm.Students.Should().HaveCount(3);
    }

    #endregion
}
