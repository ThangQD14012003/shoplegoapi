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
                Name = model.Name,
                Price = model.Price,
                Description = "",
                Image = ""
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

            product.Name = model.Name;
            product.Price = model.Price;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
