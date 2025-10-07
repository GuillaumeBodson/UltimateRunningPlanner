using Microsoft.EntityFrameworkCore;
using User.API.Data;

namespace User.API.Repository;

public class UserRepository(UserDbContext context)
{
    private readonly UserDbContext _context = context;

    public async Task<IEnumerable<Models.User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Athlete)
            .Include(u => u.AthletePreferences)
            .ToListAsync();
    }

    public async Task<Models.User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Athlete)
            .Include(u => u.AthletePreferences)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Models.User> AddUserAsync(Models.User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(Models.User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
