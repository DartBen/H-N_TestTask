namespace Normalizer.Application
{
    public static class PeriodParser
    {
        /// <summary>
        /// Для парсинга строк в api
        /// </summary>
        /// <param name="period">Поддерживаем форматы: "1h", "24h", "7d", "30d"</param>
        /// <returns></returns>
        public static DateTime ParsePeriod(string? period)
        {
            if (string.IsNullOrWhiteSpace(period))
                return DateTime.UtcNow.AddHours(-24);

            // 
            var input = period.Trim().ToLowerInvariant();

            if (input.EndsWith("h") && long.TryParse(input[..^1], out var hours))
            {
                return DateTime.UtcNow.AddHours(-hours);
            }

            if (input.EndsWith("d") && long.TryParse(input[..^1], out var days))
            {
                return DateTime.UtcNow.AddDays(-days);
            }

            return DateTime.UtcNow.AddHours(-24);
        }
    }
}
