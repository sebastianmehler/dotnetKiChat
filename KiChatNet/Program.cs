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
            string firstMessage = null;
            string modelName = null;

            if (commandArgs.SystemPromptPath != null)
                systemPrompt = File.ReadAllText(commandArgs.SystemPromptPath, Encoding.GetEncoding("ISO-8859-1"));

            if (commandArgs.FirstMessagePath != null)
                firstMessage = File.ReadAllText(commandArgs.FirstMessagePath, Encoding.GetEncoding("ISO-8859-1"));

            if (commandArgs.ModelName != null)
            {
                modelName = commandArgs.ModelName;
            }

                var config = commandArgs.ConfigFileName == null ? new ConfigService() : new ConfigService(commandArgs.ConfigFileName);

            if (firstMessage == null && config.FirstUserMessage != null)
            {
                firstMessage = config.FirstUserMessage;
            }

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(config.Configuration.GetSection("Logging"))
                    .AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();


            if (systemPrompt == null)
            {
                systemPrompt = config.SystemPrompt;
            }

            if (modelName == null)
            {
                modelName = config.ModelName;
            }

            var chatHistory = new ChatHistory();

            chatHistory.AddMessage(new Message(Roles.System, systemPrompt));

            var chatService = new ChatService(config.EndpointUrl, config.ModelName, logger);

            while (true)
            {
                Console.Write("\nDu: ");

                string userInput;

                if (!chatHistory.HasUserMessage && !string.IsNullOrEmpty(firstMessage))
                {
                    userInput = firstMessage;
                    Console.WriteLine(firstMessage);
                }
                else
                {
                    userInput = Console.ReadLine();
                }


                if (string.IsNullOrWhiteSpace(userInput)) break;

                chatHistory.AddMessage(new Message(Roles.User, userInput));

                var context = chatHistory.Messages;

                Console.Write("Assistent: ");
                var responseText = new StringBuilder();

                await chatService.StreamChatAsync(context, token =>
                {
                    Console.Write(token);
                    responseText.Append(token);
                });

                chatHistory.AddMessage(new Message(Roles.Assistant, responseText.ToString()));
            }
        }
    }
}