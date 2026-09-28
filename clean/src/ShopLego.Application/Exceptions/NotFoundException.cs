namespace ShopLego.Application;

public sealed class NotFoundException(string message) : Exception(message);
