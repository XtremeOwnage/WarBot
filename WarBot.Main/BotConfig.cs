using Newtonsoft.Json;
using System.IO;

namespace WarBot
{
    /// <summary>
    /// This class will store bot-specific configuration items.
    /// </summary>
    public class BotConfig
    {
        private static JsonSerializerSettings storageSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
        };
        public static BotConfig Load()
        {
            var Text = File.ReadAllText("./Config/BotConfig");
            var Cfg = JsonConvert.DeserializeObject<BotConfig>(Text, storageSettings);
            Cfg.OnConfigLoaded();

            return Cfg;
        }
        public void Save()
        {
            //Make sure the config is loaded, before we save it....
            if (ConfigLoaded)
            {
                var Text = JsonConvert.SerializeObject(this, storageSettings);
                File.WriteAllText("./Config/BotConfig", Text);
            }
        }

        private void OnConfigLoaded()
        {
            this.ConfigLoaded = true;
        }
        private void PropertyChanged(string Property)
        {
            //Save the config.
            Save();
        }
        /// <summary>
        /// Discord Bot List, API Token.
        /// </summary>
        public string BotList_API_Token
        {
            get => this.apiToken_BotList;
            set
            {
                this.apiToken_BotList = value;
                PropertyChanged(nameof(BotList_API_Token));
            }
        }

        /// <summary>
        /// Discord API token, for bot.
        /// </summary>
        public string Discord_API_Token
        {
            get => this.apiToken_Discord;
            set
            {
                this.apiToken_Discord = value;
                PropertyChanged(nameof(Discord_API_Token));
            }
        }

        /// <summary>
        /// The Bot's ID.
        /// </summary>
        /// 
        public ulong BotId
        {
            get => botID;
            set
            {
                this.botID = value;
                PropertyChanged(nameof(BotId));
            }
        }

        public string GuildConfigPath
        {
            get => guildConfigPath;
            set
            {
                this.guildConfigPath = value;
                PropertyChanged(nameof(GuildConfigPath));
            }
        }
        public Core.Environment Environment
        {
            get => environment;
            set
            {
                this.environment = value;
                PropertyChanged(nameof(Environment));
            }
        }

        public string ConnString
        {
            get => db_Connection_String;
            set
            {
                this.db_Connection_String = value;
                PropertyChanged(nameof(ConnString));
            }
        }


        #region Private Fields
        private bool ConfigLoaded = false;
        private string apiToken_BotList;
        private string apiToken_Discord;
        private ulong botID;
        private string guildConfigPath;
        private Core.Environment environment;
        private string db_Connection_String;
        #endregion
    }
}
