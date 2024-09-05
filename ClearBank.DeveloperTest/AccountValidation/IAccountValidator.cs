using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.AccountValidation;

public interface IAccountValidator
{
    public MakePaymentResult IsAccountValidForPayment(MakePaymentRequest request, Account account);
}