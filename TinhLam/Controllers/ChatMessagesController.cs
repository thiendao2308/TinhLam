using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using TinhLam.Hubs; 
using System.Security.Claims;
using TinhLam.Helpers;

namespace TinhLam.Controllers
{
    [Authorize]
    public class ChatMessagesController : Controller
    {
        private readonly TlinhContext _context;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatMessagesController(TlinhContext context, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _chatHub = chatHub;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Where(u => u.Role != "Admin")
                .ToListAsync();
            return View(users); 
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChatWithUser(int userId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            ViewBag.UserId = userId;
            ViewBag.ReceiverName = _context.Users.FirstOrDefault(u => u.UserId == userId)?.Username;
            return View("ChatAdmin", messages); 
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyChat()
        {
            var userId = int.Parse(User.FindFirst(MySetting.CLAIM_CUSTOMERID).Value);

            var messages = await _context.ChatMessages
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            ViewBag.UserId = userId;
            return View("ChatUser", messages);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetMessagesByUserId()
        {
            var userId = int.Parse(User.FindFirst(MySetting.CLAIM_CUSTOMERID).Value);

            var messagesRaw = await _context.ChatMessages
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync(); // load xong mới xử lý ToString

            var messages = messagesRaw.Select(m => new
            {
                content = m.Content,
                timestamp = m.Timestamp != null
                    ? m.Timestamp.Value.ToString("HH:mm dd/MM")
                    : ""
            });

            return Json(messages);
        }


        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageInput input)
        {
            var isAdmin = User.IsInRole("Admin");
            var senderId = isAdmin ? 0 : int.Parse(User.FindFirst(MySetting.CLAIM_CUSTOMERID)?.Value ?? "0");

            var message = new ChatMessage
            {
                UserId = input.UserId,
                Content = (isAdmin ? "[Admin]: " : "") + input.Content,
                Timestamp = DateTime.Now
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            await _chatHub.Clients.Group($"user-{input.UserId}").SendAsync("ReceiveMessage", message.Content, message.Timestamp?.ToString("g"));

            return Ok();
        }

        public class ChatMessageInput
        {
            public int UserId { get; set; }
            public string Content { get; set; }
        }

    }
}
