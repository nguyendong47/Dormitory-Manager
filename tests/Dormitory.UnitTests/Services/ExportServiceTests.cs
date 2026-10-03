using ClosedXML.Excel;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử đơn vị cho dịch vụ xuất dữ liệu Excel (ExportService)
/// </summary>
public class ExportServiceTests
{
    private readonly IExportService _exportService;

    public ExportServiceTests()
    {
        _exportService = new ExportService();
    }

    [Fact]
    public async Task ExportRoomsToExcelAsync_WithValidRooms_ShouldReturnNonEmptyBytesAndValidWorkbook()
    {
        // Sắp đặt danh sách phòng mẫu
        var rooms = new List<RoomDto>
        {
            new()
            {
                Id = 1,
                RoomNumber = "P101",
                Building = "Tòa A",
                Floor = 1,
                Capacity = 4,
                CurrentOccupancy = 2,
                PricePerMonth = 500000m,
                Type = RoomType.Standard,
                AllowedGender = Gender.Male,
                Status = RoomStatus.Available,
                Description = "Phòng tầng 1 thoáng mát"
            },
            new()
            {
                Id = 2,
                RoomNumber = "P201",
                Building = "Tòa B",
                Floor = 2,
                Capacity = 2,
                CurrentOccupancy = 2,
                PricePerMonth = 1200000m,
                Type = RoomType.Vip,
                AllowedGender = Gender.Female,
                Status = RoomStatus.Occupied,
                Description = "Phòng VIP máy lạnh"
            }
        };

        // Thực hiện
        var result = await _exportService.ExportRoomsToExcelAsync(rooms);

        // Kiểm tra byte array hợp lệ
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        // Mở lại workbook kiểm tra tính toàn vẹn
        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        worksheet.Should().NotBeNull();
        worksheet.Name.Should().Be("Danh sách phòng");

        // Kiểm tra dòng tiêu đề
        worksheet.Cell(1, 1).GetString().Should().Be("STT");
        worksheet.Cell(1, 2).GetString().Should().Be("Số phòng");
        worksheet.Cell(1, 3).GetString().Should().Be("Tòa nhà");
        worksheet.Cell(1, 4).GetString().Should().Be("Tầng");
        worksheet.Cell(1, 5).GetString().Should().Be("Loại phòng");
        worksheet.Cell(1, 6).GetString().Should().Be("Giới tính");
        worksheet.Cell(1, 7).GetString().Should().Be("Sức chứa");
        worksheet.Cell(1, 8).GetString().Should().Be("Đang ở");
        worksheet.Cell(1, 9).GetString().Should().Be("Đơn giá (VNĐ)");
        worksheet.Cell(1, 10).GetString().Should().Be("Trạng thái");
        worksheet.Cell(1, 11).GetString().Should().Be("Ghi chú");

        // Kiểm tra dữ liệu dòng 1 (hàng 2)
        worksheet.Cell(2, 1).GetDouble().Should().Be(1);
        worksheet.Cell(2, 2).GetString().Should().Be("P101");
        worksheet.Cell(2, 3).GetString().Should().Be("Tòa A");
        worksheet.Cell(2, 4).GetDouble().Should().Be(1);
        worksheet.Cell(2, 7).GetDouble().Should().Be(4);
        worksheet.Cell(2, 8).GetDouble().Should().Be(2);
        worksheet.Cell(2, 9).GetDouble().Should().Be(500000);

        // Kiểm tra tổng số dòng sử dụng: 1 dòng header + 2 dòng data = 3
        worksheet.LastRowUsed()?.RowNumber().Should().Be(3);
    }

