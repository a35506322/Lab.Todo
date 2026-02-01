using System.Text.RegularExpressions;
using TodoAPI.Infrastructures.ValidationMessage;

namespace TodoAPI.Infrastructures.ExceptionHandler.ValidationMessage;

/// <summary>
/// 將 Minimal API 驗證預設英文錯誤訊息替換為資源檔的本地化訊息。
/// 在 CustomizeProblemDetails 中對 ValidationProblemDetails.Errors 事後替換使用。
/// </summary>
public static class ValidationErrorLocalizer
{
    /// <summary>
    /// 將框架產生的預設錯誤訊息轉成資源檔文案；無法對應時回傳原訊息。
    /// 顯示名稱等參數皆從 defaultMessage 內文解析，無須傳入 propertyKey。
    /// </summary>
    /// <param name="defaultMessage">框架產生的預設錯誤訊息。</param>
    /// <returns>本地化訊息或原訊息。</returns>
    public static string Localize(string defaultMessage)
    {
        try
        {
            if (string.IsNullOrEmpty(defaultMessage))
                return defaultMessage;

            // ValidationMetadata（DataAnnotations）常見預設英文格式
            var localized = TryLocalizeValidationMetadata(defaultMessage);
            if (localized is not null)
                return localized;

            // ModelBinding 常見預設英文格式
            localized = TryLocalizeModelBinding(defaultMessage);
            if (localized is not null)
                return localized;

            return defaultMessage;
        }
        catch (Exception ex)
        {
            return defaultMessage;
        }
    }

    private static string? TryLocalizeValidationMetadata(string message)
    {
        // Required: "The {0} field is required." — 從訊息中取出已被框架代入的顯示名稱（如 Display 的「帳號」）
        var requiredMatch = Regex.Match(
            message,
            @"The (.+) field is required\.",
            RegexOptions.IgnoreCase
        );
        if (requiredMatch.Success)
            return string.Format(
                ValidationMetadataMessage.RequiredAttribute_ValidationError,
                requiredMatch.Groups[1].Value.Trim()
            );

        // Range: "The field {0} must be between {1} and {2}."
        var rangeMatch = Regex.Match(
            message,
            @"The field (.+) must be between (.+) and (.+)\.",
            RegexOptions.IgnoreCase
        );
        if (rangeMatch.Success)
            return string.Format(
                ValidationMetadataMessage.RangeAttribute_ValidationError,
                rangeMatch.Groups[1].Value.Trim(),
                rangeMatch.Groups[2].Value.Trim(),
                rangeMatch.Groups[3].Value.Trim()
            );

        // StringLength max only: "The field {0} must be a string with a maximum length of {1}."
        var maxLenMatch = Regex.Match(
            message,
            @"The field (.+) must be a string with a maximum length of (\d+)\.",
            RegexOptions.IgnoreCase
        );
        if (
            maxLenMatch.Success
            && !message.Contains("minimum length", StringComparison.OrdinalIgnoreCase)
        )
            return string.Format(
                ValidationMetadataMessage.StringLengthAttribute_ValidationError,
                maxLenMatch.Groups[1].Value.Trim(),
                maxLenMatch.Groups[2].Value
            );

        // StringLength min and max: "The field {0} ... minimum length of '{2}' and a maximum length of '{1}'."
        var minMaxMatch = Regex.Match(
            message,
            @"The field (.+) must be.*minimum length of '(\d+)'.*maximum length of '(\d+)'",
            RegexOptions.IgnoreCase
        );
        if (minMaxMatch.Success)
            return string.Format(
                ValidationMetadataMessage.StringLengthAttribute_ValidationErrorIncludingMinimum,
                minMaxMatch.Groups[1].Value.Trim(),
                minMaxMatch.Groups[3].Value,
                minMaxMatch.Groups[2].Value
            );

        // MinLength: "The field {0} must be a string or array type with a minimum length of '{1}'."
        var minLenMatch = Regex.Match(
            message,
            @"The field (.+) must be.*minimum length of '(\d+)'",
            RegexOptions.IgnoreCase
        );
        if (
            minLenMatch.Success
            && !message.Contains("maximum length", StringComparison.OrdinalIgnoreCase)
        )
            return string.Format(
                ValidationMetadataMessage.MinLengthAttribute_ValidationError,
                minLenMatch.Groups[1].Value.Trim(),
                minLenMatch.Groups[2].Value
            );

        // MaxLength: "The field {0} must be a string or array type with a maximum length of '{1}'."
        var maxLenMatch2 = Regex.Match(
            message,
            @"The field (.+) must be.*maximum length of '(\d+)'",
            RegexOptions.IgnoreCase
        );
        if (maxLenMatch2.Success)
            return string.Format(
                ValidationMetadataMessage.MaxLengthAttribute_ValidationError,
                maxLenMatch2.Groups[1].Value.Trim(),
                maxLenMatch2.Groups[2].Value
            );

        // EmailAddress: "The {0} field is not a valid e-mail address."
        var emailMatch = Regex.Match(
            message,
            @"The (.+) field is not a valid e-mail address\.",
            RegexOptions.IgnoreCase
        );
        if (emailMatch.Success)
            return string.Format(
                ValidationMetadataMessage.EmailAddressAttribute_Invalid,
                emailMatch.Groups[1].Value.Trim()
            );

        // Phone: "The {0} field is not a valid phone number."
        var phoneMatch = Regex.Match(
            message,
            @"The (.+) field is not a valid phone number\.",
            RegexOptions.IgnoreCase
        );
        if (phoneMatch.Success)
            return string.Format(
                ValidationMetadataMessage.PhoneAttribute_Invalid,
                phoneMatch.Groups[1].Value.Trim()
            );

        // Url: "The {0} field is not a valid fully-qualified http, https, or ftp URL."
        var urlMatch = Regex.Match(
            message,
            @"The (.+) field is not a valid fully-qualified.*http.*url",
            RegexOptions.IgnoreCase
        );
        if (urlMatch.Success)
            return string.Format(
                ValidationMetadataMessage.UrlAttribute_Invalid,
                urlMatch.Groups[1].Value.Trim()
            );

        // CreditCard: "The {0} field is not a valid credit card number."
        var cardMatch = Regex.Match(
            message,
            @"The (.+) field is not a valid credit card",
            RegexOptions.IgnoreCase
        );
        if (cardMatch.Success)
            return string.Format(
                ValidationMetadataMessage.CreditCardAttribute_Invalid,
                cardMatch.Groups[1].Value.Trim()
            );

        // RegularExpression: "The field {0} must match the regular expression '{1}'."
        var regexMatch = Regex.Match(
            message,
            @"The field (.+) must match the regular expression '(.+)'",
            RegexOptions.IgnoreCase
        );
        if (regexMatch.Success)
            return string.Format(
                ValidationMetadataMessage.RegularExpressionAttribute_ValidationError,
                regexMatch.Groups[1].Value.Trim(),
                regexMatch.Groups[2].Value
            );

        // FileExtensions: "The {0} field only accepts files with the following extensions: {1}"
        var extMatch = Regex.Match(
            message,
            @"The (.+) field only accepts files with the following extensions: (.+)",
            RegexOptions.IgnoreCase
        );
        if (extMatch.Success)
            return string.Format(
                ValidationMetadataMessage.FileExtensionsAttribute_Invalid,
                extMatch.Groups[1].Value.Trim(),
                extMatch.Groups[2].Value.Trim()
            );

        // Compare: "'{0}' and '{1}' do not match." — 若 resx 新增 CompareAttribute_MustMatch 可在此用資源替換
        return null;
    }

