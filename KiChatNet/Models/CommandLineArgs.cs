using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiChatNet.Models
{
    public class CommandLineArgs
    {
        public string? SystemPromptPath { get; set; }
        public string? FirstMessagePath { get; set; }

        public CommandLineArgs(string[] args)
        {
            foreach (var arg in args)
            {
                string[] parts = arg.Split('=');
                switch (parts.First())
                {
                    case "systemprompt":
                    case "s":
                        SystemPromptPath = parts[1];
                        break;

                    case "firstusermessage":
                    case "u":
                        FirstMessagePath = parts[1];
                        break;

                    default:
                        throw new ArgumentException($"Argument {parts.First()} ist kein gültiger Parameter");
                        break;
                }
            }
        }
    }
}
