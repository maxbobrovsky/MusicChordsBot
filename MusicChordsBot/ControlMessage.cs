using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;
using xNet;
using Telegraph.Net.Models;
using Telegraph.Net;
using System.Net;

namespace AccordsBot
{
    class ControlMessage
    {
        public static async void MessageReceiving(object sender, MessageEventArgs args)
        {
            var client = sender as TelegramBotClient;
            var Message = args.Message;

            if (Message.Text == null)
            {
                return;
            }

            User currentUser = new User(Message.From.Id);

            Console.WriteLine($"{DateTime.Now} {Message.From.FirstName} {Message.From.LastName} {Message.From.Id} {Message.Text}");

            if (Message.Text == "Вернуться в Главное Меню 🚀")
            {
                currentUser.Stage = State.Stage.Default;
                currentUser.State = State.Categories.Default;
                try
                {
                    await client.SendTextMessageAsync(
                        Message.Chat.Id, "Выберите раздел меню 🎯",
                        replyMarkup:
                        Keyboards.GetMainMenu());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                currentUser.SaveUser();
                return;
            }

            if (currentUser.State == State.Categories.Default)
            {
                currentUser.State = GetMessageCategory(Message.Text);
            }

            switch (currentUser.State)
            {
                case State.Categories.Start:
                    {
                        SendStartMessage(client, currentUser.id, Message.Chat.FirstName);
                        currentUser.State = State.Categories.Default;
                        break;
                    }

                case State.Categories.Search:
                    {

                        if (currentUser.Stage == State.Stage.Default)
                        {
                            try
                            {
                                await client.SendTextMessageAsync(
                                    Message.Chat.Id,
                                    "SEARCH 🔎\nЧто ты хочешь найти?\nВведите название песни или исполнителя ✏️",
                                    replyMarkup: Keyboards.GetReturnKeyboard());

                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.Message);
                            }

                            currentUser.Stage = State.Stage.Alpha;
                        }
                        else if (currentUser.Stage == State.Stage.Alpha)
                        {
                            SongProcessing(client, Message, currentUser);
                        }
                        break;
                    }   

                case State.Categories.Settings:
                    {
                        if (currentUser.Stage == State.Stage.Default)
                        {
                            try
                            {
                                await client.SendTextMessageAsync(
                                    Message.Chat.Id, "НАСТРОЙКИ ⚙️",
                                    replyMarkup:
                                    Keyboards.GetSettingsMenu());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            currentUser.Stage = State.Stage.Alpha;

                        }
                        else if (currentUser.Stage == State.Stage.Alpha)
                        {
                            switch (Message.Text)
                            {
                                case "Сменить количество результатов 🎲":
                                    {
                                        try
                                        {
                                            await client.SendTextMessageAsync(
                                                Message.Chat.Id, "Отправь количество результатов в разделе ПОИСК ✏️",
                                                replyMarkup:
                                                Keyboards.GetNumberKeyboard());
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                        currentUser.Stage = State.Stage.SearchLimit;
                                        break;
                                    }

                                case "Связаться с разработчиком 📝":
                                    {
                                        try
                                        {
                                            await client.SendTextMessageAsync(
                                                Message.Chat.Id, "Отправь свое сообщение ✏️ Оно будет прочитано разработчиком 📌",
                                                replyMarkup:
                                                Keyboards.GetRemoveKeyboard());
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                        currentUser.Stage = State.Stage.SendMessage;
                                        break;
                                    }
                                default:
                                    {
                                        try
                                        {
                                            await client.SendTextMessageAsync(
                                                Message.Chat.Id, "Выбери раздел меню 🎯",
                                                replyMarkup:
                                                Keyboards.GetSettingsMenu());
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                        break;
                                    }
                            }
                        }
                        else if (currentUser.Stage == State.Stage.SearchLimit)
                        {
                            int count;
                            if (int.TryParse(Message.Text, out count) && count <= 10)
                            {
                                currentUser.LimitResultsNumber = count;
                            }
                            else
                            {
                                currentUser.LimitResultsNumber = 5;
                            }
                            try
                            {
                                await client.SendTextMessageAsync(
                                    Message.Chat.Id, "НАСТРОЙКИ ⚙️\n" +
                                                     "Выбери раздел меню 🎯",
                                    replyMarkup:
                                    Keyboards.GetSettingsMenu());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            currentUser.Stage = State.Stage.Alpha;
                        }
                        else if (currentUser.Stage == State.Stage.SendMessage)
                        {
                            try
                            {
                                await client.SendTextMessageAsync(
                                    Message.Chat.Id, "Спасибо за твое сообщение 💎", replyMarkup: Keyboards.GetRemoveKeyboard());
                                await client.SendTextMessageAsync(
                                     Message.Chat.Id, "Настройки ⚙️\n" +
                                                      "Выбери раздел меню 🎯",
                                     replyMarkup:
                                     Keyboards.GetSettingsMenu());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            currentUser.Stage = State.Stage.Alpha;
                        }
                        break;
                    }
                case State.Categories.Popular:
                    {
                        try
                        {
                            var query = "https://amdm.ru/akkordi/popular/week/";

                            var urls = await FindSongs(query, 10);
                            if (urls.Count == 0)
                            {
                                client.SendTextMessageAsync(
                                    Message.Chat.Id, "Извините, раздел временно не работает!",
                                    replyMarkup: Keyboards.GetReturnKeyboard());
                                return;
                            }

                            client.SendTextMessageAsync(
                                Message.Chat.Id, "Пожалуйста, подожди несколько секунд ⏳",
                                replyMarkup: Keyboards.GetReturnKeyboard());

                            foreach (var urlPair in urls)
                            {
                                var songName = urlPair.Key;
                                var songUrl = urlPair.Value;
                                if (songUrl != string.Empty)
                                {
                                    string chordsUrl;
                                    if (DataBase.CheckSong(songName, out chordsUrl))
                                    {
                                        if (chordsUrl != string.Empty)
                                        {
                                            SendFilesByFileId(client, new Dictionary<string, string> { { chordsUrl, songName } }, currentUser.id, true);
                                            continue;
                                        }
                                    }
                                    SongSending(client, currentUser.id, songName, songUrl);

                                }
                            }

                            try
                            {
                                client.SendTextMessageAsync(
                                    Message.Chat.Id, $"Найденные {urls.Count} Песни 💎\n" +
                                                        $"Ты можешь сменить количество результатов в разделе НАСТРОЙКИ ⚙️",
                                    replyMarkup: Keyboards.GetReturnKeyboard());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    }
               case State.Categories.Playlist:
                {

                        var urls = DataBase.GetMusicLibrary(currentUser.id);

                        try
                        {
                            if (urls.Count == 0)
                            {
                                await client.SendTextMessageAsync(
                                Message.Chat.Id, "Тут будут твои любимые песни 🎯",
                                replyMarkup: Keyboards.GetMainMenu());
                            }
                            else
                            {
                                SendFilesByFileId(client, urls, currentUser.id, false);
                            }

                        }
                        catch (Exception ex) {
                        }

                        currentUser.State = State.Categories.Default;
                        break;
                }
                default:
                    {
                        try
                        {
                            await client.SendTextMessageAsync(
                                Message.Chat.Id, "Выбери рздел меню 🎯",
                                replyMarkup: Keyboards.GetMainMenu());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        currentUser.Stage = State.Stage.Default;
                        break;
                    }
            }
            currentUser.SaveUser();
        }


        private static async void SendFilesByFileId(TelegramBotClient Client, Dictionary<string, string> songs, long chatId, bool isFavourite, bool otherUser = false)
        {
            foreach (var song in songs)
            {
                var url = song.Key;
                var songName = song.Value;
                try
                {
                    var message = await Client.SendTextMessageAsync(chatId, url, replyMarkup: Keyboards.GetSongMenuKeyboard(isFavourite));
                    if (otherUser)
                    {
                        AddSongToBase(chatId, message.MessageId, url, songName);
                    }
                    else
                    {
                        DataBase.UpdateString(chatId, "MessageId", message.MessageId.ToString(), "url", url);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }



        public static async void SongProcessing(TelegramBotClient Client, Message Message, User currentUser)
        {
            try
            {
                var query = $"https://amdm.ru/search/?q={Message.Text}";

                var urls = await FindSongs(query, currentUser.LimitResultsNumber);
                if (urls.Count == 0)
                {
                    Client.SendTextMessageAsync(
                        Message.Chat.Id, "Трек не найден ❌",
                        replyMarkup: Keyboards.GetReturnKeyboard());
                    return;
                }

                Client.SendTextMessageAsync(
                    Message.Chat.Id, "Пожалуйста, подожди пару секунд ⏳",
                    replyMarkup: Keyboards.GetReturnKeyboard());

                foreach (var urlPair in urls)
                {
                    var songName = urlPair.Key;
                    var songUrl = urlPair.Value;
                    if (songUrl != string.Empty)
                    {
                        string chordsUrl;
                        if (DataBase.CheckSong(songName, out  chordsUrl))
                        {
                            if (chordsUrl != string.Empty)
                            {
                                SendFilesByFileId(Client, new Dictionary<string, string> { { chordsUrl, songName } }, currentUser.id, true);
                                continue;
                            }
                        }
                        await SongSending(Client, currentUser.id, songName, songUrl);

                    }
                }

            try
            {
                Client.SendTextMessageAsync(
                    Message.Chat.Id, $"Найденные {urls.Count} Песни 💎\n" +
                                        $"Ты можешь сменить количество результатов в разделе НАСТРОЙКИ ⚙️",
                    replyMarkup: Keyboards.GetReturnKeyboard());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static async Task<Dictionary<string, string>> FindSongs(string urlAddress, int limit)
        {
            var songCounter = 0;
            var songs = new Dictionary<string, string>();
            var website = new HtmlWeb();
            website.OverrideEncoding = System.Text.Encoding.GetEncoding("utf-8");
            try
            {
                HtmlDocument doc = website.Load(urlAddress);
                foreach (var node in doc?.DocumentNode?.SelectNodes("//*[@class=\"artist_name\"]"))
                {
                    try
                    {
                        var songName = node?.InnerText;
                        var songUrl = string.Format("https:{0}",node?.ChildNodes[2]?.Attributes[0]?.Value);
                        if (!songs.ContainsKey(songName))
                        {
                            songs.Add(songName, songUrl);
                            songCounter++;
                        }
                        
                        if (songCounter >= limit) {
                            return songs;
                        }
                        
                    }
                    catch (Exception ex){
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return songs;
        }

        public static async Task<bool> SongSending(TelegramBotClient client, long id, string songName, string songUrl) {
            var website = new HtmlWeb();
            website.OverrideEncoding = System.Text.Encoding.GetEncoding("utf-8");
            try
            {
                HtmlDocument doc = website.Load(songUrl);

                var songTextNode = doc?.DocumentNode?.SelectSingleNode("//*[@class=\"b-podbor__text\"]");

                var songText = songTextNode?.InnerText;

                var songsChordsImages = new List<string>();
                foreach (var node in doc?.DocumentNode?.SelectNodes("//*[@id=\"song_chords\"]"))
                {
                    foreach (var img in node?.ChildNodes)
                    {
                        songsChordsImages.Add($"https:{img?.Attributes[0]?.Value}");
                    }
                }
                var uniqueCode = DateTime.Now.Ticks;
                if (await DownloadImages(uniqueCode, songsChordsImages)) {
                    var content = new List<NodeElement>();
                    content.Add(new NodeElement("img", new Dictionary<string, string>
                    {
                        ["src"] = $"https://telegra.ph/file/6e0006d12cf4319268af4.jpg"
                    }));

                    var compositeImages = await CreateCompositeImages(uniqueCode, songsChordsImages.Count);

                    foreach (var image in compositeImages)
                    {
                        var url = await UploadImage(image);
                        content.Add(new NodeElement("img", new Dictionary<string, string> { ["src"] = "https://telegra.ph" + url }));
                    }

                    content.Add(new NodeElement("div", null, children: songText.Replace("\r\n\r\n", "\r\n").Replace("&#180;", "´")));

                    var telegraphClient = new TelegraphClient();
                    var _tokenClient = telegraphClient.GetTokenClient($"{AppSettings.TelegraphToken }");
                    var page = _tokenClient.CreatePageAsync(
                        title: songName,
                        authorName: "Music Accords Bot",
                        authorUrl: "https://t.me/muschords_bot",
                        returnContent: true,
                        content: content.ToArray()
                    ).Result;

                    var message = await client.SendTextMessageAsync(id, page.Url, replyMarkup: Keyboards.GetSongMenuKeyboard(true));
                    AddSongToBase(id, message.MessageId, page.Url, songName);
                    DataBase.AddSongToLibrary(songName, page.Url);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private static void AddSongToBase(long id, int messageId, string url, string songName)
        {
            DataBase.InsertString(id, "MessageId", messageId.ToString());
            DataBase.UpdateString(id, "url", url, "MessageId", messageId.ToString());
            DataBase.UpdateString(id, "song", songName, "MessageId", messageId.ToString());
        }

        public static async Task<string> UploadImage(string imagePath)
        {
            using (var request = new HttpRequest())
            {
                var multipartContent = new MultipartContent() {
                    {new StringContent("file"), "name"},
                    {new StringContent("chord"), "filename"},
                    {new StringContent("image/jpeg"), "Content-Type"},
                    {new FileContent(imagePath), "chord", "chord.jpg"}
                };

                var url = request.Post("https://telegra.ph/upload", multipartContent).ToString().Replace("\\/", "/");

                Regex regex = new Regex($"\"src\":\"([^\"]+)");
                Match match = regex.Match(url);
                return match.Groups[1].Value;
            }
        }


        public static async Task<List<string>> CreateCompositeImages(long uniqueCode, int countImages) {
            var imagesPath = new List<string>();

            var width = 75;
            var height = 80;
            var countImageInRow = 5;
            var countImage = (int)countImages / 5;
            if (countImages % countImageInRow != 0)
            {
                countImage++;
            }

            var imageCounter = 0;
            try
            {
                for (int i = 0; i < countImage; i++)
                {
                    var listImages = new List<Image>();
                    for (int j = 0; j < countImageInRow; j++)
                    {
                        string url = imageCounter >= countImages
                            ? $"Photos\\background.jpg"
                            : $"Photos\\{uniqueCode}_{imageCounter}.jpg";

                        listImages.Add(Bitmap.FromFile(url));
                        imageCounter++;
                    }

                    using (var result = new Bitmap(width * countImageInRow, height))
                    {
                        Graphics g = Graphics.FromImage(result);
                        var counter = 0;
                        foreach (var image in listImages)
                        {
                            g.DrawImage(image, counter * width, 0);
                            counter++;
                        }
                        var songPath = $"out{uniqueCode}_{i}.jpg";
                        result.Save(songPath);
                        imagesPath.Add(songPath);
                    }

                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }


            return imagesPath;
        }

        public static async Task<bool> DownloadImages(long uniqueCode, List<string> imagesUrls)
        {
            try
            {
                WebClient webClient = new WebClient();
                var ind = 0;
                foreach (var image in imagesUrls)
                {
                    webClient.DownloadFileAsync(new Uri(image), $"Photos\\{uniqueCode}_{ind}.jpg");
                    while (webClient.IsBusy)
                    {
                        Thread.Sleep(1000);
                    }
                    ind++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }


        public static State.Categories GetMessageCategory(string text)
        {

            switch (text)
            {
                case "/start":
                    {
                        return State.Categories.Start;
                    }
                case "ПЛЕЙЛИСТ  🎼":
                    {
                        return State.Categories.Playlist;
                    }
                case "ПОИСК 🔎":
                    {
                        return State.Categories.Search;
                    }
                case "НАСТРОЙКИ 🔧":
                    {
                        return State.Categories.Settings;
                    }
                case "ПОПУЛЯРНЫЕ 🔧":
                    {
                        return State.Categories.Popular;
                    }
                default:
                    {
                        return State.Categories.Default;
                    }
            }
        }

        public static async void SendStartMessage(ITelegramBotClient Bot, long id, string name)
        {
            try
            {
                var image = new InputOnlineFile("https://telegra.ph/file/178ade71533890e6400c2.png");
                var caption = $"Привет, {name}!\n" +
                                $"Я Telegram бот для поиска гитарных аккордов";

                await Bot.SendPhotoAsync(id, image, caption, replyMarkup: Keyboards.GetMainMenu());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
