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
            //Make it easy to read with vscode/notepad/etc...
            Formatting = Formatting.Indented,
        };
        public static BotConfig Load()
        {
            var Text = File.ReadAllText("./Config/BotConfig");
            var Cfg = JsonConvert.DeserializeObject<BotConfig>(Text, storageSettings);

            return Cfg;
        }

        //Commented- Because, there should never  be a use where we have to save these settings. Changes should be performed via text-editor.
        //public void Save()
        //{
        //    var Text = JsonConvert.SerializeObject(this, storageSettings);
        //    File.WriteAllText("./Config/BotConfig", Text);
        //}


        public string Discord_API_Token { get; set; }
        public ulong BotId { get; set; }
        public string GuildConfigPath { get; set; }
        public ulong Log_CH_Chat { get; set; }
        public ulong Log_CH_Debug { get; set; }
        public ulong Log_CH_Errors { get; set; }
        public ulong Log_CH_Guilds { get; set; }
    }
}
