using Microsoft.EntityFrameworkCore;
using ShopLego.Application;
using ShopLego.Domain.Constants;
using ShopLego.Domain.Entities;
using ShopLego.Infrastructure.Persistence;
namespace ShopLego.Infrastructure.Services;

public sealed class IdentityService(ShopLegoDbContext db, ITokenService tokens) : IIdentityService
{
    public async Task<int> RegisterAsync(RegisterRequest r, CancellationToken ct) { var email = r.Email.Trim().ToLowerInvariant(); if (await db.Users.AnyAsync(x => x.Email.ToLower() == email, ct)) throw new ConflictException("Email already registered."); var u = new User { FullName = r.FullName.Trim(), Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password), Phone = r.Phone.Trim(), Address = r.Address.Trim(), Role = Roles.User, CreatedAt = DateTime.UtcNow }; db.Users.Add(u); await db.SaveChangesAsync(ct); return u.Id; }
    public async Task<AuthResponse?> LoginAsync(LoginRequest r, CancellationToken ct) { var email = r.Email.Trim().ToLowerInvariant(); var u = await db.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == email, ct); return u is null || !BCrypt.Net.BCrypt.Verify(r.Password, u.PasswordHash) ? null : Response(u); }
    public async Task<AuthResponse?> RefreshAsync(string token, CancellationToken ct) { var id = tokens.ReadUserId(token); var u = id is null ? null : await db.Users.FindAsync([id.Value], ct); return u is null ? null : Response(u); }
    private AuthResponse Response(User u) { var t = tokens.Create(u); return new(t.Token, t.RefreshToken, new UserDto(u.Id, u.FullName, u.Email, u.Role, u.Address)); }
}
