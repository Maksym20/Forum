using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class EditMessageM
    {
        public int MessageId { get; set; }
        public int ThreadId { get; set; }
        public string Message { get; set; }
        public string ThreadName { get; set; }
    }
}
