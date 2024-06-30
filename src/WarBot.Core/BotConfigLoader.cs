using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;

namespace WarBot.Core
{
    public static class BotConfigLoader
    {
        public static void LoadConfig(IConfigurationRoot ConfigRoot = null)
        {
            {
                ConfigRoot ??= new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var botConfig = ConfigRoot.GetSection("BotConfig").Get<BotConfigModel>();

                BotConfig.SUPERADMIN_USER_IDS = GetConfigValue("SUPERADMIN_USER_IDS", botConfig.SUPERADMIN_USER_IDS);
                BotConfig.ADMIN_GUILDS = GetConfigValue("ADMIN_GUILDS", botConfig.ADMIN_GUILDS);
                BotConfig.DB_HOST = GetConfigValue("DB_HOST", botConfig.DB_HOST);
                BotConfig.DB_PORT = GetConfigValue("DB_PORT", botConfig.DB_PORT);
                BotConfig.DB_NAME = GetConfigValue("DB_NAME", botConfig.DB_NAME);
                BotConfig.DB_USER = GetConfigValue("DB_USER", botConfig.DB_USER);
                BotConfig.DB_PASS = GetConfigValue("DB_PASS", botConfig.DB_PASS);
                BotConfig.DISCORD_TOKEN = GetConfigValue("DISCORD_TOKEN", botConfig.DISCORD_TOKEN);
                BotConfig.DISCORD_ID = GetConfigValue("DISCORD_ID", botConfig.DISCORD_ID);
                BotConfig.DISCORD_SECRET = GetConfigValue("DISCORD_SECRET", botConfig.DISCORD_SECRET);
                BotConfig.PUBLIC_URL = GetConfigValue("PUBLIC_URL", botConfig.PUBLIC_URL);
            }
        }

        private static T GetConfigValue<T>(string key, T defaultValue)
        {
            var envValue = Environment.GetEnvironmentVariable(key);

            if (envValue != null)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)envValue;
                }
                else if (typeof(T).IsArray && typeof(T).GetElementType() == typeof(ulong))
                {
                    var stringValues = envValue.Split(',');
                    var ulongValues = Array.ConvertAll(stringValues, ulong.Parse);
                    return (T)(object)ulongValues;
                }
                else if (typeof(T) == typeof(ulong))
                {
                    return (T)(object)ulong.Parse(envValue);
                }
                else if (typeof(T) == typeof(int))
                {
                    return (T)(object)int.Parse(envValue);
                }
                else if (typeof(T) == typeof(uint))
                {
                    return (T)(object)uint.Parse(envValue);
                }
                else if (typeof(T) == typeof(double))
                {
                    return (T)(object)double.Parse(envValue);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
                }
            }

            if (defaultValue == null || defaultValue.Equals(default(T)))
            {
                throw new ArgumentNullException(key, $"{key} environment variable or configuration value is required.");
            }

            return defaultValue;
        }
    }
}
