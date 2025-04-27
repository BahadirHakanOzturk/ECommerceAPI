using ECommerceAPI.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;

namespace ECommerceAPI.Infrastructure.Services.Storage;

public class StorageService : IStorageService
{
	private readonly IStorage storage;

    public StorageService(IStorage storage)
    {
        this.storage = storage;
    }

	public string StorageName { get => storage.GetType().Name; }

	public Task DeleteAsync(string pathOrContainerName, string fileName)
		=> storage.DeleteAsync(pathOrContainerName, fileName);

	public List<string> GetFiles(string pathOrContainerName)
		=>storage.GetFiles(pathOrContainerName);

	public bool HasFile(string pathOrContainerName, string fileName)
		=>storage.HasFile(pathOrContainerName, fileName);

	public Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files)
		=> storage.UploadAsync(pathOrContainerName, files);
}
