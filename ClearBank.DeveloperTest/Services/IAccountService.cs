using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services;

public interface IAccountService
{
    public Account GetAccount(string accountNumber);
    
    public void UpdateAccountBalance(Account account, decimal amount);
}