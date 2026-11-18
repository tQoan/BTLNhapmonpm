using QuanLyCuaHangTheThao.Services;
using Xunit;

namespace QuanLyCuaHangTheThao.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldProducePBKDF2Format()
    {
        var result = PasswordHasher.Hash("password123");

        Assert.StartsWith("PBKDF2:", result);
        var parts = result.Split(':');
        Assert.Equal(4, parts.Length);
        Assert.Equal("100000", parts[1]);
        Assert.NotEmpty(parts[2]);
        Assert.NotEmpty(parts[3]);
    }

    [Fact]
    public void Hash_ShouldProduceDifferentSalts_ForSamePassword()
    {
        var hash1 = PasswordHasher.Hash("password123");
        var hash2 = PasswordHasher.Hash("password123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_ShouldReturnTrue_ForCorrectPassword()
    {
        var hash = PasswordHasher.Hash("admin123");

        Assert.True(PasswordHasher.Verify("admin123", hash));
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForWrongPassword()
    {
        var hash = PasswordHasher.Hash("admin123");

        Assert.False(PasswordHasher.Verify("wrongpassword", hash));
    }

    [Fact]
    public void Verify_ShouldReturnTrue_ForLegacyPlaintext()
    {
        Assert.True(PasswordHasher.Verify("admin123", "admin123"));
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForEmptyStored()
    {
        Assert.False(PasswordHasher.Verify("admin123", ""));
        Assert.False(PasswordHasher.Verify("admin123", null!));
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForMalformedHash()
    {
        Assert.False(PasswordHasher.Verify("admin123", "PBKDF2:100000:only2parts"));
        Assert.False(PasswordHasher.Verify("admin123", "PBKDF2:notanumber:abc:def"));
    }
}
