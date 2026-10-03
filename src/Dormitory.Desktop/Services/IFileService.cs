using System.Threading.Tasks;

namespace Dormitory.Desktop.Services;

/// <summary>
/// Giao diện dịch vụ lưu tệp và tương tác với File Dialog của hệ thống
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Hiển thị hộp thoại lưu tệp và ghi dữ liệu nhị phân vào tệp được chọn
    /// </summary>
    /// <param name="defaultFileName">Tên tệp gợi ý ban đầu (chưa gắn timestamp hay extension)</param>
    /// <param name="extension">Phần mở rộng tệp (ví dụ: xlsx)</param>
    /// <param name="fileTypeFilter">Nhãn hiển thị bộ lọc tệp (ví dụ: Excel Files)</param>
    /// <param name="content">Nội dung byte cần lưu</param>
    /// <returns>True nếu lưu tệp thành công, False nếu người dùng hủy thao tác</returns>
    Task<bool> SaveFileAsync(string defaultFileName, string extension, string fileTypeFilter, byte[] content);

    /// <summary>
    /// Hiển thị hộp thoại chọn tệp để mở và đọc dữ liệu nhị phân của tệp được chọn
    /// </summary>
    /// <param name="title">Tiêu đề hộp thoại mở tệp</param>
    /// <param name="extensions">Danh sách phần mở rộng hợp lệ (ví dụ: new[] { "bak", "db" })</param>
    /// <returns>Mảng byte nội dung tệp nếu chọn thành công, null nếu người dùng hủy</returns>
    Task<byte[]?> OpenFileAsync(string title, string[] extensions);
}
