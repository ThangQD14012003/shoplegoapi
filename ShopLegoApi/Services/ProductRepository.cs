using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopLegoApi.Datas;
using ShopLegoApi.Model;

namespace ShopLegoApi.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly LegoDataContext _context;
        //private readonly IMapper _mapper;

        public ProductRepository(LegoDataContext context)
        {
            _context = context;
            //_mapper = mapper;
        }
        public async Task<int> Add(ProductModel model)
        {
            var product = new Product
            {
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                AvailableQuantity = model.AvailableQuantity,
                ImageUrl = model.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }
        public async Task DeleteById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(ProductModel model, int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return;

            product.CategoryId = model.CategoryId;
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.AvailableQuantity = model.AvailableQuantity;
            product.ImageUrl = model.ImageUrl;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
