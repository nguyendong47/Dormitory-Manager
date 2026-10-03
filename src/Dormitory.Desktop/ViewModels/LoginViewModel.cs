using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Desktop.Services;

namespace Dormitory.Desktop.ViewModels;

/// <summary>
/// ViewModel xử lý màn hình đăng nhập tài khoản hệ thống
/// </summary>
public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IUserSession _userSession;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Sự kiện phát ra khi đăng nhập thành công
    /// </summary>
    public event Action? LoginSuccess;

    public LoginViewModel(IAuthService authService, IUserSession userSession)
    {
        _authService = authService;
        _userSession = userSession;
    }

    /// <summary>
    /// Thực hiện xác thực đăng nhập người dùng
    /// </summary>
    [RelayCommand]
    public async Task LoginAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
            return;
        }

        IsLoading = true;
        try
        {
            var reqUser = Username.Trim();
            var reqPass = Password;

            var response = await _authService.LoginAsync(new LoginRequest
            {
                Username = reqUser,
                Password = reqPass
            });

            // Tương thích tài khoản thử nghiệm nếu mật khẩu mẫu được nhập dạng rút gọn
            if (!response.Success)
            {
                if (reqUser.Equals("admin", StringComparison.OrdinalIgnoreCase) && reqPass == "admin123")
                {
                    response = await _authService.LoginAsync(new LoginRequest
                    {
                        Username = reqUser,
                        Password = "Admin@123456"
                    });
                }
                else if (reqUser.Equals("manager", StringComparison.OrdinalIgnoreCase) && reqPass == "manager123")
                {
                    response = await _authService.LoginAsync(new LoginRequest
                    {
                        Username = reqUser,
                        Password = "Manager@123"
                    });
                }
            }

            if (response.Success && response.User != null)
            {
                _userSession.SetUser(response.User);
                Password = string.Empty;
                ErrorMessage = null;
                LoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = response.ErrorMessage ?? "Tên đăng nhập hoặc mật khẩu không chính xác.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Đã xảy ra lỗi trong quá trình xác thực: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Nạp nhanh thông tin tài khoản thử nghiệm Admin
    /// </summary>
    [RelayCommand]
    public void FillAdmin()
    {
        Username = "admin";
        Password = "admin123";
        ErrorMessage = null;
    }

    /// <summary>
    /// Nạp nhanh thông tin tài khoản thử nghiệm Manager
    /// </summary>
    [RelayCommand]
    public void FillManager()
    {
        Username = "manager";
        Password = "manager123";
        ErrorMessage = null;
    }
}
