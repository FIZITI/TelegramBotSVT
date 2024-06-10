using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;

namespace TelegramBot
{
    internal class Program
    {
        private readonly static string _token = "7449942283:AAGK6AejV0saoHDFEBOBhQ8iIYNsqe0NhDQ";
        private static TelegramBotClient _client;
        private static ReplyKeyboardMarkup _reaplyFirstMenuKeyboard;
        private static ReplyKeyboardMarkup _reaplyTemplesKeyboard;
        private static ReplyKeyboardMarkup _reaplyMuseumsKeyboard;
        private static ReplyKeyboardMarkup _reaplyCafeKeyboard;
        private static ReplyKeyboardMarkup _reaplyEntertainmentKeyboard;
        private static ReplyKeyboardMarkup _reaplyMonumentsKeyboard;
        private static ReplyKeyboardMarkup _reaplyPhotosKeyboard;
        private static string[] _responsesKeys;
        private static string[] _responsesTemplesKeys;
        private static string[] _responsesMuseumsKeys;
        private static string[] _responsesCafeKeys;
        private static string[] _responsesEntertainmentKeys;
        private static string[] _responsesMonumentsKeys;
        private static string[] _responsesPhotosKeys;
        private static string _backToMenu;
        private static InputFile _photo;
        private static Dictionary<string, ReplyKeyboardMarkup> _buttonResponses;
        private static Dictionary<string, string>[] _reaplyMenuAnswers;

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

                try
                {
                    if (message.Chat.Username != null)
                    {
                        Console.WriteLine($"{message.Chat.Username} | написал: {message.Text}");
                    }
                    else
                    {
                        Console.WriteLine($"anon | написал: {message.Text}");
                    }

                    if (message.Text.ToLower().Contains("/start".ToLower()))
                    {
                        await SendMessage(botClient, message.Chat.Id, "Привет. Выберите опцию:", _reaplyFirstMenuKeyboard);
                        return;
                    }
                    else if (message.Text.ToLower().Contains(_backToMenu.ToLower()))
                    {
                        await SendMessage(botClient, message.Chat.Id, "Возврат к главному меню", _reaplyFirstMenuKeyboard);
                    }
                    else
                    {
                        foreach (var button in _buttonResponses)
                        {
                            if (message.Text == button.Key)
                            {
                                await SendMessage(botClient, message.Chat.Id, $"Ты выбрал {button.Key}", button.Value);
                                return;
                            }
                        }

                        foreach (var dictionary in _reaplyMenuAnswers)
                        {
                            foreach (var key in dictionary)
                            {
                                if (message.Text == key.Key)
                                {
                                    await SendMessage(botClient, message.Chat.Id, key.Value);
                                    return;
                                }
                            }
                        }

                        await SendMessage(botClient, message.Chat.Id, "Не понял тебя");
                    }
                }
                catch (ApiRequestException apiEx) when (apiEx.ErrorCode == 403)
                {
                    Console.WriteLine($"Bot was blocked by the user: {message.Chat.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private static async Task SendMessage(ITelegramBotClient botClient, long chatId, string text, IReplyMarkup replyMarkup = null)
        {
            await botClient.SendTextMessageAsync(chatId, text, replyMarkup: replyMarkup);
        }

        private static async Task SendMessageWithPhoto(ITelegramBotClient botClient, ChatId chatId, InputFile photo)
        {
            using (Stream stream = System.IO.File.OpenRead("C:/Users/User/FProject/TelegramBot/Photos/bye.png"))
            {
                photo = InputFile.FromStream(stream);
                await botClient.SendPhotoAsync(chatId, photo);
            }
        }

        private static void Initialize()
        {
            _client = new TelegramBotClient(_token);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };
            _client.StartReceiving(Update, Error, receiverOptions);

            _responsesKeys = new[] { "ХРАМЫ, КОСТЕЛЫ", "МУЗЕИ, ВЫСТАВОЧНЫЕ ЗАЛЫ", "КАФЕ,РЕСТОРАНЫ", "АКТИВНЫЙ ОТДЫХ, РАЗВЛЕЧЕНИЕ", "ПАМЯТНИКИ, МЕМОРИАЛЫ", "ФОТОЗОНЫ",};
            _responsesTemplesKeys = new[] { "Костел Вознесения Святого Креста", "Храм Преображения Господня", "Храм во имя Кирилла Туровского", "Церковь Благодать", "Храм святителя Николая Чудотворца", "Часовня Дионисия Полоцкого", "Церковь Двенадцати апостолов", "Новоапостольская церковь", "Церковь Петра и Павла",};
            _responsesMuseumsKeys = new[] { "Военно-морской музей", "Дом ремесел", "Картинная галерея", "Историко-краеведческий музей",};
            _responsesCafeKeys = new[] { "Либерика", "Суши-бар «Тунец»", "Pizza Smile", "Beer and Burger", "Капибара", "Moon club", "Розмарин", "Кофейня «Кофеин»", $"{"Premium hookah club"}", "Lollipub", "Континент", "СушиЕм", "Маентак", "Studio Lounge bar", "Ресторан, боулинг«Matrix»", "Корица", };
            _responsesEntertainmentKeys = new[] { "Ресторан, боулинг«Matrix»", "Ледовый дворец", "Popcorn club (большая зона развлечений и детская площадка)", "Этажи развлечений (Игра лазертаг, квест-комната, лабиринты, VR арена полного погружения)", $"Верёвочный парк {"Green Adrenaline"}", $"Конный двор {"Дакота"} (катание на лошадях. Фотосессия)", "Республиканский заказник «Выдрица»", "Бильярд «Континент»", "Кинотеатр «Спутник»", };
            _responsesMonumentsKeys = new[] { "Памятник воинам-интернационалистам", "Скорбящий колокол", "Мемориальный комплекс «Ола»", "Памятник «Роман Шатила»", "Памятный знак «Братство четырех флотов»", "Аллея Героев Советского Союза", "Монумент тепловозу ТГК-2", "Курган Славы", "Братская могила", "Мемориал Великой Отечественной войны", };
            _responsesPhotosKeys = new[] { "Я люблю Светлогорск", "Ника и Прометей", "Аллея скамеек", "Прометей", "Городские часы", };
            _backToMenu = "Назад";

            _reaplyFirstMenuKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _responsesKeys[0], _responsesKeys[1] },
                new KeyboardButton[] { _responsesKeys[2], _responsesKeys[3] },
                new KeyboardButton[] { _responsesKeys[4], _responsesKeys[5]},
            })
            {
                ResizeKeyboard = true
            };

