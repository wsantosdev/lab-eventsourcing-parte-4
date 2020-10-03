using Lab.EventSourcing.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Lab.EventSourcing.Core
{
    public class EventStore
    {
        private readonly EventStoreDbContext _eventStoreContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        
        public static EventStore Create(IDomainEventDispatcher domainEventDispatcher) =>
            new EventStore(domainEventDispatcher);

        private EventStore(IDomainEventDispatcher domainEventDispatcher)
        {
            _eventStoreContext = new EventStoreDbContext(new DbContextOptionsBuilder<EventStoreDbContext>()
                                                                .UseInMemoryDatabase(databaseName: "EventStore")
                                                                .EnableSensitiveDataLogging()
                                                                .Options);
            _domainEventDispatcher = domainEventDispatcher;
        }

        public void Commit<TModel>(TModel model) where TModel : EventSourcingModel<TModel>
        {
            var events = model.PendingEvents.Select(e => Event.Create(model.Id,
                                                                      ((ModelEventBase)e).ModelVersion,
                                                                      e.GetType().AssemblyQualifiedName,
                                                                      ((ModelEventBase)e).When,
                                                                      JsonConvert.SerializeObject(e)));

            _eventStoreContext.Events.AddRange(events);
            _eventStoreContext.SaveChanges();

            _domainEventDispatcher.Dispatch(model.DomainEvents);

            model.Commit();
        }

        public IEnumerable<ModelEventBase> GetById(Guid id) =>
            GetEvents(e => e.ModelId == id);

        public IEnumerable<ModelEventBase> GetByVersion(Guid id, int version) =>
            GetEvents(e => e.ModelId == id && e.ModelVersion <= version);

        public IEnumerable<ModelEventBase> GetByTime(Guid id, DateTime until) =>
            GetEvents(e => e.ModelId == id && e.When <= until);

        private IEnumerable<ModelEventBase> GetEvents(Expression<Func<Event, bool>> expression)
        {
            var events = _eventStoreContext.Events.Where(expression)
                                     .OrderBy(e => e.ModelVersion)
                                     .Select(e => JsonConvert.DeserializeObject(e.Data, Type.GetType(e.Type)))
                                     .Cast<ModelEventBase>();
            
            return events;
        }

        private class EventStoreDbContext : DbContext
        {
            public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options) { }

            public DbSet<Event> Events { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder) =>
                modelBuilder.Entity<Event>().HasKey(k => new { k.ModelId, k.ModelVersion });
        }
    }
}
