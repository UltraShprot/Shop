using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Enum;
using Shop.Interfaces;
using Shop.Models;
using Shop.Response;
using System.Security.Claims;

namespace Shop.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IBaseRepository<CartProduct> _cartRepository;
		private readonly IBaseRepository<PurchasedProduct> _purchasedProductsRepository;
		private readonly UserManager<AppIdentityUser> _userManager;
        public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
        public PurchaseService(
            IBaseRepository<Product> productRepository, 
            UserManager<AppIdentityUser> userManager, 
            IHttpContextAccessor httpContextAccessor,
			IBaseRepository<PurchasedProduct> purchasedProductsRepository,
			IBaseRepository<CartProduct> cartRepository) 
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _cartRepository = cartRepository;
            _purchasedProductsRepository = purchasedProductsRepository;
        }
        public async Task<IBaseResponse<Product>> AddToCart(int productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);
                if (product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                await _cartRepository.Create(new CartProduct() {ProductId = product.Id, UserId = user.Id });
                return new BaseResponse<Product>()
                {
                    Data = product,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>()
                {
                    Description = $"[AddToCart] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<List<Product>>> GetProductToCartCountable()
        {
            try
            {
                var products = await GetProductsToCart();
                var countableProducts = new List<Product>();
                var countedId = new List<int>();
                if (products.StatusCode != StatusCode.OK)
                {
                    return new BaseResponse<List<Product>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                foreach (var product in products.Data)
                {
                    if (countedId.Contains(product.Id)) 
                    {
                        continue; 
                    }
                    var i = products.Data.Where(x => (x.Id == product.Id)).ToList();
                    var countableProduct = i[0];
                    countableProduct.Count = i.Count;
                    countableProduct.Price = countableProduct.Price * countableProduct.Count;
                    countableProducts.Add(countableProduct);
                    countedId.Add(product.Id);
                }
                if (countableProducts.Any())
                {
                    return new BaseResponse<List<Product>>()
                    {
                        StatusCode = StatusCode.OK,
                        Data = countableProducts
                    };
                }
                return new BaseResponse<List<Product>>()
                {
                    StatusCode = StatusCode.NotFound
                };
        }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[GetProductToCartCountable] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
    };
}

        }
        public async Task<IBaseResponse<List<Product>>> GetProductsToCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var cartProducts = _cartRepository.GetAll();
                var products = _productRepository.GetAll();
                var result = new List<Product>();
                if (products == null || cartProducts == null)
                {
                    return new BaseResponse<List<Product>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                var quarry = from _cartProducts in cartProducts.AsEnumerable() 
                             where _cartProducts.UserId == user.Id
                             orderby _cartProducts.ProductId
                             join _products in products.AsEnumerable()
                             on _cartProducts.ProductId equals _products.Id
                             into productsJoin
                             select new { Id = _cartProducts.ProductId};

                if (quarry.Count() == 0)
                {
                    return new BaseResponse<List<Product>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                foreach (var product in quarry)
                {
                    var r = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == product.Id);
                    if (!r.isAvailable)
                    {
                        await DeleteFromCart(r.Id);
                        continue;
                    }
                    result.Add(r);
                }
                return new BaseResponse<List<Product>>()
                {
                    Data = result,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[GetProductsToCart] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<List<Product>>> GetProductsToPurchaseHistory()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var purchasedProducts = _purchasedProductsRepository.GetAll();
                var result = new List<Product>();
                var products = _productRepository.GetAll();
                if (products == null || purchasedProducts == null)
                {
                    return new BaseResponse<List<Product>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                var quarry = from _purchasedProducts in purchasedProducts
                             where _purchasedProducts.UserId == user.Id
                             select _purchasedProducts;
                if (quarry.Count() == 0)
                {
                    return new BaseResponse<List<Product>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                foreach (var product in quarry)
                {
                    var r = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == product.ProductId);
                    if (r != null) result.Add(r);
                }
                return new BaseResponse<List<Product>>()
                {
                    Data = result,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[GetProductsToCart] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<AppIdentityUser>> Buy()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var products = await GetProductsToCart();
				decimal finalPrice = 0;
                foreach (var product in products.Data)
                {
                    finalPrice += product.Price;
                }
                if (finalPrice > user.Money)
                {
                    return new BaseResponse<AppIdentityUser>()
                    {
                        Data = user,
                        StatusCode = StatusCode.NoMoney
                    };
                }
                else
                {
                    user.Money -= finalPrice;
                    user.Money = Math.Round(user.Money, 2);
                    foreach (var product in products.Data)
                    {
                       await _purchasedProductsRepository.Create(
                            new PurchasedProduct() 
                            {
                                ProductId = product.Id, 
                                UserId = user.Id
                            });
                    }
                    await ClearCart();
                    await _userManager.UpdateAsync(user);
                    return new BaseResponse<AppIdentityUser>()
                    {
                        Data = user,
                        StatusCode = StatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<AppIdentityUser>()
                {
                    Description = $"[ClearCart] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AppIdentityUser>> ClearCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var cartProducts = _cartRepository.GetAll()
                    .Where(item => item.UserId.Equals(user.Id));
                await _cartRepository.DeleteAll(cartProducts);
                return new BaseResponse<AppIdentityUser>()
                {
                    Data = user,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AppIdentityUser>()
                {
                    Description = $"[ClearCart] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AppIdentityUser>> DeleteFromCart(int productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);
                var _cartProduct = await _cartRepository.GetAll().FirstOrDefaultAsync(x =>  (x.ProductId == productId && x.UserId == user.Id) );
                if (_cartProduct != null)
                {
                    await _cartRepository.Delete(_cartProduct);

					return new BaseResponse<AppIdentityUser>()
					{
						Data = user,
                        Description = product.Description,
                        StatusCode = StatusCode.OK
                    };
                }
                return new BaseResponse<AppIdentityUser>()
                {
                    Data = user,
                    Description = "Not found Product in the cart!",
                    StatusCode = StatusCode.NotFound
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AppIdentityUser>()
                {
                    Description = $"[DeleteFromCart] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<AppIdentityUser>> PlusBalance(decimal count)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (count > 0)
                {
                    user.Money = user.Money + count;
                }
                else
                {
					return new BaseResponse<AppIdentityUser>()
					{
						Data = user,
						StatusCode = StatusCode.NoMoney
					};
				}
                await _userManager.UpdateAsync(user);
                return new BaseResponse<AppIdentityUser>()
                {
                    Data = user,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<AppIdentityUser>()
                {
                    Description = $"[PlusBalance] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
