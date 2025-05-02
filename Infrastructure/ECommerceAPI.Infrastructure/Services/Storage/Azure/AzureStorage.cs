using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ECommerceAPI.Application.Abstractions.Storage.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerceAPI.Infrastructure.Services.Storage.Azure;

public class AzureStorage : Storage, IAzureStorage
{
	readonly BlobServiceClient blobServiceClient;
	BlobContainerClient blobContainerClient;

	public AzureStorage(IConfiguration configuration)
	{
		blobServiceClient = new(configuration["Storage:Azure"]);
	}

	public async Task DeleteAsync(string containerName, string fileName)
	{
		blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
		await blobClient.DeleteAsync();
	}

	public List<string> GetFiles(string containerName)
	{
		blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		return blobContainerClient.GetBlobs().Select(b => b.Name).ToList();
	}

	public bool HasFile(string containerName, string fileName)
	{
		blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		return blobContainerClient.GetBlobs().Any(b => b.Name == fileName);
	}

	public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string containerName, IFormFileCollection files)
	{
		blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
		await blobContainerClient.CreateIfNotExistsAsync();
		await blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

		List<(string fileName, string pathOrContainerName)> datas = new();

		foreach (IFormFile file in files)
		{
			string fileNewName = await FileRenameAsync(containerName, file.Name, HasFile);

			BlobClient blobClient = blobContainerClient.GetBlobClient(fileNewName);
			await blobClient.UploadAsync(file.OpenReadStream());
			datas.Add((fileNewName, $"{containerName}/{fileNewName}"));
		}
		return datas;
	}
}
