using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Interfaces;

namespace Shop.Controllers
{
	[Authorize]
	public class PurchaseController : Controller
	{
		private readonly IPurchaseService _purchaseService;
        public PurchaseController(IPurchaseService purchaseService)
        {
			_purchaseService = purchaseService;
        }
		public async Task<IActionResult> Index()
		{
			var response = await _purchaseService.GetProductToCartCountable();
			if (response.StatusCode == Enum.StatusCode.OK)
			{
                return View(response.Data);
            }
            return View();
		}
		public async Task<IActionResult> ClearCart()
		{
			var response = await _purchaseService.ClearCart();
			if (response.StatusCode == Enum.StatusCode.OK)
			{
				return RedirectToAction("Index");
			}
			return NoContent();
		}
		public async Task<IActionResult> DeleteFromCart(int id)
		{
			var response = await _purchaseService.DeleteFromCart(id);
			if (response.StatusCode == Enum.StatusCode.OK)
			{
				return RedirectToAction("Index");

			}
			return NoContent();
		}
		public async Task<IActionResult> Buy()
		{
			var response = await _purchaseService.Buy();
			if (response.StatusCode == Enum.StatusCode.OK)
			{
				return Redirect("/Identity/Account/Manage/History");
			}
			else
			{
				return RedirectToAction("UpWallet");
			}
		}
        public IActionResult UpWallet()
		{
			return View();
		}
		public async Task<IActionResult> PlusBalance(decimal num)
		{
			var response = await _purchaseService.PlusBalance(num);
			if (response.StatusCode == Enum.StatusCode.OK)
			{
                return RedirectToAction("Buy");
            }
			else
			{
				return RedirectToAction("UpWallet");
			}
		}
	}
}
