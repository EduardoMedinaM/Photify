using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Photify.Uploader.Helpers;

namespace Photify.Uploader
{
    public class ResizerFunction
    {
        [FunctionName(nameof(ResizerFunction))]
        public async Task Run(
            [BlobTrigger("photify-photos/{name}", Connection = Literals.StorageConnectionString)]Stream currentBlob,
            string name,
            [Blob("photify-photos-small/{name}", FileAccess.Write, Connection = Literals.StorageConnectionString)] Stream imageSmall,
            [Blob("photify-photos-medium/{name}", FileAccess.Write, Connection = Literals.StorageConnectionString)] Stream imageMedium,
            ILogger logger)
        {
            logger?.LogInformation($"Resizing image {name}");

            try
            {
                using var msMedium = Resizer.CreateMemoryStream(currentBlob, ImageSize.Medium);
                await msMedium.CopyToAsync(imageMedium);

                using var msSmall = Resizer.CreateMemoryStream(currentBlob, ImageSize.Small);
                await msSmall.CopyToAsync(imageSmall);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
            }
            finally
            {
                imageSmall.Close();
                imageMedium.Close();
            }
        }
    }
}