    [Fact]
    public async Task ExportStudentsToExcelAsync_WithValidStudents_ShouldHaveCorrectHeadersAndData()
    {
        // Sắp đặt danh sách sinh viên mẫu
        var students = new List<StudentDto>
        {
            new()
            {
                Id = 1,
                StudentCode = "SV001",
                FullName = "Nguyễn Văn A",
                DateOfBirth = new DateTime(2003, 5, 15),
                Gender = Gender.Male,
                IdentityCard = "001203001234",
                PhoneNumber = "0987654321",
                Email = "sv001@example.com",
                HomeTown = "Hà Nội",
                ClassName = "CNTT1",
                Faculty = "Công nghệ thông tin",
                CurrentRoomNumber = "P101",
                ParentName = "Nguyễn Văn B",
                ParentPhoneNumber = "0912345678",
                Address = "Số 1 Đại Cồ Việt, Hà Nội"
            }
        };

        // Thực hiện
        var result = await _exportService.ExportStudentsToExcelAsync(students);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        worksheet.Should().NotBeNull();
        worksheet.Name.Should().Be("Danh sách sinh viên");

        // Kiểm tra tiêu đề các cột chính theo yêu cầu ("Mã SV", "Họ và tên", v.v.)
        worksheet.Cell(1, 1).GetString().Should().Be("STT");
        worksheet.Cell(1, 2).GetString().Should().Be("Mã SV");
        worksheet.Cell(1, 3).GetString().Should().Be("Họ và tên");
        worksheet.Cell(1, 4).GetString().Should().Be("Ngày sinh");
        worksheet.Cell(1, 5).GetString().Should().Be("Giới tính");
        worksheet.Cell(1, 6).GetString().Should().Be("CCCD/CMND");
        worksheet.Cell(1, 7).GetString().Should().Be("Số điện thoại");
        worksheet.Cell(1, 8).GetString().Should().Be("Email");
        worksheet.Cell(1, 9).GetString().Should().Be("Quê quán");
        worksheet.Cell(1, 10).GetString().Should().Be("Lớp");
        worksheet.Cell(1, 11).GetString().Should().Be("Khoa");
        worksheet.Cell(1, 12).GetString().Should().Be("Phòng hiện tại");

        // Kiểm tra dữ liệu sinh viên dòng 2
        worksheet.Cell(2, 2).GetString().Should().Be("SV001");
        worksheet.Cell(2, 3).GetString().Should().Be("Nguyễn Văn A");
        worksheet.Cell(2, 5).GetString().Should().Be("Nam");
        worksheet.Cell(2, 10).GetString().Should().Be("CNTT1");
        worksheet.Cell(2, 12).GetString().Should().Be("P101");
    }

