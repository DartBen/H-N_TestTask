using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Normalizer.Infrastructure.Options;
using Normalizer.Infrastructure.Repositories;
using Normalizer.Infrastructure.Services;

namespace Normalizer.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет зависимости нормализатора.
        /// </summary>
        public static IServiceCollection AddNormalizerInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<NormalizerSettings>(
                configuration.GetSection(NormalizerSettings.SectionName));

            services.AddHttpClient();

            services.AddSingleton<IStatusEventRepository, InMemoryStatusEventRepository>();

            services.AddSingleton<IStatusCalculator, StatusCalculator>();
            services.AddSingleton<INormalizerOrchestrator, NormalizerOrchestrator>();

            services.AddSingleton<NormalizerBackgroundService>();
            services.AddHostedService(sp => sp.GetRequiredService<NormalizerBackgroundService>());
            services.AddSingleton<INormalizerService>(sp =>
                sp.GetRequiredService<NormalizerBackgroundService>());

            services.AddSingleton<NormalizerSettings>(sp =>
            {
                var opts = sp.GetRequiredService<IOptions<NormalizerSettings>>().Value;
                return new NormalizerSettings
                {
                    SimulatorUrl = opts.SimulatorUrl,
                    NominalSpeedPcsPerHour = opts.NominalSpeedPcsPerHour,
                    LowSpeedThresholdFactor = opts.LowSpeedThresholdFactor,
                    StoppedThresholdSeconds = opts.StoppedThresholdSeconds,
                    NoDataThresholdSeconds = opts.NoDataThresholdSeconds,
                    PollingIntervalMs = opts.PollingIntervalMs
                };
            });

            return services;
        }
    }
}
