using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KiChatNet.Models
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        public Message()
        {
            
        }

        public Message(Roles role, string content)
        {
            Role = role.ToString().ToLower();
            Content = content;
        }
    }


}
