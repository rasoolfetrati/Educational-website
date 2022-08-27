using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningWebSite.Core.ViewModel.WalletVM
{
    public class WalletViewModel
    {
        public int WalletId { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsPay { get; set; }
    }
    public class ShowWalletViewModel
    {
        public int Amount { get; set; }
        public string Description { get; set; }
        public string CreateDate { get; set; }
        public string Code { get; set; }
        public string IsPay { get; set; }

    }
    public class ShowFactorsViewModel
    {
        public int WalletId { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string Code { get; set; }
        public bool IsPay { get; set; }

    }
}
