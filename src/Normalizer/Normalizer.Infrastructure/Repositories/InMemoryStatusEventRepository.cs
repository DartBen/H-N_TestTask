using Normalizer.Domain.Events;
using System.Collections.Concurrent;

namespace Normalizer.Infrastructure.Repositories
{
    public class InMemoryStatusEventRepository : IStatusEventRepository
    {
        private readonly ConcurrentBag<LineStatusChangedEvent> _events = new();

        public void Add(LineStatusChangedEvent @event)
        {
            _events.Add(@event);
        }

        public IReadOnlyList<LineStatusChangedEvent> GetEventsSince(DateTime since)
        {
            return _events.Where(e => e.Timestamp >= since).ToList();
        }
    }
}
