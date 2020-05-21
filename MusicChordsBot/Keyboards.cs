using Telegram.Bot.Types.ReplyMarkups;

namespace AccordsBot
{
    class Keyboards
    {
        public static ReplyKeyboardMarkup GetMainMenu()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                     new []
                    {
                        new KeyboardButton("ПОПУЛЯРНЫЕ 🔧"),
                         new KeyboardButton("ПОИСК 🔎"),
                    },
                    
                    // new []
                    //{
                    //    new KeyboardButton("ПОИСК 🔎"),
                       
                    //},
                    new []
                    {
                        new KeyboardButton("ПЛЕЙЛИСТ  🎼"),
                          new KeyboardButton("НАСТРОЙКИ 🔧"),

                    },
                    //new []
                    //{
                      
                    //},

                   
                },
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup GetSettingsMenu()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new []
                    {
                        new KeyboardButton("Сменить количество результатов 🎲"),
                    },
                   
                    new []
                    {
                        new KeyboardButton("Связаться с разработчиком 📝"),
                    },
                    new[]
                    {
                        new KeyboardButton("Вернуться в Главное Меню 🚀")
                    }
                },
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup GetNumberKeyboard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                    new []
                    {
                        new KeyboardButton("1"),
                    },
                    new []
                    {
                        new KeyboardButton("2"),
                    },
                    new []
                    {
                        new KeyboardButton("3"),
                    },
                    new []
                    {
                        new KeyboardButton("5"),
                    },
                    new []
                    {
                        new KeyboardButton("10"),
                    }
                },
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public static ReplyKeyboardRemove GetRemoveKeyboard()
        {
            return new ReplyKeyboardRemove();
        }

        public static ReplyKeyboardMarkup GetReturnKeyboard()
        {
            return new ReplyKeyboardMarkup( new[]
            {
                new []
                {
                    new KeyboardButton("Вернуться в Главное Меню 🚀")
                },
            },
            resizeKeyboard: true);
        }

        public static InlineKeyboardMarkup GetSongMenuKeyboard(bool mode) => new InlineKeyboardMarkup(
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(mode?"Добавить в избранные ✅":"", "addToFavourite"),
                        InlineKeyboardButton.WithCallbackData(mode?"":"Удалить из избранных ❌","removeFromFavourite")
                    }
                });
    }
}
