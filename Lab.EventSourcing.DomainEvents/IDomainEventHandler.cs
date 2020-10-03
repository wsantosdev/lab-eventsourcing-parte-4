namespace Lab.EventSourcing.DomainEvents
{
    public interface IDomainEventHandler
    {
        void Handle(IDomainEvent domainEvent);
    }
}
