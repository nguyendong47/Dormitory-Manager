using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dormitory.Application.DTOs;
using Dormitory.Application.Interfaces;
using Dormitory.Core.Enums;
using Dormitory.Desktop.Services;
using Dormitory.Desktop.ViewModels;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Dormitory.UnitTests.ViewModels;

/// <summary>
/// Kiểm thử đơn vị cho PaymentTransactionListViewModel và AssignBillDialogViewModel sử dụng NSubstitute
/// </summary>
public class PaymentTransactionListViewModelTests
{
    private readonly IPaymentReconciliationService _reconciliationService;
    private readonly IBillService _billService;
    private readonly IDialogService _dialogService;

    public PaymentTransactionListViewModelTests()
    {
        _reconciliationService = Substitute.For<IPaymentReconciliationService>();
        _billService = Substitute.For<IBillService>();
        _dialogService = Substitute.For<IDialogService>();

        _billService.GetAllBillsAsync(Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<int?>(), Arg.Any<BillStatus?>())
            .Returns(new List<BillDto>());
    }

    private PaymentTransactionListViewModel CreateViewModel()
    {
        return new PaymentTransactionListViewModel(
            _reconciliationService,
            _billService,
            _dialogService);
    }

    private List<PaymentTransactionDto> CreateSampleTransactions()
    {
        return new List<PaymentTransactionDto>
        {
            new()
            {
                Id = 1,
                TransactionId = "TXN001",
                Amount = 1500000m,
                Description = "KTX HD001",
                Status = PaymentTransactionStatus.Success,
                TransactionDate = DateTime.UtcNow.AddMinutes(-30),
                AccountNumber = "123456789"
            },
            new()
            {
                Id = 2,
                TransactionId = "TXN002",
                Amount = 500000m,
                Description = "KTX HD002 thieu tien",
                Status = PaymentTransactionStatus.PartiallyPaid,
                TransactionDate = DateTime.UtcNow.AddMinutes(-20),
                AccountNumber = "123456789"
            },
            new()
            {
                Id = 3,
                TransactionId = "TXN003",
                Amount = 800000m,
                Description = "Nguyen Van A chuyen tien",
                Status = PaymentTransactionStatus.Unmatched,
                TransactionDate = DateTime.UtcNow.AddMinutes(-10),
                AccountNumber = "123456789"
            },
            new()
            {
                Id = 4,
                TransactionId = "TXN004",
                Amount = 300000m,
                Description = "Trùng lặp giao dịch",
                Status = PaymentTransactionStatus.Duplicate,
                TransactionDate = DateTime.UtcNow.AddMinutes(-5),
                AccountNumber = "123456789"
            }
        };
    }

    [Fact]
    public async Task Should_LoadTransactions_And_Update_TransactionsCollection()
    {
        // Arrange
        var sampleTransactions = CreateSampleTransactions();
        _reconciliationService.GetTransactionsAsync(Arg.Any<PaymentTransactionFilterDto>())
            .Returns(sampleTransactions);

        var vm = CreateViewModel();

        // Act
        await vm.LoadTransactionsAsync();

        // Assert
        vm.Transactions.Should().HaveCount(4);
        vm.Transactions[0].TransactionId.Should().Be("TXN001");
        vm.Transactions[0].StatusText.Should().Be("Thành công");
        vm.Transactions[2].StatusText.Should().Be("Chưa khớp");
    }

    [Fact]
    public async Task Should_Calculate_KPIs_Correctly()
    {
        // Arrange
        var sampleTransactions = CreateSampleTransactions();
        _reconciliationService.GetTransactionsAsync(Arg.Any<PaymentTransactionFilterDto>())
            .Returns(sampleTransactions);

        var vm = CreateViewModel();

        // Act
        await vm.LoadTransactionsAsync();

        // Assert
        // TotalReceivedAmount = Success (1,500,000) + PartiallyPaid (500,000) = 2,000,000
        vm.TotalReceivedAmount.Should().Be(2000000m);
        vm.SuccessCount.Should().Be(1);
        vm.UnmatchedCount.Should().Be(1);
    }

