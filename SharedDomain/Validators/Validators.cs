namespace SharedDomain.Validators;

public class PlayStoreUrlAttribute : ValidationAttribute
{
    private const string _pattern = @"https:\/\/play\.google\.com\/store\/apps\/details\?id=";

    public override bool IsValid(object? value)
    {
        if (value == null)
            return false;

        string url = value.ToString()!;
        return Regex.IsMatch(url, _pattern);
    }
}

public class ThisLessThanAttribute : ValidationAttribute
{
    private readonly string _stopPropertyName;

    public ThisLessThanAttribute(string stopPropertyName)
    {
        _stopPropertyName = stopPropertyName;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult($"Field cannot be empty.");

        var min = (int)value;
        var max = (int)validationContext.ObjectType.GetProperty(_stopPropertyName).GetValue(validationContext.ObjectInstance, null);

        if (min > max)
        {
            return new ValidationResult($"This value must be less than {validationContext.ObjectType.GetProperty(_stopPropertyName)} value.");
        }

        return ValidationResult.Success!;
    }
}

public class ThisGreaterThanAttribute : ValidationAttribute
{
    private readonly string _stopPropertyName;

    public ThisGreaterThanAttribute(string stopPropertyName)
    {
        _stopPropertyName = stopPropertyName;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult($"Field cannot be empty.");

        var min = (int)validationContext.ObjectType.GetProperty(_stopPropertyName).GetValue(validationContext.ObjectInstance, null);
        var max = (int)value;

        if (min > max)
        {
            return new ValidationResult($"This value must be greater than {validationContext.ObjectType.GetProperty(_stopPropertyName)} value.");
        }

        return ValidationResult.Success!;
    }
}

public class ValidEmailAttribute : ValidationAttribute
{
    private readonly Regex _regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult($"Field cannot be empty.");

        var email = value as string;

        if (email == null)
        {
            return new ValidationResult("Email is required.");
        }

        if (!_regex.IsMatch(email))
        {
            return new ValidationResult("Invalid email format.");
        }

        return ValidationResult.Success!;
    }
}

public class PopularValidEmailAttribute : ValidationAttribute
{
    private readonly string[] _providers = PopularEmails.emails;
    private readonly Regex _regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult($"Field cannot be empty.");

        var email = value as string;

        if (email == null)
        {
            return new ValidationResult("Email is required.");
        }

        if (!_regex.IsMatch(email))
        {
            return new ValidationResult("Invalid email format.");
        }

        var provider = email.Split('@')[1];

        if (!_providers.Contains(provider))
        {
            return new ValidationResult("Invalid email provider. Your email provider must be popular eg. google.com, yahoo.com...");
        }

        return ValidationResult.Success!;
    }
}

public class LettersAndDigitsAttribute : RegularExpressionAttribute
{
    public LettersAndDigitsAttribute() : base(@"^[a-zA-Z0-9]+$")
    {
        ErrorMessage = "Field must contain only letters and digits.";
    }
}

public class LettersDigitsAndDashesAttribute : RegularExpressionAttribute
{
    public LettersDigitsAndDashesAttribute() : base(@"^[a-zA-Z0-9_-]+$")
    {
        ErrorMessage = "Field must contain only letters and digits.";
    }
}

public class NoWhitespaceAttribute : RegularExpressionAttribute
{
    public NoWhitespaceAttribute() : base(@"^[^\s]+$")
    {
        ErrorMessage = "Field must not contain white characters.";
    }
}