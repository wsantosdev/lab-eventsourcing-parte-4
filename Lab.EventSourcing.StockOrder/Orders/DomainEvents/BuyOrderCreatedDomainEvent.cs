using Lab.EventSourcing.DomainEvents;
using System;

namespace Lab.EventSourcing.StockOrder.Orders.DomainEvents
{
    public class BuyOrderCreatedDomainEvent : IDomainEvent
    {
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }

        public BuyOrderCreatedDomainEvent(Guid accountId, decimal amount) =>
            (AccountId, Amount) = (accountId, amount);
    }
}
