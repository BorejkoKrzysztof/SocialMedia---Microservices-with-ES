using System;

namespace CQRS.Core.Handlers
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregate);
        Task<T> GetByIdAsync(Guid id);
    }
}
