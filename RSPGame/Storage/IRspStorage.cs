using System.Collections.Generic;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Storage
{
    public interface IRspStorage
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<bool> TryAddUserAsync(User user);

        Task SaveToFile();

        void TryUpdate(string userName, User newUser);

        Task<User> GetUserByUserNameAsync(string userName);
    }
}