using Lab.EventSourcing.Core;
using Lab.EventSourcing.DomainEvents;
using Lab.EventSourcing.StockOrder.Orders.DomainEvents;
using Xunit;

namespace Lab.EventSourcing.StockOrder.Test
{
    public class DomainEventsCase
    {
        [Fact]
        public void Should_Debit_On_Order_Creation()
        {
            //Arrange
            var domainEventDispatcher = new DomainEventDispatcher();
            var eventStore = EventStore.Create(domainEventDispatcher);

            domainEventDispatcher.RegisterHandler<BuyOrderCreatedDomainEvent>(new BuyOrderCreatedHandler(eventStore));
            

            //Act
            var account = Account.Create(1_000);
            eventStore.Commit(account);
            
            var order = Order.Create(account.Id, OrderSide.Buy, "PETR4", 10, 10M);
            eventStore.Commit(order);

            //Assert
            var storedAccount = Account.Load(eventStore.GetById(account.Id));
            Assert.Equal(900, storedAccount.Ballance);
        }

        [Fact]
        public void Should_Credit_On_Order_Cancel()
        {
            //Arrange
            var domainEventDispatcher = new DomainEventDispatcher();
            var eventStore = EventStore.Create(domainEventDispatcher);

            domainEventDispatcher.RegisterHandler<BuyOrderCreatedDomainEvent>(new BuyOrderCreatedHandler(eventStore));
            domainEventDispatcher.RegisterHandler<BuyOrderCancelledDomainEvent>(new BuyOrderCancelledHandler(eventStore));

            //Act
            var account = Account.Create(1_000);
            eventStore.Commit(account);

            var order = Order.Create(account.Id, OrderSide.Buy, "PETR4", 10, 10M);
            eventStore.Commit(order);
            
            order.Cancel();
            eventStore.Commit(order);

            //Assert
            var storedAccount = Account.Load(eventStore.GetById(account.Id));
            Assert.Equal(3, storedAccount.Version);
            Assert.Equal(1_000, storedAccount.Ballance);
        }
    }
}
