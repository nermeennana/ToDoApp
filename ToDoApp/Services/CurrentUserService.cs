using System;
using System.Security.Claims;
using ServiceAbstractionLayer;

namespace ServiceLayer
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string UserName =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "System";
    }
}
