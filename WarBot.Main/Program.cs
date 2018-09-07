using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WarBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var Bot = new WARBOT();

            Bot.Start().Wait();

            try
            {
                //Block the main thread, until the bot stops.
                Task.Delay(-1, Bot.StopToken.Token).Wait();
                Console.WriteLine("Exiting WARBOT.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

 
}
