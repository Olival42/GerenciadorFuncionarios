using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using GerenciadorFuncionarios.Infrastructure.Security;

public class JwtEventsConfigTests
{
    [Fact]
    public async Task OnChallenge_Should_Return_401_With_Error_Response()
    {
        var events = JwtEventsConfig.GetEvents();

        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var challengeContext = new JwtBearerChallengeContext(
            context,
            new AuthenticationScheme("Bearer", "Bearer", typeof(JwtBearerHandler)),
            new JwtBearerOptions(),
            new AuthenticationProperties()
        );

        await events.OnChallenge(challengeContext);

        Assert.Equal(401, context.Response.StatusCode);

        responseBody.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(responseBody).ReadToEndAsync();

        Assert.Contains("UNAUTHORIZED", json);
    }

    [Fact]
    public async Task OnForbidden_Should_Return_403_With_Error_Response()
    {
        var events = JwtEventsConfig.GetEvents();

        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var forbiddenContext = new ForbiddenContext(
            context,
            new AuthenticationScheme("Bearer", "Bearer", typeof(JwtBearerHandler)),
            new JwtBearerOptions()
        );

        await events.OnForbidden(forbiddenContext);

        Assert.Equal(403, context.Response.StatusCode);

        responseBody.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(responseBody).ReadToEndAsync();

        Assert.Contains("FORBIDDEN", json);
    }
}
