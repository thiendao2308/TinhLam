using TinhLam.Data;
using Microsoft.EntityFrameworkCore;

namespace TinhLam.Helpers
{
    public interface IRewardPointService
    {
        Task<int> CalculateEarnedPoints(decimal orderAmount);
        Task<bool> AddRewardPointsToUser(int userId, int points);
        Task<bool> UseRewardPoints(int userId, int points, int orderId, decimal discountAmount);
        Task<int> GetUserRewardPoints(int userId);
        Task<decimal> CalculateDiscountAmount(int points);
        Task<int> CalculateRequiredPoints(decimal discountAmount);
    }

    public class RewardPointService : IRewardPointService
    {
        private readonly TlinhContext _context;

        public RewardPointService(TlinhContext context)
        {
            _context = context;
        }

        // Tính điểm tích lũy: 1 điểm cho mỗi 100.000 VND
        public async Task<int> CalculateEarnedPoints(decimal orderAmount)
        {
            return (int)(orderAmount / 100000);
        }

        // Cộng điểm cho user
        public async Task<bool> AddRewardPointsToUser(int userId, int points)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.RewardPoints = (user.RewardPoints ?? 0) + points;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Sử dụng điểm thưởng
        public async Task<bool> UseRewardPoints(int userId, int points, int orderId, decimal discountAmount)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || (user.RewardPoints ?? 0) < points) return false;

                // Trừ điểm từ user
                user.RewardPoints -= points;

                // Tạo record sử dụng điểm
                var redeemPoint = new RedeemPoint
                {
                    UserId = userId,
                    OrderId = orderId,
                    PointsUsed = points,
                    DiscountAmount = discountAmount,
                    RedeemDate = DateTime.Now,
                    Description = $"Sử dụng {points} điểm để giảm {discountAmount:N0} VND"
                };

                _context.RedeemPoints.Add(redeemPoint);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy số điểm hiện tại của user
        public async Task<int> GetUserRewardPoints(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.RewardPoints ?? 0;
        }

        // Tính số tiền giảm giá từ điểm (1 điểm = 1.000 VND)
        public async Task<decimal> CalculateDiscountAmount(int points)
        {
            return points * 1000;
        }

        // Tính số điểm cần thiết cho một số tiền giảm giá
        public async Task<int> CalculateRequiredPoints(decimal discountAmount)
        {
            return (int)(discountAmount / 1000);
        }
    }
} 