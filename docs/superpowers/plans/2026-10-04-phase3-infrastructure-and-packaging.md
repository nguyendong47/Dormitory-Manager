# Giai Đoạn 3: Tối Ưu Hạ Tầng, Sao Lưu CSDL & Đóng Gói Đa Nền Tảng Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Bổ sung tính năng sao lưu và phục hồi CSDL SQLite an toàn (.bak / .db) với kiểm tra toàn vẹn dữ liệu, tách biệt cấu hình chuỗi kết nối vào `appsettings.json`, và xây dựng bộ script tự động đóng gói ứng dụng đa nền tảng (macOS .app/.dmg, Windows single-file .exe, Linux binary).

**Architecture:** Mở rộng tầng Application với `IDatabaseService` quản trị snapshot CSDL SQLite (`VACUUM INTO` / file copy lock) và phục hồi (`SqliteConnection.ClearAllPools`); tạo màn hình Cài đặt hệ thống (`SystemSettingsView`) trong Avalonia Desktop được bảo vệ bởi RBAC Admin; tích hợp `Microsoft.Extensions.Configuration.Json` đọc cấu hình từ `appsettings.json`; cung cấp bộ script đóng gói self-contained trong `scripts/package/`.

**Tech Stack:** .NET 8 LTS, Avalonia UI 11.2.5, Microsoft.Data.Sqlite, Microsoft.Extensions.Configuration.Json 8.0.1, Bash / PowerShell scripts, xUnit.

**Spec:** `docs/spec-modernization.md`

## Global Constraints

- **Language Conventions**: Tên class, method, variable, interface 100% bằng Tiếng Anh PascalCase/camelCase; Comment trong code và giao diện UI 100% bằng Tiếng Việt chuẩn mực.
- **Naming Conventions**: Prefix project và namespace là `Dormitory.*`.
- **RBAC Security**: Tính năng Sao lưu và Phục hồi CSDL là thao tác nhạy cảm, chỉ tài khoản vai trò `UserRole.Admin` mới có quyền thực hiện.
- **Database Safety**: Khi khôi phục CSDL, phải đóng toàn bộ connection pools (`SqliteConnection.ClearAllPools()`), xác nhận cảnh báo dữ liệu hiện tại bị thay thế qua `IDialogService.ShowConfirmAsync`, và kiểm tra tính toàn vẹn của file backup (`PRAGMA integrity_check`) trước khi áp dụng.
- **Pre-commit Gate**: Luôn chạy `node .gitnexus/run.cjs detect-changes --scope all --repo .` trước khi commit.

## Review Focus

1. **Khóa kết nối SQLite khi Phục hồi (File Lock Conflict)**: Khôi phục file SQLite khi ứng dụng đang mở kết nối có thể gây `IOException: The process cannot access the file`. Phải gọi `SqliteConnection.ClearAllPools()`, dọn dẹp kết nối của DbContext trước khi ghi đè tệp.
2. **File backup không hợp lệ hoặc bị hỏng**: Nếu người dùng chọn một file ngẫu nhiên không phải SQLite DB hoặc file hỏng, quá trình phục hồi phải bắt lỗi, giữ nguyên file DB hiện tại và báo lỗi rõ ràng.
3. **Cấu hình fallback khi thiếu `appsettings.json`**: Nếu chạy ứng dụng ở môi trường thiếu file `appsettings.json`, ứng dụng phải fallback an toàn về giá trị mặc định `"Data Source=dormitory.db"`, không để crash ứng dụng ngay khi khởi động.
4. **Quyền Admin được thực thi ở cả UI và Service**: Người dùng role `Manager` không được phép nhìn thấy hoặc kích hoạt nút Phục hồi CSDL.
5. **Đóng gói Self-Contained chạy độc lập**: Ứng dụng sau khi publish với `PublishSingleFile=true` phải chứa đầy đủ native binaries của SkiaSharp và SQLite, khởi động trơn tru trên máy tính đích mà không cần cài trước .NET Runtime.

---

### Task 1: Database Backup & Restore Infrastructure Service (TDD)

**Files:**
- Create: `src/Dormitory.Application/DTOs/DatabaseDtos.cs`
- Create: `src/Dormitory.Application/Interfaces/IDatabaseService.cs`
- Create: `src/Dormitory.Infrastructure/Services/DatabaseService.cs`
- Create: `tests/Dormitory.UnitTests/Services/DatabaseServiceTests.cs`

