namespace ShopLego.Application;

public sealed record AuthResponse(string Token, string RefreshToken, UserDto User);
