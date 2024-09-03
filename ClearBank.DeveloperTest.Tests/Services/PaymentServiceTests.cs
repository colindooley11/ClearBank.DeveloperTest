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
    public void Given_A_Bacs_Payment_Request_When_Making_A_Payment_Then_The_Payment_Is_Successful()
    {
        var paymentService = new TestablePaymentService(new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        });
        var result = paymentService.MakePayment(new MakePaymentRequest() { PaymentScheme = PaymentScheme.Bacs});
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Given_A_Faster_Payments_Payment_Request_When_Making_A_Payment_Then_The_Payment_Is_Successful()
    {
        var paymentService = new TestablePaymentService(new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Balance = 1000M
        });
        var result = paymentService.MakePayment(new MakePaymentRequest() { PaymentScheme = PaymentScheme.FasterPayments, Amount = 500M});
        Assert.True(result.Success);
    }

    private class TestablePaymentService : PaymentService
    {
        private readonly Account _account;

        public TestablePaymentService(Account account)
        {
            _account = account;
        }

        protected override Account GetAccount(MakePaymentRequest request, AccountDataStore accountDataStore)
        {
            return _account;
        }
    }
    
}