namespace ShopLego.Application;

public sealed record EmailLogDto(int Id, int OrderId, string ReceiverEmail, string Subject, DateTime SendTime, bool Status);
