using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services;

public class AccountValidator : IAccountValidator
{
    public MakePaymentResult IsAccountValidForRequest(MakePaymentRequest request, Account account)
    {
        return request.PaymentScheme switch
        {
            PaymentScheme.Bacs => IsBacsEligble(account),
            PaymentScheme.FasterPayments => IsFasterPaymentsEligible(request, account),
            PaymentScheme.Chaps => IsChapsEligible(account),
            _ => new MakePaymentResult()
        };
    }

    private MakePaymentResult IsBacsEligble(Account account)
    {
        var result = new MakePaymentResult { Success = true };
        if (account == null)
        {
            result.Success = false;
        }
        else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
        {
            result.Success = false;
        }

        return result; 
    }

    private MakePaymentResult IsChapsEligible(Account account)
    {
        var result = new MakePaymentResult { Success = true };
        if (account == null)
        {
            result.Success = false;
        }
        else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
        {
            result.Success = false;
        }
        else if (account.Status != AccountStatus.Live)
        {
            result.Success = false;
        }

        return result;
    }

    private MakePaymentResult IsFasterPaymentsEligible(MakePaymentRequest request, Account account)
    {
        var result = new MakePaymentResult { Success = true};
        if (account == null)
        {
            result.Success = false;
        }
        else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
        {
            result.Success = false;
        }
        else if (account.Balance < request.Amount)
        {
            result.Success = false;
        }

        return result;
    }
   
}