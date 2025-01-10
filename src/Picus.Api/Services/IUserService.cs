using Picus.Api.Models;

namespace Picus.Api.Services;

public interface IUserService
{
    Task<User?> GetUserByAuth0IdAsync(string auth0Id);
    Task<User> CreateUserAsync(string auth0Id, string email);
    Task<User?> GetOrCreateUserAsync(string auth0Id, string email);
}
