using Normalizer.Domain.Entities;
using Normalizer.Domain.Enums;
using Normalizer.Infrastructure.Options;

namespace Normalizer.Infrastructure.Services
{
    public interface IStatusCalculator
    {
        (LineStatus Status, double Speed) Calculate(
            CounterReading? current,
            CounterReading? previous,
            DateTime lastFetchTime,
            DateTime now,
            NormalizerSettings settings);
    }
}
