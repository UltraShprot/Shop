using Microsoft.EntityFrameworkCore;
using Shop.Enum;
using Shop.Interfaces;
using Shop.Models;
using Shop.Response;
using System.Security.Claims;

namespace Shop.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseRepository<Product> _productsRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
        public ProductService(IBaseRepository<Product> baseRepository, IHttpContextAccessor httpContextAccessor, IBaseRepository<Category> categoryRepository,IUnitOfWork unitOfWork)
        {
            _productsRepository = baseRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        private List<Product> FindProducts(byte[] categories, List<Product> products)
        {
            var productsList = new List<Product>();
            foreach (var category in categories)
            {
                var quarry = from product in products
                             where product.CategoryId.Equals(category)
                             select product;
                if (quarry.Count() == 0)
                {
                    continue;
                }
                productsList.AddRange(quarry);
            }
            return productsList;
        }
        private List<Product> FindProducts(string search, List<Product> products)
        {
            var productsList = products.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            return productsList.ToList();
        }
        public string GetSearchString()
        {
            var searchString = _httpContextAccessor.HttpContext?.Session.GetString("search" + User?.Identity?.Name);
            return searchString != null ? searchString : "" ;
        }
        public IBaseResponse<List<Product>> Search(string search)
        {
            try
            {
                if (search != null)
                {
                    _httpContextAccessor.HttpContext?.Session
                        .SetString("search" + User?.Identity?.Name, search);
                }
                else
                {
                    _httpContextAccessor.HttpContext?.Session
                          .SetString("search" + User?.Identity?.Name, "");
                }
                var choisedCategories = _httpContextAccessor.HttpContext?.Session.Get("c" + User?.Identity?.Name);
                if (choisedCategories == null)
                {
                    choisedCategories = new byte[0];
                }
                var products = _productsRepository.GetAll().ToList();
                if (!products.Any())
                {
                    return new BaseResponse<List<Product>>()
                    {
                        Description = "Products is Not Found",
                        StatusCode = StatusCode.OK
                    };
                }
                if (choisedCategories.Any() && (string.IsNullOrEmpty(search)))
                {
                    var trueProducts = FindProducts(choisedCategories, products);
                    return new BaseResponse<List<Product>>()
                    {
                        Data = trueProducts,
                        StatusCode = StatusCode.OK
                    };
                }
                else if (choisedCategories.Any() && (!string.IsNullOrEmpty(search)))
                {
                    var trueProducts = FindProducts(choisedCategories, products);
                    trueProducts = FindProducts(search, trueProducts);
                    return new BaseResponse<List<Product>>()
                    {
                        Description = "Results:" + search,
                        Data = trueProducts,
                        StatusCode = StatusCode.OK
                    };
                }
                else if (!string.IsNullOrEmpty(search))
                {
                    var trueProducts = FindProducts(search, products);
                    return new BaseResponse<List<Product>>()
                    {
                        Description = "Results:" + search,
                        Data = trueProducts,
                        StatusCode = StatusCode.OK
                    };
                }
                return new BaseResponse<List<Product>>()
                {
                    Data = products,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[Search] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productsRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        Description = "Product is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                return new BaseResponse<Product>()
                {
                    Data = product,
                    Description = product.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex) 
            {
                return new BaseResponse<Product>()
                {
                    Description = $"[GetProduct] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<Product>> Delete(int id)
        {
            try
            {
                var product = await _productsRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        Description = "Product is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                await _productsRepository.Delete(product);
                return new BaseResponse<Product>()
                {
                    Description = product.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>()
                {
                    Description = $"[Delete] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<Product>> Create(Product product)
        {
            try
            {
                var _product = await _productsRepository.GetAll().FirstOrDefaultAsync(x => x.Id == product.Id);
                if (product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        Description = "Product is NULL",
                        StatusCode = StatusCode.NotFound
                    };
				}
                if (product.Price <= 0)
                {
					return new BaseResponse<Product>()
					{
						Description = "Wrong price",
						StatusCode = StatusCode.NoMoney
					};
				}
                decimal price = Math.Round(product.Price, 2);
                product.Price = price;
                if (product.file != null)
                {
                    _unitOfWork.UploadImage(product.file);
                    product.ImageName = product.file.FileName;
                }
                else
                {
                    product.ImageName = "1.jpg";
                }
                await _productsRepository.Create(product);
                return new BaseResponse<Product>()
                {
                    Description = product.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>()
                {
                    Description = $"[Create] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public IBaseResponse<List<Product>> GetProducts()
        {
            try
            {
                var products = _productsRepository.GetAll().ToList();
                if (!products.Any())
                {
                    return new BaseResponse<List<Product>>()
                    {
                        Description = "Product is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                return new BaseResponse<List<Product>>()
                {
                    Data = products,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[GetProducts] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<Product>> Update(long id, Product product)
        {
            try
            {
                var _product = await _productsRepository.GetAll().FirstOrDefaultAsync(x => x.Id == product.Id);
                if (_product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        Description = "Product is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                decimal price = Math.Round(product.Price, 2);
                product.Price = price;
                if (product.file != null)
                {
                    _unitOfWork.UploadImage(product.file);
                    product.ImageName = product.file.FileName;
                }
                else
                {
                    product.ImageName = _product.ImageName;
                }
                await _productsRepository.Update(product);
                return new BaseResponse<Product>()
                {
                    Description = product.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>()
                {
                    Description = $"[Update] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<Category>> CreateCategory(Category category)
        {
            try
            {
                var categories = _categoryRepository.GetAll().ToList();
                if (category == null)
                {
					return new BaseResponse<Category>()
					{
						Description = "Empty",
						StatusCode = StatusCode.OK
					};
				}
                foreach (var _category in categories)
                {
                    if (category.Name == _category.Name)
                    {
                        return new BaseResponse<Category>()
                        {
                            Description = "Duplicate",
                            StatusCode = StatusCode.Duplicate
                        };
                    }
                }
                await _categoryRepository.Create(category);
                return new BaseResponse<Category>()
                {
                    Description = category.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Category>()
                {
                    Description = $"[CreateCategory] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
		public IBaseResponse<Category> ChooseCategory(byte id)
		{
			try
			{

                var choisedCategories = _httpContextAccessor.HttpContext.Session.Get("c" + User?.Identity?.Name);
                if (choisedCategories == null)
                {
                    choisedCategories = new byte[0];
                }
                var category = GetCategory(id).Result.Data;
                if (category == null)
                {
					return new BaseResponse<Category>()
                    {
                        Description = "category is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                if (IsCategorySelested(category).Data)
                {
                    byte[] _Ids = new byte[choisedCategories.Length - 1];
                    int n = 0;
                    for (int i = 0; i < choisedCategories.Length; i++)
                    {
                        if (choisedCategories[i] == id)
                        {
                            continue;
                        }
                        _Ids[n] = choisedCategories[i];
                        n++;
                    }
                    _httpContextAccessor.HttpContext.Session.Set("c" + User?.Identity?.Name, _Ids);
                    return new BaseResponse<Category>()
                    {
                        StatusCode = StatusCode.OK
                    };
                }
                byte[] Ids = new byte[choisedCategories.Length + 1];
                for (int i = 0; i < choisedCategories.Length + 1; i++)
                {
                    if (i == 0)
                    {
                        Ids[i] = id;
                    }
                    else
                    {
                        Ids[i] = choisedCategories[i - 1];
                    }
                   
                }
                _httpContextAccessor.HttpContext.Session.Set("c" + User?.Identity?.Name, Ids);
                return new BaseResponse<Category>()
				{
					StatusCode = StatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<Category>()
				{
					Description = $"[CreateCategory] : {ex.Message}",
					StatusCode = StatusCode.InternalServerError
				};
			}
		}
        public IBaseResponse<bool> IsCategorySelested(Category category)
        {
            try
            {
                var choisedCategories = _httpContextAccessor.HttpContext.Session.Get("c" + User?.Identity?.Name);
                if (choisedCategories == null)
                {
                    choisedCategories = new byte[0];
                }
                foreach (var _category in choisedCategories)
                {
                    if (_category == category.Id)
                    {
                        return new BaseResponse<bool>()
                        {
                            StatusCode = StatusCode.OK,
                            Data = true
                        };
                    }

				}
				return new BaseResponse<bool>()
				{
					StatusCode = StatusCode.OK,
					Data = false
				};
			}
            catch (Exception ex)
            {
				return new BaseResponse<bool>()
				{
					Description = $"[GetCategories] : {ex.Message}",
					StatusCode = StatusCode.InternalServerError
				};
			}
        }
		public IBaseResponse<List<Category>> GetCategories()
        {
            try
            {
                var categories = _categoryRepository.GetAll().ToList();
                if (!categories.Any())
                {
                    return new BaseResponse<List<Category>>()
                    {
                        Description = "category is NULL",
                        StatusCode = StatusCode.OK
                    };
                }
                return new BaseResponse<List<Category>>()
                {
                    Data = categories,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Category>>()
                {
                    Description = $"[GetCategories] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
		public async Task<IBaseResponse<Category>> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return new BaseResponse<Category>()
                    {
                        Description = "category is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                await _categoryRepository.Delete(category);
                return new BaseResponse<Category>()
                {
                    Description = category.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Category>()
                {
                    Description = $"[DeleteCategory] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
        public async Task<IBaseResponse<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return new BaseResponse<Category>()
                    {
                        Description = "category is NULL",
                        StatusCode = StatusCode.NotFound
                    };
                }
                return new BaseResponse<Category>()
                {
                    Data = category,
                    Description = category.Description,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Category>()
                {
                    Description = $"[GetCategory] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
