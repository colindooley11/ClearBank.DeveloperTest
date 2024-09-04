using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    [Fact]
    public void Given_An_Incomplete_Payment_Request_When_Making_A_Payment_Then_The_Payment_Is_Not_Successful()
    {
        var paymentService = new PaymentService();
        var result = paymentService.MakePayment(new MakePaymentRequest());
        Assert.False(result.Success);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Given_A_Bacs_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_Payment_Is_Successful(bool isBackupBankAccount)
    {
        var paymentService = new PaymentServiceBuilder()
            .WithBacsAccount(1000M)
            .WithBackupAccount(isBackupBankAccount)
            .Build();
        
        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 100M});
        Assert.True(result.Success);
        Assert.Equal(900M, paymentService.AccountSpy.Balance);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Given_A_Faster_Payments_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_Payment_Is_Successful(bool isBackupBankAccount)
    {
        var paymentService = new PaymentServiceBuilder()
            .WithFasterPaymentAccount(1000M)
            .WithBackupAccount(isBackupBankAccount)
            .Build();
        
        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 500M });
        Assert.True(result.Success);
        Assert.Equal(500M, paymentService.AccountSpy.Balance);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Given_A_Chaps_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_The_Payment_Is_Successful(bool isBackupBankAccount)
    {
        var paymentService = new PaymentServiceBuilder()
            .WithChapsAccount(1000M)
            .WithBackupAccount(isBackupBankAccount)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps, Amount = 800M});

        Assert.True(result.Success);
        Assert.Equal(200M, paymentService.AccountSpy.Balance);
    }
    

    private class PaymentServiceBuilder
    {
        private Account _account;
        private bool _isBackupAccount;

        public PaymentServiceBuilder WithBacsAccount(decimal balance)
        {
            _account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = balance
            };
            return this;
        }

        public PaymentServiceBuilder WithChapsAccount(decimal balance, AccountStatus accountStatus = AccountStatus.Live)
        {
            _account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = accountStatus,
                Balance = balance
            };
            return this;
        }

        public PaymentServiceBuilder WithFasterPaymentAccount(decimal balance)
        {
            _account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = balance
            };
            return this;
        }

        public PaymentServiceBuilder WithBackupAccount(bool isBackupAccount)
        {
            _isBackupAccount = isBackupAccount;
            return this;
        }

        public TestablePaymentService Build()
        {
            return new TestablePaymentService(_account, _isBackupAccount);
        }
    }

    private class TestablePaymentService : PaymentService
    {
        private readonly Account _account;
        private readonly bool _isBackupAccount;
        public Account AccountSpy; 

        public TestablePaymentService(Account account, bool isBackupAccount)
        {
            _account = account;
            _isBackupAccount = isBackupAccount;
        }

        protected override string GetDataStoreType()
        {
            return _isBackupAccount ? "Backup" : string.Empty;
        }

        protected override Account GetAccount(MakePaymentRequest request, AccountDataStore accountDataStore)
        {
            return _account;
        }

        protected override Account GetBackupAccount(MakePaymentRequest request, BackupAccountDataStore accountDataStore)
        {
            return _account;
        }

        protected override void UpdateAccount(AccountDataStore accountDataStore, Account account)
        {
            AccountSpy = account;
        }

        protected override void UpdateBackupBankAccount(BackupAccountDataStore accountDataStore, Account account)
        {
            AccountSpy = account;
        }
    }
}