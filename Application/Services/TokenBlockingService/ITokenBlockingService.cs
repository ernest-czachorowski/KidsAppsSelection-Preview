using System;
namespace Application.Services.TokenBlockingService;

public interface ITokenBlockingService
{
    public Task<bool> IsTokenBlockedOrInvalid(string token);
    public Task BlockAuthTokens(Guid userId);
    public Task RemoveExpiredTokensData();
}