    [Fact]
    public async Task ExportBillsToExcelAsync_WithValidBills_ShouldCreateCorrectNumberOfRows()
    {
        // Sắp đặt 3 hóa đơn mẫu
        var bills = new List<BillDto>
        {
            new()
            {
                Id = 1,
                BillCode = "HD202410-001",
                RoomId = 101,
                RoomNumber = "P101",
                Building = "Tòa A",
                Month = 10,
                Year = 2024,
                RoomFee = 600000m,
                OldElectricIndex = 100m,
                NewElectricIndex = 150m,
                ElectricRate = 3500m,
                OldWaterIndex = 10m,
                NewWaterIndex = 15m,
                WaterRate = 15000m,
                OtherServiceFee = 50000m,
                Status = BillStatus.Paid,
                DueDate = new DateTime(2024, 10, 15),
                PaidDate = new DateTime(2024, 10, 12),
                Note = "Đã thanh toán qua chuyển khoản"
            },
            new()
            {
                Id = 2,
                BillCode = "HD202410-002",
                RoomId = 102,
                RoomNumber = "P102",
                Building = "Tòa A",
                Month = 10,
                Year = 2024,
                RoomFee = 600000m,
                OldElectricIndex = 80m,
                NewElectricIndex = 120m,
                ElectricRate = 3500m,
                OldWaterIndex = 5m,
                NewWaterIndex = 8m,
                WaterRate = 15000m,
                OtherServiceFee = 50000m,
                Status = BillStatus.Unpaid,
                DueDate = new DateTime(2024, 10, 15),
                PaidDate = null,
                Note = null
            },
            new()
            {
                Id = 3,
                BillCode = "HD202410-003",
                RoomId = 201,
                RoomNumber = "P201",
                Building = "Tòa B",
                Month = 10,
                Year = 2024,
                RoomFee = 1200000m,
                OldElectricIndex = 200m,
                NewElectricIndex = 260m,
                ElectricRate = 3500m,
                OldWaterIndex = 20m,
                NewWaterIndex = 28m,
                WaterRate = 15000m,
                OtherServiceFee = 70000m,
                Status = BillStatus.Overdue,
                DueDate = new DateTime(2024, 10, 10),
                PaidDate = null,
                Note = "Nhắc nhở thanh toán"
            }
        };

        // Thực hiện
        var result = await _exportService.ExportBillsToExcelAsync(bills);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        worksheet.Should().NotBeNull();
        worksheet.Name.Should().Be("Danh sách hóa đơn");

        // Dòng header là 1 + 3 hóa đơn => tổng cộng 4 dòng
        worksheet.LastRowUsed()?.RowNumber().Should().Be(4);

        // Kiểm tra dòng 1: header
        worksheet.Cell(1, 1).GetString().Should().Be("STT");
        worksheet.Cell(1, 2).GetString().Should().Be("Mã hóa đơn");
        worksheet.Cell(1, 3).GetString().Should().Be("Số phòng");
        worksheet.Cell(1, 4).GetString().Should().Be("Tòa nhà");
        worksheet.Cell(1, 5).GetString().Should().Be("Tháng/Năm");
        worksheet.Cell(1, 6).GetString().Should().Be("Tiền phòng (VNĐ)");
        worksheet.Cell(1, 10).GetString().Should().Be("Tiền điện (VNĐ)");
        worksheet.Cell(1, 14).GetString().Should().Be("Tiền nước (VNĐ)");
        worksheet.Cell(1, 16).GetString().Should().Be("Tổng tiền (VNĐ)");
        worksheet.Cell(1, 19).GetString().Should().Be("Trạng thái");

        // Kiểm tra dữ liệu hóa đơn 1
        worksheet.Cell(2, 2).GetString().Should().Be("HD202410-001");
        worksheet.Cell(2, 3).GetString().Should().Be("P101");
        worksheet.Cell(2, 5).GetString().Should().Be("10/2024");
        worksheet.Cell(2, 6).GetDouble().Should().Be(600000);
        // Tiêu thụ điện 50 kWh * 3500 = 175,000
        worksheet.Cell(2, 10).GetDouble().Should().Be(175000);
        // Tiêu thụ nước 5 m3 * 15000 = 75,000
        worksheet.Cell(2, 14).GetDouble().Should().Be(75000);
        // Tổng tiền: 600,000 + 175,000 + 75,000 + 50,000 = 900,000
        worksheet.Cell(2, 16).GetDouble().Should().Be(900000);
        worksheet.Cell(2, 19).GetString().Should().Be("Đã thanh toán");

        // Kiểm tra trạng thái hóa đơn 2 và 3
        worksheet.Cell(3, 19).GetString().Should().Be("Chưa thanh toán");
        worksheet.Cell(4, 19).GetString().Should().Be("Quá hạn");
    }

    [Fact]
    public async Task ExportRoomsToExcelAsync_WithEmptyList_ShouldGenerateHeadersOnly()
    {
        // Sắp đặt danh sách rỗng
        var emptyRooms = new List<RoomDto>();

        // Thực hiện
        var result = await _exportService.ExportRoomsToExcelAsync(emptyRooms);

        // Kiểm tra
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        worksheet.Should().NotBeNull();
        worksheet.Cell(1, 1).GetString().Should().Be("STT");
        worksheet.Cell(1, 2).GetString().Should().Be("Số phòng");
        worksheet.LastRowUsed()?.RowNumber().Should().Be(1);
    }
}
