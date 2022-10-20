using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Server.Db;
using Server.Model;

namespace Server.Services;

public class UserService
{
    private readonly ServerDbContext _db;

    public UserService(ServerDbContext db)
    {
        this._db = db;
    }

    public async Task<List<User>> GetList()
    {
        return await this._db.Users.ToListAsync();
    }
    
    public async Task<User?> Find(Guid id)
    {
        return await this._db.Users.FindAsync(id);
    }
    
    public async Task<User?> Find(string username)
    {
        return await this._db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public bool CheckPassword(User user, string password)
    {
        return user.PasswordHash == hashPassword(password);
    }
    
    public async Task<User> RegisterUser(string username, string password)
    {
        var entry = this._db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = hashPassword(password)
        });

        await this._db.SaveChangesAsync();

        return entry.Entity;
    }

    private string hashPassword(string password)
    {
        return Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(password)));
    }
}