**Interfaces:**
- Consumes: `IDormitoryDbContext`, `IConfiguration`
- Produces:
  - `DatabaseInfoDto`: `string DatabasePath`, `long FileSizeBytes`, `string FormattedFileSize`, `int TotalRecords`, `DateTime LastModified`
  - `IDatabaseService`:
    - `Task<DatabaseInfoDto> GetDatabaseInfoAsync();`
    - `Task<byte[]> BackupDatabaseAsync();`
    - `Task<bool> RestoreDatabaseAsync(byte[] backupBytes);`
    - `Task<bool> VerifyDatabaseIntegrityAsync(string dbFilePath);`

- [ ] **Step 1: Write failing tests in `tests/Dormitory.UnitTests/Services/DatabaseServiceTests.cs`**

```csharp
[Fact]
public async Task BackupDatabaseAsync_ShouldReturnNonEmptyByteArray()
{
    // Arrange: test SQLite db with some seed data
    // Act: byte[] backup = await service.BackupDatabaseAsync();
    // Assert: backup != null && backup.Length > 0;
}

[Fact]
public async Task RestoreDatabaseAsync_WithCorruptData_ShouldReturnFalseAndNotCorruptCurrentDb()
{
    // Arrange: invalid byte array
    // Act: bool result = await service.RestoreDatabaseAsync(invalidBytes);
    // Assert: result is false; current database remains intact
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test tests/Dormitory.UnitTests/ --filter FullyQualifiedName~DatabaseServiceTests`
Expected: FAIL (types not defined).

- [ ] **Step 3: Define DTOs in `DatabaseDtos.cs` & Interface in `IDatabaseService.cs`**

- `DatabaseDtos.cs`: `DatabaseInfoDto`, `DatabaseRestoreResultDto`.
- `IDatabaseService.cs`: Hợp đồng sao lưu, phục hồi và kiểm tra toàn vẹn CSDL.

- [ ] **Step 4: Implement `DatabaseService.cs` in Infrastructure**

- Lấy đường dẫn file DB từ ConnectionString SQLite (`Data Source=...`).
- `BackupDatabaseAsync()`: Dùng `VACUUM INTO '{tempFile}'` của SQLite hoặc copy file an toàn sang mảng byte.
- `RestoreDatabaseAsync(byte[] backupBytes)`:
  - Ghi tạm ra file temp, chạy `PRAGMA integrity_check` kiểm tra.
  - Nếu hợp lệ: Gọi `SqliteConnection.ClearAllPools()`, backup file hiện tại thành `.db.old`, copy file temp vào file DB chính.
  - Trả về `true`.
- `GetDatabaseInfoAsync()`: Tính tổng số records (Rooms + Students + Contracts + Bills + Users + Employees) và kích thước file `.db`.

- [ ] **Step 5: Run tests to verify they pass**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Expected: PASS (All tests pass).

- [ ] **Step 6: Commit**

```bash
git add src/ tests/
git commit -m "feat(database): implement database backup, restore, and integrity check service with unit tests"
```

---

### Task 2: System Settings & Database Management UI (Desktop Integration)

**Files:**
- Modify: `src/Dormitory.Desktop/Services/IFileService.cs`
- Modify: `src/Dormitory.Desktop/Services/FileService.cs`
- Create: `src/Dormitory.Desktop/ViewModels/SystemSettingsViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/SystemSettingsView.axaml`
- Create: `src/Dormitory.Desktop/Views/SystemSettingsView.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/MainWindowViewModel.cs`
- Modify: `src/Dormitory.Desktop/MainWindow.axaml`
- Modify: `src/Dormitory.Desktop/App.axaml.cs`

**Interfaces:**
- Consumes: `IDatabaseService`, `IFileService`, `IDialogService`, `IUserSession`
- Produces:
  - `IFileService.OpenFileAsync(string title, string[] extensions)`: `Task<byte[]?>`
  - `SystemSettingsViewModel`: `BackupCommand`, `RestoreCommand`, `RefreshDbInfoCommand`, `IsAdmin`
  - `MainWindow.axaml`: Nút "⚙️ Cài đặt hệ thống" trên Sidebar (ẩn/vô hiệu hóa nếu không phải Admin)

