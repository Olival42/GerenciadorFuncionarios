using Xunit;
using Moq;
using GerenciadorFuncionarios.Services.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class UserContextServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly UserContextService _userContextService;

    public UserContextServiceTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _userContextService = new UserContextService(_httpContextAccessor.Object);
    }

    [Fact]
    public void GetUserId_Should_ReturnUserId_WhenClaimExists()
    {
        var userId = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        _httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var result = _userContextService.GetUserId();

        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetUserId_Should_ReturnNull_WhenHttpContextIsNull()
    {
        _httpContextAccessor.Setup(x => x.HttpContext)
            .Returns((HttpContext?)null);

        var result = _userContextService.GetUserId();

        Assert.Null(result);
    }

    
    [Fact]
    public void GetUserId_Should_ReturnNull_WhenClaimNotFound()
    {
        var httpContext = new DefaultHttpContext();

        _httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var result = _userContextService.GetUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_Should_ReturnNull_WhenClaimValueIsNull()
    {
        var user = new ClaimsPrincipal();

        var httpContext = new DefaultHttpContext
        {
            User = user
        };

        _httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var result = _userContextService.GetUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetUserId_Should_ReturnNull_WhenUserIsNull()
    {
        var httpContext = new DefaultHttpContext
        {
            User = null!
        };

        _httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);

        var result = _userContextService.GetUserId();

        Assert.Null(result);
    }
}