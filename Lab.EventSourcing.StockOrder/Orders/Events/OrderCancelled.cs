using Lab.EventSourcing.Core;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class OrderCancelled : ModelEventBase
    {
        public OrderCancelled(Guid modelId, int modelVersion)
            : base(modelId, modelVersion) { }
    }
}
