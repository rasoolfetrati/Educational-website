using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services.WalletService;
using LearningWebSite.Core.ViewModel.WalletVM;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Areas.User.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    [Area("User")]
    public class WalletController : UserControllerBase
    {
        private readonly IFactorService _walletService;
        private readonly SignInManager<CustomUser> _signInManager;
        public WalletController(IFactorService walletService, SignInManager<CustomUser> signInManager) : base(signInManager)
        {
            _walletService = walletService;
            _signInManager = signInManager;
        }
        [HttpPost]
        public async Task<IActionResult> ChargeWallet(int amount)
        {
            if (amount == 0 && amount == null)
            {
                return RedirectAndShowAlert
                    (OperationResult.Error("مبلغ را لطفا وارد نمایید"),
                    RedirectToAction("Index", "Home", new { area = "User" }));
            }
            var walletId =  _walletService.AddFactor(amount, User.Identity.Name);
            var payment = new ZarinpalSandbox.Payment(amount);
            var response = await payment
                .PaymentRequest("شارژ حساب", "http://localhost:17109/OnlinePayment/" + walletId,
                    "rasoulfetrati2@gmail.com", "09902036655");
            if (response.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + response.Authority);
            }
            return RedirectAndShowAlert(OperationResult.Error("مشکلی پیش اومد..."), RedirectToAction("Index", "Home", new { area = "User" }));
        }
        [Route("OnlinePayment/{id}")]
        public async Task<IActionResult> OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower().Trim() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];
                var wallet = await _walletService.GetWalletById(id);
                if (wallet == null)
                {
                    return RedirectAndShowAlert(OperationResult.Error("خطایی پیش اومد"), RedirectToAction("Index", "Home", new { area = "User" }));
                }
                var payment = new ZarinpalSandbox.Payment(wallet.Amount);
                var response = await payment.Verification(authority);
                if (response.Status == 100)
                {
                    string code = response.RefId.ToString();
                    _walletService.UpdateWallet(id, code);
                    return RedirectAndShowAlert(OperationResult.Success(),
                        RedirectToAction("PaymentResult", new { id = wallet.FactorId }));

                }
                else
                {
                    return RedirectAndShowAlert(OperationResult.Error("خطایی پیش اومد"), RedirectToAction("Index", "Home", new { area = "User" }));
                }
            }
            else if (
                HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower().Trim() == "nok" &&
                HttpContext.Request.Query["Authority"] != ""
                )
            {
                return RedirectAndShowAlert(OperationResult.Error("شما پرداخت را لغو کردید"),
                          RedirectToAction("PaymentResult", new { id = id }));
            }

            return RedirectAndShowAlert(OperationResult.Error("مشکلی پیش اومد..."), RedirectToAction("Index", "Home", new { area = "User" }));
        }
        [Route("PaymentResult/{id}")]
        public async Task<IActionResult> PaymentResult(int id)
        {
            var res = await _walletService.GetWalletById(id,User?.Identity?.Name.ToString());
            if (res!=null)
            {
                if (res.IsPay)
                {
                    ViewBag.Message = "پرداخت شما با موفقیت انجام شد";
                    ShowWalletViewModel walletViewModel = new()
                    {
                        Description = res.Description,
                        CreateDate = res.CreateDate.ToShamsi(),
                        Code = res.Code,
                        Amount = res.Amount,
                        IsPay = "موفق"
                    };
                    return View(walletViewModel);
                }
                else
                {
                    ViewBag.Message = "پرداخت شما موفق نبود";
                    ShowWalletViewModel walletViewModel = new()
                    {
                        Description = res.Description,
                        CreateDate = res.CreateDate.ToShamsi(),
                        Code = "",
                        Amount = res.Amount,
                        IsPay = "ناموفق",
                    };
                    return View(walletViewModel);
                }
            }
            return RedirectAndShowAlert(OperationResult.Error("مشکلی پیش اومد..."), RedirectToAction("Index", "Home", new { area = "User" }));
        }
    }
}
