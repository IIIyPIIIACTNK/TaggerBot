using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TaggerBot
{
    internal class Program
    {
        static string token = "5845183741:AAHYI2YCizTaEGOeboE_JT9h21s4dqfrtiI";
        static TelegramBotClient botClient = new TelegramBotClient(token);
        static event Action<string,Message> OnAudioTagget;
        static event Action OnExit;
        static TagRepository rep;


        public static ReturnIdFromRepository ReturnIdFromRepository;
        static void Main(string[] args)
        {
            rep = new TagRepository();
            //rep.SaveRepository();
            OnExit += rep.SaveRepository;
            OnAudioTagget += rep.AppendRepository;
            botClient.StartReceiving(Update, Error);
            Console.ReadLine();
            // Выход из приложения
            OnExit();
        }

        static async Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken arg3)
        {
        }

        static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken arg3)
        {
            var recivedMessage = update.Message;

            Task savingMessagesToJson = new Task(() => MessageToJson(recivedMessage)); //Поток для сохранения 
            savingMessagesToJson.Start();                                              //сообщения в JSON
                                                                                       
            var chatID = recivedMessage.Chat.Id;
            Console.WriteLine("----------------------");
            Console.WriteLine(recivedMessage.Text);
            Console.WriteLine(recivedMessage.Type);
            Console.WriteLine(recivedMessage.Date);

            switch (recivedMessage.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                    Console.WriteLine("Audio");
                    if (recivedMessage.Caption.Contains("/tag"))
                    {
                        if (recivedMessage.Audio == null)
                        {
                            Console.WriteLine("no audio");
                        }
                        else
                        {
                            string tag = ClearName(recivedMessage.Caption);
                            Console.WriteLine("tagget");
                            botClient.SendTextMessageAsync(chatID, $"File tagget \nTag: {tag}");
                            OnAudioTagget?.Invoke(tag, recivedMessage);
                        }
                    }
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    if (recivedMessage.Text.Contains("/sendfile"))
                    {
                        var fileIds = ReturnIdFromRepository(ClearName(recivedMessage.Text));
                        List<IAlbumInputMedia> filesToSend = new List<IAlbumInputMedia>();
                        foreach (var fileId in fileIds)
                        {
                            filesToSend.Add(new InputMediaAudio(fileId));
                        }
                            Console.WriteLine("sendfile 213");

                        botClient.SendMediaGroupAsync(chatID, filesToSend);
                    }
                    break;
            }
        }


        static string ClearName(string name)
        {
            StringBuilder sb = new StringBuilder(name);
            sb.Replace(" ", "");
            if (name.Contains("/tag"))
            {
                sb.Replace("/tag", "");
            }
            else if (name.Contains("/sendfile"))
            {
                sb.Replace("/sendfile", "");
            }
            Console.WriteLine($"|{sb}|") ;
            return sb.ToString();
        }

        private static void MessageToJson(Message message)
        {
            using (StreamWriter fs = new StreamWriter(@"messages.json",true))
            {
                fs.Write(JsonConvert.SerializeObject(message, Formatting.Indented));
            }
        }
    }
}