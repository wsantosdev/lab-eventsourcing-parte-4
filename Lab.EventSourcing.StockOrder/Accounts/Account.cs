using Lab.EventSourcing.Core;
using System;
using System.Collections.Generic;

namespace Lab.EventSourcing.StockOrder
{
    public class Account : EventSourcingModel<Account>
    {
        public static readonly Account Empty = new Account();

        protected Account(IEnumerable<ModelEventBase> persistedEvents = null) 
            : base(persistedEvents) { }

        public decimal Ballance { get; private set; }

        public static Account Create(decimal initialDeposit)
        {
            if (initialDeposit < 0)
                throw new ArgumentException("A non-negative amount must be provided.", nameof(initialDeposit));

            var account = new Account();
            account.RaiseEvent(new AccountCreated(Guid.NewGuid(), initialDeposit));

            return account;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("A positive amount must be provided.", nameof(amount));

            RaiseEvent(new AmountCredited(Id, NextVersion, amount));
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("A positive amount must be provided.", nameof(amount));

            if (amount > Ballance)
                throw new ArgumentException($"Insuficient funds. Total available: {Ballance:C2}", nameof(amount));

            RaiseEvent(new AmountDebited(Id, NextVersion, amount));
        }

        protected override void Apply(IEvent pendingEvent)
        {
            switch (pendingEvent)
            {
                case AccountCreated created:
                    Apply(created);
                    break;
                case AmountCredited deposited:
                    Apply(deposited);
                    break;
                case AmountDebited withdrawn:
                    Apply(withdrawn);
                    break;
                default:
                    throw new ArgumentException($"Unsuported event type { pendingEvent.GetType() }", nameof(pendingEvent));
            }
        }

        private void Apply(AccountCreated created)
        {
            Id = created.ModelId;
            Ballance = created.InitialDeposit;
        }

        private void Apply(AmountCredited deposited) =>
            Ballance += deposited.Amount;

        private void Apply(AmountDebited withdrawn) =>
            Ballance -= withdrawn.Amount;
    }
}