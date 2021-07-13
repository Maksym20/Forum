using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.Models;
using Forum.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Forum.ViewModels;
using System.Security.Claims;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbConcext;
        public HomeController(ApplicationDbContext dbConcext)
        {
            _dbConcext = dbConcext;
        }

        //główna strona z tematami forumu
        public IActionResult Index() 
        {
            var vm = new RootModel();
            vm.Threads = _dbConcext.Threads.ToList();
            
            return View(vm);
        }


        // strona Privacy()
        public IActionResult Privacy()
        {
            return View();
        }

        // strona która wyswietla błąd  (ResponseCache - zgienerowana automatycznie
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]

        // tworzy wątek i wiadomość
        public IActionResult CreateContent()
        {
            var thread = new ForumThread()
            {
                AuthorName = "Maksym",
                AuthorId = "Maksym",
                CreatedAt = DateTime.Now,
                ThreadName = "Projekt"
            };

            var threadId = _dbConcext.Threads.Add(thread);
            _dbConcext.SaveChanges();

            var messages = new List<ThreadMessage>()
                {
                    new ThreadMessage ()
                    {
                        CreatedAt = DateTime.Now,
                        UserName = "Maksym",
                        Text = "Cześć!",
                        ForumThreadId = threadId.Entity.Id
                    }
                };

            //dodaje wiadomość i zapisuje bazu danych 
            _dbConcext.Messages.AddRange(messages);
            _dbConcext.SaveChanges();

            //Strona główna 
            return RedirectToAction(nameof(Index));
            return RedirectToAction("home/index");
        }

        //stworzenie nowego wątku
        [Authorize]
        public IActionResult CreateForumThread()
        {
            var vm = new CreateRootThreadM();
            return View(vm);
        }

        //stworzenie nowego wątku Post(приймає клас і з нього приймає базу даних)
        [Authorize]
        [HttpPost]
        public IActionResult CreateForumThread(CreateRootThreadM model)
        {
            var thread = new ForumThread()
            {
                AuthorName = User.Identity.Name,
                AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now,
                ThreadName = model.ThreadName
            };

            var threadId = _dbConcext.Threads.Add(thread);
            _dbConcext.SaveChanges();

            //stworzenia wiadomości Post(приймає клас і з нього приймає базу даних)
            var message = new ThreadMessage ()
                {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        CreatedAt = DateTime.Now,
                        UserName = User.Identity.Name,
                        Text = model.Message,
                        ForumThreadId = threadId.Entity.Id
                };

            _dbConcext.Messages.Add(message);
            _dbConcext.SaveChanges();

            //przekierowuje  nа ViewForumThread - pokazuje wątek
            return RedirectToAction(nameof(ViewForumThread), new { threadId = threadId.Entity.Id });
        }

        //pokazuje wątek
        [Authorize]
        public IActionResult ViewForumThread(int threadId)
        {
            //wyciąga z bazy danych  wątek z wiadomościami
            var sr = _dbConcext.Threads.Include(a => a.Messages).FirstOrDefault(a => a.Id == threadId);
            if (sr == null)
                return Error();

            //wyciąga id 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var usr = User;

            //wrzuca do konstruktora model 
            var vm = new ThreadVM(sr, User.FindFirstValue(ClaimTypes.NameIdentifier));

            // potem te modeli widoczne na stronie 
            return View(vm);                     //walidacja 
        }

        //pokazuje informację o użytkowniku
        [Authorize]
        public IActionResult ViewUser(string userId)
        {
            var sr = _dbConcext.Users.FirstOrDefault(a => a.Id == userId);
            if (sr == null)
                return Error();

            return View(sr);
        }

        //tworzy albo modyfikuje wiadomość
        [Authorize]
        public IActionResult CreateorUpdateForumMessage(int? messageId, int? threadId)
        {
            var message = new EditMessageM();

            if (messageId.HasValue) 
            {
                var msg = _dbConcext.Messages.FirstOrDefault(a => a.Id == messageId);
                if (msg == null)
                    return Error(); 

                //wypełnia model wiadomością 
                message.MessageId = messageId.Value;
                message.Message = msg.Text;
            }

            if (threadId.HasValue)
            {
                //tworzy nowy wiadomość
                var thr = _dbConcext.Threads.FirstOrDefault(a => a.Id == threadId);
                if (thr == null)
                    return Error();

                message.ThreadId = threadId.Value;
                message.ThreadName = thr.ThreadName;
            }
            // zwraca na stronę z tworzeniem albo edycią wiadomości
            return View("CreateorUpdateForumMessage", message);

            //return RedirectToAction(nameof(ViewForumThread), new { threadId = model.ThreadId });
        }

        //tworzy albo modyfikuje wiadomość Post
        [Authorize]
        [HttpPost]
        public IActionResult CreateorUpdateForumMessage(CreateMessageM model)
        {
            // tworzenie modeli
            var message = new ThreadMessage()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAt = DateTime.Now,
                UserName = User.Identity.Name,
                Text = model.Message,
                ForumThreadId = model.ThreadId
            };

            if (model.MessageId != 0) //wyciąga model i aktualizuje go (обновляє)
            {
                var msg = _dbConcext.Messages.FirstOrDefault(a => a.Id == model.MessageId);
                if (msg == null)
                    return Error();
                msg.Text = model.Message;
                msg.CreatedAt = DateTime.UtcNow;

                _dbConcext.Messages.Update(msg);
            }
            else  //tworzy nową wiadomość
            {
                _dbConcext.Messages.Add(message);
            }
            //save bazu danych 
            _dbConcext.SaveChanges();
            //zwraza na stronę z  wątkiem
            return RedirectToAction(nameof(ViewForumThread), new { threadId = model.ThreadId });
        }
    }
}
