using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KiChatNet.Models
{
    public class ChatHistory
    {
        [JsonPropertyName("messages"), JsonPropertyOrder(2)]
        public List<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages.Clear();
                _messages.AddRange(value);
            }
        }
        [JsonPropertyName("CreationDate"), JsonPropertyOrder(0)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonPropertyName("model"), JsonPropertyOrder(1)]
        public string ModelName { get; set; }

        public bool HasUserMessage => _messages.Any(m => m.Role=="user");

        public event Action<Message> MessageAdded;

        private readonly List<Message> _messages = [];
       

        public void AddMessage(Message message)
        {
            _messages.Add(message);

            MessageAdded?.Invoke(message);
        }


    }

}
