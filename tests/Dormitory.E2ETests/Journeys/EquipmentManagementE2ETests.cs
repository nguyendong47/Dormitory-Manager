using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.E2ETests.Journeys;

/// <summary>
/// Kiểm thử luồng E2E cho quy trình quản lý danh sách tài sản/thiết bị phòng, lọc dữ liệu, thêm mới và xóa
/// </summary>
public class EquipmentManagementE2ETests : IDisposable
{
    private readonly TestFixture _fixture = new();

    /// <summary>
    /// Kịch bản E2E: Tải danh sách -> Lọc theo trạng thái -> Thêm mới thiết bị -> Xóa thiết bị
    /// </summary>
    [AvaloniaFact]
    public async Task Should_Load_Filter_Add_And_Delete_Equipments_Successfully()
    {
        // 1. Khởi tạo EquipmentListViewModel từ TestFixture
        var vm = _fixture.CreateEquipmentListViewModel();

        // Đảm bảo phiên đăng nhập có quyền Quản trị viên (Admin) để có quyền xóa thiết bị
        _fixture.UserSession.SetUser(new UserDto
        {
            Id = 1,
            Username = "admin",
            FullName = "Quản Trị Viên",
            Role = UserRole.Admin
        });

        // 2. Tải danh sách thiết bị ban đầu (dữ liệu seed mẫu)
        await vm.LoadEquipmentsCommand.ExecuteAsync(null);

        vm.Equipments.Should().NotBeEmpty("Danh sách trang thiết bị phải có dữ liệu sau khi nạp");
        vm.Equipments.Count.Should().BeGreaterThan(0);
        vm.TotalCount.Should().BeGreaterThan(0, "Tổng số lượng thiết bị KPI phải lớn hơn 0");
        var initialEquipmentCount = vm.Equipments.Count;

        // 3. Kiểm tra chức năng lọc theo trạng thái: "Hoạt động tốt"
        vm.SelectedStatusOption = "Hoạt động tốt";
        await vm.LoadEquipmentsCommand.ExecuteAsync(null);

        vm.Equipments.Should().NotBeEmpty();
        vm.Equipments.Should().OnlyContain(e => e.Status == EquipmentStatus.Good,
            "Bộ lọc chỉ được trả về các thiết bị có trạng thái Hoạt động tốt");
        vm.Equipments.Sum(e => e.Quantity).Should().Be(vm.GoodCount,
            "Số lượng hiển thị phải khớp với chỉ số thống kê GoodCount trên KPI");

        // Đặt lại bộ lọc về "Tất cả"
        vm.SelectedStatusOption = "Tất cả";
        await vm.LoadEquipmentsCommand.ExecuteAsync(null);
        vm.Equipments.Count.Should().Be(initialEquipmentCount);

        // 4. Thêm thiết bị mới thông qua modal AddEquipment
        const string newEquipmentCode = "TB-E2E-999";
        const string newEquipmentName = "Quạt cây Panasonic E2E";
        const int newEquipmentQuantity = 4;
        const decimal newEquipmentPrice = 1250000m;

        _fixture.DialogService.OnShowDialogAsync = async dialog =>
        {
            if (dialog.DataContext is EquipmentDialogViewModel dialogVm)
            {
                dialogVm.SelectedRoom = dialogVm.Rooms.FirstOrDefault();
                dialogVm.EquipmentCode = newEquipmentCode;
                dialogVm.Name = newEquipmentName;
                dialogVm.Status = EquipmentStatus.Good;
                dialogVm.Quantity = newEquipmentQuantity;
                dialogVm.Price = newEquipmentPrice;
                dialogVm.Notes = "Tài sản kiểm thử E2E tự động";

                await dialogVm.SaveCommand.ExecuteAsync(null);
                return true;
            }
            return false;
        };

        await vm.AddEquipmentCommand.ExecuteAsync(null);

        // Xác minh thiết bị mới đã hiển thị trên giao diện DataGrid
        vm.Equipments.Should().Contain(e => e.EquipmentCode == newEquipmentCode,
            "Thiết bị mới tạo phải xuất hiện ngay trong danh sách hiển thị");
        var addedItem = vm.Equipments.First(e => e.EquipmentCode == newEquipmentCode);
        addedItem.Name.Should().Be(newEquipmentName);
        addedItem.Quantity.Should().Be(newEquipmentQuantity);

        // Xác minh thiết bị mới đã được lưu kiên cố trong cơ sở dữ liệu SQLite
        var dbEntity = await _fixture.Context.Equipments.FirstOrDefaultAsync(e => e.EquipmentCode == newEquipmentCode);
        dbEntity.Should().NotBeNull("Bản ghi phải tồn tại trong cơ sở dữ liệu SQLite");
        dbEntity!.Name.Should().Be(newEquipmentName);
        dbEntity.Price.Should().Be(newEquipmentPrice);

        // 5. Xóa thiết bị vừa thêm
        vm.SelectedEquipment = addedItem;
        _fixture.DialogService.ConfirmResult = true;

        await vm.DeleteEquipmentCommand.ExecuteAsync(null);

        // Xác minh thiết bị đã bị loại bỏ khỏi danh sách trên ViewModel
        vm.Equipments.Should().NotContain(e => e.EquipmentCode == newEquipmentCode,
            "Bản ghi phải bị loại bỏ khỏi danh sách trên giao diện sau khi xóa");

        // Xác minh thiết bị đã bị xóa khỏi cơ sở dữ liệu SQLite
        var deletedDbEntity = await _fixture.Context.Equipments.FirstOrDefaultAsync(e => e.EquipmentCode == newEquipmentCode);
        deletedDbEntity.Should().BeNull("Bản ghi không được tồn tại trong cơ sở dữ liệu sau khi xóa");
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
