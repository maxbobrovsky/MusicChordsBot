using System;
using Telegram.Bot;
namespace AccordsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBotClient(AppSettings.TelegramToken);
            bot.DeleteWebhookAsync();
            bot.StartReceiving();
            bot.OnMessage += ControlMessage.MessageReceiving;
            bot.OnCallbackQuery += CallbackProcessing.HandleCallbackQuery;
            Console.ReadLine();
        }

        
    }
}
