using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Services;

public class PaymentServiceTests
{
    [Fact]
    public void Given_An_Incomplete_Payment_Request_When_Making_A_Payment_Then_The_Payment_Is_Not_Successful()
    {
        var paymentService = new PaymentService();
        var result = paymentService.MakePayment(new MakePaymentRequest());
        Assert.False(result.Success);
    }

    [Fact]
    public void Given_A_Bacs_Payment_Request_When_Making_A_Payment_Then_The_Bank_Account_Is_Debited_And_Payment_Is_Successful()
    {
        var paymentService = new PaymentServiceBuilder()
            .WithBacsAccount(1000M)
            .Build();
        
        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 100M});
        Assert.True(result.Success);
        Assert.Equal(900M, paymentService._accountSpy.Balance);
    }

    [Fact]
    public void Given_A_Faster_Payments_Payment_Request_When_Making_A_Payment_Then_The_Payment_Is_Successful()
    {
        var paymentService = new PaymentServiceBuilder()
            .WithFasterPaymentAccount()
            .Build();
        
        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 500M });
        Assert.True(result.Success);
        
    }

    [Fact]
    public void Given_A_Chaps_Payment_Request_When_Making_A_Payment_Then_The_Payment_Is_Successful()
    {
        var paymentService = new PaymentServiceBuilder()
            .WithChapsAccount()
            .Build();

        var result = paymentService.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

        Assert.True(result.Success);
    }

    private class PaymentServiceBuilder
    {
        private Account _account;

        public PaymentServiceBuilder WithBacsAccount(decimal balance)
        {
            _account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Balance = balance
            };
            return this;
        }

        public PaymentServiceBuilder WithChapsAccount(AccountStatus accountStatus = AccountStatus.Live)
        {
            _account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = accountStatus
            };
            return this;
        }

        public PaymentServiceBuilder WithFasterPaymentAccount(decimal balance = 1000M)
        {
            _account = new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Balance = balance
            };
            return this;
        }

        public TestablePaymentService Build()
        {
            return new TestablePaymentService(_account);
        }
    }

    private class TestablePaymentService : PaymentService
    {
        private readonly Account _account;
        public Account _accountSpy; 

        public TestablePaymentService(Account account)
        {
            _account = account;
        }

        protected override Account GetAccount(MakePaymentRequest request, AccountDataStore accountDataStore)
        {
            return _account;
        }

        protected override void UpdateAccount(AccountDataStore accountDataStore, Account account)
        {
            _accountSpy = account;
        }
    }
}