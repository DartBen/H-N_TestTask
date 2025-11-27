using Normalizer.Domain.Entities;
using Normalizer.Domain.Enums;
using Normalizer.Infrastructure.Options;

namespace Normalizer.Infrastructure.Services
{
    public class StatusCalculator : IStatusCalculator
    {
        public (LineStatus Status, double Speed) Calculate(
            CounterReading? current, 
            CounterReading? previous, 
            DateTime lastFetchTime,
            DateTime now,
            NormalizerSettings settings)
        {
            if (current == null)
                return (LineStatus.NoData, 0.0);

            bool counterChanged = previous == null || current.CounterValue != previous.CounterValue;
            double speed = 0.0;

            if (counterChanged && previous != null && current.Timestamp > previous.Timestamp)
            {
                var counterDiff = CalculateCounterDiff(
                    current.CounterValue,
                    previous.CounterValue,
                    maxCounterValue: 10_000);
                var timeDiffHours = (current.Timestamp - previous.Timestamp).TotalHours;
                if (timeDiffHours > 0)
                    speed = counterDiff / timeDiffHours;
            }

            // Проверка NoData по времени последнего запроса
            if ((now - lastFetchTime).TotalSeconds > settings.NoDataThresholdSeconds)
                return (LineStatus.NoData, speed);

            if (counterChanged)
            {
                var status = speed < settings.NominalSpeedPcsPerHour * settings.LowSpeedThresholdFactor
                    ? LineStatus.LowSpeed
                    : LineStatus.Running;
                return (status, speed);
            }
            else
            {
                // Проверка Stopped по времени последнего изменения счётчика
                var lastChangeTime = previous?.Timestamp ?? now;
                if ((now - lastChangeTime).TotalSeconds >= settings.StoppedThresholdSeconds)
                    return (LineStatus.Stopped, 0.0);
                else
                    return (LineStatus.Running, speed);
            }
        }
        private static long CalculateCounterDiff(long current, long previous, long maxCounterValue)
        {
            return current >= previous
                ? current - previous
                : (maxCounterValue - previous) + current;
        }
    }
}
