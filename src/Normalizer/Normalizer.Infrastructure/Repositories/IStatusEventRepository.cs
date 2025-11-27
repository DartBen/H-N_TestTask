using Normalizer.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Normalizer.Infrastructure.Repositories
{
    public interface IStatusEventRepository
    {
        void Add(LineStatusChangedEvent @event);
        IReadOnlyList<LineStatusChangedEvent> GetEventsSince(DateTime since);
    }
}
