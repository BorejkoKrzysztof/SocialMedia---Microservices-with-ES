using System;
using CQRS.Core.Domain;

namespace Post.Cmd.Infrastructure.Infrastructure
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;
        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InserOneAsync(@event).ConfigureAwait(false);
        }

        public async Task<List<EventModel>> FindAggregatebyId(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(x => x.AggregateIdentifier == @aggregateId)
                                                .ToListAsync().ConfigureAwait(false);
        }
    }
}
