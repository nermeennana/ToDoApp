using System;

namespace ServiceAbstractionLayer
{
    public interface ICurrentUserService
    {
        string UserName { get; }
    }
}