    [Fact]
    public async Task Should_Filter_Transactions_By_Status()
    {
        // Arrange
        PaymentTransactionFilterDto? capturedFilter = null;
        _reconciliationService.GetTransactionsAsync(Arg.Do<PaymentTransactionFilterDto>(f => capturedFilter = f))
            .Returns(new List<PaymentTransactionDto>());

        var vm = CreateViewModel();

        // Act - lọc Chưa khớp
        vm.SelectedStatusFilter = "Chưa khớp";
        await vm.LoadTransactionsAsync();

        // Assert
        capturedFilter.Should().NotBeNull();
        capturedFilter!.Status.Should().Be(PaymentTransactionStatus.Unmatched);

        // Act - lọc Thành công
        vm.SelectedStatusFilter = "Thành công";
        await vm.LoadTransactionsAsync();

        // Assert
        capturedFilter!.Status.Should().Be(PaymentTransactionStatus.Success);

        // Act - lọc Tất cả
        vm.SelectedStatusFilter = "Tất cả";
        await vm.LoadTransactionsAsync();

        // Assert
        capturedFilter!.Status.Should().BeNull();
    }

    [Fact]
    public void CanAssignBill_Should_Be_True_Only_When_SelectedTransaction_Is_Unmatched()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act & Assert 1: Null
        vm.SelectedTransaction = null;
        vm.CanAssignBill.Should().BeFalse();

        // Act & Assert 2: Success
        vm.SelectedTransaction = new PaymentTransactionDto
        {
            Id = 1,
            Status = PaymentTransactionStatus.Success
        };
        vm.CanAssignBill.Should().BeFalse();

        // Act & Assert 3: Unmatched
        vm.SelectedTransaction = new PaymentTransactionDto
        {
            Id = 2,
            Status = PaymentTransactionStatus.Unmatched
        };
        vm.CanAssignBill.Should().BeTrue();

