using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Lab.EventSourcing.DomainEvents
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly ConcurrentDictionary<Type, List<IDomainEventHandler>> _handlers = 
                new ConcurrentDictionary<Type, List<IDomainEventHandler>>();

        public void RegisterHandler<TEvent>(IDomainEventHandler handler)
            where TEvent : IDomainEvent
        {
            if (_handlers.ContainsKey(typeof(TEvent)) 
                && _handlers[typeof(TEvent)].Any(h => h.GetType() == handler.GetType()))
                    throw new ArgumentException($"Handler of type {handler.GetType()} already registered.", nameof(handler));
            
            _handlers.AddOrUpdate(typeof(TEvent), 
                                  new List<IDomainEventHandler> { handler }, 
                                  (type, list) => { list.Add(handler); return list; });
        }

        public void Dispatch(IEnumerable<IDomainEvent> domainEvents)
        {
            if (domainEvents is null)
                throw new ArgumentNullException("A domain event collection must be provided.", nameof(domainEvents));

            foreach(var domainEvent in domainEvents)
                foreach (var handler in _handlers[domainEvent.GetType()])
                    handler.Handle(domainEvent);
        }
    }
}
