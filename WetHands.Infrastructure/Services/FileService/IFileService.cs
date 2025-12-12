using System.Threading.Tasks;

namespace WetHands.Infrastructure.Services.FileService
{
    public interface IFileService
    {
        Task<bool> DeleteFileFromServer(string fileName, string path, string subPath);

    }
}