using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiChatNet.Models
{
    public class ChatHistory
    {
        public List<Message> Messages => _messages;

        public bool HasUserMessage => _messages.Any(m => m.Role=="user");

        private readonly List<Message> _messages = [];
       

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }


    }

}
