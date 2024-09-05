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
        Given_A_Bacs_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_Payment_Is_Successful()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000M
        };

        var paymentService = new PaymentServiceBuilder()
            .WithAccount(account)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest
            { PaymentScheme = PaymentScheme.Bacs, Amount = 100M });
        Assert.True(result.Success);
        Assert.Equal(900M, account.Balance);
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

        var paymentService = new PaymentServiceBuilder()
            .WithAccount(account)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest
            { PaymentScheme = PaymentScheme.FasterPayments, Amount = 500M });
        Assert.True(result.Success);
        Assert.Equal(500M, account.Balance);
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

        var paymentService = new PaymentServiceBuilder()
            .WithAccount(account)
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest
            { PaymentScheme = PaymentScheme.Chaps, Amount = 800M });

        Assert.True(result.Success);
        Assert.Equal(200M, account.Balance);
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
        private Account _account;

        public PaymentServiceBuilder WithAccount(Account account)
        {
            _account = account;
            return this;
        }

        public PaymentServiceBuilder WithNoAccount()
        {
            _account = null;
            return this;
        }

        public PaymentService Build()
        {
            var accountDataStoreFactory =
                new Mock<IAccountDataStoreFactory>();
            accountDataStoreFactory.Setup(factory => factory.GetAccountDataStore(It.IsAny<string>()))
                .Returns(new TestableAccountDataStore(_account));

            return new PaymentService(
                new AccountValidator(),
                new AccountService(accountDataStoreFactory.Object, new TestableConfiguration()));
        }
    }

    private class TestableConfiguration : IDataStoreConfiguration
    {
        public string DataStoreType { get; } = "Dummy";
    }

    private class TestableAccountDataStore : IAccountDataStore
    {
        public Account UpdatedAccount;
        private Account _retrievedAccount;

        public TestableAccountDataStore(Account account)
        {
            _retrievedAccount = account;
        }

        public Account GetAccount(string accountNumber)
        {
            return _retrievedAccount;
        }

        public void UpdateAccount(Account account)
        {
            UpdatedAccount = account;
        }
    }
}