        // Act & Assert 4: Duplicate
        vm.SelectedTransaction = new PaymentTransactionDto
        {
            Id = 3,
            Status = PaymentTransactionStatus.Duplicate
        };
        vm.CanAssignBill.Should().BeFalse();
    }

    [Fact]
    public async Task AssignBillCommand_Should_Open_Dialog_And_Reload_When_Assigned()
    {
        // Arrange
        _reconciliationService.GetTransactionsAsync(Arg.Any<PaymentTransactionFilterDto>())
            .Returns(CreateSampleTransactions());

        var vm = CreateViewModel();
        await vm.LoadTransactionsAsync();

        vm.SelectedTransaction = new PaymentTransactionDto
        {
            Id = 3,
            TransactionId = "TXN003",
            Status = PaymentTransactionStatus.Unmatched,
            Amount = 800000m
        };

        bool dialogShown = false;
        vm.ShowAssignBillDialogHandler = dialogVm =>
        {
            dialogShown = true;
            dialogVm.Transaction.Id.Should().Be(3);
            return Task.FromResult(true); // Giả lập kế toán gán thành công
        };

        _reconciliationService.ClearReceivedCalls();

        // Act
        await vm.AssignBillCommand.ExecuteAsync(null);

        // Assert
        dialogShown.Should().BeTrue();
        // Sau khi gán thành công, GetTransactionsAsync được gọi lại 1 lần để reload
        await _reconciliationService.Received(1).GetTransactionsAsync(Arg.Any<PaymentTransactionFilterDto>());
    }

    [Fact]
    public async Task AssignBillCommand_Should_Not_Reload_When_Dialog_Cancelled()
    {
        // Arrange
        _reconciliationService.GetTransactionsAsync(Arg.Any<PaymentTransactionFilterDto>())
            .Returns(CreateSampleTransactions());

        var vm = CreateViewModel();
        await vm.LoadTransactionsAsync();

        vm.SelectedTransaction = new PaymentTransactionDto
        {
            Id = 3,
            TransactionId = "TXN003",
            Status = PaymentTransactionStatus.Unmatched,
            Amount = 800000m
        };

        vm.ShowAssignBillDialogHandler = _ => Task.FromResult(false); // Người dùng hủy dialog
        _reconciliationService.ClearReceivedCalls();

        // Act
        await vm.AssignBillCommand.ExecuteAsync(null);

        // Assert - GetTransactionsAsync không được gọi thêm
        await _reconciliationService.DidNotReceiveWithAnyArgs().GetTransactionsAsync(default!);
    }

    // --- AssignBillDialogViewModel Tests ---

    [Fact]
    public async Task AssignBillDialog_LoadUnpaidBills_Should_Populate_UnpaidBills()
    {
        // Arrange
        var sampleTransaction = new PaymentTransactionDto
        {
            Id = 10,
            TransactionId = "TXN010",
            Amount = 1200000m,
            Status = PaymentTransactionStatus.Unmatched
        };

        var unpaidBills = new List<BillDto>
        {
            new() { Id = 101, BillCode = "HD101", RoomNumber = "101", Month = 10, Year = 2026, RoomFee = 1000000m },
            new() { Id = 102, BillCode = "HD102", RoomNumber = "102", Month = 10, Year = 2026, RoomFee = 1200000m }
        };

        _billService.GetAllBillsAsync(status: BillStatus.Unpaid).Returns(unpaidBills);

        // Act
        var dialogVm = new AssignBillDialogViewModel(sampleTransaction, _billService, _reconciliationService, _dialogService);
        await dialogVm.LoadUnpaidBillsAsync();

        // Assert
        dialogVm.UnpaidBills.Should().HaveCount(2);
        dialogVm.UnpaidBills[0].BillCode.Should().Be("HD101");
        dialogVm.UnpaidBills[1].BillCode.Should().Be("HD102");
    }

    [Fact]
    public async Task AssignBillDialog_ConfirmAssign_Should_Call_ReconciliationService_And_Close()
    {
        // Arrange
        var sampleTransaction = new PaymentTransactionDto
        {
            Id = 10,
            TransactionId = "TXN010",
            Amount = 1200000m,
            Status = PaymentTransactionStatus.Unmatched
        };

        var selectedBill = new BillDto { Id = 101, BillCode = "HD101" };
        _reconciliationService.ManuallyAssignBillAsync(10, 101, "Gán thủ công").Returns(true);

        bool? closedResult = null;
        var dialogVm = new AssignBillDialogViewModel(sampleTransaction, _billService, _reconciliationService, _dialogService)
        {
            SelectedBill = selectedBill,
            Note = "Gán thủ công",
            CloseAction = res => closedResult = res
        };

        // Act
        await dialogVm.ConfirmAssignCommand.ExecuteAsync(null);

        // Assert
        await _reconciliationService.Received(1).ManuallyAssignBillAsync(10, 101, "Gán thủ công");
        closedResult.Should().BeTrue();
    }

    [Fact]
    public async Task AssignBillDialog_ConfirmAssign_Without_Selection_Should_Show_Message()
    {
        // Arrange
        var sampleTransaction = new PaymentTransactionDto { Id = 10, Status = PaymentTransactionStatus.Unmatched };
        _billService.GetAllBillsAsync(status: BillStatus.Unpaid).Returns(new List<BillDto>());

        var dialogVm = new AssignBillDialogViewModel(sampleTransaction, _billService, _reconciliationService, _dialogService)
        {
            SelectedBill = null
        };
        _dialogService.ClearReceivedCalls();

        // Act
        await dialogVm.ConfirmAssignCommand.ExecuteAsync(null);

        // Assert
        await _reconciliationService.DidNotReceiveWithAnyArgs().ManuallyAssignBillAsync(default, default, default!);
        await _dialogService.Received(1).ShowMessageAsync("Thông báo", "Vui lòng chọn một hóa đơn cần gán.");
    }

    [Fact]
    public async Task AssignBillDialog_Cancel_Should_Close_With_False()
    {
        // Arrange
        var sampleTransaction = new PaymentTransactionDto { Id = 10 };
        bool? closedResult = null;
        var dialogVm = new AssignBillDialogViewModel(sampleTransaction, _billService, _reconciliationService, _dialogService)
        {
            CloseAction = res => closedResult = res
        };

        // Act
        await dialogVm.CancelCommand.ExecuteAsync(null);

        // Assert
        closedResult.Should().BeFalse();
    }
}
