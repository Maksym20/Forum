using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Models
{
    public class HasId
    {
        public int Id { get; set; }
    }
    public class DBRecord : HasId
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public class RootModel
    {
        public List<ForumThread> Threads { get; set; }
    }
    public class ForumThread : DBRecord
    {
        public string ThreadName { get; set; }
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public List<ThreadMessage> Messages { get; set; }
    }
    public class ThreadMessage : DBRecord
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public int ForumThreadId { get; set; }
        [ForeignKey(nameof(ForumThreadId))]
        public ForumThread ForumThread { get; set; }
    }
}
