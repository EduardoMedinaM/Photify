using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photify.Uploader.Helpers;

namespace Photify.Uploader
{
    public static class DownloaderFunction
    {
        [FunctionName(nameof(DownloaderFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "photos/{id}")] HttpRequest req,
            Guid id,
            [Blob("photos-small/{id}.jpg", FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream smallImage,
            [Blob("photos-medium/{id}.jpg", FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream mediumImage,
            [Blob("photos/{id}.jpg", FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream originalImage,
            ILogger logger)
        {
            logger?.LogInformation("Downloading {Id}", id);

            var size = req.Query["size"];
            if (string.IsNullOrEmpty(size))
            {
                return new BadRequestResult();
            }

            byte[] image = default;
            if (size.IsEqualsTo("small"))
            {
                image = await GetBytesFromStreamAsync(smallImage);
            }
            if (size.IsEqualsTo("medium"))
            {
                image = await GetBytesFromStreamAsync(mediumImage);
            }
            if (size.IsEqualsTo("original"))
            {
                image = await GetBytesFromStreamAsync(originalImage);
            }

            return image is null
                ? new NotFoundResult()
                : new FileContentResult(image, "image/jpeg")
            {
                FileDownloadName = $"{id}.jpg",
            };
        }

        private static async Task<byte[]> GetBytesFromStreamAsync(Stream stream)
        {
            if(stream is null) throw new ArgumentNullException(nameof(stream));

            byte[] data = new byte[stream.Length];
            await stream.ReadAsync(data);
            return data;
        }
    }
}
