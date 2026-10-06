using Dormitory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện DbContext cho tầng Application truy cập dữ liệu
/// </summary>
public interface IDormitoryDbContext
{
    DbSet<Room> Rooms { get; }
    DbSet<Student> Students { get; }
    DbSet<Contract> Contracts { get; }
    DbSet<Bill> Bills { get; }
    DbSet<Employee> Employees { get; }
    DbSet<User> Users { get; }
    DbSet<Equipment> Equipments { get; }
    DbSet<Violation> Violations { get; }
    DbSet<ReportHistory> ReportHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
