using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        public PaymentService()
        {
            
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

            var result = new MakePaymentResult();

            result.Success = true;
            
            IsAccountEligibleForPaymentScheme(request, account, result);

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

        private void IsAccountEligibleForPaymentScheme(MakePaymentRequest request, Account account, MakePaymentResult result)
        {
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    IsBacsEligble(account, result);
                    break;

                case PaymentScheme.FasterPayments:
                    IsFasterPaymentsEligible(request, account, result);
                    break;

                case PaymentScheme.Chaps:
                    IsChapsEligible(account, result);
                    break;
            }
        }

        private static void IsBacsEligble(Account account, MakePaymentResult result)
        {
            if (account == null)
            {
                result.Success = false;
            }
            else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
            {
                result.Success = false;
            }
        }

        private static void IsChapsEligible(Account account, MakePaymentResult result)
        {
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

            return;
        }

        private static void IsFasterPaymentsEligible(MakePaymentRequest request, Account account, MakePaymentResult result)
        {
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
