using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RSPGame.Services
{
    public class FileWorker : IFileWorker
    {
        public async Task SaveToFileAsync<T>(string path, T obj)
        {
            await using var fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write);
            
            await JsonSerializer.SerializeAsync(fileStream, obj);
        }

        public async Task<T> DeserializeAsync<T>(string path)
        {
            await using var fileStream = new FileStream(path, FileMode.Open);

            var objects = await JsonSerializer.DeserializeAsync<T>(fileStream);

            return objects;
        }
    }
}