using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;

namespace TinhLam.Hubs
{
    public class ChatHub : Hub
    {
        // Gán connectionId với userId
        private static ConcurrentDictionary<string, int> ConnectionUserMap = new ConcurrentDictionary<string, int>();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userIdString = httpContext.Request.Query["userId"];
            if (int.TryParse(userIdString, out int userId))
            {
                // Gán connectionId với userId
                ConnectionUserMap[Context.ConnectionId] = userId;

                // Thêm connection vào group "user-{userId}"
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectionUserMap.TryRemove(Context.ConnectionId, out int userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Gửi từ User
        public async Task SendMessageFromUser(int userId, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("Message is null or empty.");
                }

                // Gửi tới Admin group
                await Clients.Group("admin").SendAsync("ReceiveMessageFromUser", userId, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendMessageFromUser Error: " + ex.Message);
                throw;
            }
        }


        // Gửi từ Admin đến User
        public async Task SendMessageFromAdmin(int toUserId, string message)
        {
            await Clients.Group($"user-{toUserId}").SendAsync("ReceiveMessageFromAdmin", message);
        }

        // Khi admin kết nối → cho vào group admin
        public Task JoinAsAdmin()
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        }

    }
}
