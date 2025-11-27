using Simulator.Domain.Entities;
using Simulator.Domain.Enums;

namespace Simulator.Infrastructure.Services
{
    public interface ISimulatorStateService
    {
        CounterReading GetCurrentReading();
        SimulatorState CurrentState { get; }
    }
}
