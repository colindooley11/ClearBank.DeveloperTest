namespace ClearBank.DeveloperTest.Data;

public interface IAccountDataStoreFactory
{
    public IAccountDataStore GetAccountDataStore(string dataStoreType);
}