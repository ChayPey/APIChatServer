using System;
using System.Collections.Generic;
using System.Text;

namespace APIChatClient
{
    class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Personal { get; set; }
        public int ReceiverId { get; set; }
    }
}
