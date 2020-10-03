using Lab.EventSourcing.Core;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class AccountCreated : ModelEventBase
    { 
        public decimal InitialDeposit { get; private set; }

        public AccountCreated(Guid modelId, decimal initialDeposit) 
            : base(modelId, 1) =>
                InitialDeposit = initialDeposit;
    }
}
