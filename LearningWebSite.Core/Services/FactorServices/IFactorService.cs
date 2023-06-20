using LearningWebSite.Core.ViewModel.WalletVM;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Basket;
using LearningWebSite.DataLayer.Entities.UserWallet;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.Core.Services.WalletService
{
    public interface IFactorService
    {
        int AddFactor(int amount, string username);
        void AddOnlineFactor(int orderId, string username, string code);
        void AddWalletFactor(int orderId, string username, string code);
        WalletViewModel GetWalletViewModel(int walletId, string username);
        void UpdateWallet(int walletId, string code);
        Task<Factor> GetWalletById(int Walletid);
        Task<Factor> GetWalletById(int Walletid, string username);
        Factor GetUserWallet(int Walletid, string username);
        List<ShowFactorsViewModel> GetFactors(string username);
        Task DeleteWallet(string username, List<int> courseIds);
    }

    public class FactorService : IFactorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        public FactorService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public int AddFactor(int amount, string username)
        {
            Factor factor = new Factor()
            {
                Amount = amount,
                Username = username,
                UserOperationType = UserOperationType.Charge,
                CreateDate = DateTime.UtcNow,
                Description = "شارژ کیف پول",
                IsPay = false
            };
            _context.Factors.Add(factor);
            _context.SaveChanges();
            return factor.FactorId;

        }

        public void AddOnlineFactor(int orderId, string username, string code)
        {
            int ordersum = _context.Orders.First(o => o.OrderId == orderId).OrderSum;
            Factor factor = new Factor()
            {
                Username = username,
                UserOperationType = UserOperationType.None,
                CreateDate = DateTime.UtcNow,
                Description = "خرید دوره آنلاین",
                IsPay = true,
                Amount = ordersum,
                Code = code,
            };
            _context.Factors.Add(factor);
            _context.SaveChanges();
        }

        public void AddWalletFactor(int orderId, string username, string code)
        {

            int ordersum = _context.Orders.First(o => o.OrderId == orderId).OrderSum;
            Factor factor = new Factor()
            {
                Username = username,
                UserOperationType = UserOperationType.Collect,
                CreateDate = DateTime.UtcNow,
                Description = "خرید دوره با کیف پول",
                IsPay = true,
                Amount = ordersum,
                Code = code,
            };
            _context.Factors.Add(factor);
            _context.SaveChanges();
        }

        public async Task DeleteWallet(string username, List<int> courseIds)
        {
            var userBasket = _context.Baskets
                             .Where(t => courseIds.Contains(t.CourseId) && t.UserName == username).ToList();
            _context.Baskets.RemoveRange(userBasket);
            await _context.SaveChangesAsync();
            await Task.CompletedTask;
        }

        public List<ShowFactorsViewModel> GetFactors(string username)
        {
            return _context.Factors.Where(w => w.Username == username).Select(w => new ShowFactorsViewModel()
            {
                Amount = w.Amount,
                Code = w.Code,
                Description = w.Description,
                CreateDate = w.CreateDate,
                IsPay = w.IsPay,
                WalletId = w.FactorId,
            }).OrderByDescending(f => f.CreateDate).Take(10).AsNoTracking().ToList();
        }

        public Factor GetUserWallet(int Walletid, string username)
        {
            return _context.Factors.Where(w => w.FactorId == Walletid && w.Username == username).Single();
        }

        public async Task<Factor> GetWalletById(int Walletid)
        {
            return await _context.Factors.FindAsync(Walletid);
        }

        public async Task<Factor> GetWalletById(int Walletid, string username)
        {
            return await _context.Factors.Where(w => w.FactorId == Walletid && w.Username == username).SingleOrDefaultAsync();
        }

        public WalletViewModel GetWalletViewModel(int walletId, string username)
        {
            return _context.Factors.Where(w => w.FactorId == walletId && w.Username == username).Select(w => new WalletViewModel()
            {
                WalletId = w.FactorId,
                Amount = w.Amount,
                CreateDate = w.CreateDate,
                Description = w.Description,
                IsPay = w.IsPay
            }).Single();
        }


        public void UpdateWallet(int walletId, string code)
        {
            var data = _context.Factors.Find(walletId);
            data.IsPay = true;
            data.UserOperationType = UserOperationType.Charge;
            data.Code = code;
            _context.Factors.Update(data);
            _context.SaveChanges();
        }
    }
}
