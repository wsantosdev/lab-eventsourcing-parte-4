using Lab.EventSourcing.Core;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class AmountDebited : ModelEventBase
    {
        public decimal Amount { get; private set; }

        public AmountDebited(Guid modelId, int modelVersion, decimal amount)
            : base(modelId, modelVersion) =>
                Amount = amount;
    }
}
