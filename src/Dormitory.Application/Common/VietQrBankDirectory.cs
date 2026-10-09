using Dormitory.Application.DTOs;

namespace Dormitory.Application.Common;

/// <summary>
/// Danh mục ngân hàng tĩnh chuẩn hóa hỗ trợ thanh toán VietQR / Napas 247 tại Việt Nam.
/// Chứa thông tin 40 ngân hàng thương mại và liên doanh với mã BIN định danh tiêu chuẩn.
/// </summary>
public static class VietQrBankDirectory
{
    private static readonly List<BankInfoDto> SupportedBanks = new()
    {
        new BankInfoDto { Bin = "970436", ShortName = "Vietcombank", Name = "Ngân hàng TMCP Ngoại thương Việt Nam", Code = "VCB" },
        new BankInfoDto { Bin = "970415", ShortName = "VietinBank", Name = "Ngân hàng TMCP Công thương Việt Nam", Code = "CTG" },
        new BankInfoDto { Bin = "970418", ShortName = "BIDV", Name = "Ngân hàng TMCP Đầu tư và Phát triển Việt Nam", Code = "BIDV" },
        new BankInfoDto { Bin = "970405", ShortName = "Agribank", Name = "Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam", Code = "VBA" },
        new BankInfoDto { Bin = "970422", ShortName = "MBBank", Name = "Ngân hàng TMCP Quân đội", Code = "MB" },
        new BankInfoDto { Bin = "970407", ShortName = "Techcombank", Name = "Ngân hàng TMCP Kỹ thương Việt Nam", Code = "TCB" },
        new BankInfoDto { Bin = "970432", ShortName = "VPBank", Name = "Ngân hàng TMCP Việt Nam Thịnh Vượng", Code = "VPB" },
        new BankInfoDto { Bin = "970416", ShortName = "ACB", Name = "Ngân hàng TMCP Á Châu", Code = "ACB" },
        new BankInfoDto { Bin = "970423", ShortName = "TPBank", Name = "Ngân hàng TMCP Tiên Phong", Code = "TPB" },
        new BankInfoDto { Bin = "970403", ShortName = "Sacombank", Name = "Ngân hàng TMCP Sài Gòn Thương Tín", Code = "STB" },
        new BankInfoDto { Bin = "970437", ShortName = "HDBank", Name = "Ngân hàng TMCP Phát triển TP.HCM", Code = "HDB" },
        new BankInfoDto { Bin = "970441", ShortName = "VIB", Name = "Ngân hàng TMCP Quốc tế Việt Nam", Code = "VIB" },
        new BankInfoDto { Bin = "970443", ShortName = "SHB", Name = "Ngân hàng TMCP Sài Gòn - Hà Nội", Code = "SHB" },
        new BankInfoDto { Bin = "970426", ShortName = "MSB", Name = "Ngân hàng TMCP Hàng hải Việt Nam", Code = "MSB" },
        new BankInfoDto { Bin = "970448", ShortName = "OCB", Name = "Ngân hàng TMCP Phương Đông", Code = "OCB" },
        new BankInfoDto { Bin = "970449", ShortName = "LPBank", Name = "Ngân hàng TMCP Lộc Phát Việt Nam", Code = "LPB" },
        new BankInfoDto { Bin = "970440", ShortName = "SeABank", Name = "Ngân hàng TMCP Đông Nam Á", Code = "SSB" },
        new BankInfoDto { Bin = "970428", ShortName = "NamABank", Name = "Ngân hàng TMCP Nam Á", Code = "NAB" },
        new BankInfoDto { Bin = "970409", ShortName = "BacABank", Name = "Ngân hàng TMCP Bắc Á", Code = "BAB" },
        new BankInfoDto { Bin = "970452", ShortName = "Kienlongbank", Name = "Ngân hàng TMCP Kiên Long", Code = "KLB" },
        new BankInfoDto { Bin = "970438", ShortName = "BaoVietBank", Name = "Ngân hàng TMCP Bảo Việt", Code = "BVB" },
        new BankInfoDto { Bin = "970412", ShortName = "PVcomBank", Name = "Ngân hàng TMCP Đại Chúng Việt Nam", Code = "PVCB" },
        new BankInfoDto { Bin = "970433", ShortName = "VietBank", Name = "Ngân hàng TMCP Việt Nam Thương Tín", Code = "VBB" },
        new BankInfoDto { Bin = "970400", ShortName = "Saigonbank", Name = "Ngân hàng TMCP Sài Gòn Công thương", Code = "SGICB" },
        new BankInfoDto { Bin = "970431", ShortName = "Eximbank", Name = "Ngân hàng TMCP Xuất Nhập khẩu Việt Nam", Code = "EIB" },
        new BankInfoDto { Bin = "970430", ShortName = "PGBank", Name = "Ngân hàng TMCP Thịnh vượng và Phát triển", Code = "PGB" },
        new BankInfoDto { Bin = "970454", ShortName = "BVBank", Name = "Ngân hàng TMCP Bản Việt", Code = "BVBANK" },
        new BankInfoDto { Bin = "970419", ShortName = "NCB", Name = "Ngân hàng TMCP Quốc Dân", Code = "NCB" },
        new BankInfoDto { Bin = "970429", ShortName = "SCB", Name = "Ngân hàng TMCP Sài Gòn", Code = "SCB" },
        new BankInfoDto { Bin = "970414", ShortName = "OceanBank", Name = "Ngân hàng Thương mại TNHH MTV Đại Dương", Code = "OCEANBANK" },
        new BankInfoDto { Bin = "970408", ShortName = "GPBank", Name = "Ngân hàng Thương mại TNHH MTV Dầu Khí Toàn Cầu", Code = "GPB" },
        new BankInfoDto { Bin = "970424", ShortName = "ShinhanBank", Name = "Ngân hàng TNHH MTV Shinhan Việt Nam", Code = "SHBVN" },
        new BankInfoDto { Bin = "970457", ShortName = "WooriBank", Name = "Ngân hàng TNHH MTV Woori Việt Nam", Code = "WOO" },
        new BankInfoDto { Bin = "970458", ShortName = "UOB", Name = "Ngân hàng United Overseas Bank Việt Nam", Code = "UOB" },
        new BankInfoDto { Bin = "970439", ShortName = "PublicBank", Name = "Ngân hàng TNHH MTV Public Việt Nam", Code = "PBVN" },
        new BankInfoDto { Bin = "970442", ShortName = "HongLeongBank", Name = "Ngân hàng TNHH MTV Hong Leong Việt Nam", Code = "HLBVN" },
        new BankInfoDto { Bin = "970434", ShortName = "IVB", Name = "Ngân hàng TNHH Indovina", Code = "IVB" },
        new BankInfoDto { Bin = "970421", ShortName = "VRB", Name = "Ngân hàng Liên doanh Việt - Nga", Code = "VRB" },
        new BankInfoDto { Bin = "970446", ShortName = "CIMB", Name = "Ngân hàng TNHH MTV CIMB Việt Nam", Code = "CIMB" },
        new BankInfoDto { Bin = "970410", ShortName = "StandardChartered", Name = "Ngân hàng TNHH MTV Standard Chartered Việt Nam", Code = "SCVN" }
    };

