using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using task4.Models;
using task4.Data;
using System.Linq;

namespace task4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("MessageBoard/Filter")]
        public IActionResult MessageBoard(string sender, DateTime? fromDate, DateTime? toDate, string isRead, string sortOrder)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var messages = from m in _context.Messages
                select m;

            if (!String.IsNullOrEmpty(sender))
            {
                messages = messages.Where(m => m.From.Contains(sender));
            }

            if (fromDate.HasValue)
            {
                messages = messages.Where(m => m.SendDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                messages = messages.Where(m => m.SendDate <= toDate.Value);
            }

            if (isRead == "unread")
            {
                messages = messages.Where(m => !m.IsRead);
            }

            switch (sortOrder)
            {
                case "date_desc":
                    messages = messages.OrderByDescending(m => m.SendDate);
                    break;
                default:
                    messages = messages.OrderBy(m => m.SendDate);
                    break;
            }

            ViewData["sender"] = sender;
            ViewData["fromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["toDate"] = toDate?.ToString("yyyy-MM-dd");
            ViewData["isRead"] = isRead;

            return View(messages.ToList());
        }

        [HttpGet]
        public IActionResult GetMessageText(int id)
        {
            var message = _context.Messages.Find(id);
            if (message == null)
            {
                return NotFound();
            }

            return Content(message.Text);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                // Redirect to user's message board
                return RedirectToAction("MessageBoard", new { userId = user.Id });
            }
            ViewBag.Error = "Invalid login or password.";
            return View();
        }

        [HttpGet("MessageBoard/User/{userId}")]
        public IActionResult MessageBoard(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            var messages = _context.Messages.Where(m => m.To == user.Login).ToList();
            ViewBag.User = user;
            ViewBag.Messages = messages;
            return View(messages);
        }

        [HttpPost]
        public IActionResult SendMessage(int userId, string to, string title, string text)
        {
            var sender = _context.Users.Find(userId);
            if (sender == null)
            {
                return NotFound();
            }

            var recipient = _context.Users.FirstOrDefault(u => u.Login == to);
            if (recipient == null)
            {
                ViewBag.Error = "Recipient not found.";
                return RedirectToAction("MessageBoard", new { userId = userId });
            }

            var message = new Message
            {
                From = sender.Login,
                To = to,
                Title = title,
                Text = text,
                SendDate = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("MessageBoard", new { userId = userId });
        }
    }
}