            _reaplyTemplesKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _responsesTemplesKeys[0], _responsesTemplesKeys[1] },
                new KeyboardButton[] { _responsesTemplesKeys[2], _responsesTemplesKeys[3] },
                new KeyboardButton[] { _responsesTemplesKeys[4], _responsesTemplesKeys[5] },
                new KeyboardButton[] { _responsesTemplesKeys[6], _responsesTemplesKeys[7] },
                new KeyboardButton[] { _responsesTemplesKeys[8], _backToMenu},
            })
            {
                ResizeKeyboard = true
            };

            _reaplyMuseumsKeyboard = new ReplyKeyboardMarkup(new[]
{
                new KeyboardButton[] { _responsesMuseumsKeys[0], _responsesMuseumsKeys[1] },
                new KeyboardButton[] { _responsesMuseumsKeys[2], _responsesMuseumsKeys[3] },
                new KeyboardButton[] { _backToMenu },
            })
            {
                ResizeKeyboard = true
            };

            _reaplyCafeKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _responsesCafeKeys[0], _responsesCafeKeys[1] },
                new KeyboardButton[] { _responsesCafeKeys[2], _responsesCafeKeys[3] },
                new KeyboardButton[] { _responsesCafeKeys[4], _responsesCafeKeys[5] },
                new KeyboardButton[] { _responsesCafeKeys[6], _responsesCafeKeys[7] },
                new KeyboardButton[] { _responsesCafeKeys[8], _responsesCafeKeys[9] },
                new KeyboardButton[] { _responsesCafeKeys[10], _responsesCafeKeys[11] },
                new KeyboardButton[] { _responsesCafeKeys[12], _responsesCafeKeys[13] },
                new KeyboardButton[] { _responsesCafeKeys[14], _responsesCafeKeys[15] },
                new KeyboardButton[] { _backToMenu},
            })
            {
                ResizeKeyboard = true
            };

            _reaplyEntertainmentKeyboard = new ReplyKeyboardMarkup(new[]
{
                new KeyboardButton[] { _responsesEntertainmentKeys[0], _responsesEntertainmentKeys[1] },
                new KeyboardButton[] { _responsesEntertainmentKeys[2], _responsesEntertainmentKeys[3] },
                new KeyboardButton[] { _responsesEntertainmentKeys[4], _responsesEntertainmentKeys[5] },
                new KeyboardButton[] { _responsesEntertainmentKeys[6], _responsesEntertainmentKeys[7] },
                new KeyboardButton[] { _responsesEntertainmentKeys[8], _backToMenu},
            })
            {
                ResizeKeyboard = true
            };

            _reaplyMonumentsKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _responsesMonumentsKeys[0], _responsesMonumentsKeys[1] },
                new KeyboardButton[] { _responsesMonumentsKeys[2], _responsesMonumentsKeys[3] },
                new KeyboardButton[] { _responsesMonumentsKeys[4], _responsesMonumentsKeys[5] },
                new KeyboardButton[] { _responsesMonumentsKeys[6], _responsesMonumentsKeys[7] },
                new KeyboardButton[] { _responsesMonumentsKeys[8], _responsesMonumentsKeys[9] },
                new KeyboardButton[] { _backToMenu},
            })
            {
                ResizeKeyboard = true
            };

            _reaplyPhotosKeyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { _responsesPhotosKeys[0], _responsesPhotosKeys[1] },
                new KeyboardButton[] { _responsesPhotosKeys[2], _responsesPhotosKeys[3] },
                new KeyboardButton[] { _responsesPhotosKeys[4], _backToMenu },
            })
            {
                ResizeKeyboard = true
            };

            _reaplyMenuAnswers = new Dictionary<string, string>[]
            {
                new Dictionary<string, string>
                {
                    { _responsesTemplesKeys[0], $"https://g.co/kgs/o18pSvA \nАдрес: Ул. Советская , дом 76а. \nКонтакты: +375234225492 +375234290996" },
                    { _responsesTemplesKeys[1], "https://g.co/kgs/vWkVKRT \nУл. Ленина, дом 71. \nКонтакы: +375234275183 +375234270335" },
                    { _responsesTemplesKeys[2], "https://yandex.by/maps/org/tserkov_vo_imya_kirilla_turovskogo/210280659590/ \nУл. Свердлова, дом 9в. \nКонтакы: +375291464067" },
                    { _responsesTemplesKeys[3], "https://vk.com/grace146 \nУл. Советская, дом 146. \nКонтакы: +3752919150" },
                    { _responsesTemplesKeys[4], "https://yakimova.cerkov.ru/ \nД. Якимова Слабода, Светлогорский район, ул. Солнечная, дом 13а. \nКонтакы: +375 2342 2-07-20" },
                    { _responsesTemplesKeys[5], "https://ruskontur.com/svetlogorsk-chasovnya-dionisiya-poloczkogo/ \nУл. Авиационная. \nКонтакы: +375 29 697-02-44" },
                    { _responsesTemplesKeys[6], "https://medkov.cerkov.ru/ \nАгрогородок Чирковичи, Светлогорский район, ул. П. Стефановского, дом 9а. \nКонтакы: + 375 234 22-50-09" },
                    { _responsesTemplesKeys[7], "Ул. Социалистическая, дом 21." },
                    { _responsesTemplesKeys[8], "https://drevo-info.ru/articles/13678405.html \nУл. Ленина, дом 71. \nКонтакы: +375234275183 +375234270335" },
                },

                new Dictionary<string, string>
                {
                    { _responsesMuseumsKeys[0], "https://sv-biznes.by/sight/history/voenno-morskoy-muzey/ \nУл. Советская, дом 95" },
                    { _responsesMuseumsKeys[1], "http://svetlogorsk.by/ru/pages/social/kulture/uchrezhdeniya/dom-remesel/ \nУл. Ленина, дом 9. \nКонтакы: +375234231489 +375234221400" },
                    { _responsesMuseumsKeys[2], "http://gallery.svet.museum.by/ \nМ-н Первомайский, дом 65. \nКонтакы: +375234244179. \nРежим работы: 9.00-18.00 Обед 13.00-14.00" },
                    { _responsesMuseumsKeys[3], "http://svetlogorsk.museum.by/ \nУл. Зеленая, дом 1. \nКонтакы: +375 2342 7 20 19, +375 2342 7 20 29, +375 2342 7 20 56. \nРежим работы: с 1 мая по 30 сентября 10.00-18.00 \nс 1 октября по 30 апреля 10.00-19.00 \nобеденный перерыв 13.00-14.00 \nвыходной – понедельник" },
                },

                new Dictionary<string, string>
                {
                    { _responsesCafeKeys[0], "https://www.instagram.com/liberica_svt/ \nУл.Советская, дом 101 а. \nКонтакы: Через директ в Instagram. \nРежим работы: 8.00-23.00 (будни) \n9.00-23.00 (выходные)" },
                    { _responsesCafeKeys[1], "https://svetlogorsk.sushi-tunec.by/ \nУл.Ленина, дом 14-8. \nКонтакы: +375297882288. \nРежим работы: 11.00-22.00" },
                    { _responsesCafeKeys[2], "https://g.co/kgs/brR6YYB \nУл. Калинина, дом 6 (тц «Березки»). М-н Октябрьский, дом 33 (маг «Алми). \nКонтакы: +375291402345. \nРежим работы: 11.00-23.00" },
                    { _responsesCafeKeys[3], "https://www.instagram.com/beer.burger.sv/ \nУл. Калинина дом 6 (тц «Березки»). \nКонтакы: +375293216321. \nРежим работы: Вс.-чт. 12.00-22.00 \nпт.-сб. 12.00-23.00" },
                    { _responsesCafeKeys[4], "https://kapibaras.by/svetlogorsk \nУл. Спортивная, дом 11 ( «Гостиный двор»). \nКонтакы: +375333575555. \nРежим работы: Будни 10.00-21.45 \nВыходные 11.00-21.45" },
                    { _responsesCafeKeys[5], "https://vk.com/moon_club \nУл. Калинина. Дом 9/1. \nКонтакы: +375291711720. \nРежим работы: Вт.-чт.11.00-23.00 \nПт.-сб. 11.00-4.00" },
                    { _responsesCafeKeys[6], "https://sv-biznes.by/organization/eda/kafe-restoran/15224/ \nУл.Ленина , дом 43а. \nКонтакы: +375296910269. \nРежим работы: 11.00-23.00" },
                    { _responsesCafeKeys[7], "https://yandex.by/maps/org/kofein/122914683093/?ll=29.737766%2C52.635696&z=15 \nУл. Ленина, дом 49а (БЦ «Светлый»). \nКонтакы: +375293096255. \nРежим работы: 10.00-21.00" },
                    { _responsesCafeKeys[8], $"https://sv-biznes.by/organization/eda/kafe-restoran/46085/ \nул. Калинина, 6 (тц {"Березки"}). \nКонтакы: +375293232362. \nРежим работы: Вс.-чт.16.00-2.00 \nПт.-сб.-16.00-4.00" },
                    { _responsesCafeKeys[9], "https://sv-biznes.by/organization/eda/kafe-restoran/22939/ \nУл. Ленина, дом 49а. \nКонтакы: +375296268589. \nРежим работы: Пн.-чт.-12.00-22.00 \nпт.-сб.-20.00-5.00" },
                    { _responsesCafeKeys[10], "https://sv-biznes.by/organization/eda/kafe-restoran/14392/ \nУл. Советская, дом 109. \nКонтакы: +375299529197. \nРежим работы: Пн.-чт -12.00-2.00 \nПт.-вс. -12.00-4.00" },
                    { _responsesCafeKeys[11], "https://sushiem.by/ \nУл. 50 лет Октября, дом 2б -195. \nКонтакы: +375255162484. \nРежим работы: 12.00-21.45" },
                    { _responsesCafeKeys[12], "https://yandex.by/maps/org/mayontak/97508603470/?ll=29.659442%2C52.679729&z=15 \nСветлогорский район, д. Чирковичи. \nКонтакы: +375445439006. \nРежим работы: Вт.-чт.,вс.-12.00-22.00 \nПт.-сб.-12.00-2.00" },
                    { _responsesCafeKeys[13], "https://sv-biznes.by/organization/eda/kafe-restoran/43551/ \nул. Социалистическая, дом 4. \nКонтакы: +375291971117. \nРежим работы: Пн.-чт.,вс.-17.00-2.00 \nПт.-сб.-17.00-4.00" },
                    { _responsesCafeKeys[14], "https://sv-biznes.by/organization/otdykh/bilyard-bouling/21827/ \nул. Лесная, дом  7. \nКонтакы: +375447070718. \nРежим работы: Вт.-чт.,вс. -12.00-23.00 \nПт.,сб.-12.00-2.00" },
                    { _responsesCafeKeys[15], "https://sv-biznes.by/organization/eda/kafe-restoran/11685/ \nул. Калинина, дом 25. \nКонтакы: +375201093000. \nРежим работы: 11.00-23.00" },
                },

                new Dictionary<string, string>
                {
                    { _responsesEntertainmentKeys[0], "https://sv-biznes.by/organization/otdykh/bilyard-bouling/21827/ \nул. Лесная, дом  7. \nКонтакы: +375447070718. \nРежим работы: Вт.-чт.,вс. -12.00-23.00 \nПт.,сб.-12.00-2.00" },
                    { _responsesEntertainmentKeys[1], "https://www.dir.by/belarus/svetlogorsk/ledovyj_dvorec/?lang=rus \nУл. Свердлова. \nКонтакы: +375234277151 \nБронирование коньков +375298748842. \nРежим работы: Касса работает \nБудние 17.00-20.00 \nПраздничные и выходные 12.00-19.00" },
                    { _responsesEntertainmentKeys[2], $"https://lovesun.by/society/unikalnoe-svetlogorskoe-kafe-s-bolshoj-zonoj-razvlechenij-i-detskoj-igrovoj-ploshhadkoj.html \nУл. Спортивная, 11/1 (мц {"Гостиный двор"}, 2 этаж). \nКонтакы: +375447150303. \nРежим работы: 11.00-23.00" },
                    { _responsesEntertainmentKeys[3], "https://sv-biznes.by/organization/otdykh/razvlecheniya/26549/ \nУл. Дружбы, дом 6. \n+375291184077. \nРежим работы: 11.00-21.00" },
                    { _responsesEntertainmentKeys[4], "https://vk.com/ga_sv \n https://rutube.ru/video/963ceb19ecdfb9bcbd6775585cd62fa5/ \nУл. Свердлова (напротив ТЦ «Березки»). \nКонтакы: +375 (25) 608-57-94. \nРежим работы: С 11.00 -до заката" },
                    { _responsesEntertainmentKeys[5], "https://www.pixwox.com/ru/profile/konnydvor.dakota/ \n https://vk.com/away.php?to=https%3A%2F%2Finstagram.com%2Fkonnydvor.dakota%3Futm_medium%3Dcopy_link&post=-16955674_260950&cc_key= \nСветлогорский район д. Чирковичи. \nКонтакы: +3757088397" },
                    { _responsesEntertainmentKeys[6], "https://sv-biznes.by/sight/other/respublikanskiy-zakaznik-vydritsa-/ \nСветлогорский и Жлобинский районы. \nКонтакы: +375 (29) 393-13-18" },
                    { _responsesEntertainmentKeys[7], "https://sv-biznes.by/organization/eda/kafe-restoran/14392/ \nУл. Советская, дом 109. \nКонтакы: +375299529197. \nРежим работы: Пн.-чт -12.00-2.00 \nПт.-вс. -12.00-4.00" },
                    { _responsesEntertainmentKeys[8], "https://vk.com/chirkovichi.sputnik \nСветлогорский район д. Чирковичи. \nКонтакы: +375291870637. \nРежим работы: 10-00 - 22-00" },
                },

                new Dictionary<string, string>
                {
                    { _responsesMonumentsKeys[0], "https://www.youtube.com/watch?v=atOM98-e1EA \nНабережная города" },
                    { _responsesMonumentsKeys[1], "https://sv-biznes.by/sight/history/pamyatnik-skorbyashchiy-kolokol/ \nНабережная города" },
                    { _responsesMonumentsKeys[2], "http://ola.svetlogorsk.museum.by/ \nГомельская область, Светлогорский район, 34-й километр дороги Р-149 Светлогорск—Жлобин" },
                    { _responsesMonumentsKeys[3], "https://sv-biznes.by/sight/history/pamyatnik-roman-shatila/ \nРядом с городской площадью" },
                    { _responsesMonumentsKeys[4], "https://sv-biznes.by/sight/other/pamyatnyy-znak-bratstvo-chetyrekh-flotov/ \nНабережная города" },
                    { _responsesMonumentsKeys[5], "https://www.youtube.com/watch?v=h-Zn4zJgboY \nВозле Городского Дома Культуры" },
                    { _responsesMonumentsKeys[6], "https://sv-biznes.by/sight/interesnoe/monument-teplovozu-tgk-2/ \nЖд вокзал" },
                    { _responsesMonumentsKeys[7], "https://yandex.by/maps/org/kurgan_slavy/208627014127/?ll=29.392445%2C52.757776&z=10 \nГомельская область, Светлогорский район, Паричский сельсовет" },
                    { _responsesMonumentsKeys[8], "https://shatilki.by/memorial/g-svetlogorsk-bratskaia-mogila-u-ssh-no-4/ \nМемориал у средней школы №4 по ул. Мирошниченко" },
                    { _responsesMonumentsKeys[9], "https://yandex.by/maps/org/memorial_velikoy_otechestvennoy_voyny/184908704491/?ll=29.714816%2C52.632394&z=17 \nУл. Авиационная (жд вокзал)" },
                },

                new Dictionary<string, string>
                {
                    { _responsesPhotosKeys[0], "https://www.google.com/imgres?imgurl=https://ekskursii.by/images/obj1/117/c54he5_9_true.jpg&tbnid=ww5URobN_MbsQM&vet=1&imgrefurl=https://ekskursii.by/?Goroda_Belarusi%3D117_svetlogorsk&docid=FHOVYcakIzdlgM&w=800&h=533&hl=ru-RU&source=sh/x/im/m1/4&kgs=ac0f8ae27ee9da32&shem=abme,ssic,trie \nНа въезде в город." },
                    { _responsesPhotosKeys[1], "https://images.app.goo.gl/prsXoEsAWPVGieZ79 https://images.app.goo.gl/x5B6hwVggeRRZkRd6 \nНа въезде в город." },
                    { _responsesPhotosKeys[2], "https://sv-biznes.by/sight/interesnoe/alleya-skameek/ \nУл. 50 лет Октября." },
                    { _responsesPhotosKeys[3], "https://yandex.by/maps/org/prometey/69556779450/?ll=29.727584%2C52.622594&z=12 \nВозле ДКЭ." },
                    { _responsesPhotosKeys[4], "https://sv-biznes.by/sight/interesnoe/gorodskie-chasy/ \nПлощадь города." },
                },
            };

            _buttonResponses = new Dictionary<string, ReplyKeyboardMarkup>
            {
                { _responsesKeys[0], _reaplyTemplesKeyboard },
                { _responsesKeys[1], _reaplyMuseumsKeyboard },
                { _responsesKeys[2], _reaplyCafeKeyboard },
                { _responsesKeys[3], _reaplyEntertainmentKeyboard },
                { _responsesKeys[4], _reaplyMonumentsKeyboard },
                { _responsesKeys[5], _reaplyPhotosKeyboard },
            };
        }
    }
}