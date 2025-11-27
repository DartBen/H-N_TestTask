using Normalizer.Domain.Entities;
using Normalizer.Domain.Enums;
using Normalizer.Infrastructure.Options;
using Normalizer.Infrastructure.Services;
using Xunit;

namespace Normalizer.Tests
{
    public class StatusCalculatorTests
    {
        private readonly StatusCalculator _calculator;
        private readonly NormalizerSettings _settings;

        public StatusCalculatorTests()
        {
            _calculator = new StatusCalculator();
            _settings = new NormalizerSettings
            {
                NominalSpeedPcsPerHour = 10_000,
                LowSpeedThresholdFactor = 0.95,
                StoppedThresholdSeconds = 10,
                NoDataThresholdSeconds = 5
            };
        }

        [Fact]
        public void Calculate_Should_Return_NoData_When_Reading_Is_Null()
        {
            var (status, speed) = _calculator.Calculate(
                current: null,
                previous: null,
                lastFetchTime: DateTime.UtcNow.AddSeconds(-10),
                now: DateTime.UtcNow,
                settings: _settings);

            Assert.Equal(LineStatus.NoData, status);
            Assert.Equal(0.0, speed);
        }

        [Fact]
        public void Calculate_Should_Return_Stopped_When_Counter_Not_Changed_For_More_Than_Threshold()
        {
            var now = DateTime.UtcNow;
            var previous = new CounterReading { Timestamp = now.AddSeconds(-15), CounterValue = 1000 };
            var current = new CounterReading { Timestamp = now, CounterValue = 1000 };

            var (status, speed) = _calculator.Calculate(
                current: current,
                previous: previous,
                lastFetchTime: now,
                now: now,
                settings: _settings);

            Assert.Equal(LineStatus.Stopped, status);
            Assert.Equal(0.0, speed);
        }

        [Fact]
        public void Calculate_Should_Return_LowSpeed_When_Speed_Below_Threshold()
        {
            var now = DateTime.UtcNow;
            var previous = new CounterReading { Timestamp = now.AddSeconds(-3600), CounterValue = 0 };
            var current = new CounterReading { Timestamp = now, CounterValue = 9000 };

            var (status, speed) = _calculator.Calculate(
                current: current,
                previous: previous,
                lastFetchTime: now,
                now: now,
                settings: _settings);

            Assert.Equal(LineStatus.LowSpeed, status);
            Assert.True(speed > 8999 && speed < 9001);
        }

        [Fact]
        public void Calculate_Should_Return_Running_When_Speed_Above_Threshold()
        {
            var now = DateTime.UtcNow;
            var previous = new CounterReading { Timestamp = now.AddSeconds(-3600), CounterValue = 0 };
            var current = new CounterReading { Timestamp = now, CounterValue = 9600 };

            var (status, speed) = _calculator.Calculate(
                current: current,
                previous: previous,
                lastFetchTime: now,
                now: now,
                settings: _settings);

            Assert.Equal(LineStatus.Running, status);
            Assert.True(speed > 9599 && speed < 9601);
        }

        [Fact]
        public void Calculate_Should_Handle_Counter_Reset()
        {
            var now = DateTime.UtcNow;
            var previous = new CounterReading { Timestamp = now.AddSeconds(-1), CounterValue = 9990 };
            var current = new CounterReading { Timestamp = now, CounterValue = 10 };

            var (status, speed) = _calculator.Calculate(
                current: current,
                previous: previous,
                lastFetchTime: now,
                now: now,
                settings: _settings);

            Assert.True(speed > 70_000);
        }
    }
}