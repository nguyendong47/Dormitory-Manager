using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Bộ kiểm thử đơn vị cho VisitorListViewModel - Quản lý danh sách khách ra vào KTX và thẻ KPI
/// </summary>
public class VisitorListViewModelTests
{
    private readonly IVisitorService _visitorService;
    private readonly IRoomService _roomService;
    private readonly IDialogService _dialogService;
    private readonly IStudentService _studentService;

    public VisitorListViewModelTests()
    {
        _visitorService = Substitute.For<IVisitorService>();
        _roomService = Substitute.For<IRoomService>();
        _dialogService = Substitute.For<IDialogService>();
        _studentService = Substitute.For<IStudentService>();

        // Cấu hình dữ liệu giả định mặc định cho các service
        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromResult(CreateSampleRooms()));

        _visitorService.GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default)
            .Returns(Task.FromResult<IReadOnlyList<VisitorLogDto>>(CreateSampleVisitors()));

        _visitorService.GetVisitorSummaryAsync(Arg.Any<DateTime?>(), default)
            .Returns(Task.FromResult(new VisitorSummaryDto
            {
                TotalVisitorsToday = 5,
                ActiveVisitorsCount = 3,
                OverstayedCount = 1
            }));
    }

    private VisitorListViewModel CreateViewModel(bool includeStudentService = true)
    {
        return new VisitorListViewModel(
            _visitorService,
            _roomService,
            _dialogService,
            includeStudentService ? _studentService : null);
    }

    private static List<RoomDto> CreateSampleRooms()
    {
        return new List<RoomDto>
        {
            new() { Id = 1, RoomNumber = "101", Building = "A", Capacity = 4, CurrentOccupancy = 3 },
            new() { Id = 2, RoomNumber = "102", Building = "A", Capacity = 4, CurrentOccupancy = 4 },
            new() { Id = 3, RoomNumber = "201", Building = "B", Capacity = 6, CurrentOccupancy = 2 }
        };
    }

    private static List<VisitorLogDto> CreateSampleVisitors()
    {
        return new List<VisitorLogDto>
        {
            new()
            {
                Id = 1,
                VisitorName = "Nguyen Van A",
                IdentificationNumber = "001200001111",
                PhoneNumber = "0901111222",
                RoomId = 1,
                RoomNumber = "101",
                StudentId = 10,
                StudentName = "Tran Sinh Vien A",
                CheckInTime = DateTime.UtcNow.AddHours(-2),
                Status = VisitorStatus.InDormitory
            },
            new()
            {
                Id = 2,
                VisitorName = "Tran Thi B",
                IdentificationNumber = "001200002222",
                PhoneNumber = "0902222333",
                RoomId = 2,
                RoomNumber = "102",
                StudentId = 20,
                StudentName = "Pham Sinh Vien B",
                CheckInTime = DateTime.UtcNow.AddHours(-5),
                CheckOutTime = DateTime.UtcNow.AddHours(-1),
                Status = VisitorStatus.CheckedOut
            },
            new()
            {
                Id = 3,
                VisitorName = "Le Van C",
                IdentificationNumber = "001200003333",
                PhoneNumber = "0903333444",
                RoomId = 1,
                RoomNumber = "101",
                StudentId = 11,
                StudentName = "Hoang Sinh Vien C",
                CheckInTime = DateTime.UtcNow.AddHours(-8),
                Status = VisitorStatus.Overstayed
            }
        };
    }

    [Fact]
    public void Constructor_WithNullArguments_Should_ThrowArgumentNullException()
    {
        // Kiểm tra ném ngoại lệ khi thiếu các dependency bắt buộc
        var act1 = () => new VisitorListViewModel(null!, _roomService, _dialogService);
        var act2 = () => new VisitorListViewModel(_visitorService, null!, _dialogService);
        var act3 = () => new VisitorListViewModel(_visitorService, _roomService, null!);

        act1.Should().Throw<ArgumentNullException>().WithParameterName("visitorService");
        act2.Should().Throw<ArgumentNullException>().WithParameterName("roomService");
        act3.Should().Throw<ArgumentNullException>().WithParameterName("dialogService");
    }

    [Fact]
    public async Task Constructor_Should_InitializeFilters_And_LoadInitialData()
    {
        // Act: Khởi tạo ViewModel
        var vm = CreateViewModel();
        await Task.Delay(50); // Chờ InitializeAsync hoàn tất

        // Assert: Bộ lọc trạng thái mặc định
        vm.StatusFilterOptions.Should().ContainInOrder("Tất cả", "Trong KTX", "Đã rời KTX", "Quá giờ");
        vm.SelectedStatusFilter.Should().Be("Tất cả");

        // Assert: Danh sách phòng được tải
        vm.RoomFilterOptions.Should().HaveCount(3);
        vm.RoomFilterOptions[0].RoomNumber.Should().Be("101");

        // Assert: Danh sách khách và số liệu KPI
        vm.Visitors.Should().HaveCount(3);
        vm.TotalVisitorsToday.Should().Be(5);
        vm.ActiveVisitorsCount.Should().Be(3);
        vm.OverstayedCount.Should().Be(1);
        vm.StatusMessage.Should().Be("Đã tải 3 lượt khách.");
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task LoadRoomsFilterAsync_WhenServiceThrows_Should_SetStatusMessage()
    {
        // Arrange
        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromException<List<RoomDto>>(new Exception("Lỗi kết nối cơ sở dữ liệu")));

        var vm = CreateViewModel();

        // Act
        await vm.LoadRoomsFilterAsync();

        // Assert
        vm.StatusMessage.Should().Contain("Lỗi tải bộ lọc phòng: Lỗi kết nối cơ sở dữ liệu");
    }

    [Fact]
    public async Task LoadVisitorsAsync_WhenServiceThrows_Should_SetStatusMessage_And_ShowDialog()
    {
        // Arrange
        _visitorService.GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default)
            .Returns(Task.FromException<IReadOnlyList<VisitorLogDto>>(new InvalidOperationException("Mất kết nối mạng")));

        var vm = CreateViewModel();
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.LoadVisitorsAsync();

        // Assert
        vm.StatusMessage.Should().Contain("Lỗi tải dữ liệu khách: Mất kết nối mạng");
        await _dialogService.Received(1).ShowMessageAsync("Lỗi", Arg.Is<string>(msg => msg.Contains("Mất kết nối mạng")));
        vm.IsLoading.Should().BeFalse();
    }

    [Theory]
    [InlineData("Tất cả", null)]
    [InlineData("Trong KTX", VisitorStatus.InDormitory)]
    [InlineData("Đã rời KTX", VisitorStatus.CheckedOut)]
    [InlineData("Quá giờ", VisitorStatus.Overstayed)]
    public async Task LoadVisitorsAsync_WithDifferentStatusFilters_Should_PassCorrectStatusToService(
        string filterOption,
        VisitorStatus? expectedStatus)
    {
        // Arrange
        VisitorFilterDto? capturedFilter = null;
        _visitorService.GetVisitorsAsync(Arg.Do<VisitorFilterDto>(f => capturedFilter = f), default)
            .Returns(Task.FromResult<IReadOnlyList<VisitorLogDto>>(new List<VisitorLogDto>()));

        var vm = CreateViewModel();

        // Act
        vm.SelectedStatusFilter = filterOption;
        await vm.LoadVisitorsAsync();

        // Assert
        capturedFilter.Should().NotBeNull();
        capturedFilter!.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task LoadVisitorsAsync_WithCombinedFilters_Should_PassAllCriteriaToService()
    {
        // Arrange
        VisitorFilterDto? capturedFilter = null;
        _visitorService.GetVisitorsAsync(Arg.Do<VisitorFilterDto>(f => capturedFilter = f), default)
            .Returns(Task.FromResult<IReadOnlyList<VisitorLogDto>>(new List<VisitorLogDto>()));

        var vm = CreateViewModel();
        var selectedRoom = new RoomDto { Id = 2, RoomNumber = "102" };
        var fromDate = new DateTime(2026, 10, 1);
        var toDate = new DateTime(2026, 10, 11);

        // Act: Thiết lập các bộ lọc nâng cao
        vm.SelectedRoomFilter = selectedRoom;
        vm.SearchKeyword = "  Nguyen Van A  ";
        vm.FromDate = fromDate;
        vm.ToDate = toDate;
        vm.OnlyOverstayed = true;

        await vm.LoadVisitorsAsync();

        // Assert
        capturedFilter.Should().NotBeNull();
        capturedFilter!.RoomId.Should().Be(2);
        capturedFilter.SearchKeyword.Should().Be("Nguyen Van A");
        capturedFilter.FromDate.Should().Be(fromDate);
        capturedFilter.ToDate.Should().Be(toDate);
        capturedFilter.OnlyOverstayed.Should().BeTrue();
    }

    [Fact]
    public void CanCheckOut_Should_ReturnTrue_OnlyWhenVisitorIsSelectedAndNotInCheckedOutStatus()
    {
        // Arrange
        var vm = CreateViewModel();

        // Case 1: Chưa chọn khách -> False
        vm.SelectedVisitor = null;
        vm.CanCheckOut.Should().BeFalse();

        // Case 2: Khách đang trong KTX -> True
        vm.SelectedVisitor = new VisitorLogDto { Id = 1, Status = VisitorStatus.InDormitory };
        vm.CanCheckOut.Should().BeTrue();

        // Case 3: Khách quá giờ quy định -> True
        vm.SelectedVisitor = new VisitorLogDto { Id = 2, Status = VisitorStatus.Overstayed };
        vm.CanCheckOut.Should().BeTrue();

        // Case 4: Khách đã rời KTX -> False
        vm.SelectedVisitor = new VisitorLogDto { Id = 3, Status = VisitorStatus.CheckedOut };
        vm.CanCheckOut.Should().BeFalse();
    }

    [Fact]
    public async Task CheckOutAsync_WhenNoVisitorSelected_Should_ShowNotification_And_NotCallService()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.SelectedVisitor = null;
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.CheckOutCommand.ExecuteAsync(null);

        // Assert
        await _dialogService.Received(1).ShowMessageAsync("Thông báo", "Vui lòng chọn khách đang lưu trú để check-out.");
        await _visitorService.DidNotReceiveWithAnyArgs().CheckOutAsync(default);
    }

    [Fact]
    public async Task CheckOutAsync_WhenVisitorAlreadyCheckedOut_Should_ShowNotification_And_NotCallService()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.SelectedVisitor = new VisitorLogDto
        {
            Id = 5,
            VisitorName = "Khach Cu",
            Status = VisitorStatus.CheckedOut
        };
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.CheckOutCommand.ExecuteAsync(null);

        // Assert
        await _dialogService.Received(1).ShowMessageAsync("Thông báo", "Vui lòng chọn khách đang lưu trú để check-out.");
        await _visitorService.DidNotReceiveWithAnyArgs().CheckOutAsync(default);
    }

    [Fact]
    public async Task CheckOutAsync_WhenUserCancelsConfirmDialog_Should_NotCallCheckOutService()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.SelectedVisitor = new VisitorLogDto
        {
            Id = 1,
            VisitorName = "Nguyen Van A",
            Status = VisitorStatus.InDormitory
        };

        _dialogService.ShowConfirmAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(false));

        // Act
        await vm.CheckOutCommand.ExecuteAsync(null);

        // Assert
        await _dialogService.Received(1).ShowConfirmAsync(
            "Xác nhận Check-out",
            "Bạn có chắc chắn muốn ghi nhận khách 'Nguyen Van A' rời khỏi KTX?");
        await _visitorService.DidNotReceiveWithAnyArgs().CheckOutAsync(default);
    }

    [Fact]
    public async Task CheckOutAsync_WhenUserConfirms_Should_CallCheckOut_ReloadData_And_ShowSuccess()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.SelectedVisitor = new VisitorLogDto
        {
            Id = 1,
            VisitorName = "Nguyen Van A",
            Status = VisitorStatus.InDormitory
        };

        _dialogService.ShowConfirmAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(true));

        _visitorService.CheckOutAsync(1, Arg.Any<string?>(), default)
            .Returns(Task.FromResult(new VisitorLogDto
            {
                Id = 1,
                VisitorName = "Nguyen Van A",
                Status = VisitorStatus.CheckedOut
            }));

        _visitorService.ClearReceivedCalls();
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.CheckOutCommand.ExecuteAsync(null);

        // Assert
        await _visitorService.Received(1).CheckOutAsync(1);
        await _visitorService.Received(1).GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default);
        await _dialogService.Received(1).ShowMessageAsync("Thành công", "Khách 'Nguyen Van A' đã rời khỏi KTX thành công!");
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task CheckOutAsync_WhenServiceThrows_Should_SetStatusMessage_And_ShowErrorDialog()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.SelectedVisitor = new VisitorLogDto
        {
            Id = 1,
            VisitorName = "Nguyen Van A",
            Status = VisitorStatus.InDormitory
        };

        _dialogService.ShowConfirmAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(true));

        _visitorService.CheckOutAsync(1, Arg.Any<string?>(), default)
            .Returns(Task.FromException<VisitorLogDto>(new InvalidOperationException("Lượt khách này đã rời KTX trước đó")));

        // Act
        await vm.CheckOutCommand.ExecuteAsync(null);

        // Assert
        vm.StatusMessage.Should().Contain("Lỗi check-out: Lượt khách này đã rời KTX trước đó");
        await _dialogService.Received(1).ShowMessageAsync("Lỗi", Arg.Is<string>(msg => msg.Contains("Lượt khách này đã rời KTX trước đó")));
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task MarkOverstayedAsync_Should_CallService_ReloadData_And_ShowNotification()
    {
        // Arrange
        _visitorService.MarkOverstayedVisitorsAsync(Arg.Any<DateTime?>(), default)
            .Returns(Task.FromResult(2));

        var vm = CreateViewModel();
        _visitorService.ClearReceivedCalls();
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.MarkOverstayedCommand.ExecuteAsync(null);

        // Assert
        await _visitorService.Received(1).MarkOverstayedVisitorsAsync();
        await _visitorService.Received(1).GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default);
        await _dialogService.Received(1).ShowMessageAsync("Quét quá giờ", "Đã phát hiện và gắn cờ quá giờ cho 2 lượt khách lưu trú sau 22:00.");
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task MarkOverstayedAsync_WhenServiceThrows_Should_CatchError_And_ShowErrorDialog()
    {
        // Arrange
        _visitorService.MarkOverstayedVisitorsAsync(Arg.Any<DateTime?>(), default)
            .Returns(Task.FromException<int>(new Exception("Lỗi đồng hồ hệ thống")));

        var vm = CreateViewModel();

        // Act
        await vm.MarkOverstayedCommand.ExecuteAsync(null);

        // Assert
        vm.StatusMessage.Should().Contain("Lỗi quét quá giờ: Lỗi đồng hồ hệ thống");
        await _dialogService.Received(1).ShowMessageAsync("Lỗi", Arg.Is<string>(msg => msg.Contains("Lỗi đồng hồ hệ thống")));
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshAsync_Should_ResetAllFilters_And_ReloadData()
    {
        // Arrange
        var vm = CreateViewModel();

        // Đặt các giá trị bộ lọc khác mặc định
        vm.SearchKeyword = "Khach Can Tim";
        vm.SelectedStatusFilter = "Quá giờ";
        vm.SelectedRoomFilter = new RoomDto { Id = 1, RoomNumber = "101" };
        vm.FromDate = DateTime.Today.AddDays(-7);
        vm.ToDate = DateTime.Today;
        vm.OnlyOverstayed = true;

        _visitorService.ClearReceivedCalls();
        _roomService.ClearReceivedCalls();

        // Act
        await vm.RefreshCommand.ExecuteAsync(null);

        // Assert: Bộ lọc được reset về giá trị mặc định
        vm.SearchKeyword.Should().BeEmpty();
        vm.SelectedStatusFilter.Should().Be("Tất cả");
        vm.SelectedRoomFilter.Should().BeNull();
        vm.FromDate.Should().BeNull();
        vm.ToDate.Should().BeNull();
        vm.OnlyOverstayed.Should().BeFalse();

        // Assert: Nạp lại danh sách phòng và khách
        await _roomService.Received(1).GetAllRoomsAsync();
        await _visitorService.Received(1).GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default);
    }

    [Fact]
    public async Task CheckInAsync_WhenStudentServiceIsNull_Should_ShowErrorNotification()
    {
        // Arrange: ViewModel không có StudentService
        var vm = CreateViewModel(includeStudentService: false);
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.CheckInCommand.ExecuteAsync(null);

        // Assert
        await _dialogService.Received(1).ShowMessageAsync("Lỗi", "Không tìm thấy dịch vụ quản lý sinh viên.");
    }

    [Fact]
    public async Task CheckInAsync_WhenDialogReturnsTrue_Should_ReloadData_And_ShowSuccess()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.ShowCheckInDialogHandler = dialogVm => Task.FromResult(true);

        _visitorService.ClearReceivedCalls();
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.CheckInCommand.ExecuteAsync(null);

        // Assert
        await _visitorService.Received(1).GetVisitorsAsync(Arg.Any<VisitorFilterDto>(), default);
        await _dialogService.Received(1).ShowMessageAsync("Thành công", "Đăng ký khách vào KTX thành công!");
    }

    [Fact]
    public async Task CheckInAsync_WhenDialogReturnsFalse_Should_NotReload_And_NotShowSuccess()
    {
        // Arrange
        var vm = CreateViewModel();
        vm.ShowCheckInDialogHandler = dialogVm => Task.FromResult(false);

        _visitorService.ClearReceivedCalls();
        _dialogService.ClearReceivedCalls();

        // Act
        await vm.CheckInCommand.ExecuteAsync(null);

        // Assert
        await _visitorService.DidNotReceiveWithAnyArgs().GetVisitorsAsync(default!, default);
        await _dialogService.DidNotReceiveWithAnyArgs().ShowMessageAsync(default!, default!);
    }
}

