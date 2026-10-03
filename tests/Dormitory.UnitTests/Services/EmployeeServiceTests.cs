using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Application.Services;
using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dormitory.UnitTests.Services;

/// <summary>
/// Kiểm thử các chức năng xử lý nghiệp vụ quản lý nhân viên (EmployeeService)
/// </summary>
public class EmployeeServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DormitoryDbContext _context;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        // Khởi tạo kết nối SQLite In-Memory riêng biệt cho mỗi bài kiểm thử
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DormitoryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DormitoryDbContext(options);
        _context.Database.EnsureCreated();

        _service = new EmployeeService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateEmployeeAsync_WithValidRequest_ShouldCreateAndSaveEmployee()
    {
        // Sắp đặt
        var request = new CreateOrUpdateEmployeeRequest
        {
            EmployeeCode = "NV001",
            FullName = "Nguyễn Văn An",
            DateOfBirth = new DateTime(1990, 1, 15),
            Gender = Gender.Male,
            PhoneNumber = "0912345678",
            IdentityCard = "001090001234",
            Position = "Kế toán viên",
            Department = "Phòng Kế toán",
            Address = "Hà Nội"
        };

        // Thực hiện
        var result = await _service.CreateEmployeeAsync(request);

        // Kiểm tra kết quả trả về
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.EmployeeCode.Should().Be("NV001");
        result.FullName.Should().Be("Nguyễn Văn An");
        result.Position.Should().Be("Kế toán viên");
        result.Department.Should().Be("Phòng Kế toán");

        // Kiểm tra lưu trong cơ sở dữ liệu
        var dbEmployee = await _context.Employees.FindAsync(result.Id);
        dbEmployee.Should().NotBeNull();
        dbEmployee!.EmployeeCode.Should().Be("NV001");
        dbEmployee.FullName.Should().Be("Nguyễn Văn An");
    }

    [Fact]
    public async Task CreateEmployeeAsync_WithDuplicateEmployeeCode_ShouldThrowInvalidOperationException()
    {
        // Sắp đặt nhân viên đã tồn tại
        var existing = new Employee
        {
            EmployeeCode = "NV001",
            FullName = "Nguyễn Văn Cũ",
            DateOfBirth = new DateTime(1988, 5, 10),
            Gender = Gender.Male,
            PhoneNumber = "0900000001",
            IdentityCard = "001088000001",
            Position = "Nhân viên",
            Department = "Kỹ thuật"
        };
        _context.Employees.Add(existing);
        await _context.SaveChangesAsync();

        var request = new CreateOrUpdateEmployeeRequest
        {
            EmployeeCode = "NV001", // Trùng mã nhân viên
            FullName = "Nguyễn Văn Mới",
            DateOfBirth = new DateTime(1992, 3, 20),
            Gender = Gender.Male,
            PhoneNumber = "0900000002",
            IdentityCard = "001092000002",
            Position = "Bảo vệ",
            Department = "Bảo vệ"
        };

        // Thực hiện & Kiểm tra
        var act = async () => await _service.CreateEmployeeAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*NV001*đã tồn tại*");
    }

    [Fact]
    public async Task GetAllEmployeesAsync_WithSearchQueryAndDepartmentFilter_ShouldReturnFilteredList()
    {
        // Sắp đặt danh sách nhân viên mẫu
        _context.Employees.AddRange(
            new Employee
            {
                EmployeeCode = "NV001",
                FullName = "Nguyễn Văn An",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = Gender.Male,
                PhoneNumber = "0911111111",
                IdentityCard = "001090000001",
                Position = "Kế toán viên",
                Department = "Kế toán"
            },
            new Employee
            {
                EmployeeCode = "NV002",
                FullName = "Trần Thị Bích",
                DateOfBirth = new DateTime(1992, 2, 2),
                Gender = Gender.Female,
                PhoneNumber = "0922222222",
                IdentityCard = "001092000002",
                Position = "Thủ quỹ",
                Department = "Kế toán"
            },
            new Employee
            {
                EmployeeCode = "NV003",
                FullName = "Lê Văn Cường",
                DateOfBirth = new DateTime(1985, 3, 3),
                Gender = Gender.Male,
                PhoneNumber = "0933333333",
                IdentityCard = "001085000003",
                Position = "Quản lý tòa nhà",
                Department = "Quản lý tòa"
            }
        );
        await _context.SaveChangesAsync();

        // 1. Lọc theo phòng ban "Kế toán"
        var keToanList = await _service.GetAllEmployeesAsync(department: "Kế toán");
        keToanList.Should().HaveCount(2);
        keToanList.Select(e => e.EmployeeCode).Should().Contain(new[] { "NV001", "NV002" });

        // 2. Tìm kiếm theo tên "Bích"
        var searchName = await _service.GetAllEmployeesAsync(searchQuery: "Bích");
        searchName.Should().HaveCount(1);
        searchName[0].FullName.Should().Be("Trần Thị Bích");

        // 3. Tìm kiếm theo mã "NV003"
        var searchCode = await _service.GetAllEmployeesAsync(searchQuery: "NV003");
        searchCode.Should().HaveCount(1);
        searchCode[0].EmployeeCode.Should().Be("NV003");

        // 4. Lọc kết hợp phòng ban và từ khóa
        var combined = await _service.GetAllEmployeesAsync(searchQuery: "An", department: "Kế toán");
        combined.Should().HaveCount(1);
        combined[0].EmployeeCode.Should().Be("NV001");
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_WithExistingId_ShouldReturnDto()
    {
        // Sắp đặt
        var employee = new Employee
        {
            EmployeeCode = "NV010",
            FullName = "Đỗ Văn Mười",
            DateOfBirth = new DateTime(1995, 10, 10),
            Gender = Gender.Male,
            PhoneNumber = "0910101010",
            IdentityCard = "001095010010",
            Position = "Kỹ thuật viên",
            Department = "Kỹ thuật"
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Thực hiện
        var result = await _service.GetEmployeeByIdAsync(employee.Id);

        // Kiểm tra
        result.Should().NotBeNull();
        result!.EmployeeCode.Should().Be("NV010");
        result.FullName.Should().Be("Đỗ Văn Mười");
    }

    [Fact]
    public async Task UpdateEmployeeAsync_WithValidData_ShouldUpdateAndReturnTrue()
    {
        // Sắp đặt
        var employee = new Employee
        {
            EmployeeCode = "NV005",
            FullName = "Phạm Văn Năm",
            DateOfBirth = new DateTime(1991, 5, 5),
            Gender = Gender.Male,
            PhoneNumber = "0955555555",
            IdentityCard = "001091000005",
            Position = "Bảo vệ ca sáng",
            Department = "Bảo vệ"
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var updateRequest = new CreateOrUpdateEmployeeRequest
        {
            EmployeeCode = "NV005",
            FullName = "Phạm Văn Năm (Đã đổi)",
            DateOfBirth = new DateTime(1991, 5, 5),
            Gender = Gender.Male,
            PhoneNumber = "0988888888",
            IdentityCard = "001091000005",
            Position = "Tổ trưởng Bảo vệ",
            Department = "Bảo vệ",
            Address = "Hà Nội"
        };

        // Thực hiện
        var success = await _service.UpdateEmployeeAsync(employee.Id, updateRequest);

        // Kiểm tra
        success.Should().BeTrue();
        var updated = await _context.Employees.FindAsync(employee.Id);
        updated!.FullName.Should().Be("Phạm Văn Năm (Đã đổi)");
        updated.Position.Should().Be("Tổ trưởng Bảo vệ");
        updated.PhoneNumber.Should().Be("0988888888");
        updated.Address.Should().Be("Hà Nội");
    }

    [Fact]
    public async Task UpdateEmployeeAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Sắp đặt
        var request = new CreateOrUpdateEmployeeRequest
        {
            EmployeeCode = "NV999",
            FullName = "Không tồn tại",
            DateOfBirth = DateTime.UtcNow,
            Gender = Gender.Male,
            PhoneNumber = "0999999999",
            IdentityCard = "001099999999",
            Position = "N/A",
            Department = "N/A"
        };

        // Thực hiện
        var success = await _service.UpdateEmployeeAsync(999, request);

        // Kiểm tra
        success.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteEmployeeAsync_WithExistingId_ShouldDeleteAndReturnTrue()
    {
        // Sắp đặt
        var employee = new Employee
        {
            EmployeeCode = "NV007",
            FullName = "Vũ Thị Bảy",
            DateOfBirth = new DateTime(1993, 7, 7),
            Gender = Gender.Female,
            PhoneNumber = "0977777777",
            IdentityCard = "001093000007",
            Position = "Tạp vụ",
            Department = "Dịch vụ"
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Thực hiện
        var success = await _service.DeleteEmployeeAsync(employee.Id);

        // Kiểm tra
        success.Should().BeTrue();
        var deleted = await _context.Employees.FindAsync(employee.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteEmployeeAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Thực hiện
        var success = await _service.DeleteEmployeeAsync(999);

        // Kiểm tra
        success.Should().BeFalse();
    }
}
