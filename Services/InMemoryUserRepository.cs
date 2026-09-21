using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId;

    public IReadOnlyCollection<User> GetAll() => _users.Values.ToArray();

    public User? GetById(int id) =>
        _users.TryGetValue(id, out var user) ? user : null;

    public User Add(User user)
    {
        user.Id = Interlocked.Increment(ref _nextId);
        _users[user.Id] = user;
        return user;
    }

    public bool Update(int id, User user)
    {
        user.Id = id;

        while (_users.TryGetValue(id, out var existingUser))
        {
            if (_users.TryUpdate(id, user, existingUser))
            {
                return true;
            }
        }

        return false;
    }

    public bool Delete(int id) => _users.TryRemove(id, out _);
}
