using Microsoft.AspNetCore.DataProtection;

namespace AndreGoepel.Marten.Configuration.Tests;

public sealed class DataProtectorExtensionsTests
{
    private readonly IDataProtector protector =
        new EphemeralDataProtectionProvider().CreateProtector(
            "AndreGoepel.Marten.Configuration.Tests.DataProtectorExtensions"
        );

    [Fact]
    public void ProtectOrKeepExisting_NewPlaintextGiven_ProtectsTheNewValue()
    {
        // Act
        var ciphertext = protector.ProtectOrKeepExisting("new-secret", "old-ciphertext");

        // Assert
        Assert.NotNull(ciphertext);
        Assert.NotEqual("old-ciphertext", ciphertext);
        Assert.Equal("new-secret", protector.Unprotect(ciphertext));
    }

    [Fact]
    public void ProtectOrKeepExisting_NewPlaintextEmpty_KeepsExistingCiphertext()
    {
        // Act
        var ciphertext = protector.ProtectOrKeepExisting(string.Empty, "old-ciphertext");

        // Assert
        Assert.Equal("old-ciphertext", ciphertext);
    }

    [Fact]
    public void ProtectOrKeepExisting_NewPlaintextNull_KeepsExistingCiphertext()
    {
        // Act
        var ciphertext = protector.ProtectOrKeepExisting(null, "old-ciphertext");

        // Assert
        Assert.Equal("old-ciphertext", ciphertext);
    }

    [Fact]
    public void ProtectOrKeepExisting_NeitherNewValueNorExistingCiphertextGiven_ReturnsNull()
    {
        // Act
        var ciphertext = protector.ProtectOrKeepExisting(null, null);

        // Assert
        Assert.Null(ciphertext);
    }

    [Fact]
    public void ProtectOrKeepExisting_RoundTripsThroughUnprotect()
    {
        // Act
        var ciphertext = protector.ProtectOrKeepExisting("correct horse battery staple", null);

        // Assert
        Assert.NotNull(ciphertext);
        Assert.Equal("correct horse battery staple", protector.Unprotect(ciphertext));
    }

    [Fact]
    public void ProtectOrKeepExisting_NullProtector_Throws()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() =>
            DataProtectorExtensions.ProtectOrKeepExisting(null!, "value", null)
        );
    }
}
