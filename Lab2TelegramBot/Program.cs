using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static string demotivatorFolderPath = @"D:\TgDemotivators\";

    private static string token = "свій токен сюди";
    private static TelegramBotClient botClient;
    private static bool isAwaitingMeme = false;
    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient(token);
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        botClient.StartReceiving(
        HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");
        while (true)
        {
            await Task.Delay(1000);
        }
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            var fromUser = message.From.Username != null ? message.From.Username : message.From.Id.ToString();
            Console.WriteLine($"Received message from: {fromUser}");
            Console.WriteLine($"Message content: {message.Text}");

            if (message.Text != null)
            {
                switch (message.Text.ToLower())
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Запуск бота",
                            replyMarkup: GetMainMenu(),
                            cancellationToken: cancellationToken
                        );
                        break;
                    case "/help":
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Бабич Кирило 612пст",
                            cancellationToken: cancellationToken
                        );
                        break;
                    case "/filmuniverse":
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "https://github.com/donutkrll/movieuniverse",
                            cancellationToken: cancellationToken
                        );
                        break;
                    case "/memes":
                        isAwaitingMeme = true;
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Надішліть зображення та текст для створення демотиватора.",
                            cancellationToken: cancellationToken
                        );
                        break;
                    default:
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Невідома команда",
                            replyMarkup: GetMainMenu(),
                            cancellationToken: cancellationToken
                        );
                        break;
                }
            }
            else if (message.Photo != null && isAwaitingMeme)
            {
                var photoId = message.Photo[^1].FileId; 
                var file = await botClient.GetFileAsync(photoId, cancellationToken);
                var filePath = file.FilePath;

                using (var stream = new MemoryStream())
                {
                    await botClient.DownloadFileAsync(filePath, stream, cancellationToken);
                    var text = message.Caption ?? "А писать я за тебя буду?";
                    var demotivator = CreateDemotivator(stream.ToArray(), text);
                    await botClient.SendPhotoAsync(
                        message.Chat.Id,
                        new InputOnlineFile(demotivator, "demotivator.jpg"),
                        "Ось ваш демотиватор!",
                        cancellationToken: cancellationToken
                    );
                }

                isAwaitingMeme = false;
            }
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private static ReplyKeyboardMarkup GetMainMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton("/start"),
            new KeyboardButton("/help"),
            new KeyboardButton("/filmuniverse"),
            new KeyboardButton("/memes")
        })
        {
            ResizeKeyboard = true
        };
    }

    private static Stream CreateDemotivator(byte[] imageBytes, string text)
    {
        using (var ms = new MemoryStream(imageBytes))
        {
            var image = Image.FromStream(ms);

            int width = image.Width + 100;
            int height = image.Height + 350;
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Black);

                int xOffset = 50;
                int yOffset = 100;

                int borderSize = 1;
                using (var pen = new Pen(Color.White, borderSize))
                {
                    g.DrawRectangle(pen, xOffset - borderSize, yOffset - borderSize, image.Width + 2 * borderSize, image.Height + 2 * borderSize);
                }

                g.DrawImage(image, xOffset, yOffset, image.Width, image.Height);

                var font = new Font("Lobster", 48, FontStyle.Regular, GraphicsUnit.Pixel);
                var brush = new SolidBrush(Color.White);

                var stringSize = g.MeasureString(text, font);

                g.DrawString(text, font, brush, (width - stringSize.Width) / 2, image.Height + 200);
            }

            string fileName = $"{Guid.NewGuid()}.jpg";
            string filePath = Path.Combine(demotivatorFolderPath, fileName);

            bitmap.Save(filePath, ImageFormat.Jpeg);
            var resultStream = new MemoryStream();
            bitmap.Save(resultStream, ImageFormat.Jpeg);
            resultStream.Position = 0;
            return resultStream;
        }
    }



}
