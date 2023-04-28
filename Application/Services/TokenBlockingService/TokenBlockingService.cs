namespace Application.Services.TokenBlockingService;

public class TokenBlockingService : ITokenBlockingService
{
    private const int _allowedReadersWriters = 1000;
    private const int _readerTimeout_ms = 2500;
    private const int _writerTimeout_ms = 30000;

    private System.Collections.Generic.Dictionary<Guid, DateTime> _blockedTokensInfo = new();
    private AsyncReaderWriterLock _rwlock = new AsyncReaderWriterLock(_allowedReadersWriters);

    private readonly double _tokenExpirationTime;
    private readonly ILogger<TokenBlockingService> _logger;

    public TokenBlockingService( IConfiguration configuration, ILogger<TokenBlockingService> logger)
    {
        _logger = logger;
        _tokenExpirationTime = Double.Parse(configuration.GetSection("AppSettings:TokenTime").Value!);
    }

    public async Task<bool> IsTokenBlockedOrInvalid(string token)
    {
        JwtSecurityToken? decodedToken = null;

        try
        {
            decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            _logger.LogError($"Cannot read token: {token}");
            return true;
        }

        if (decodedToken is null || decodedToken.Payload is null)
        {
            _logger.LogError($"Token or payload is null.");
            return true;
        }

        if (decodedToken.Payload.Iat is null)
        {
            _logger.LogError($"Token doesn't have 'Iat' payload. Token: {token}");
            return true;
        }

        if (!long.TryParse(decodedToken.Payload.Iat.ToString(), out long iat))
        {
            _logger.LogError($"Token has incorrect 'Iat' payload: {decodedToken.Payload.Iat}. Token: {token}");
            return true;
        }

        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(iat).UtcDateTime;

        var userId = decodedToken.Claims
            .FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError($"Token doesn't have {ClaimTypes.NameIdentifier} payload. Token: {token}");
            return true;
        }

        if (!Guid.TryParse(userId, out Guid guid))
        {
            _logger.LogError($"User guid {userId} is incorrect. Token: {token}");
            return true;
        }

        try
        {
            using (await _rwlock.AcquireReadLockAsync(TimeSpan.FromMilliseconds(_readerTimeout_ms)))
            {
                if(_blockedTokensInfo.ContainsKey(guid))
                {
                    // This log will be removed later
                    _logger.LogInformation($"User with id: {guid} has invalid tokens if they were created before: {_blockedTokensInfo[guid]}.\r\n - Current token has been created at: {issuedAt}.");
                    return issuedAt <= _blockedTokensInfo[guid];
                }
            }
        }
        catch (TimeoutException e)
        {
            _logger.LogError($"Cannot read from {nameof(_blockedTokensInfo)}. Timeout exception occurred.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Cannot read from {nameof(_blockedTokensInfo)}. Exception occurred: {e}");
        }

        return false;
    }

    public async Task BlockAuthTokens(Guid userId)
    {
        try
        {
            using (await _rwlock.AcquireWriteLockAsync(TimeSpan.FromMilliseconds(_writerTimeout_ms)))
            {
                _blockedTokensInfo[userId] = DateTime.UtcNow;
            }
        }
        catch (TimeoutException e)
        {
            _logger.LogError($"Cannot write to {nameof(_blockedTokensInfo)}. Timeout exception occurred.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Cannot write to {nameof(_blockedTokensInfo)}. Exception occurred: {e}");
        }
    }

    public async Task RemoveExpiredTokensData()
    {
        try
        {
            using (await _rwlock.AcquireWriteLockAsync(TimeSpan.FromMilliseconds(_writerTimeout_ms)))
            {
                if (_blockedTokensInfo.Count > 0)
                {
                    _blockedTokensInfo = _blockedTokensInfo
                        .Where(el => el.Value.AddMinutes(_tokenExpirationTime) >= DateTime.UtcNow)
                        .ToDictionary(el => el.Key, el => el.Value);

                    // This log will be removed later
                    _logger.LogInformation($"Removing expired tokens from {nameof(_blockedTokensInfo)} local list.");
                }
                else
                {
                    // This log will be removed later
                    _logger.LogInformation($"Trying to remove expired tokens from {nameof(_blockedTokensInfo)} local list but it is empty, skipping.");
                }
            }
        }
        catch (TimeoutException e)
        {
            _logger.LogError($"Cannot write to {nameof(_blockedTokensInfo)}. Timeout exception occurred.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Cannot write to {nameof(_blockedTokensInfo)}. Exception occurred: {e}");
        }
    }
}
