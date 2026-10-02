using Dormitory.Core.Entities;
using Dormitory.Core.Enums;
using Dormitory.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Data;

/// <summary>
/// Khởi tạo và nạp dữ liệu mẫu ban đầu vào cơ sở dữ liệu nếu DB còn trống
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Thực hiện seed dữ liệu khởi tạo
    /// </summary>
    public static async Task SeedAsync(DormitoryDbContext context)
    {
        // Đảm bảo cấu trúc database đã được khởi tạo
        await context.Database.EnsureCreatedAsync();

        // 1. Khởi tạo tài khoản quản trị viên nếu chưa có
        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("Admin@123456"),
                FullName = "Quản Trị Viên Hệ Thống",
                Email = "admin@dormitory.edu.vn",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var managerUser = new User
            {
                Username = "manager",
                PasswordHash = PasswordHasher.Hash("Manager@123"),
                FullName = "Nguyễn Văn Quản Lý",
                Email = "manager@dormitory.edu.vn",
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(adminUser, managerUser);
            await context.SaveChangesAsync();

            // Tạo thông tin nhân viên liên kết
            var employee = new Employee
            {
                EmployeeCode = "NV001",
                FullName = "Nguyễn Văn Quản Lý",
                DateOfBirth = new DateTime(1985, 5, 20),
                Gender = Gender.Male,
                PhoneNumber = "0987654321",
                IdentityCard = "001085001234",
                Position = "Trưởng ban KTX",
                Department = "Ban Quản lý Ký túc xá",
                Address = "Hà Nội",
                UserId = managerUser.Id
            };
            context.Employees.Add(employee);
        }

        // 2. Khởi tạo danh sách phòng mẫu nếu chưa có
        if (!await context.Rooms.AnyAsync())
        {
            var rooms = new List<Room>
            {
                new Room
                {
                    RoomNumber = "A101",
                    Building = "Tòa A (Nam)",
                    Floor = 1,
                    Capacity = 4,
                    CurrentOccupancy = 2,
                    PricePerMonth = 600000,
                    Status = RoomStatus.Available,
                    Type = RoomType.Standard,
                    AllowedGender = Gender.Male,
                    Description = "Phòng tiêu chuẩn 4 giường tầng, có quạt trần, ban công"
                },
                new Room
                {
                    RoomNumber = "A102",
                    Building = "Tòa A (Nam)",
                    Floor = 1,
                    Capacity = 4,
                    CurrentOccupancy = 4,
                    PricePerMonth = 600000,
                    Status = RoomStatus.Occupied,
                    Type = RoomType.Standard,
                    AllowedGender = Gender.Male,
                    Description = "Phòng tiêu chuẩn đã đầy"
                },
                new Room
                {
                    RoomNumber = "A201",
                    Building = "Tòa A (Nam)",
                    Floor = 2,
                    Capacity = 2,
                    CurrentOccupancy = 1,
                    PricePerMonth = 1200000,
                    Status = RoomStatus.Available,
                    Type = RoomType.Premium,
                    AllowedGender = Gender.Male,
                    Description = "Phòng dịch vụ 2 người, có máy lạnh, bình nóng lạnh"
                },
                new Room
                {
                    RoomNumber = "B101",
                    Building = "Tòa B (Nữ)",
                    Floor = 1,
                    Capacity = 4,
                    CurrentOccupancy = 1,
                    PricePerMonth = 650000,
                    Status = RoomStatus.Available,
                    Type = RoomType.Standard,
                    AllowedGender = Gender.Female,
                    Description = "Phòng nữ tiêu chuẩn 4 người"
                },
                new Room
                {
                    RoomNumber = "B102",
                    Building = "Tòa B (Nữ)",
                    Floor = 1,
                    Capacity = 2,
                    CurrentOccupancy = 0,
                    PricePerMonth = 1500000,
                    Status = RoomStatus.Available,
                    Type = RoomType.Vip,
                    AllowedGender = Gender.Female,
                    Description = "Phòng VIP đầy đủ tiện nghi khép kín"
                }
            };
            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();

            // 3. Khởi tạo sinh viên mẫu
            var roomA101 = await context.Rooms.FirstAsync(r => r.RoomNumber == "A101");
            var roomB101 = await context.Rooms.FirstAsync(r => r.RoomNumber == "B101");

            var student1 = new Student
            {
                StudentCode = "SV2021001",
                FullName = "Trần Minh Quân",
                DateOfBirth = new DateTime(2003, 3, 15),
                Gender = Gender.Male,
                IdentityCard = "034203001122",
                PhoneNumber = "0912345678",
                Email = "quan.tm@sinhvien.edu.vn",
                HomeTown = "Hải Phòng",
                ClassName = "CNTT1-K62",
                Faculty = "Công nghệ Thông tin",
                ParentName = "Trần Văn An",
                ParentPhoneNumber = "0903112233",
                CurrentRoomId = roomA101.Id
            };

            var student2 = new Student
            {
                StudentCode = "SV2021002",
                FullName = "Lê Hoàng Nam",
                DateOfBirth = new DateTime(2003, 7, 22),
                Gender = Gender.Male,
                IdentityCard = "034203004455",
                PhoneNumber = "0923456789",
                Email = "nam.lh@sinhvien.edu.vn",
                HomeTown = "Nam Định",
                ClassName = "DTVT2-K62",
                Faculty = "Điện tử Viễn thông",
                ParentName = "Lê Văn Bình",
                ParentPhoneNumber = "0913223344",
                CurrentRoomId = roomA101.Id
            };

            var student3 = new Student
            {
                StudentCode = "SV2021003",
                FullName = "Nguyễn Mai Phương",
                DateOfBirth = new DateTime(2004, 11, 8),
                Gender = Gender.Female,
                IdentityCard = "001204008899",
                PhoneNumber = "0934567890",
                Email = "phuong.nm@sinhvien.edu.vn",
                HomeTown = "Hà Nội",
                ClassName = "KTPM-K63",
                Faculty = "Công nghệ Thông tin",
                ParentName = "Nguyễn Thị Dung",
                ParentPhoneNumber = "0988776655",
                CurrentRoomId = roomB101.Id
            };

            context.Students.AddRange(student1, student2, student3);
            await context.SaveChangesAsync();

            // 4. Khởi tạo hợp đồng mẫu
            var contract1 = new Contract
            {
                ContractNumber = "HD-2024-001",
                StudentId = student1.Id,
                RoomId = roomA101.Id,
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 6, 30),
                DepositAmount = 1000000,
                MonthlyRate = roomA101.PricePerMonth,
                Status = ContractStatus.Active,
                Notes = "Hợp đồng năm học 2024 - 2025"
            };

            var contract2 = new Contract
            {
                ContractNumber = "HD-2024-002",
                StudentId = student3.Id,
                RoomId = roomB101.Id,
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 6, 30),
                DepositAmount = 1000000,
                MonthlyRate = roomB101.PricePerMonth,
                Status = ContractStatus.Active,
                Notes = "Hợp đồng năm học 2024 - 2025"
            };

            context.Contracts.AddRange(contract1, contract2);

            // 5. Khởi tạo hóa đơn mẫu
            var bill1 = new Bill
            {
                BillCode = "HD-202410-A101",
                RoomId = roomA101.Id,
                Month = 10,
                Year = 2024,
                RoomFee = roomA101.PricePerMonth,
                OldElectricIndex = 120,
                NewElectricIndex = 185, // Tiêu thụ 65 kWh
                ElectricRate = 3500,
                OldWaterIndex = 40,
                NewWaterIndex = 52, // Tiêu thụ 12 m3
                WaterRate = 15000,
                OtherServiceFee = 50000, // Vệ sinh, mạng
                Status = BillStatus.Unpaid,
                DueDate = new DateTime(2024, 11, 10),
                CreatedAt = DateTime.UtcNow
            };

            context.Bills.Add(bill1);
            await context.SaveChangesAsync();
        }
    }
}
