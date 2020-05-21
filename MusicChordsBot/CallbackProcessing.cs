using Telegram.Bot;
using Telegram.Bot.Args;

namespace AccordsBot
{
    public class CallbackProcessing
    {
        public static async void HandleCallbackQuery(object sender, CallbackQueryEventArgs queryEventArgs)
        {
            var Client = sender as TelegramBotClient;
            var Callback = queryEventArgs.CallbackQuery;

            User currentUser = new User(Callback.From.Id);
            try
            {
                switch (Callback.Data)
                {
                    case "addToFavourite":
                        await Client.EditMessageTextAsync(Callback.From.Id, Callback.Message.MessageId, Callback.Message.Text, replyMarkup: Keyboards.GetSongMenuKeyboard(false));

                        DataBase.UpdateString(currentUser.id, "IsFavourite", "1", "MessageId", Callback.Message.MessageId.ToString());
                        break;
                    case "removeFromFavourite":
                        await Client.EditMessageTextAsync(Callback.From.Id, Callback.Message.MessageId, Callback.Message.Text, replyMarkup: Keyboards.GetSongMenuKeyboard(true));
                        DataBase.UpdateString(currentUser.id, "IsFavourite", "0", "MessageId", Callback.Message.MessageId.ToString());
                        break;

                }
            }
            catch { }

            currentUser.SaveUser();
        }
    }
}