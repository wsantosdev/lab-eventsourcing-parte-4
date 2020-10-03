using Lab.EventSourcing.Core;
using Lab.EventSourcing.StockOrder.Orders.DomainEvents;
using System;
using System.Collections.Generic;

namespace Lab.EventSourcing.StockOrder
{
    public class Order : EventSourcingModel<Order>
    {
        public Guid AccountId { get; private set; }
        public OrderSide Side { get; private set; }
        public string Symbol { get; private set; }
        public uint Quantity { get; private set; }
        public decimal Price { get; private set; }
        public OrderStatus Status { get; private set; }
        
        protected Order(IEnumerable<ModelEventBase> persistedEvents = null) 
            : base(persistedEvents) { }

        public static Order Create(Guid accountId, OrderSide side, string symbol, uint quantity, decimal price)
        {
            var order = new Order();
            order.RaiseEvent(new OrderCreated(Guid.NewGuid(), accountId, side, symbol, quantity, price));
            if (side == OrderSide.Buy)
                order.AddDomainEvent(new BuyOrderCreatedDomainEvent(accountId, quantity * price));

            return order;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.New)
                throw new InvalidOperationException("Only new orders can be cancelled.");

            RaiseEvent(new OrderCancelled(Id, NextVersion));

            if(Side == OrderSide.Buy)
                AddDomainEvent(new BuyOrderCancelledDomainEvent(AccountId, Price * Quantity));
        }

        protected override void Apply(IEvent pendingEvent)
        {
            switch (pendingEvent)
            {
                case OrderCreated created:
                    Apply(created);
                    break;
                case OrderCancelled executed:
                    Apply(executed);
                    break;
                default:
                    throw new ArgumentException($"Unsuported event type {pendingEvent.GetType()}", nameof(pendingEvent));
            }
        }

        private void Apply(OrderCreated created)
        {
            Id = created.ModelId;
            AccountId = created.AccountId;
            Side = created.Side;
            Symbol = created.Symbol;
            Quantity = created.Quantity;
            Price = created.Price;
            Status = OrderStatus.New;
        }

        private void Apply(OrderCancelled executed) =>
            Status = OrderStatus.Cancelled;
    }
}