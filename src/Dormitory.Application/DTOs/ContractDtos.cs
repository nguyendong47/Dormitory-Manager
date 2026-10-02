using Dormitory.Core.Enums;

namespace Dormitory.Application.DTOs;

/// <summary>
/// DTO thông tin hợp đồng thuê phòng KTX
/// </summary>
public class ContractDto
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal MonthlyRate { get; set; }
    public ContractStatus Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO yêu cầu lập hợp đồng thuê phòng mới
/// </summary>
public class CreateContractRequest
{
    public int StudentId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal MonthlyRate { get; set; }
    public string? Notes { get; set; }
}
