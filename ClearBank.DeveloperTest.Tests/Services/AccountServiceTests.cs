// using System.Xml.Schema;
// using ClearBank.DeveloperTest.Configuration;
// using ClearBank.DeveloperTest.Data;
// using ClearBank.DeveloperTest.Services;
// using ClearBank.DeveloperTest.Types;
// using Moq;
// using Xunit;
//
// namespace ClearBank.DeveloperTest.Tests.Services;
//
// public class AccountServiceTests
// {
//     [Fact]
//     public void Given_A_Retrieval_Of_An_Account_When_Getting_It_Then_It_Is_Returned()
//     {
//         var accountDataStoreFactory =
//             new Mock<IAccountDataStoreFactory>();
//         accountDataStoreFactory.Setup(factory => factory.GetAccountDataStore(It.IsAny<string>()))
//             .Returns(new TestableAccountDataStore());
//
//         var result = new AccountService(accountDataStoreFactory.Object, new TestableConfiguration())
//             .GetAccount("abc123");
//
//         Assert.Equal("abc123", result.AccountNumber);
//     }
//
//     [Fact]
//     public void Given_An_Account_Update_When_It_Is_Updated_Then_It_Is_Updated()
//     {
//         var accountDataStoreFactory =
//             new Mock<IAccountDataStoreFactory>();
//         var testableAccountDataStore = new TestableAccountDataStore();
//         accountDataStoreFactory.Setup(factory => factory.GetAccountDataStore(It.IsAny<string>()))
//             .Returns(testableAccountDataStore);
//
//         new AccountService(accountDataStoreFactory.Object, new TestableConfiguration())
//             .UpdateAccount(new Account { AccountNumber = "def456" });
//
//         Assert.Equal("def456", testableAccountDataStore.UpdatedAccount.AccountNumber);
//     }
//
//     internal class TestableConfiguration : IDataStoreConfiguration
//     {
//         public string DataStoreType { get; } = "Dummy";
//     }
//
//     internal class TestableAccountDataStore : IAccountDataStore
//     {
//         public Account UpdatedAccount;
//
//         public Account GetAccount(string accountNumber)
//         {
//             if (accountNumber == "abc123")
//             {
//                 return new Account() { AccountNumber = "abc123" };
//             }
//
//             return null;
//         }
//
//         public void UpdateAccount(Account account)
//         {
//             UpdatedAccount = account;
//         }
//     }
// }