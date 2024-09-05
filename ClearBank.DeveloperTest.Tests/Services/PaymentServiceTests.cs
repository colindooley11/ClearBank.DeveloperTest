using ClearBank.DeveloperTest.AccountValidation;
using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Services;

public class PaymentServiceTests
{
    [Fact]
    public void
        Given_An_Invalid_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Not_Debited_And_Payment_Is_Not_Successful()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000M
        };
        var accountDataStoreSpy = new AccountDataStoreSpy(account);

        var paymentService = new PaymentServiceBuilder()
            .WithAccountDataStoreSpy(accountDataStoreSpy)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest());
        
        Assert.False(result.Success);
        Assert.Equal(1000M, account.Balance);
        Assert.False(accountDataStoreSpy.AccountUpdated);
    }


    [Fact]
    public void
        Given_A_Bacs_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_Payment_Is_Successful()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000M
        };
        var accountDataStoreSpy = new AccountDataStoreSpy(account);

        var paymentService = new PaymentServiceBuilder()
            .WithAccountDataStoreSpy(accountDataStoreSpy)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest
            { PaymentScheme = PaymentScheme.Bacs, Amount = 100M });
        
        Assert.True(result.Success);
        Assert.Equal(900M, account.Balance);
        Assert.True(accountDataStoreSpy.AccountUpdated);
    }

    [Fact]
    public void
        Given_A_Faster_Payments_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_Payment_Is_Successful()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 1000M
        };

        var accountDataStoreSpy = new AccountDataStoreSpy(account);

        var paymentService = new PaymentServiceBuilder()
            .WithAccountDataStoreSpy(accountDataStoreSpy)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest
            { PaymentScheme = PaymentScheme.FasterPayments, Amount = 500M });
        
        Assert.True(result.Success);
        Assert.Equal(500M, account.Balance);
        Assert.True(accountDataStoreSpy.AccountUpdated);
    }

    [Fact]
    public void
        Given_A_Chaps_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_The_Payment_Is_Successful()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live,
            Balance = 1000M
        };

        var accountDataStoreSpy = new AccountDataStoreSpy(account);

        var paymentService = new PaymentServiceBuilder()
            .WithAccountDataStoreSpy(accountDataStoreSpy)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest
            { PaymentScheme = PaymentScheme.Chaps, Amount = 800M });

        Assert.True(result.Success);
        Assert.Equal(200M, account.Balance);
        Assert.True(accountDataStoreSpy.AccountUpdated);
    }


    [Theory]
    [InlineData(PaymentScheme.Bacs)]
    [InlineData(PaymentScheme.FasterPayments)]
    [InlineData(PaymentScheme.Chaps)]
    public void
        Given_A_Payment_Request_When_It_There_Is_No_Associate_Account_An_Account_Then_The_Payment_Is_Not_Successful(
            PaymentScheme paymentScheme)
    {
        var paymentService = new PaymentServiceBuilder()
            .WithNoAccount()
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = paymentScheme });

        Assert.False(result.Success);
    }

    private class PaymentServiceBuilder
    {
        private IAccountDataStore _accountDataStoreSpy;

        public PaymentServiceBuilder WithNoAccount()
        {
            _accountDataStoreSpy = new AccountDataStoreSpy(null);
            return this;
        }

        public PaymentServiceBuilder WithAccountDataStoreSpy(IAccountDataStore accountDataStoreSpy)
        {
            _accountDataStoreSpy = accountDataStoreSpy;
            return this;
        }

        public PaymentService Build()
        {
            var accountDataStoreFactory =
                new Mock<IAccountDataStoreFactory>();
            accountDataStoreFactory.Setup(factory => factory.GetAccountDataStore(It.IsAny<string>()))
                .Returns(_accountDataStoreSpy);

            return new PaymentService(
                new AccountValidator(),
                new AccountService(accountDataStoreFactory.Object, new DummyDataStoreConfiguration()));
        }
    }

    private class DummyDataStoreConfiguration : IDataStoreConfiguration
    {
        public string DataStoreType => "Dummy";
    }

    private class AccountDataStoreSpy : IAccountDataStore
    {
        public bool AccountUpdated;
        private readonly Account _retrievedAccount;

        public AccountDataStoreSpy(Account account)
        {
            _retrievedAccount = account;
        }

        public Account GetAccount(string accountNumber)
        {
            return _retrievedAccount;
        }

        public void UpdateAccount(Account account)
        {
            AccountUpdated = true;
        }
    }
}