- [ ] **Step 1: Extend `IFileService` with `OpenFileAsync`**

- Thêm `Task<byte[]?> OpenFileAsync(string title, string[] extensions);` vào `IFileService.cs`.
- Triển khai trong `FileService.cs` sử dụng `mainWindow.StorageProvider.OpenFilePickerAsync(...)` đọc nội dung file trả về `byte[]`.

- [ ] **Step 2: Create `SystemSettingsViewModel.cs`**

- Properties: `DatabaseInfo` (`DatabaseInfoDto`), `IsLoading`, `IsAdmin` (`_userSession.IsAdmin`), `StatusMessage`.
- `BackupCommand`: Gọi `_databaseService.BackupDatabaseAsync()`, sau đó gọi `_fileService.SaveFileAsync("dormitory_backup", "bak", "Backup Files (*.bak;*.db)", bytes)`.
- `RestoreCommand`:
  - Kiểm tra `_userSession.IsAdmin`. Nếu không phải: báo lỗi từ chối.
  - Gọi `_fileService.OpenFileAsync("Chọn tệp sao lưu để phục hồi", new[] { "bak", "db" })`.
  - Nếu người dùng chọn file: hiển thị `_dialogService.ShowConfirmAsync("CẢNH BÁO PHỤC HỒI", "Toàn bộ dữ liệu hiện tại sẽ được thay thế bằng tệp sao lưu này. Bạn có chắc chắn muốn khôi phục?")`.
  - Nếu xác nhận: gọi `_databaseService.RestoreDatabaseAsync(bytes)`. Thông báo thành công và yêu cầu làm mới dữ liệu.
- `RefreshDbInfoCommand`: Tải lại `DatabaseInfo`.

- [ ] **Step 3: Create `SystemSettingsView.axaml` and `.axaml.cs`**

- Thiết kế giao diện Fluent:
  - Khối 1: Thông tin CSDL hiện tại (Đường dẫn, Dung lượng, Tổng số bản ghi, Lần sửa đổi cuối).
  - Khối 2: Thẻ Sao lưu dữ liệu (Icon 💾, mô tả, nút "Sao lưu CSDL ngay").
  - Khối 3: Thẻ Phục hồi dữ liệu (Icon ⚠️, cảnh báo an toàn, nút "Phục hồi từ file..." màu cam/đỏ, `IsEnabled="{Binding IsAdmin}"`).

- [ ] **Step 4: Register in `MainWindowViewModel`, `MainWindow.axaml` and `App.axaml.cs`**

- Thêm `NavigateToSettingsCommand` vào `MainWindowViewModel`.
- Thêm nút Sidebar: `<Button Content="⚙️  Cài đặt hệ thống" Command="{Binding NavigateToSettingsCommand}" .../>`.
- Đăng ký DI trong `App.axaml.cs`: `services.AddScoped<IDatabaseService, DatabaseService>(); services.AddTransient<SystemSettingsViewModel>();`.

- [ ] **Step 5: Verify build and compile**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln && dotnet test Dormitory.sln`
Expected: Build 0 errors, 0 warnings; all tests pass.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): add System Settings view with database backup and restore capabilities"
```

---

### Task 3: Dynamic Configuration via `appsettings.json`

**Files:**
- Create: `src/Dormitory.Desktop/appsettings.json`
- Modify: `src/Dormitory.Desktop/Dormitory.Desktop.csproj`
- Modify: `src/Dormitory.Desktop/App.axaml.cs`

**Interfaces:**
- Consumes: `Microsoft.Extensions.Configuration.Json`
- Produces: Cấu hình `IConfiguration` tập trung, đọc chuỗi kết nối động và hỗ trợ chuyển đổi provider

- [ ] **Step 1: Add package `Microsoft.Extensions.Configuration.Json` 8.0.1**

Thêm package vào `src/Dormitory.Desktop/Dormitory.Desktop.csproj`:
`<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.1" />`

- [ ] **Step 2: Create `src/Dormitory.Desktop/appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DormitoryDb": "Data Source=dormitory.db"
  },
  "DatabaseProvider": "Sqlite",
  "AppSettings": {
    "AppName": "Dormitory Manager",
    "Version": "2.0.0",
    "AutoBackupOnExit": false
  }
}
```
Cấu hình trong `.csproj` để file luôn copy sang thư mục Output:
`<None Update="appsettings.json"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>`

