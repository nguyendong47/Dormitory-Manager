using System.Threading.Tasks;
using Avalonia.Controls;

namespace Dormitory.Desktop.Services;

/// <summary>
/// Dịch vụ hiển thị các hộp thoại (dialog/modal) trong ứng dụng desktop.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Hiển thị hộp thoại xác nhận với 2 lựa chọn: Đồng ý / Hủy.
    /// </summary>
    /// <param name="title">Tiêu đề hộp thoại</param>
    /// <param name="message">Nội dung thông điệp</param>
    /// <returns>True nếu người dùng chọn Đồng ý, False nếu chọn Hủy hoặc đóng hộp thoại</returns>
    Task<bool> ShowConfirmAsync(string title, string message);

    /// <summary>
    /// Hiển thị hộp thoại thông báo thông tin với nút Đóng.
    /// </summary>
    /// <param name="title">Tiêu đề hộp thoại</param>
    /// <param name="message">Nội dung thông điệp</param>
    Task ShowMessageAsync(string title, string message);

    /// <summary>
    /// Hiển thị một cửa sổ hộp thoại tùy chỉnh dạng modal trên MainWindow.
    /// </summary>
    /// <typeparam name="TResult">Kiểu dữ liệu kết quả trả về từ dialog</typeparam>
    /// <param name="dialog">Đối tượng Window cần hiển thị</param>
    /// <returns>Kết quả trả về từ dialog</returns>
    Task<TResult?> ShowDialogAsync<TResult>(Window dialog);
}
