using System.Security.Claims;

namespace PitVendasApi.Services
{
    public interface ICurrentUser
    {
        string? Id { get; }
        bool IsAuthenticated { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        public string? Id =>
            _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool IsAuthenticated => !string.IsNullOrEmpty(Id);
    }
}
