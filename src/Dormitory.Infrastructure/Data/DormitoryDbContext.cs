using Dormitory.Application.Interfaces;
using Dormitory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext quản trị toàn bộ dữ liệu Ký túc xá
/// </summary>
public class DormitoryDbContext : DbContext, IDormitoryDbContext
{
    public DormitoryDbContext()
    {
    }

    public DormitoryDbContext(DbContextOptions<DormitoryDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Bảng quản lý phòng ký túc xá
    /// </summary>
    public DbSet<Room> Rooms => Set<Room>();

    /// <summary>
    /// Bảng hồ sơ sinh viên
    /// </summary>
    public DbSet<Student> Students => Set<Student>();

    /// <summary>
    /// Bảng hợp đồng thuê phòng
    /// </summary>
    public DbSet<Contract> Contracts => Set<Contract>();

    /// <summary>
    /// Bảng hóa đơn điện nước và dịch vụ hàng tháng
    /// </summary>
    public DbSet<Bill> Bills => Set<Bill>();

    /// <summary>
    /// Bảng thông tin nhân viên
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>
    /// Bảng tài khoản người dùng đăng nhập hệ thống
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Bảng quản lý trang thiết bị, tài sản phòng ký túc xá
    /// </summary>
    public DbSet<Equipment> Equipments => Set<Equipment>();

    /// <summary>
    /// Bảng quản lý biên bản vi phạm nội quy và kỷ luật KTX
    /// </summary>
    public DbSet<Violation> Violations => Set<Violation>();

    /// <summary>
    /// Bảng lịch sử các lượt xuất báo cáo nghiệp vụ
    /// </summary>
    public DbSet<ReportHistory> ReportHistories => Set<ReportHistory>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Mặc định sử dụng SQLite database với đường dẫn an toàn qua DatabasePathResolver
            var connectionString = DatabasePathResolver.BuildConnectionString("Data Source=dormitory.db");
            optionsBuilder.UseSqlite(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng Room
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
            entity.Property(r => r.Building).IsRequired().HasMaxLength(50);
            entity.Property(r => r.PricePerMonth).HasPrecision(18, 2);
            entity.HasIndex(r => new { r.Building, r.RoomNumber }).IsUnique();
        });

        // Cấu hình bảng Student
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.StudentCode).IsRequired().HasMaxLength(20);
            entity.Property(s => s.FullName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.IdentityCard).IsRequired().HasMaxLength(20);
            entity.Property(s => s.PhoneNumber).HasMaxLength(15);
            entity.Property(s => s.ClassName).HasMaxLength(50);
            entity.Property(s => s.Faculty).HasMaxLength(100);
            entity.HasIndex(s => s.StudentCode).IsUnique();
            entity.HasIndex(s => s.IdentityCard).IsUnique();

            // Quan hệ với phòng hiện tại
            entity.HasOne(s => s.CurrentRoom)
                .WithMany()
                .HasForeignKey(s => s.CurrentRoomId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Cấu hình bảng Contract
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ContractNumber).IsRequired().HasMaxLength(50);
            entity.Property(c => c.DepositAmount).HasPrecision(18, 2);
            entity.Property(c => c.MonthlyRate).HasPrecision(18, 2);
            entity.HasIndex(c => c.ContractNumber).IsUnique();

            entity.HasOne(c => c.Student)
                .WithMany(s => s.Contracts)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Room)
                .WithMany(r => r.Contracts)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình bảng Bill
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.BillCode).IsRequired().HasMaxLength(50);
            entity.Property(b => b.RoomFee).HasPrecision(18, 2);
            entity.Property(b => b.OldElectricIndex).HasPrecision(18, 2);
            entity.Property(b => b.NewElectricIndex).HasPrecision(18, 2);
            entity.Property(b => b.ElectricRate).HasPrecision(18, 2);
            entity.Property(b => b.OldWaterIndex).HasPrecision(18, 2);
            entity.Property(b => b.NewWaterIndex).HasPrecision(18, 2);
            entity.Property(b => b.WaterRate).HasPrecision(18, 2);
            entity.Property(b => b.OtherServiceFee).HasPrecision(18, 2);
            entity.Property(b => b.Note).HasMaxLength(500);
            entity.HasIndex(b => b.BillCode).IsUnique();

            entity.HasOne(b => b.Room)
                .WithMany(r => r.Bills)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình bảng Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IdentityCard).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.EmployeeCode).IsUnique();

            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Cấu hình bảng User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
        });

        // Cấu hình bảng Equipment
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EquipmentCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Equipments)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cấu hình bảng Violation
        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.ViolationCode).IsRequired().HasMaxLength(50);
            entity.Property(v => v.Title).IsRequired().HasMaxLength(200);
            entity.Property(v => v.FineAmount).HasPrecision(18, 2);
            entity.Property(v => v.RecordedBy).HasMaxLength(100);
            entity.Property(v => v.ResolutionNotes).HasMaxLength(500);

            entity.HasOne(v => v.Student)
                .WithMany()
                .HasForeignKey(v => v.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.Room)
                .WithMany()
                .HasForeignKey(v => v.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình bảng ReportHistory
        modelBuilder.Entity<ReportHistory>(entity =>
        {
            entity.HasKey(rh => rh.Id);
            entity.Property(rh => rh.Title).IsRequired().HasMaxLength(200);
            entity.Property(rh => rh.FileName).HasMaxLength(260);
            entity.Property(rh => rh.FilePath).HasMaxLength(500);
            entity.Property(rh => rh.ErrorMessage).HasMaxLength(1000);
            entity.Property(rh => rh.GeneratedBy).HasMaxLength(100);

            entity.HasIndex(rh => rh.GeneratedAt);
            entity.HasIndex(rh => rh.ReportType);
            entity.HasIndex(rh => new { rh.ReportType, rh.GeneratedAt });
        });
    }
}
