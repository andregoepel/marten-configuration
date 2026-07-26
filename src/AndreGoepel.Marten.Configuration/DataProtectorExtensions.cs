using Microsoft.AspNetCore.DataProtection;

namespace AndreGoepel.Marten.Configuration;

/// <summary>
/// Shared round trip for a DataProtection-encrypted secret field inside a
/// <see cref="SettingsDocument"/> (an SMTP password, an API token, a provider credential, ...).
/// </summary>
public static class DataProtectorExtensions
{
    /// <summary>
    /// Protects <paramref name="newPlaintext"/> when the caller supplied one; otherwise keeps
    /// <paramref name="existingCiphertext"/> unchanged. This is the "leave the field blank to
    /// keep the current secret" idiom used by every settings form that edits a protected value.
    /// Returns <c>null</c> when there is neither a new value nor an existing one — callers
    /// decide what that means for them (reject the save, or protect an explicit empty string
    /// when a blank secret is itself a valid saved state).
    /// </summary>
    public static string? ProtectOrKeepExisting(
        this IDataProtector protector,
        string? newPlaintext,
        string? existingCiphertext
    )
    {
        ArgumentNullException.ThrowIfNull(protector);
        return !string.IsNullOrEmpty(newPlaintext)
            ? protector.Protect(newPlaintext)
            : existingCiphertext;
    }
}
