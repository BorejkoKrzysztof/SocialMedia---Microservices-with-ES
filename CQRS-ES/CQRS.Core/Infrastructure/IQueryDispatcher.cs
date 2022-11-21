using System;

namespace CQRS.Core.Infrastructure
{
    public interface IQueryDispatcher<TEntity>
    {
        void RegisterQueryHandler<TQuery>(Func<TQuery, Task<list<TEntity>>> handler) where TQuery : BaseQuery;
        Task<List<TEntity>> SendAsync(BaseQuery query);
    }
}
