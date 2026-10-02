using Dormitory.Application.DTOs;
using Dormitory.Core.Enums;

namespace Dormitory.Application.Interfaces;

/// <summary>
/// Giao diện dịch vụ quản lý hợp đồng thuê phòng KTX
/// </summary>
public interface IContractService
{
    Task<List<ContractDto>> GetAllContractsAsync(ContractStatus? status = null, int? studentId = null, int? roomId = null);
    Task<ContractDto?> GetContractByIdAsync(int id);
    Task<ContractDto> CreateContractAsync(CreateContractRequest request);
    Task<bool> TerminateContractAsync(int id, string? reason = null);
    Task<bool> RenewContractAsync(int id, DateTime newEndDate);
}
