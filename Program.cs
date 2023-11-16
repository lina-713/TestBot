using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TestBot // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static public Dictionary<long, Update> messageHistory = new Dictionary<long, Update>();
        static async Task Main(string[] args)
        {
            var api = ConfigurationManager.AppSettings["ApiKey"].ToString();
            var botClient = new TelegramBotClient(api);
            botClient.StartReceiving(Menu, ExceptionMetod);
            Console.ReadLine();
        }

        static async Task Menu(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message.Text == null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите одну из команд и тогда я смогу дать вам то, чего вы хотите!");
                return;

            }
            
            List<string> innList = message.Text.Split(' ').ToList();
            if (innList.Count > 1)
            {
                foreach (var inn in innList)
                {
                    if (messageHistory.ContainsKey(message.Chat.Id) && messageHistory[message.Chat.Id].Message.Text == "/inn")
                    {
                        if ((inn.Length == 11 && message.Text[0] == 'F') || (inn.Length == 10 || inn.Length == 12))
                        {
                            var list = message.Text.Split(' ').ToList();
                            var findInn = new InnFinder();
                            update.Message.Text = "/inn " + update.Message.Text;
                            SetAction(message.Chat.Id, update);
                            await botClient.SendTextMessageAsync(message.Chat.Id, await findInn.GetInfoByInn(list));
                            Console.WriteLine(message.Chat.FirstName);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы ввели не ИНН :(");
                        }
                    }
                }
            }
            else
            {
                switch (message.Text)
                {
                    case "/start":
                        try
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Доброе время суток! Этот бот поможет вам найти нужную или нужные организации по ИНН!");
                            SetAction(message.Chat.Id, update);
                            update.Message.Text = "/help";
                            await Menu(botClient, update, token);

                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine($"/start - {exp}");
                        }
                        break;

                    case "/help":
                        try
                        {
                            var str = MessageHelp();
                            await botClient.SendTextMessageAsync(message.Chat.Id, str);
                            SetAction(message.Chat.Id, update);
                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine($"/help - {exp}");
                        }
                        break;

                    case "/hello":
                        try
                        {
                            var str = MessageHello();
                            await botClient.SendTextMessageAsync(message.Chat.Id, str);
                            SetAction(message.Chat.Id, update);

                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine($"/hello - {exp}");
                        }

                        break;

                    case "/inn":
                        {
                            botClient.SendTextMessageAsync(message.Chat.Id, "Введите ИНН через пробелы!");
                            SetAction(message.Chat.Id, update);
                        }
                        break;

                    case "/last":
                        {
                            if (messageHistory.ContainsKey(message.Chat.Id))
                            {

                                var lastAction = messageHistory[message.Chat.Id];
                                List<string> strings = lastAction.Message.Text.Split(' ').ToList();
                                if (strings.First() == "/inn" && strings.Count > 1)
                                {
                                    var findInn = new InnFinder();
                                    strings.RemoveAt(0);
                                    await botClient.SendTextMessageAsync(message.Chat.Id, await findInn.GetInfoByInn(strings));
                                }
                                else
                                {
                                    await Menu(botClient, lastAction, token);
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Вы должны ввести какую-ниубудь команду, чтобы выполнить команду /last");
                            }
                        }
                        break;

                    default:
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите одну из команд и тогда я смогу дать вам то, чего вы хотите!");
                            update.Message.Text = "/help";
                            await Menu(botClient, update, token);
                        }
                        break;
                }
            }
            

        }

        static public void SetAction(long id, Update action)
        {
            if (!messageHistory.ContainsKey(id))
                messageHistory.Add(id, action);
            else
                messageHistory[id] = action;
        }

        static private Task ExceptionMetod(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        static public string MessageHelp()
        {
            var str = System.IO.File.ReadAllText("menuFile.txt");
            return str;
        }

        static public string MessageHello()
        {
            var str = System.IO.File.ReadAllText("infoAbout.txt");
            return str;
        }
    }
}