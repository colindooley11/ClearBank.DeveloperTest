using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services;

public interface IAccountValidator
{
    public MakePaymentResult IsAccountValidForRequest(MakePaymentRequest request, Account account);
}