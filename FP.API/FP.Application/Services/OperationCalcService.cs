using FP.Domain;
using FP.Domain.Enums;

namespace FP.Application.Services
{
    public static class OperationCalcService
    {
        public static void ApplyOperation(Account account, Operation operation)
        {
            AdjustBalance(account, operation, isApplying: true);
        }

        public static void RemoveOperation(Account account, Operation operation)
        {
            AdjustBalance(account, operation, isApplying: false);
        }

        private static void AdjustBalance(Account account, Operation operation, bool isApplying)
        {
            var adjustmentFactor = isApplying ? 1 : -1;

            switch (operation.Type)
            {
                case OperationType.Expense:
                    account.Balance -= adjustmentFactor * operation.Amount;
                    break;

                case OperationType.Income:
                    account.Balance += adjustmentFactor * operation.Amount;
                    break;

                case OperationType.Transfer:
                    if (operation.SourceAccountId == account.Id)
                    {
                        account.Balance -= adjustmentFactor * operation.Amount;
                    }
                    else
                    {
                        account.Balance += adjustmentFactor * operation.Amount;
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported operation type: {operation.Type}");
            }
        }
    }
}