    /// <summary>
    /// Lấy toàn bộ danh sách các ngân hàng hỗ trợ chuẩn Napas / VietQR
    /// </summary>
    public static IReadOnlyList<BankInfoDto> GetSupportedBanks()
    {
        return SupportedBanks.AsReadOnly();
    }

    /// <summary>
    /// Lấy toàn bộ danh sách các ngân hàng hỗ trợ chuẩn Napas / VietQR (alias cho GetSupportedBanks)
    /// </summary>
    public static IReadOnlyList<BankInfoDto> GetAllBanks() => GetSupportedBanks();

    /// <summary>
    /// Tìm kiếm thông tin ngân hàng theo mã BIN 6 chữ số (ví dụ: "970436")
    /// </summary>
    /// <param name="bin">Mã BIN ngân hàng cần tra cứu</param>
    /// <returns>BankInfoDto nếu tìm thấy; ngược lại trả về null</returns>
    public static BankInfoDto? FindByBin(string? bin)
    {
        if (string.IsNullOrWhiteSpace(bin))
            return null;

        var cleanBin = bin.Trim();
        return SupportedBanks.FirstOrDefault(b => string.Equals(b.Bin, cleanBin, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Tìm kiếm thông tin ngân hàng theo mã Code hoặc tên viết tắt ShortName (không phân biệt hoa thường)
    /// </summary>
    /// <param name="code">Mã ngân hàng (ví dụ: "VCB", "MB", "TCB", "Vietcombank")</param>
    /// <returns>BankInfoDto nếu tìm thấy; ngược lại trả về null</returns>
    public static BankInfoDto? FindByCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var cleanCode = code.Trim();
        return SupportedBanks.FirstOrDefault(b =>
            string.Equals(b.Code, cleanCode, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(b.ShortName, cleanCode, StringComparison.OrdinalIgnoreCase));
    }
}
