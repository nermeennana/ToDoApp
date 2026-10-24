using System;

namespace Shared.DataTransferObjects
{
    public class AuthResponseDto
    {
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
