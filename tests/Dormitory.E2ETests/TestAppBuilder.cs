using Avalonia;
using Avalonia.Headless;
using Dormitory.Desktop;
using Dormitory.E2ETests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Dormitory.E2ETests;

/// <summary>
/// Khởi tạo và cấu hình ứng dụng Avalonia chạy dưới chế độ Headless cho việc chạy kiểm thử tự động E2E
/// </summary>
public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = true
            });
}
