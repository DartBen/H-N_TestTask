using Simulator.Domain.Enums;
using Simulator.Infrastructure.Services;
using Xunit;

namespace Simulator.Tests
{
    public class SimulatorEngineTests
    {
        private readonly SimulatorEngine _engine;

        public SimulatorEngineTests()
        {
            _engine = new SimulatorEngine();
        }

        [Theory]
        [InlineData(0, SimulatorState.Normal)]
        [InlineData(44.9, SimulatorState.Normal)]
        [InlineData(45, SimulatorState.LowSpeed)]
        [InlineData(89.9, SimulatorState.LowSpeed)]
        [InlineData(90, SimulatorState.Stopped)]
        [InlineData(134.9, SimulatorState.Stopped)]
        [InlineData(135, SimulatorState.NoData)]
        [InlineData(179.9, SimulatorState.NoData)]
        [InlineData(180, SimulatorState.Normal)]
        [InlineData(224.9, SimulatorState.Normal)]
        public void Advance_Should_Return_Correct_State_Based_On_Total_Seconds_In_State(
            double totalSecondsInState,
            SimulatorState expectedState)
        {
            // Act
            var (actualState, _) = _engine.Advance(
                totalSecondsInCurrentState: totalSecondsInState,
                nominalSpeedPcsPerHour: 10_000,
                maxPhysicalSpeedPcsPerSec: 3.0,
                lowSpeedThresholdFactor: 0.95,
                maxCounterValue: 10_000,
                elapsedSecondsForIncrement: 1.0);

            // Assert
            Assert.Equal(expectedState, actualState);
        }

        [Fact]
        public void Advance_Should_Return_Zero_Increment_When_State_Is_Stopped()
        {
            // Act
            var (_, increment) = _engine.Advance(
                totalSecondsInCurrentState: 100, // Stopped
                nominalSpeedPcsPerHour: 10_000,
                maxPhysicalSpeedPcsPerSec: 3.0,
                lowSpeedThresholdFactor: 0.95,
                maxCounterValue: 10_000,
                elapsedSecondsForIncrement: 1.5);

            // Assert
            Assert.Equal(0, increment);
        }

        [Fact]
        public void Advance_Should_Return_Zero_Increment_When_State_Is_NoData()
        {
            // Act
            var (_, increment) = _engine.Advance(
                totalSecondsInCurrentState: 150, // NoData
                nominalSpeedPcsPerHour: 10_000,
                maxPhysicalSpeedPcsPerSec: 3.0,
                lowSpeedThresholdFactor: 0.95,
                maxCounterValue: 10_000,
                elapsedSecondsForIncrement: 2.0);

            // Assert
            Assert.Equal(0, increment);
        }

        [Fact]
        public void Advance_Should_Return_Positive_Increment_When_State_Is_Normal()
        {
            // Act
            var (_, increment) = _engine.Advance(
                totalSecondsInCurrentState: 10, // Normal
                nominalSpeedPcsPerHour: 10_000,
                maxPhysicalSpeedPcsPerSec: 3.0,
                lowSpeedThresholdFactor: 0.95,
                maxCounterValue: 10_000,
                elapsedSecondsForIncrement: 1.0);

            Assert.True(increment > 0);
            Assert.InRange(increment, 2, 3);
        }

        [Fact]
        public void Advance_Should_Return_Smaller_Increment_When_State_Is_LowSpeed()
        {
            // Act
            var (_, increment) = _engine.Advance(
                totalSecondsInCurrentState: 50, // LowSpeed
                nominalSpeedPcsPerHour: 10_000,
                maxPhysicalSpeedPcsPerSec: 3.0,
                lowSpeedThresholdFactor: 0.95,
                maxCounterValue: 10_000,
                elapsedSecondsForIncrement: 1.0);

            Assert.Equal(2, increment);
        }
    }
}