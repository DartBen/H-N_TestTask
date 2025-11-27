using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Simulator.Infrastructure.Options;
using Simulator.Infrastructure.Services;

namespace Simulator.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimulatorInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SimulatorOptions>(
                configuration.GetSection(SimulatorOptions.SectionName));

            services.AddSingleton<ISimulatorEngine, SimulatorEngine>();

            services.AddSingleton<SimulatorStateService>();
            services.AddHostedService(sp => sp.GetRequiredService<SimulatorStateService>());

            services.AddSingleton<ISimulatorStateService>(sp => sp.GetRequiredService<SimulatorStateService>());

            return services;
        }
    }
}