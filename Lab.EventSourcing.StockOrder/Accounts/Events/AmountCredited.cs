using Lab.EventSourcing.Core;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class AmountCredited : ModelEventBase
    {
        public decimal Amount { get; private set; }

        public AmountCredited(Guid modelId, int modelVersion, decimal amount)
            : base(modelId, modelVersion) =>
                Amount = amount;
    }
}
