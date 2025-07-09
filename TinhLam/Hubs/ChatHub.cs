//using Microsoft.AspNetCore.SignalR;
//using System.Threading.Tasks;
//using TinhLam.Data;
//using TinhLam.Models;

//namespace TinhLam.Hubs
//{
//    public class ChatHub : Hub
//    {
//        private readonly TLinhContext _context;

//        public ChatHub(TLinhContext context)
//        {
//            _context = context;
//        }

//        public async Task SendMessage(int userId, string message)
//        {
//            var newMessage = new ChatMessage
//            {
//                UserId = userId,
//                Content = message,
//                Timestamp = DateTime.Now
//            };

//            _context.ChatMessages.Add(newMessage);
//            await _context.SaveChangesAsync();

//            await Clients.All.SendAsync("ReceiveMessage", userId, message);
//        }
//    }
//}
