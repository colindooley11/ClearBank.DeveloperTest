namespace ClearBank.DeveloperTest.Data;

public class AccountDataStoreFactory : IAccountDataStoreFactory
{
    private const string Backup = "Backup";

    public IAccountDataStore GetAccountDataStore(string dataStoreType)
    {
        if (dataStoreType == Backup)
        {
            return new BackupAccountDataStore();
        }

        return new AccountDataStore();
    }
}