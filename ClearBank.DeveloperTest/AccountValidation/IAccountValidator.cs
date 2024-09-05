using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.AccountValidation;

public interface IAccountValidator
{
    public MakePaymentResult IsAccountValidForPayment(Account account, MakePaymentRequest request);
}