using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubbyhole.WP.Utils
{
    public delegate void NewMessageEventHandler(object sender, EventArgs e);

    public class InformerManager
    {
        private List<Message> _messages { get; set; } 
        public event NewMessageEventHandler NewMessage;

        public InformerManager()
        {
            _messages = new List<Message>();
        }
        public List<Message> GetMessages()
        {
            return _messages;
        }

        public void AddMessage(String title, String message)
        {
            _messages.Add(new Message(title, message));
            NewMessage(this, new EventArgs());
        }
    }


    public class Message
    {
        public Message(String title, String message)
        {
            this.Content = message;
            this.Title = title;
        }
        public String Title;
        public String Content;
    }

    public class InformerManagerLocator
    {
        private static InformerManager _manager;
        public InformerManager InformerManager
        {
            get
            {
                return _manager ?? (_manager = new InformerManager());
            }
        }
    }
}
