using KiChatNet.Models;
using KiChatNet.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace KiChatNet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CommandLineArgs commandArgs = new CommandLineArgs(args);

            string? systemPrompt = null;
            string? firstMessage = null;
            string modelName = null;
            string? historyFileName = null;

            if (commandArgs.SystemPromptPath != null)
                systemPrompt = File.ReadAllText(commandArgs.SystemPromptPath);

            if (commandArgs.FirstMessagePath != null)
                firstMessage = File.ReadAllText(commandArgs.FirstMessagePath /*, Encoding.GetEncoding("ISO-8859-1")*/);

            if (commandArgs.ModelName != null)
            {
                modelName = commandArgs.ModelName;
            }

            if (commandArgs.HistoryFileName != null)
            {
                historyFileName = commandArgs.HistoryFileName;
            }

            var config = commandArgs.ConfigFileName == null ? new ConfigService() : new ConfigService(commandArgs.ConfigFileName);




            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(config.Configuration.GetSection("Logging"))
                    .AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();



            ResolvePathService resolvePathService = new ResolvePathService(logger);

         

            if (systemPrompt == null)
            {
                systemPrompt = resolvePathService.ResolvePath(config.SystemPrompt);
            }

            if (modelName == null)
            {
                modelName = config.ModelName;
            }

            if (firstMessage == null && config.FirstUserMessage != null)
            {
                firstMessage = resolvePathService.ResolvePath( config.FirstUserMessage);
            }



            HistoryFileService chatHistoryFileService;
            ChatHistory chatHistory;
            if (historyFileName == null)
            {
                chatHistoryFileService = HistoryFileService.CreateNew();
                chatHistory = new();
                chatHistory.ModelName = modelName;
            }
            else
            {
                chatHistoryFileService = new(historyFileName);
                chatHistory = await chatHistoryFileService.LoadAsync();
                PrintHistory(chatHistory, config);
            }



            logger.LogInformation($"Chat History: {chatHistoryFileService.Filename}");

            chatHistory.MessageAdded += async (message) =>
            {
                await chatHistoryFileService.SaveAsnyc(chatHistory);
            };


            chatHistory.AddMessage(new Message(Roles.System, systemPrompt));

            var chatService = new ChatService(config.EndpointUrl, config.ModelName, logger);

            while (true)
            {
                Console.Write($"\n{config.UserName}: \n");

                string userInput;

                Console.ForegroundColor = config.UserColor;

                if (!chatHistory.HasUserMessage && !string.IsNullOrEmpty(firstMessage))
                {
                    userInput = firstMessage;
                    Console.WriteLine(firstMessage);
                }
                else
                {
                    userInput=string.Empty;
                    string input;
                    do
                    {
                        input = Console.ReadLine();
                        userInput += input + Environment.NewLine;
                    } while (!string.IsNullOrWhiteSpace(input));
                }

                Console.ResetColor();

                if (string.IsNullOrWhiteSpace(userInput)) break;

                chatHistory.AddMessage(new Message(Roles.User, userInput));

                var context = chatHistory.Messages;

                Console.Write($"{config.AssistentName}: \n");
                var responseText = new StringBuilder();

                await chatService.StreamChatAsync(context, token =>
                {
                    Console.ForegroundColor = config.AssistentColor;
                    Console.Write(token);
                    Console.ResetColor();
                    responseText.Append(token);
                });

                chatHistory.AddMessage(new Message(Roles.Assistant, responseText.ToString()));
            }
        }

        private static void PrintHistory(ChatHistory chatHistory, ConfigService config)
        {
            foreach (var item in chatHistory.Messages.Where(m => (new[] { Roles.User, Roles.Assistant }).Select(r => r.ToString().ToLower()).Contains(m.RoleName)))
            {
                Console.Write($"{GetNameForRole(item.RoleName, config)}:");
                Console.ForegroundColor = GetColorForRole(item.RoleName, config);
                Console.WriteLine(item.Content);
                Console.ResetColor();
            }
        }

        private static string GetNameForRole(string role, ConfigService service)
        {
            return role switch
            {
                "user" => service.UserName,
                "assistant" => service.AssistentName,
                _ => role
            };
        }

        private static ConsoleColor GetColorForRole(string role, ConfigService service)
        {
            return role switch
            {
                "user" => service.UserColor,
                "assistant" => service.AssistentColor,
                _ => Console.ForegroundColor
            };
        }


    }
}