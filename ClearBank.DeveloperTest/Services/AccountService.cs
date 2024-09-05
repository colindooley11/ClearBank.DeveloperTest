using System;
using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services;

public class AccountService : IAccountService
{
    private readonly IAccountDataStore _accountDataStore;

    public AccountService(IAccountDataStoreFactory accountDataStoreFactory,
        IDataStoreConfiguration dataStoreConfiguration)
    {
        if (accountDataStoreFactory == null) throw new ArgumentNullException(nameof(accountDataStoreFactory));
        if (dataStoreConfiguration == null) throw new ArgumentNullException(nameof(dataStoreConfiguration));
        _accountDataStore = accountDataStoreFactory.GetAccountDataStore(dataStoreConfiguration.DataStoreType);
    }

    public Account GetAccount(string accountNumber)
    {
        return _accountDataStore.GetAccount(accountNumber);
    }

    public void UpdateAccount(Account account)
    {
        _accountDataStore.UpdateAccount(account);
    }
}