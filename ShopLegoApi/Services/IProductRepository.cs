using ShopLegoApi.Datas;
using ShopLegoApi.Model;

namespace ShopLegoApi.Services
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetAll();
        public Task<Product> GetById(int id);
        public Task<int> Add(ProductModel model);
        public Task DeleteById(int id);
        public Task Update(ProductModel model, int id);
    }
}
