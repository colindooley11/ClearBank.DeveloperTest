using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
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

            var result = new MakePaymentResult();

            result.Success = true;
            
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (account == null)
                    {
                        result.Success = false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = false;
                    }
                    break;

                case PaymentScheme.FasterPayments:
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
                    break;

                case PaymentScheme.Chaps:
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
                    break;
            }

            if (result.Success)
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

            return result;
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