    private static string? TryLocalizeModelBinding(string message)
    {
        // "A non-empty request body is required."
        if (Regex.IsMatch(message, @"non-empty request body is required", RegexOptions.IgnoreCase))
            return ModelBindingMessage.MissingRequestBodyRequiredValue;

        // "A value for the '{0}' property was not provided."
        var missingBindMatch = Regex.Match(
            message,
            @"value for the '(.+)' property was not provided",
            RegexOptions.IgnoreCase
        );
        if (missingBindMatch.Success)
            return string.Format(
                ModelBindingMessage.MissingBindRequiredValue,
                missingBindMatch.Groups[1].Value.Trim()
            );

        // "The value '{0}' is not valid for {1}."
        var attemptedMatch = Regex.Match(
            message,
            @"The value '(.+)' is not valid for (.+)\.",
            RegexOptions.IgnoreCase
        );
        if (attemptedMatch.Success)
            return string.Format(
                ModelBindingMessage.AttemptedValueIsInvalid,
                attemptedMatch.Groups[1].Value,
                attemptedMatch.Groups[2].Value.Trim()
            );

        // "The value '{0}' is not valid." — 訊息內無欄位名，用「此欄位」
        if (Regex.IsMatch(message, @"The value '.+' is not valid\.", RegexOptions.IgnoreCase))
            return string.Format(ModelBindingMessage.ValueIsInvalid, "此欄位");

        // "The supplied value is invalid."
        if (Regex.IsMatch(message, @"supplied value is invalid", RegexOptions.IgnoreCase))
            return ModelBindingMessage.NonPropertyUnknownValueIsInvalid;

        // "The field must be a number."
        if (Regex.IsMatch(message, @"must be a number", RegexOptions.IgnoreCase))
            return ModelBindingMessage.NonPropertyValueMustBeANumber;

        // "A value is required." — 訊息內無欄位名，用「此欄位」
        if (Regex.IsMatch(message, @"value is required", RegexOptions.IgnoreCase))
            return string.Format(ModelBindingMessage.ValueMustNotBeNull, "此欄位");

        return null;
    }
}
