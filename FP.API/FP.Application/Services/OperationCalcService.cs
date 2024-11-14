using FP.Application.DTOs;
using FP.Domain;
using FP.Domain.Enums;

namespace FP.Application.Services
{
    public static class OperationCalcService
    {
        public static decimal ApplyOperation(decimal startAmount, Operation operation)
        {
            var amount = operation.Amount;
            if (operation.Type == OperationType.Expense)
            {
                amount = -amount;
            }
            startAmount += amount;
            return startAmount;
        }

        public static decimal RemoveOperation(decimal startAmount, Operation operation)
        {
            var amount = operation.Amount;
            if (operation.Type == OperationType.Income)
            {
                amount = -amount;
            }
            startAmount += amount;
            return startAmount;
        }

        public static decimal ApplyOperation(decimal startAmount, OperationDto operation)
        {
            return ApplyOperation(startAmount, new Operation { Amount = operation.Amount });
        }
    }
}
