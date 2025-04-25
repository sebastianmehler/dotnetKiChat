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

        private readonly List<Message> _messages = new();
       

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }


    }

}
