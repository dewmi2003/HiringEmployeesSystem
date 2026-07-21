using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recruitment.Application.Interfaces.Services;


namespace Recruitment.Infrastructure.Storage
{
    public class AzureBlobStorageService : IStorageService
    {

        private readonly BlobContainerClient _container;
        private readonly ILogger<AzureBlobStorageService> _logger;


        public AzureBlobStorageService(
            IOptions<StorageSettings> settings,
            ILogger<AzureBlobStorageService> logger)
        {

            _logger = logger;


            var client =
                new BlobServiceClient(
                    settings.Value.ConnectionString);


            _container =
                client.GetBlobContainerClient(
                    settings.Value.ContainerName);



            _container.CreateIfNotExists(
                PublicAccessType.None);
        }



        public async Task<string> UploadAsync(
            Stream stream,
            string fileName,
            string contentType)
        {

            var blob =
                _container.GetBlobClient(fileName);



            await blob.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                });



            _logger.LogInformation(
                "Uploaded {File}",
                fileName);



            return blob.Name;
        }




        public async Task<Stream?> DownloadAsync(
            string fileName)
        {

            var blob =
                _container.GetBlobClient(fileName);



            if(!await blob.ExistsAsync())
            {
                return null;
            }



            var memory =
                new MemoryStream();



            await blob.DownloadToAsync(memory);



            memory.Position = 0;


            return memory;
        }





        public async Task DeleteAsync(
            string fileName)
        {

            var blob =
                _container.GetBlobClient(fileName);



            await blob.DeleteIfExistsAsync();


            _logger.LogInformation(
                "Deleted {File}",
                fileName);
        }

    }
}