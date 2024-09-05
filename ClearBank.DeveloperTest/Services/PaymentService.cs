using System;
using ClearBank.DeveloperTest.AccountValidation;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services;

public class PaymentService : IPaymentService
{
    private readonly IAccountValidator _accountValidator;
    private readonly IAccountService _accountService;

    public PaymentService(IAccountValidator accountValidator, IAccountService accountService)
    {
        _accountValidator = accountValidator ?? throw new ArgumentNullException(nameof(accountValidator));
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        var account = _accountService.GetAccount(request.DebtorAccountNumber);
        var makePaymentResult = _accountValidator.IsAccountValidForPayment(request, account);

        if (PaymentCanBeMade(makePaymentResult))
        {
            account.Balance -= request.Amount;
            _accountService.UpdateAccount(account);
        }

        return makePaymentResult;
    }

    private static bool PaymentCanBeMade(MakePaymentResult result)
    {
        return result.Success;
    }
}