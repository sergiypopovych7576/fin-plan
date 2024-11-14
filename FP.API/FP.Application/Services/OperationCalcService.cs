using FP.Application.DTOs;
using FP.Domain;
using FP.Domain.Enums;

namespace FP.Application.Services
{
    public static class OperationCalcService
    {
        public static decimal ApplyOperation(decimal startAmount, Operation operation)
        {
            if (operation.Type == OperationType.Expense)
            {
                return startAmount - operation.Amount;
            }
            else
            {
                return startAmount + operation.Amount;
            }
        }

        public static decimal RemoveOperation(decimal startAmount, Operation operation)
        {
            if (operation.Type == OperationType.Expense)
            {
                return startAmount + operation.Amount;
            }
            else
            {
                return startAmount - operation.Amount;
            }
        }

        public static decimal ApplyOperation(decimal startAmount, OperationDto operation)
        {
            return ApplyOperation(startAmount, new Operation { Amount = operation.Amount, Type = operation.Type });
        }
    }
}
