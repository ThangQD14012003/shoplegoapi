using ShopLegoApi.Datas;
using ShopLegoApi.Model;

namespace ShopLegoApi.Services
{
    public interface IJwtService
    {
        public string GenerateToken(UserModel user); 
    }
}
