using Microsoft.EntityFrameworkCore;
using ShopLegoApi.Datas;
using ShopLegoApi.Model;

namespace ShopLegoApi.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly LegoDataContext _context;

        public UserRepository(LegoDataContext context)
        {
            _context = context;
        }

        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> Register(UserModel model)
        {
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Phone = model.Phone,
                Address = model.Address,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        //public async Task<bool> Login(string email, string password)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null) return false;

        //    return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        //}

        public async Task<UserModel?> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(o=> o.Email == email);

            if(user == null)
            {
                return null; 
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            var userModel = new UserModel
            {
                Email = user.Email,
                Address = user.Address,
                FullName = user.FullName,
                Id = user.Id,
                Role = user.Role,
            }; 
            return userModel; 
        }


    }
}
