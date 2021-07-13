using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    public class ThreadVM
    {
        public int Id { get; set; }
        public string ThreadName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }

        public List<ThreadMessageVm> Messages { get; set; } = new List<ThreadMessageVm>();

        public ThreadVM(ForumThread threadM, string userId)
        {
            Id = threadM.Id;
            CreatedAt = threadM.CreatedAt;
            ThreadName = threadM.ThreadName;
            AuthorName = threadM.AuthorName;

            Messages = threadM.Messages.Select(m => new ThreadMessageVm() {
                CreatedAt = m.CreatedAt,
                ForumThreadId = m.ForumThreadId,
                Id = m.Id,
                Text = m.Text,
                UserName = m.UserName,
                UserId = m.UserId
            }).ToList();

            var messages = Messages.Where(m => m.UserId == userId).ToList();
            if (messages.Count > 0)
                messages.Last().edit = true;
        }
    }

    public class ThreadMessageVm : ThreadMessage
    {
        public bool edit { get; set; }

        public ThreadMessageVm()
        {

        }
        public ThreadMessageVm(ThreadMessage o)
        {
            this.CreatedAt = o.CreatedAt;
            this.ForumThread = o.ForumThread;
        }
    }
}