- [ ] **Step 3: Update `App.axaml.cs` to load `IConfiguration`**

- Xây dựng `ConfigurationBuilder`:
  ```csharp
  var config = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
      .Build();
  services.AddSingleton<IConfiguration>(config);
  ```
- Đọc chuỗi kết nối:
  `var connectionString = config.GetConnectionString("DormitoryDb") ?? "Data Source=dormitory.db";`
  `services.AddDbContext<DormitoryDbContext>(options => options.UseSqlite(connectionString));`

- [ ] **Step 4: Verify build and test**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln && dotnet test Dormitory.sln`
Expected: PASS 100%.

- [ ] **Step 5: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): configure dynamic database connection string via appsettings.json"
```

---

### Task 4: Cross-Platform Packaging & Publishing Automation

**Files:**
- Create: `scripts/package/build-macos.sh`
- Create: `scripts/package/build-windows.sh`
- Create: `scripts/package/build-linux.sh`
- Create: `docs/packaging-and-deployment.md`

**Interfaces:**
- Consumes: `dotnet publish` CLI với cờ self-contained
- Produces:
  - macOS: `osx-arm64` & `osx-x64` .app bundle và .dmg script
  - Windows: `win-x64` single-file executable .exe
  - Linux: `linux-x64` self-contained binary

- [ ] **Step 1: Create `scripts/package/build-macos.sh`**

Script tự động:
- `dotnet publish src/Dormitory.Desktop -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=false`
- Đóng gói cấu trúc `DormitoryManager.app/Contents/MacOS/` và `Contents/Info.plist`.
- Tạo file DMG nếu có lệnh `hdiutil` trên macOS.

- [ ] **Step 2: Create `scripts/package/build-windows.sh` & `build-linux.sh`**

- `build-windows.sh`:
  `dotnet publish src/Dormitory.Desktop -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o dist/windows-x64`
- `build-linux.sh`:
  `dotnet publish src/Dormitory.Desktop -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o dist/linux-x64`

- [ ] **Step 3: Test local publish on macOS**

Run: `bash scripts/package/build-macos.sh`
Verify: Thư mục `dist/macos/` chứa ứng dụng chạy được độc lập.

- [ ] **Step 4: Create `docs/packaging-and-deployment.md`**

Hướng dẫn chi tiết từng bước đóng gói cho Quản trị viên KTX và lập trình viên.

- [ ] **Step 5: Commit**

```bash
git add scripts/package/ docs/packaging-and-deployment.md
git commit -m "feat(packaging): add cross-platform automated publish scripts for macOS, Windows, and Linux"
```

---

### Task 5: Full Verification, Documentation & GitNexus Sync

**Files:**
- Modify: `docs/user-guide.md`
- Modify: `README.md`
- Graph: `.gitnexus/`

**Interfaces:**
- Consumes: toàn bộ kết quả Tasks 1-4
- Produces:
  - 100% test pass
  - Đồng bộ knowledge graph GitNexus
  - Cập nhật tài liệu người dùng và README

- [ ] **Step 1: Run full test suite and build**

Run:
```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet test Dormitory.sln
dotnet build Dormitory.sln
```
Expected: 100% tests pass (35+ tests), 0 build errors.

- [ ] **Step 2: Detect changes and re-index GitNexus knowledge graph**

Run:
```bash
node .gitnexus/run.cjs detect-changes --scope all --repo .
node .gitnexus/run.cjs analyze --index-only
```
Expected: Index cập nhật thành công, sạch sẽ không lỗi.

- [ ] **Step 3: Update documentation in `docs/user-guide.md` & `README.md`**

- `docs/user-guide.md`: Bổ sung mục "Cài đặt hệ thống & Quản trị CSDL" hướng dẫn sao lưu, phục hồi dữ liệu và cấu hình `appsettings.json`.
- `README.md`: Đánh dấu hoàn thành toàn bộ 3 Giai đoạn của dự án Modernization, cập nhật tổng số test.

- [ ] **Step 4: Commit**

```bash
git add docs/ README.md .gitnexus/
git commit -m "docs: finalize user guide and README for Phase 3 infrastructure and packaging"
```
