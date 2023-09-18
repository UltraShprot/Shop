using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Shop.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Shop.Models;

namespace Shop.Controllers
{
	[Authorize]
	public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly IPurchaseService _purchaseService;
        public ShopController(
            IProductService productService,
            IPurchaseService purchaseService
            )
        {
            _productService = productService;
            _purchaseService = purchaseService;
        }
        public IActionResult Index(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                TempData["search"] = searchString;
            }
            var responseProduct = _productService.Search(searchString);
			var responseCategories = _productService.GetCategories();
			if (responseProduct.StatusCode == Enum.StatusCode.OK && responseCategories.StatusCode == Enum.StatusCode.OK)
            {
				var tuple = new Tuple<List<Product>, List<Category>>(responseProduct.Data, responseCategories.Data);
				return View(tuple);
            }
            return View("Error", $"{responseProduct.Description}");
        }
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProduct(id);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            return View("Error", $"{response.Description}");
        }
        public IActionResult GetCategories()
        {
            var response = _productService.GetCategories();
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return View(response.Data);
            }
            return View("Error", $"{response.Description}");
        }
		public async Task<IActionResult> AddToCart(int id)
        {
            var response = await _purchaseService.AddToCart(id);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                TempData["cart"] = response.Data.Name;
                return RedirectToAction("Index");
            }
			return View("Error", $"{response.Description}");
        }
		[Authorize(Roles = "Administrator")]
		public IActionResult Delete(int id)
		{
			return View(_productService.GetProduct(id).Result.Data);
		}
		[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeletePOST(int id)
        {
            var response = await _productService.Delete(id);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
				return RedirectToAction("Index");
			}
            return View("Error", $"{response.Description}");
        }
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(int id)
        {
            return View(_productService.GetProduct(id).Result.Data);
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdatePOST(Product product)
        {
            var response = await _productService.Update(product.Id, product);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return RedirectToAction("Index");
            }
            return View("Error", $"{response.Description}");
        }
        [Authorize(Roles = "Administrator")]
		public IActionResult Create()
		{
			return View();
		}
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> CreatePOST(Product product)
        {
            var response = await _productService.Create(product);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
				return RedirectToAction("Index");
			}
            return View("Error", $"{response.Description}");
        }
		[Authorize(Roles = "Administrator")]
		public IActionResult CreateCategory()
        { 
		    return View();
		}
		[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateCategoryPOST(Category category)
        {
            var response = await _productService.CreateCategory(category);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return RedirectToAction("Index");
            }
            return View("Error", $"{response.Description}");
        }
		[Authorize(Roles = "Administrator")]
		public IActionResult DeleteCategory(int id)
		{
			return View(_productService.GetCategory(id).Result.Data);
		}
		[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCategoryPOST(int id)
        {
            var response = await _productService.DeleteCategory(id);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
				return RedirectToAction("Index");
			}
            return View("Error", $"{response.Description}");
        }
        public IActionResult ChooseCategory(byte id)
        {
            var response = _productService.ChooseCategory(id);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return RedirectToAction("Index", "Shop", new { searchString = _productService.GetSearchString() });
            }
			return View("Error", $"{response.Description}");
		}
        public IActionResult Details(int id)
        {
            return View(_productService.GetProduct(id).Result.Data);
        }
    }
}
