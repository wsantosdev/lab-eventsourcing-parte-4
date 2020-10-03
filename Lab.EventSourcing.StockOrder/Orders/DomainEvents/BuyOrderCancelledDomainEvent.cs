using Lab.EventSourcing.DomainEvents;
using System;

namespace Lab.EventSourcing.StockOrder.Orders.DomainEvents
{
    public class BuyOrderCancelledDomainEvent : IDomainEvent
    {
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }

        public BuyOrderCancelledDomainEvent(Guid accountId, decimal amount) =>
            (AccountId, Amount) = (accountId, amount);
    }
}