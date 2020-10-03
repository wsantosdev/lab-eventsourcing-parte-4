using System.Collections.Generic;

namespace Lab.EventSourcing.DomainEvents
{
    public interface IDomainEventDispatcher
    {
        void RegisterHandler<TEvent>(IDomainEventHandler handler)
            where TEvent : IDomainEvent;

        void Dispatch(IEnumerable<IDomainEvent> domainEvents);
    }
}
