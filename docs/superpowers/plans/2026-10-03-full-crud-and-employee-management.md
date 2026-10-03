# Full CRUD Dialogs & Employee Management Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Triển khai toàn bộ các hộp thoại CRUD (Thêm/Sửa/Xóa) cho các phân hệ Phòng ở, Sinh viên, Hợp đồng, Hóa đơn và xây dựng phân hệ Quản lý Nhân viên hoàn chỉnh trên nền tảng .NET 8 LTS & Avalonia UI 11.

**Architecture:** Áp dụng Clean Architecture (Core -> Application -> Infrastructure -> Desktop) kết hợp MVVM Pattern (CommunityToolkit.Mvvm). Sử dụng `IDialogService` giải lập hoặc kích hoạt Modal Window độc lập trên Avalonia UI 11 với cơ chế validation chặt chẽ và thông báo xác nhận an toàn trước các thao tác xóa/hủy.

**Tech Stack:** C# 12, .NET 8 LTS, Avalonia UI 11.2, CommunityToolkit.Mvvm 8.4, Entity Framework Core 8 (SQLite), xUnit, FluentAssertions.

**Spec:** [docs/spec-modernization.md](file:///Volumes/AI_Models/source/Dormitory-Manager/docs/spec-modernization.md)

## Global Constraints

- **Language Conventions**: Tên class, method, variable, interface 100% bằng Tiếng Anh; Comment trong code và giao diện UI 100% bằng Tiếng Việt.
- **Naming Conventions**: Prefix project và namespace là `Dormitory.*` (không dùng KTX viết tắt tiếng Việt).
- **Binding Rule in Avalonia 11**: Mọi DataGrid column có format ngày tháng, số tiền hoặc tính toán phải luôn sử dụng `Binding="{Binding PropertyName, Mode=OneWay}"` để tránh exception loop do Avalonia cố parse chuỗi format ngược vào model.
- **Git Commit Gate**: Phải chạy `gitnexus detect-changes --scope all --repo .` trước khi commit.

## Review Focus

1. **Duplicate Identifier Collision**: Nhập trùng Mã nhân viên (`EmployeeCode`), Mã sinh viên (`StudentCode`) hoặc Số phòng trong cùng tòa nhà (`Building` + `RoomNumber`). -> Bắt lỗi từ tầng Application / Service và hiển thị thông báo lỗi rõ ràng trên Dialog, không để crash ứng dụng.
2. **Delete with Active Dependencies**: Xóa phòng đang có sinh viên cư trú/hợp đồng còn hiệu lực, hoặc xóa sinh viên đang có hợp đồng. -> Kiểm tra điều kiện tiên quyết, từ chối xóa và hiển thị thông báo nghiệp vụ kèm hướng dẫn.
3. **Bill Meter Reading Inversion**: Nhập chỉ số điện/nước mới nhỏ hơn chỉ số cũ (`NewElectricIndex < OldElectricIndex` hoặc `NewWaterIndex < OldWaterIndex`). -> Validation trên Form Dialog chặn nút Lưu và báo đỏ ô nhập liệu.
4. **Contract Capacity Overflow & Date Invalidation**: Tạo hợp đồng vào phòng đã đầy (`CurrentOccupancy >= Capacity`) hoặc ngày kết thúc trước ngày bắt đầu (`EndDate <= StartDate`). -> Kiểm tra sức chứa phòng và thứ tự thời gian trước khi cho phép tạo.
5. **Avalonia TwoWay DataGrid Binding Recursion**: Đảm bảo tất cả cột dữ liệu hiển thị định dạng trên DataGrid dùng `Mode=OneWay`.

---

### Task 1: Employee Application Layer (DTOs, Interface, Service & Unit Tests)

**Files:**
- Create: `src/Dormitory.Application/DTOs/EmployeeDtos.cs`
- Create: `src/Dormitory.Application/Interfaces/IEmployeeService.cs`
- Create: `src/Dormitory.Application/Services/EmployeeService.cs`
- Test: `tests/Dormitory.UnitTests/Services/EmployeeServiceTests.cs`

**Interfaces:**
- Consumes: `IDormitoryDbContext`, `Dormitory.Core.Entities.Employee`
- Produces: `IEmployeeService`:
  - `Task<List<EmployeeDto>> GetAllEmployeesAsync(string? searchQuery = null, string? department = null);`
  - `Task<EmployeeDto?> GetEmployeeByIdAsync(int id);`
  - `Task<EmployeeDto> CreateEmployeeAsync(CreateOrUpdateEmployeeRequest request);`
  - `Task<bool> UpdateEmployeeAsync(int id, CreateOrUpdateEmployeeRequest request);`
  - `Task<bool> DeleteEmployeeAsync(int id);`

- [ ] **Step 1: Write the failing unit tests for EmployeeService**

Tạo file `tests/Dormitory.UnitTests/Services/EmployeeServiceTests.cs` kiểm tra:
1. `CreateEmployeeAsync` tạo mới nhân viên thành công và lưu vào database.
2. `GetAllEmployeesAsync` tìm kiếm lọc theo họ tên / mã nhân viên.
3. `UpdateEmployeeAsync` cập nhật thông tin thành công.
4. `DeleteEmployeeAsync` xóa nhân viên thành công.

- [ ] **Step 2: Run test to verify it fails**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln --filter EmployeeServiceTests`
Expected: FAIL do các interface và class chưa được định nghĩa.

- [ ] **Step 3: Implement `EmployeeDtos.cs`, `IEmployeeService.cs`, and `EmployeeService.cs`**

Triển khai DTOs (`EmployeeDto`, `CreateOrUpdateEmployeeRequest`), interface `IEmployeeService`, và class `EmployeeService` tương tác với `IDormitoryDbContext.Employees`.

- [ ] **Step 4: Run test to verify it passes**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln --filter EmployeeServiceTests`
Expected: PASS toàn bộ 4/4 tests.

- [ ] **Step 5: Commit**

```bash
git add src/Dormitory.Application/ tests/Dormitory.UnitTests/
git commit -m "feat(application): add EmployeeService, DTOs, interface and unit tests"
```

---

### Task 2: Desktop Dialog Service & Confirmation Windows

**Files:**
- Create: `src/Dormitory.Desktop/Services/IDialogService.cs`
- Create: `src/Dormitory.Desktop/Services/DialogService.cs`
- Create: `src/Dormitory.Desktop/Views/ConfirmDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/ConfirmDialogWindow.axaml.cs`
- Create: `src/Dormitory.Desktop/Views/MessageDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/MessageDialogWindow.axaml.cs`

**Interfaces:**
- Consumes: Avalonia `Window`, `IClassicDesktopStyleApplicationLifetime`
- Produces: `IDialogService`:
  - `Task<bool> ShowConfirmAsync(string title, string message);`
  - `Task ShowMessageAsync(string title, string message);`
  - `Task<TResult?> ShowDialogAsync<TResult>(Window dialog);`

- [ ] **Step 1: Write `IDialogService.cs` and `DialogService.cs`**

Triển khai service điều phối hiển thị modal dialog trên cửa sổ chính của Avalonia (`desktop.MainWindow`).

- [ ] **Step 2: Create `ConfirmDialogWindow.axaml` and `MessageDialogWindow.axaml`**

Thiết kế giao diện hộp thoại popup chuẩn Fluent Design: Tiêu đề, icon cảnh báo, nội dung thông báo, 2 nút "Xác nhận / Đồng ý" (nền xanh) và "Hủy bỏ" (nền xám).

- [ ] **Step 3: Verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build src/Dormitory.Desktop/Dormitory.Desktop.csproj`
Expected: Build succeeded with 0 errors.

- [ ] **Step 4: Commit**

```bash
git add src/Dormitory.Desktop/Services/ src/Dormitory.Desktop/Views/Confirm* src/Dormitory.Desktop/Views/Message*
git commit -m "feat(desktop): add IDialogService and confirmation/message modal windows"
```

---

### Task 3: Room CRUD Dialog & View Integration

**Files:**
- Create: `src/Dormitory.Desktop/ViewModels/RoomDialogViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/RoomDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/RoomDialogWindow.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/RoomListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/RoomListView.axaml`

**Interfaces:**
- Consumes: `IRoomService`, `IDialogService`, `RoomDto`, `CreateOrUpdateRoomRequest`
- Produces: UI thêm, sửa, xóa phòng ở ký túc xá trực tiếp trên ứng dụng.

- [ ] **Step 1: Create `RoomDialogViewModel.cs`**

Các thuộc tính: `RoomNumber`, `Building`, `Floor`, `Capacity`, `PricePerMonth`, `SelectedType`, `SelectedStatus`, `Description`, `ErrorMessage`.
Phương thức: `SaveAsync()` - kiểm tra validation dữ liệu (số phòng và tòa nhà không rỗng, giá > 0), gọi `_roomService.CreateRoomAsync` hoặc `UpdateRoomAsync`, phát sự kiện đóng dialog thành công.

- [ ] **Step 2: Create `RoomDialogWindow.axaml` and `RoomDialogWindow.axaml.cs`**

Giao diện form nhập liệu 2 cột:
- Tòa nhà (TextBox), Số phòng (TextBox), Tầng (NumericUpDown)
- Loại phòng (ComboBox: Standard, Premium, VIP), Sức chứa tối đa (NumericUpDown)
- Đơn giá thuê tháng (TextBox/NumericUpDown), Trạng thái (ComboBox: Available, Occupied, Maintenance)
- Mô tả / Ghi chú (TextBox đa dòng)
- Nút "💾 Lưu thông tin" và "❌ Hủy bỏ".

- [ ] **Step 3: Update `RoomListViewModel.cs`**

Bổ sung:
- `AddRoomCommand`: Mở `RoomDialogWindow` chế độ thêm mới, tải lại danh sách khi lưu thành công.
- `EditRoomCommand`: Mở `RoomDialogWindow` truyền vào `SelectedRoom`, cập nhật dữ liệu.
- `DeleteRoomCommand`: Gọi `_dialogService.ShowConfirmAsync` xác nhận; nếu đồng ý gọi `_roomService.DeleteRoomAsync(SelectedRoom.Id)` và thông báo kết quả.

- [ ] **Step 4: Update `RoomListView.axaml`**

Bổ sung thanh toolbar chứa các nút thao tác:
- "➕ Thêm phòng mới" (Primary button)
- "✏️ Chỉnh sửa" (Enabled khi `SelectedRoom != null`)
- "🗑️ Xóa phòng" (Enabled khi `SelectedRoom != null`)
- Double click vào dòng trong DataGrid kích hoạt `EditRoomCommand`.

- [ ] **Step 5: Verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build src/Dormitory.Desktop/Dormitory.Desktop.csproj`
Expected: Build succeeded with 0 errors.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/ViewModels/Room* src/Dormitory.Desktop/Views/Room*
git commit -m "feat(desktop): implement full Room CRUD dialog and view integration"
```

---

### Task 4: Student CRUD Dialog & View Integration

**Files:**
- Create: `src/Dormitory.Desktop/ViewModels/StudentDialogViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/StudentDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/StudentDialogWindow.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/StudentListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/StudentListView.axaml`

**Interfaces:**
- Consumes: `IStudentService`, `IRoomService`, `IDialogService`, `StudentDto`, `CreateOrUpdateStudentRequest`
- Produces: UI thêm, sửa, xóa hồ sơ sinh viên kèm gán phòng ở.

- [ ] **Step 1: Create `StudentDialogViewModel.cs`**

Thuộc tính: `StudentCode`, `FullName`, `DateOfBirth`, `SelectedGender`, `PhoneNumber`, `Email`, `IdentityCard`, `ClassName`, `Faculty`, `Hometown`, `SelectedRoomId`, `AvailableRooms`, `ErrorMessage`.
Phương thức: `SaveAsync()` - kiểm tra CCCD, mã sinh viên, họ tên hợp lệ, gọi `_studentService.CreateStudentAsync` hoặc `UpdateStudentAsync`.

- [ ] **Step 2: Create `StudentDialogWindow.axaml` and `StudentDialogWindow.axaml.cs`**

Giao diện form chuẩn Fluent Design:
- Mã SV, Họ và tên
- Ngày sinh (DatePicker), Giới tính (ComboBox: Nam/Nữ)
- Số điện thoại, Email, Số CCCD/CMND
- Lớp sinh hoạt, Khoa chuyên ngành, Quê quán
- Nút "💾 Lưu sinh viên" và "❌ Hủy".

- [ ] **Step 3: Update `StudentListViewModel.cs`**

Bổ sung:
- `AddStudentCommand`: Mở `StudentDialogWindow` thêm mới sinh viên.
- `EditStudentCommand`: Mở `StudentDialogWindow` chỉnh sửa sinh viên đang chọn.
- `DeleteStudentCommand`: Hộp thoại xác nhận -> gọi `_studentService.DeleteStudentAsync`.

- [ ] **Step 4: Update `StudentListView.axaml`**

Bổ sung toolbar thao tác:
- "➕ Thêm sinh viên"
- "✏️ Sửa thông tin" (Enabled khi `SelectedStudent != null`)
- "🗑️ Xóa hồ sơ" (Enabled khi `SelectedStudent != null`)
- Double click dòng kích hoạt sửa.

- [ ] **Step 5: Verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build src/Dormitory.Desktop/Dormitory.Desktop.csproj`
Expected: Build succeeded with 0 errors.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/ViewModels/Student* src/Dormitory.Desktop/Views/Student*
git commit -m "feat(desktop): implement full Student CRUD dialog and view integration"
```

---

### Task 5: Contract Creation & Renewal Dialogs

**Files:**
- Create: `src/Dormitory.Desktop/ViewModels/ContractDialogViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/ContractDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/ContractDialogWindow.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/ContractListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/ContractListView.axaml`

**Interfaces:**
- Consumes: `IContractService`, `IStudentService`, `IRoomService`, `IDialogService`, `ContractDto`, `CreateContractRequest`
- Produces: UI tạo hợp đồng thuê phòng, gia hạn hợp đồng, và chấm dứt hợp đồng.

- [ ] **Step 1: Create `ContractDialogViewModel.cs`**

Thuộc tính: `Students` (danh sách sinh viên để chọn), `SelectedStudent`, `Rooms` (danh sách phòng còn trống), `SelectedRoom`, `StartDate`, `EndDate`, `MonthlyRate`, `DepositAmount`, `Notes`, `ErrorMessage`.
Logic: Khi chọn phòng -> tự động điền `MonthlyRate` từ giá phòng.
Kiểm tra: `EndDate > StartDate`, phòng còn chỗ trống. Gọi `_contractService.CreateContractAsync`.

- [ ] **Step 2: Create `ContractDialogWindow.axaml` and `ContractDialogWindow.axaml.cs`**

Giao diện form hợp đồng:
- Chọn Sinh viên (ComboBox hiển thị Họ tên + Mã SV)
- Chọn Phòng ở (ComboBox hiển thị Tòa + Số phòng + Giá)
- Ngày bắt đầu thuê, Ngày kết thúc thuê (DatePicker)
- Tiền phòng hàng tháng, Tiền đặt cọc (TextBox)
- Ghi chú điều khoản hợp đồng (TextBox)
- Nút "📝 Ký hợp đồng" và "❌ Hủy".

- [ ] **Step 3: Update `ContractListViewModel.cs`**

Bổ sung:
- `CreateContractCommand`: Mở `ContractDialogWindow` tạo hợp đồng mới.
- `TerminateContractCommand`: Xác nhận chấm dứt hợp đồng -> gọi `_contractService.TerminateContractAsync(SelectedContract.Id)`.
- `RenewContractCommand`: Hiển thị hộp thoại chọn ngày gia hạn mới -> gọi `_contractService.RenewContractAsync`.

- [ ] **Step 4: Update `ContractListView.axaml`**

Bổ sung thanh toolbar:
- "➕ Lập hợp đồng mới"
- "⏹️ Chấm dứt hợp đồng" (Enabled khi `SelectedContract != null && SelectedContract.Status == ContractStatus.Active`)
- "🔄 Gia hạn hợp đồng" (Enabled khi `SelectedContract != null`)

- [ ] **Step 5: Verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build src/Dormitory.Desktop/Dormitory.Desktop.csproj`
Expected: Build succeeded with 0 errors.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/ViewModels/Contract* src/Dormitory.Desktop/Views/Contract*
git commit -m "feat(desktop): implement Contract creation, termination and renewal UI"
```

---

### Task 6: Bill Creation & Meter Calculation Dialog

**Files:**
- Create: `src/Dormitory.Desktop/ViewModels/BillDialogViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/BillDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/BillDialogWindow.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/BillListViewModel.cs`
- Modify: `src/Dormitory.Desktop/Views/BillListView.axaml`

**Interfaces:**
- Consumes: `IBillService`, `IRoomService`, `IDialogService`, `BillDto`, `CreateBillRequest`
- Produces: UI lập hóa đơn điện nước và dịch vụ hàng tháng với tự động tính toán số tiêu thụ và thành tiền.

- [ ] **Step 1: Create `BillDialogViewModel.cs`**

Thuộc tính: `Rooms`, `SelectedRoom`, `Month`, `Year`, `RoomFee`, `OldElectricIndex`, `NewElectricIndex`, `ElectricRate` (mặc định 3,500đ), `OldWaterIndex`, `NewWaterIndex`, `WaterRate` (mặc định 12,000đ), `OtherServiceFee` (mặc định 50,000đ), `CalculatedElectricAmount`, `CalculatedWaterAmount`, `TotalAmount`, `Notes`.
Tự động tính: Khi thay đổi chỉ số điện/nước -> tự động tính lượng tiêu thụ và tổng tiền ngay lập tức trên UI.
Validation: `NewElectricIndex >= OldElectricIndex` và `NewWaterIndex >= OldWaterIndex`.

- [ ] **Step 2: Create `BillDialogWindow.axaml` and `BillDialogWindow.axaml.cs`**

Giao diện chia 3 khối trực quan:
1. Thông tin kỳ hóa đơn (Phòng, Tháng, Năm, Tiền phòng).
2. Chỉ số Điện (Chỉ số cũ, Chỉ số mới, Tiêu thụ, Đơn giá kWh, Thành tiền điện).
3. Chỉ số Nước (Chỉ số cũ, Chỉ số mới, Tiêu thụ, Đơn giá m3, Thành tiền nước, Dịch vụ khác).
4. Tổng thanh toán nổi bật (Card hiển thị Tổng tiền VNĐ to rõ) + Nút "💾 Xuất hóa đơn" và "❌ Hủy".

- [ ] **Step 3: Update `BillListViewModel.cs`**

Bổ sung:
- `CreateBillCommand`: Mở `BillDialogWindow`.
- `MarkPaidCommand`: Xác nhận thanh toán hóa đơn -> gọi `_billService.MarkAsPaidAsync(SelectedBill.Id)`.
- `DeleteBillCommand`: Xác nhận xóa hóa đơn -> gọi `_billService.DeleteBillAsync(SelectedBill.Id)`.

- [ ] **Step 4: Update `BillListView.axaml`**

Bổ sung toolbar thao tác:
- "➕ Lập hóa đơn mới"
- "💵 Đánh dấu đã thanh toán" (Enabled khi `SelectedBill != null && SelectedBill.Status == BillStatus.Unpaid`)
- "🗑️ Xóa hóa đơn" (Enabled khi `SelectedBill != null`)

- [ ] **Step 5: Verify build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build src/Dormitory.Desktop/Dormitory.Desktop.csproj`
Expected: Build succeeded with 0 errors.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/ViewModels/Bill* src/Dormitory.Desktop/Views/Bill*
git commit -m "feat(desktop): implement Bill creation dialog with dynamic meter calculation"
```

---

### Task 7: Employee Module UI & Shell Navigation

**Files:**
- Create: `src/Dormitory.Desktop/ViewModels/EmployeeDialogViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/EmployeeDialogWindow.axaml`
- Create: `src/Dormitory.Desktop/Views/EmployeeDialogWindow.axaml.cs`
- Create: `src/Dormitory.Desktop/ViewModels/EmployeeListViewModel.cs`
- Create: `src/Dormitory.Desktop/Views/EmployeeListView.axaml`
- Create: `src/Dormitory.Desktop/Views/EmployeeListView.axaml.cs`
- Modify: `src/Dormitory.Desktop/ViewModels/MainWindowViewModel.cs`
- Modify: `src/Dormitory.Desktop/MainWindow.axaml`
- Modify: `src/Dormitory.Desktop/App.axaml.cs`

**Interfaces:**
- Consumes: `IEmployeeService`, `IDialogService`, `EmployeeDto`, `CreateOrUpdateEmployeeRequest`
- Produces: Phân hệ Quản lý Nhân viên hoàn chỉnh tích hợp vào sidebar điều hướng chính.

- [ ] **Step 1: Create `EmployeeDialogViewModel.cs` & `EmployeeDialogWindow.axaml`**

Form nhập liệu nhân viên: Mã NV, Họ tên, Ngày sinh, Giới tính, SĐT, CCCD, Chức vụ, Bộ phận, Địa chỉ.

- [ ] **Step 2: Create `EmployeeListViewModel.cs` & `EmployeeListView.axaml`**

Bao gồm:
- Thanh tìm kiếm theo tên / mã nhân viên.
- DataGrid hiển thị danh sách nhân viên: Mã NV, Họ tên, Giới tính, Chức vụ, Phòng ban, Số điện thoại, CCCD.
- Toolbar: "➕ Thêm nhân viên", "✏️ Chỉnh sửa", "🗑️ Xóa nhân viên".
- Double click dòng kích hoạt chỉnh sửa.

- [ ] **Step 3: Update `MainWindowViewModel.cs` & `MainWindow.axaml`**

- Thêm `EmployeeListViewModel` vào constructor `MainWindowViewModel`.
- Thêm `NavigateToEmployeesCommand` và gán `ActiveMenu = "Employees"`.
- Bổ sung DataTemplate cho `EmployeeListViewModel` vào `MainWindow.axaml`.
- Bổ sung nút bấm sidebar `👤  Nhân viên` với icon và styling đồng nhất.

- [ ] **Step 4: Register in `App.axaml.cs`**

Đăng ký `IEmployeeService` -> `EmployeeService`, `IDialogService` -> `DialogService`, và `EmployeeListViewModel` trong `ServiceCollection`.

- [ ] **Step 5: Verify build & run test**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: Build và test hoàn toàn thành công 100%.

- [ ] **Step 6: Commit**

```bash
git add src/Dormitory.Desktop/
git commit -m "feat(desktop): integrate Employee management module and sidebar navigation"
```

---

### Task 8: Full Regression Testing & GitNexus Sync

**Files:**
- Modify: `docs/user-guide.md`
- Modify: `README.md`

- [ ] **Step 1: Run all unit tests**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet test Dormitory.sln`
Expected: Tất cả unit tests (bao gồm các test mới cho EmployeeService và các test cũ) đều PASS.

- [ ] **Step 2: Run full build**

Run: `export PATH="$HOME/.dotnet:$PATH" && dotnet build Dormitory.sln`
Expected: Build thành công 0 warning nghiêm trọng, 0 error.

- [ ] **Step 3: Run GitNexus graph change detection**

Run: `node .gitnexus/run.cjs detect-changes --scope all --repo .`
Expected: Clean status, không có broken execution flows.

- [ ] **Step 4: Re-index GitNexus knowledge graph**

Run: `node .gitnexus/run.cjs analyze --index-only --repo .`
Expected: Toàn bộ symbols mới được cập nhật vào knowledge graph.

- [ ] **Step 5: Update documentation and user guide**

Cập nhật `docs/user-guide.md` và `README.md` mô tả các tính năng thêm mới:
- Chi tiết các hộp thoại CRUD (Phòng, Sinh viên, Hợp đồng, Hóa đơn).
- Hướng dẫn quản lý Nhân viên KTX.

- [ ] **Step 6: Final commit and push**

```bash
git add docs/ README.md
git commit -m "docs: update user guide and README for full CRUD dialogs and Employee module"
git push origin master
```
