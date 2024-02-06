using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photify.Uploader.Helpers;
using Newtonsoft.Json;
using Photify.Uploader.Dtos;
using System;
using Azure.Storage.Blobs;

namespace Photify.Uploader
{
    public static class UploaderFunction
    {
        [FunctionName(nameof(UploaderFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("photos", FileAccess.ReadWrite, Connection = Literals.StorageConnectionString)] BlobContainerClient blobContainer,
            ILogger logger)
        {

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<UploadPhotoDto>(body);

            var newPhotoId = Guid.NewGuid();
            var newPhotoName = $"{newPhotoId}.jpg";

            await blobContainer.CreateIfNotExistsAsync();

            var cloudBlockBlob = blobContainer.GetBlobClient(newPhotoName);

            var photoBytes = Convert.FromBase64String(request.Photo);

            await cloudBlockBlob.UploadAsync(new BinaryData(photoBytes));

            logger?.LogInformation($"Photo successfully uploaded", newPhotoId);

            return new OkObjectResult(newPhotoId);
        }
    }
}
