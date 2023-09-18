using Shop.Data;
using Shop.Models;
using Shop.Response;

namespace Shop.Interfaces
{
    public interface IPurchaseService 
    { 
        Task<IBaseResponse<AppIdentityUser>> PlusBalance(decimal count);
        Task<IBaseResponse<AppIdentityUser>> Buy();
        Task<IBaseResponse<Product>> AddToCart(int productId);
        Task<IBaseResponse<AppIdentityUser>> DeleteFromCart(int productId);
        Task<IBaseResponse<AppIdentityUser>> ClearCart();
        Task<IBaseResponse<List<Product>>> GetProductsToCart();
        Task<IBaseResponse<List<Product>>> GetProductsToPurchaseHistory();
        Task<IBaseResponse<List<Product>>> GetProductToCartCountable();
    }
}
