using Lab.EventSourcing.Core;
using Lab.EventSourcing.DomainEvents;
using Lab.EventSourcing.StockOrder.Orders.DomainEvents;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class BuyOrderCreatedHandler : IDomainEventHandler
    {
        private readonly EventStore _eventStore;

        public BuyOrderCreatedHandler(EventStore eventStore) =>
            _eventStore = eventStore;

        public void Handle(IDomainEvent domainEvent)
        {
            var order = domainEvent as BuyOrderCreatedDomainEvent;
            if (order is null)
                throw new ArgumentException($"Unsuported event type {domainEvent.GetType()}.");

            var account = Account.Load(_eventStore.GetById(order.AccountId));
            account.Debit(order.Amount);

            _eventStore.Commit(account);
        }
    }
}
