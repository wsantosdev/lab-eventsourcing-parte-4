using Lab.EventSourcing.Core;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class OrderCreated : ModelEventBase
    {
        public Guid AccountId { get; private set; }
        public OrderSide Side { get; private set; }
        public string Symbol { get; private set; }
        public uint Quantity { get; private set; }
        public decimal Price { get; private set; }

        public OrderCreated(Guid modelId, Guid accountId, OrderSide side, string symbol, uint quantity, decimal price) 
            : base(modelId, 1) =>
                (AccountId, Side, Symbol, Quantity, Price) = (accountId, side, symbol, quantity, price);
    }
}
