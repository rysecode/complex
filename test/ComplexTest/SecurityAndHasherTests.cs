using Complex.Application.Common.Exceptions;
using Complex.Application.Common.Hashers;
using Complex.Application.Common.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ComplexTest;

public class SecurityAndHasherTests
{
    [Fact]
    public void PasswordHasher_ShouldHashAndVerifyPassword()
    {
        var (hash, salt) = PasswordHasher.Hash("secure-pass");

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.False(string.IsNullOrWhiteSpace(salt));
        Assert.True(PasswordHasher.Verify("secure-pass", hash, salt));
        Assert.False(PasswordHasher.Verify("wrong-pass", hash, salt));
    }

    [Fact]
    public void PasswordHasher_ShouldRejectShortPasswords()
    {
        Assert.Throws<DomainException>(() => PasswordHasher.Hash("short"));
    }

    [Fact]
    public void ClaimsPrincipalExtensions_ShouldReadExpectedClaims()
    {
        var user = CreatePrincipal();

        Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), user.GetUsuarioId());
        Assert.Equal(Guid.Parse("22222222-2222-2222-2222-222222222222"), user.GetEmpresaId());
        Assert.Equal(Guid.Parse("33333333-3333-3333-3333-333333333333"), user.GetPessoaId());
        Assert.Equal("user@ryse.dev", user.GetEmail());
    }

    [Fact]
    public void ClaimsPrincipalExtensions_WhenMissingClaim_ShouldThrow()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Throws<UnauthorizedAccessException>(() => user.GetUsuarioId());
        Assert.Throws<UnauthorizedAccessException>(() => user.GetEmail());
    }

    [Fact]
    public void HttpContextCurrentUser_ShouldExposeExpectedValues()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreatePrincipal()
            }
        };

        var currentUser = new HttpContextCurrentUser(accessor);

        Assert.True(currentUser.IsAuthenticated);
        Assert.Equal(Guid.Parse("11111111-1111-1111-1111-111111111111"), currentUser.UsuarioId);
        Assert.Equal(Guid.Parse("22222222-2222-2222-2222-222222222222"), currentUser.EmpresaId);
        Assert.Equal(Guid.Parse("33333333-3333-3333-3333-333333333333"), currentUser.PessoaId);
        Assert.Equal("user@ryse.dev", currentUser.Email);
    }

    [Fact]
    public void HttpContextCurrentUser_WhenNotAuthenticated_ShouldThrowOnClaimAccess()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var currentUser = new HttpContextCurrentUser(accessor);

        Assert.False(currentUser.IsAuthenticated);
        Assert.Throws<UnauthorizedAccessException>(() => currentUser.UsuarioId);
        Assert.Throws<UnauthorizedAccessException>(() => currentUser.Email);
    }

    private static ClaimsPrincipal CreatePrincipal()
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim("usuario_id", "11111111-1111-1111-1111-111111111111"),
                    new Claim("empresa_id", "22222222-2222-2222-2222-222222222222"),
                    new Claim("pessoa_id", "33333333-3333-3333-3333-333333333333"),
                    new Claim(ClaimTypes.Email, "user@ryse.dev")
                },
                authenticationType: "TestAuth"));
    }
}
