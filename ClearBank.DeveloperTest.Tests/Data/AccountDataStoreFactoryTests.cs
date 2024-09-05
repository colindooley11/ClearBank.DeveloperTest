using ClearBank.DeveloperTest.Data;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Data;

public class AccountDataStoreFactoryTests
{
    [Fact]
    public void Given_A_Request_To_Create_A_Backup_Account_Data_Store()
    {
        var result = new AccountDataStoreFactory().GetAccountDataStore("Backup");
        Assert.IsType<BackupAccountDataStore>(result);
    }
    
    [Fact]
    public void Given_A_Request_To_Create_An_Account_Data_Store()
    {
        var result = new AccountDataStoreFactory().GetAccountDataStore(string.Empty);
        Assert.IsType<AccountDataStore>(result);
    }
    
}