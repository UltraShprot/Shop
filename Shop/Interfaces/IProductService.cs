using Shop.Models;
using Shop.Response;

namespace Shop.Interfaces
{
    public interface IProductService
    {
		IBaseResponse<List<Product>> Search(string search);
        IBaseResponse<List<Product>>  GetProducts();
        Task<IBaseResponse<Product>> GetProduct(int id);
        Task<IBaseResponse<Product>> Update(long id, Product product);
        Task<IBaseResponse<Product>> Delete(int id);
        Task<IBaseResponse<Product>> Create(Product product);
        Task<IBaseResponse<Category>> GetCategory(int id);
        IBaseResponse<List<Category>> GetCategories();
        IBaseResponse<bool> IsCategorySelested(Category category);
		Task<IBaseResponse<Category>> CreateCategory(Category category);
        Task<IBaseResponse<Category>> DeleteCategory(int id);
        IBaseResponse<Category> ChooseCategory(byte category);
        string GetSearchString();

	}
}
