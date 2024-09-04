using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AccountValidator _accountValidator;

        public PaymentService(AccountValidator accountValidator)
        {
            _accountValidator = accountValidator;
        }
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = GetDataStoreType();

            Account account = null;

            if (dataStoreType == "Backup")
            {
                var accountDataStore = new BackupAccountDataStore();
                account = GetBackupAccount(request, accountDataStore);
            }
            else
            {
                var accountDataStore = new AccountDataStore();
                account = GetAccount(request, accountDataStore);
            }

            var makePaymentResult  = _accountValidator.IsAccountValidForRequest(request, account);

            if (PaymentCanBeMade(makePaymentResult))
            {
                account.Balance -= request.Amount;

                if (dataStoreType == "Backup")
                {
                    var accountDataStore = new BackupAccountDataStore();
                    UpdateBackupBankAccount(accountDataStore, account);
                }
                else
                {
                    var accountDataStore = new AccountDataStore();
                    UpdateAccount(accountDataStore, account);
                }
            }

            return makePaymentResult;
        }

        private static bool PaymentCanBeMade(MakePaymentResult result)
        {
            return result.Success;
        }

        protected virtual void UpdateBackupBankAccount(BackupAccountDataStore accountDataStore, Account account)
        {
            accountDataStore.UpdateAccount(account);
        }

        protected virtual string GetDataStoreType()
        {
            return ConfigurationManager.AppSettings["DataStoreType"];
        }

        protected virtual Account GetBackupAccount(MakePaymentRequest request, BackupAccountDataStore accountDataStore)
        {
            return accountDataStore.GetAccount(request.DebtorAccountNumber);
        }

        protected virtual void UpdateAccount(AccountDataStore accountDataStore, Account account)
        {
            accountDataStore.UpdateAccount(account);
        }

        protected virtual Account GetAccount(MakePaymentRequest request, AccountDataStore accountDataStore)
        {
            return accountDataStore.GetAccount(request.DebtorAccountNumber);
        }
    }
}
