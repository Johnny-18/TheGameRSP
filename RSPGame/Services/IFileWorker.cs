using System.Threading.Tasks;

namespace RSPGame.Services
{
    public interface IFileWorker
    {
        Task SaveToFileAsync<T>(string path, T obj);

        Task<T> DeserializeAsync<T>(string path);
    }
}