/// <summary>
/// Bộ kiểm thử đơn vị cho VisitorCheckInDialogViewModel - Hộp thoại đăng ký check-in khách
/// </summary>
public class VisitorCheckInDialogViewModelTests
{
    private readonly IVisitorService _visitorService;
    private readonly IRoomService _roomService;
    private readonly IStudentService _studentService;
    private readonly IDialogService _dialogService;

    public VisitorCheckInDialogViewModelTests()
    {
        _visitorService = Substitute.For<IVisitorService>();
        _roomService = Substitute.For<IRoomService>();
        _studentService = Substitute.For<IStudentService>();
        _dialogService = Substitute.For<IDialogService>();

        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromResult(new List<RoomDto>
            {
                new() { Id = 1, RoomNumber = "101", Capacity = 4, CurrentOccupancy = 2 },
                new() { Id = 2, RoomNumber = "102", Capacity = 4, CurrentOccupancy = 3 }
            }));

        _studentService.GetAllStudentsAsync(Arg.Any<string?>(), Arg.Any<int?>())
            .Returns(Task.FromResult(new List<StudentDto>
            {
                new() { Id = 10, FullName = "Nguyen Van Sinh Vien 1", CurrentRoomId = 1 },
                new() { Id = 11, FullName = "Tran Van Sinh Vien 2", CurrentRoomId = 1 }
            }));
    }

    private VisitorCheckInDialogViewModel CreateDialogViewModel()
    {
        return new VisitorCheckInDialogViewModel(
            _visitorService,
            _roomService,
            _studentService,
            _dialogService);
    }

    [Fact]
    public void Constructor_WithNullArguments_Should_ThrowArgumentNullException()
    {
        var act1 = () => new VisitorCheckInDialogViewModel(null!, _roomService, _studentService, _dialogService);
        var act2 = () => new VisitorCheckInDialogViewModel(_visitorService, null!, _studentService, _dialogService);
        var act3 = () => new VisitorCheckInDialogViewModel(_visitorService, _roomService, null!, _dialogService);
        var act4 = () => new VisitorCheckInDialogViewModel(_visitorService, _roomService, _studentService, null!);

        act1.Should().Throw<ArgumentNullException>().WithParameterName("visitorService");
        act2.Should().Throw<ArgumentNullException>().WithParameterName("roomService");
        act3.Should().Throw<ArgumentNullException>().WithParameterName("studentService");
        act4.Should().Throw<ArgumentNullException>().WithParameterName("dialogService");
    }

    [Fact]
    public async Task Constructor_Should_LoadRoomsAutomatically()
    {
        // Act
        var vm = CreateDialogViewModel();
        await Task.Delay(50); // Chờ LoadRoomsAsync hoàn tất

        // Assert
        vm.Rooms.Should().HaveCount(2);
        vm.Rooms[0].RoomNumber.Should().Be("101");
        vm.Rooms[1].RoomNumber.Should().Be("102");
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task LoadRoomsAsync_WhenServiceThrows_Should_SetErrorMessage()
    {
        // Arrange
        _roomService.GetAllRoomsAsync(Arg.Any<string?>(), Arg.Any<RoomStatus?>())
            .Returns(Task.FromException<List<RoomDto>>(new Exception("Lỗi đọc cơ sở dữ liệu phòng")));

        var vm = CreateDialogViewModel();

        // Act
        await vm.LoadRoomsAsync();

        // Assert
        vm.ErrorMessage.Should().Contain("Lỗi tải danh sách phòng: Lỗi đọc cơ sở dữ liệu phòng");
    }

    [Fact]
    public async Task OnSelectedRoomChanged_WhenRoomSelected_Should_LoadStudents_And_SelectFirstStudent()
    {
        // Arrange
        var sampleStudents = new List<StudentDto>
        {
            new() { Id = 10, FullName = "Nguyen Van Sinh Vien 1", CurrentRoomId = 1 },
            new() { Id = 11, FullName = "Tran Van Sinh Vien 2", CurrentRoomId = 1 }
        };

        _studentService.GetAllStudentsAsync(searchQuery: Arg.Any<string?>(), roomId: 1)
            .Returns(Task.FromResult(sampleStudents));

        var vm = CreateDialogViewModel();

        // Act: Chọn phòng 101
        var targetRoom = new RoomDto { Id = 1, RoomNumber = "101" };
        vm.SelectedRoom = targetRoom;
        await Task.Delay(50); // Chờ OnSelectedRoomChanged hoàn tất

        // Assert
        vm.StudentsInRoom.Should().HaveCount(2);
        vm.StudentsInRoom[0].FullName.Should().Be("Nguyen Van Sinh Vien 1");
        vm.SelectedStudent.Should().NotBeNull();
        vm.SelectedStudent!.Id.Should().Be(10);
    }

    [Fact]
    public async Task OnSelectedRoomChanged_WhenRoomSetToNull_Should_ClearStudents_And_SelectedStudent()
    {
        // Arrange
        var vm = CreateDialogViewModel();
        vm.SelectedRoom = new RoomDto { Id = 1, RoomNumber = "101" };
        await Task.Delay(50);

        // Act: Đặt phòng về null
        vm.SelectedRoom = null;
        await Task.Delay(20);

        // Assert
        vm.StudentsInRoom.Should().BeEmpty();
        vm.SelectedStudent.Should().BeNull();
    }

    [Fact]
    public async Task OnSelectedRoomChanged_WhenNoStudentsInRoom_Should_KeepStudentsEmpty_And_NullSelectedStudent()
    {
        // Arrange: Phòng trống không có sinh viên
        _studentService.GetAllStudentsAsync(searchQuery: Arg.Any<string?>(), roomId: 2)
            .Returns(Task.FromResult(new List<StudentDto>()));

        var vm = CreateDialogViewModel();

        // Act
        vm.SelectedRoom = new RoomDto { Id = 2, RoomNumber = "102" };
        await Task.Delay(50);

        // Assert
        vm.StudentsInRoom.Should().BeEmpty();
        vm.SelectedStudent.Should().BeNull();
    }

    [Fact]
    public async Task OnSelectedRoomChanged_WhenServiceThrows_Should_SetErrorMessage()
    {
        // Arrange
        _studentService.GetAllStudentsAsync(searchQuery: Arg.Any<string?>(), roomId: 1)
            .Returns(Task.FromException<List<StudentDto>>(new Exception("Lỗi truy vấn sinh viên")));

        var vm = CreateDialogViewModel();

        // Act
        vm.SelectedRoom = new RoomDto { Id = 1, RoomNumber = "101" };
        await Task.Delay(50);

        // Assert
        vm.ErrorMessage.Should().Contain("Không thể tải danh sách sinh viên: Lỗi truy vấn sinh viên");
    }

    [Theory]
    [InlineData("", "001200001111", "0901111222", true, true, "Vui lòng nhập họ và tên khách.")]
    [InlineData("   ", "001200001111", "0901111222", true, true, "Vui lòng nhập họ và tên khách.")]
    [InlineData("Nguyen Van Khach", "", "0901111222", true, true, "Vui lòng nhập số CCCD / CMND của khách.")]
    [InlineData("Nguyen Van Khach", "   ", "0901111222", true, true, "Vui lòng nhập số CCCD / CMND của khách.")]
    [InlineData("Nguyen Van Khach", "001200001111", "", true, true, "Vui lòng nhập số điện thoại liên hệ của khách.")]
    [InlineData("Nguyen Van Khach", "001200001111", "   ", true, true, "Vui lòng nhập số điện thoại liên hệ của khách.")]
    [InlineData("Nguyen Van Khach", "001200001111", "0901111222", false, true, "Vui lòng chọn phòng cần đến thăm.")]
    [InlineData("Nguyen Van Khach", "001200001111", "0901111222", true, false, "Vui lòng chọn sinh viên cần đến thăm.")]
    public async Task ConfirmCheckInAsync_ValidationFailures_Should_ShowNotification_And_NotCallService(
        string visitorName,
        string idNumber,
        string phone,
        bool hasRoom,
        bool hasStudent,
        string expectedError)
    {
        // Arrange
        var vm = CreateDialogViewModel();
        vm.VisitorName = visitorName;
        vm.IdentificationNumber = idNumber;
        vm.PhoneNumber = phone;
        vm.SelectedRoom = hasRoom ? new RoomDto { Id = 1, RoomNumber = "101" } : null;
        vm.SelectedStudent = hasStudent ? new StudentDto { Id = 10, FullName = "Nguyen Van A" } : null;

        _dialogService.ClearReceivedCalls();

        // Act
        await vm.ConfirmCheckInCommand.ExecuteAsync(null);

        // Assert
        vm.ErrorMessage.Should().Be(expectedError);
        await _dialogService.Received(1).ShowMessageAsync("Thông báo", expectedError);
        await _visitorService.DidNotReceiveWithAnyArgs().CheckInAsync(default!, default);
    }

    [Fact]
    public async Task ConfirmCheckInAsync_WhenDataIsValid_Should_CallService_And_InvokeCloseWithTrue()
    {
        // Arrange
        var vm = CreateDialogViewModel();
        vm.VisitorName = "  Nguyen Van Khach  ";
        vm.IdentificationNumber = "  001200001111  ";
        vm.PhoneNumber = "  0901111222  ";
        vm.SelectedRoom = new RoomDto { Id = 1, RoomNumber = "101" };
        vm.SelectedStudent = new StudentDto { Id = 10, FullName = "Nguyen Van Sinh Vien" };
        vm.Note = "  Đến thăm anh trai  ";

        bool? actionResult = null;
        bool? handlerResult = null;
        vm.CloseAction = res => actionResult = res;
        vm.CloseDialogHandler = res =>
        {
            handlerResult = res;
            return Task.CompletedTask;
        };

        CreateVisitorLogDto? capturedDto = null;
        _visitorService.CheckInAsync(Arg.Do<CreateVisitorLogDto>(dto => capturedDto = dto), default)
            .Returns(Task.FromResult(new VisitorLogDto
            {
                Id = 100,
                VisitorName = "Nguyen Van Khach",
                Status = VisitorStatus.InDormitory
            }));

        // Act
        await vm.ConfirmCheckInCommand.ExecuteAsync(null);

        // Assert: DTO được chuẩn hóa và truyền đúng vào Service
        capturedDto.Should().NotBeNull();
        capturedDto!.VisitorName.Should().Be("Nguyen Van Khach");
        capturedDto.IdentificationNumber.Should().Be("001200001111");
        capturedDto.PhoneNumber.Should().Be("0901111222");
        capturedDto.RoomId.Should().Be(1);
        capturedDto.StudentId.Should().Be(10);
        capturedDto.Note.Should().Be("Đến thăm anh trai");

        // Assert: Đóng dialog với kết quả thành công
        actionResult.Should().BeTrue();
        handlerResult.Should().BeTrue();
        vm.IsLoading.Should().BeFalse();
        vm.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public async Task ConfirmCheckInAsync_WhenServiceThrows_Should_SetErrorMessage_And_ShowErrorDialog()
    {
        // Arrange
        var vm = CreateDialogViewModel();
        vm.VisitorName = "Nguyen Van Khach";
        vm.IdentificationNumber = "001200001111";
        vm.PhoneNumber = "0901111222";
        vm.SelectedRoom = new RoomDto { Id = 1, RoomNumber = "101" };
        vm.SelectedStudent = new StudentDto { Id = 10, FullName = "Nguyen Van Sinh Vien" };

        _visitorService.CheckInAsync(Arg.Any<CreateVisitorLogDto>(), default)
            .Returns(Task.FromException<VisitorLogDto>(new InvalidOperationException("Sinh viên không thuộc phòng này")));

        bool? actionResult = null;
        vm.CloseAction = res => actionResult = res;

        // Act
        await vm.ConfirmCheckInCommand.ExecuteAsync(null);

        // Assert
        vm.ErrorMessage.Should().Contain("Lỗi khi đăng ký khách: Sinh viên không thuộc phòng này");
        await _dialogService.Received(1).ShowMessageAsync("Lỗi", Arg.Is<string>(msg => msg.Contains("Sinh viên không thuộc phòng này")));
        actionResult.Should().BeNull(); // Không được đóng dialog
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public async Task CancelAsync_Should_InvokeCloseWithFalse_And_NotCallService()
    {
        // Arrange
        var vm = CreateDialogViewModel();
        bool? actionResult = null;
        bool? handlerResult = null;

        vm.CloseAction = res => actionResult = res;
        vm.CloseDialogHandler = res =>
        {
            handlerResult = res;
            return Task.CompletedTask;
        };

        // Act
        await vm.CancelCommand.ExecuteAsync(null);

        // Assert
        actionResult.Should().BeFalse();
        handlerResult.Should().BeFalse();
        await _visitorService.DidNotReceiveWithAnyArgs().CheckInAsync(default!, default);
    }
}
