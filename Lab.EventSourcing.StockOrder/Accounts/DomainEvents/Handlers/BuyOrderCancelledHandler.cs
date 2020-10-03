using Lab.EventSourcing.Core;
using Lab.EventSourcing.DomainEvents;
using Lab.EventSourcing.StockOrder.Orders.DomainEvents;
using System;

namespace Lab.EventSourcing.StockOrder
{
    public class BuyOrderCancelledHandler : IDomainEventHandler
    {
        private readonly EventStore _eventStore;

        public BuyOrderCancelledHandler(EventStore eventStore) =>
            _eventStore = eventStore;

        public void Handle(IDomainEvent domainEvent)
        {
            var buyOrderCancelledEvent = domainEvent as BuyOrderCancelledDomainEvent;
            if (buyOrderCancelledEvent is null)
                throw new ArgumentException($"Unsuported event type {domainEvent.GetType()}.");

            var account = Account.Load(_eventStore.GetById(buyOrderCancelledEvent.AccountId));
            account.Credit(buyOrderCancelledEvent.Amount);

            _eventStore.Commit(account);
        }
    }
}
