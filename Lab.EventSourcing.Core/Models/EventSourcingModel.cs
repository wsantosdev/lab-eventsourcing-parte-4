using Lab.EventSourcing.DomainEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Lab.EventSourcing.Core
{
    public abstract class EventSourcingModel<T> where T : EventSourcingModel<T>
    {
        private readonly Queue<IEvent> _pendingEvents = new Queue<IEvent>();
        public IReadOnlyCollection<IEvent> PendingEvents { get => _pendingEvents; }

        private readonly Queue<IDomainEvent> _domainEvents = new Queue<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> DomainEvents { get => _domainEvents; }

        public Guid Id { get; protected set; }
        public int Version { get; protected set; } = 0;
        protected int NextVersion { get => Version + 1; }

        protected EventSourcingModel(IEnumerable<ModelEventBase> persistedEvents = null)
        {
            if (persistedEvents != null)
                ApplyPersistedEvents(persistedEvents);
        }

        public static T Load(IEnumerable<ModelEventBase> persistendEvents) =>
            (T)Activator.CreateInstance(typeof(T),
                                         BindingFlags.NonPublic | BindingFlags.Instance,
                                         null,
                                         new object[] { persistendEvents },
                                         CultureInfo.InvariantCulture);
                
        protected void ApplyPersistedEvents(IEnumerable<ModelEventBase> events)
        {
            foreach (var e in events)
            {
                Apply(e);
                Version = e.ModelVersion;
            }
        }

        protected void RaiseEvent<TEvent>(TEvent pendingEvent) where TEvent: ModelEventBase
        {
            Apply(pendingEvent);
            _pendingEvents.Enqueue(pendingEvent);
            Version = pendingEvent.ModelVersion;
        }

        protected abstract void Apply(IEvent pendingEvent);

        public void Commit()
        {
            _pendingEvents.Clear();
            _domainEvents.Clear();
        }

        protected void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Enqueue(domainEvent);
    }
}