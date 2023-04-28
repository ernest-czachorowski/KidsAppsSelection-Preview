namespace Presentation.Services.StateService;

public class PasswordVisibilityState
{
    private bool _showPassword = false;
    public bool ShowPassword
    { 
        get => _showPassword;
        set 
        {
            _showPassword = value;
            OnShowPasswordChanged?.Invoke();
        }
    }

    public event Action OnShowPasswordChanged;
}

public class RefreshTokenState
{
    private SetTimeout? _refreshTokenTimeout;

    public void SetRefreshTokenTimeout(SetTimeout refreshTokenTimeout)
    {
        if (_refreshTokenTimeout is not null)
            _refreshTokenTimeout.Dispose();

        _refreshTokenTimeout = refreshTokenTimeout;
        _refreshTokenTimeout.Start();
    }

    public void UnsetRefreshTokenTimeout()
    {
        if (_refreshTokenTimeout is null)
            return;

        _refreshTokenTimeout.Dispose();
        _refreshTokenTimeout = null;
    }
}