using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace KiChatNet.Services
{
    public class ConfigService
    {
        public string EndpointUrl { get; }
        public string ModelName { get; }
        public string SystemPrompt { get; }
        public string? FirstUserMessage { get; }
        public int MaxContextMessages { get; }

        public IConfiguration Configuration { get; }

        public ConfigService()
        {

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            Configuration = config;
            var section = config.GetSection("LmStudio");

            EndpointUrl = section.GetValue<string>("EndpointUrl") ?? "http://localhost:1234";
            ModelName = section.GetValue<string>("ModelName");
            SystemPrompt = section.GetValue<string>("SystemPrompt");
            MaxContextMessages = section.GetValue<int>("MaxContextMessages");
            FirstUserMessage = section.GetValue<string>("FirstUserMessage");
        }
    }
}
