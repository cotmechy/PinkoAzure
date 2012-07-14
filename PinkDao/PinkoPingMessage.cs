using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Ping message
    /// </summary>
    public class PinkoPingMessage
    {
        public string SenderMachine = string.Empty;
        public DateTime SenderDateTime = DateTime.Now;

        public string ResponderMachine = string.Empty;
        public DateTime ResponderDateTime = DateTime.Now;
    }
}
