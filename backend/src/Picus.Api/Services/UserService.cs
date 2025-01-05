using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;
using Picus.Api.Models;

namespace Picus.Api.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IRepository<User> userRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> GetUserByAuth0IdAsync(string auth0Id)
    {
        _logger.LogInformation("Looking for user with Auth0Id: {Auth0Id}", auth0Id);
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Auth0Id == auth0Id);
        _logger.LogInformation("Found user: {Found}", user != null);
        return user;
    }

    public async Task<User> CreateUserAsync(string auth0Id, string email)
    {
        _logger.LogInformation("Creating new user with Auth0Id: {Auth0Id} and email: {Email}", auth0Id, email);
        
        var user = new User
        {
            Auth0Id = auth0Id,
            Email = email,
            Username = email,
            DisplayName = email.Split('@')[0],
            IsActive = true,
            Role = "Player"
        };

        try 
        {
            await _userRepository.AddAsync(user);
            _logger.LogInformation("Successfully created user with Id: {Id}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with Auth0Id: {Auth0Id}", auth0Id);
            throw;
        }
    }

    public async Task<User?> GetOrCreateUserAsync(string auth0Id, string email)
    {
        _logger.LogInformation("Getting or creating user with Auth0Id: {Auth0Id}", auth0Id);
        var user = await GetUserByAuth0IdAsync(auth0Id);
        
        if (user != null)
        {
            _logger.LogInformation("Found existing user with Id: {Id}", user.Id);
            return user;
        }

        _logger.LogInformation("No existing user found, creating new user");
        return await CreateUserAsync(auth0Id, email);
    }
}
