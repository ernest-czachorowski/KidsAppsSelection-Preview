namespace Presentation.Razor.Default.Components;

public class PasswordBase : TwoWayBinding_T<string>, IDisposable
{
    [Inject]
    protected PasswordVisibilityState PasswordVisibilityState { get; set; }
    [Parameter]
    public string Label { get; set; } = "Password";
    [Parameter, EditorRequired]
    public Expression<Func<string>> For { get; set; }

    protected InputType passwordInput = InputType.Password;
    protected string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        PasswordVisibilityState.OnShowPasswordChanged += ShowPasswordHasChanged;

        if(PasswordVisibilityState.ShowPassword)
            PasswordVisibilityState.ShowPassword = false;
    }

    protected void ToggleShowPassword()
    {
        PasswordVisibilityState.ShowPassword = !PasswordVisibilityState.ShowPassword;
    }

    protected void ShowPasswordHasChanged()
    {
        if (!PasswordVisibilityState.ShowPassword)
        {
            passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            passwordInput = InputType.Password;
        }
        else
        {
            passwordInputIcon = Icons.Material.Filled.Visibility;
            passwordInput = InputType.Text;
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        PasswordVisibilityState.OnShowPasswordChanged -= ShowPasswordHasChanged;
    }
}