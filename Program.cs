using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class Program
    {
        private readonly static string _token = "7449942283:AAGK6AejV0saoHDFEBOBhQ8iIYNsqe0NhDQ";
        private static TelegramBotClient _client;
        private static string[] _responsesKeys = { "Option 1", "Option 2", "Option 3", "Option 4" };
        private static Dictionary<string, string> _buttonResponses = new Dictionary<string, string>
        {
            { _responsesKeys[0], "Вы выбрали Option 1" },
            { _responsesKeys[1], "Вы выбрали Option 2" },
            { _responsesKeys[2], "Вы выбрали Option 3" },
            { _responsesKeys[3], "Вы выбрали Option 4" },
        };

        private static void Main()
        {
            Initialize();
            Console.ReadLine();
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine($"Error: {arg2.Message}");
            return Task.CompletedTask;
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                var message = update.Message;

                if (message.Chat.Username != null)
                {
                    Console.WriteLine($"{message.Chat.Username} | написал: {message.Text}");
                }
                else
                {
                    Console.WriteLine("написал anon");
                }

                if (message.Text.ToLower().Contains("привет"))
                {
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { _responsesKeys[0], _responsesKeys[1] },
                        new KeyboardButton[] { _responsesKeys[2], _responsesKeys[3] },
                    })
                    {
                        ResizeKeyboard = true
                    };

                    await SendMessage(botClient, message.Chat.Id, "☻ привет. Выберите опцию:", replyKeyboard);
                    return;
                }
                else
                {
                    foreach (var button in _buttonResponses)
                    {
                        if (message.Text == button.Key)
                        {
                            await SendMessage(botClient, message.Chat.Id, button.Value);
                            return;
                        }
                    }
                    await SendMessage(botClient, message.Chat.Id, "не понял тебя");
                }
            }
        }

        private static async Task SendMessage(ITelegramBotClient botClient, long chatId, string text, IReplyMarkup replyMarkup = null)
        {
            await botClient.SendTextMessageAsync(chatId, text, replyMarkup: replyMarkup);
        }

        private static void Initialize()
        {
            _client = new TelegramBotClient(_token);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };
            _client.StartReceiving(Update, Error, receiverOptions);
        }
    }
}