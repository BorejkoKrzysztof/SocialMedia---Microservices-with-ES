using System;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStore _eventStoreRepository;
        public EventStore(IEventStore eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException();

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    Timespan = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await _eventStoreRepository.SaveEventAsync(eventModel);
            }
        }


        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (eventStream is null || !eventStream.Any())
                throw new AggregateNotFoundException("Incorrect pass ID provided");

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }
    }
}
