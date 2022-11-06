using Microsoft.AspNetCore.Http;

namespace Ididit.App;

public class PreRenderService : IPreRenderService
{
    public bool IsPreRendering { get; private set; }

    public PreRenderService()
    {
    }

    public PreRenderService(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext.Response.HasStarted)
        {
            IsPreRendering = false;
        }
        else
        {
            IsPreRendering = true;
        }
    }
}

public interface IPreRenderService
{
    bool IsPreRendering { get; }
}