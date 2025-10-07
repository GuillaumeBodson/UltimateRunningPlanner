using Microsoft.EntityFrameworkCore;
using User.API.Data;

namespace User.API.Repository;

public class UserRepository(UserDbContext context, ILogger<UserRepository> logger)
{
    private readonly UserDbContext _context = context;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<IEnumerable<Models.User>> GetAllUsersAsync()
    {
        _logger.LogInformation("Retrieving all users");
        return await _context.Users
            .Include(u => u.Athlete)
            .Include(u => u.AthletePreferences)
            .ToListAsync();
    }

    public async Task<Models.User?> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", id);
        var user = await _context.Users
            .Include(u => u.Athlete)
            .Include(u => u.AthletePreferences)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID: {UserId} not found", id);
        }
        
        return user;
    }

    public async Task<Models.User> AddUserAsync(Models.User user)
    {
        _logger.LogInformation("Adding new user");
        _context.Users.Add(user);
        
        try 
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added user with ID: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding user");
            throw;
        }
        
        return user;
    }

    public async Task UpdateUserAsync(Models.User user)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", user.Id);
        _context.Entry(user).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated user with ID: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", user.Id);
            throw;
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
                throw;
            }
        }
        else
        {
            _logger.LogWarning("Attempted to delete non-existent user with ID: {UserId}", id);
        }
    }
}
