using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Photify.Uploader.Helpers;
using System.Collections.Generic;
using Photify.Uploader.Entities;
using Microsoft.Azure.Cosmos;
using System.Linq;

namespace Photify.Uploader
{
    public static class SearcherFunction
    {
        [FunctionName("SearcherFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB("photify-photos", "metadata", Connection = Literals.CosmosDBConnectionString)] CosmosClient cosmosClient,
            ILogger logger)
        {

            var searchTerm = req.Query["searchTerm"].ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new NotFoundResult();
            }

            logger.LogInformation($"Searching by term {searchTerm}");

            Container container = cosmosClient.GetDatabase("photify-photos").GetContainer("metadata");

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM photos p WHERE CONTAINS(p.description, @searchTerm)")
                .WithParameter("@searchTerm", searchTerm);

            using var resultSet = container.GetItemQueryIterator<Photo>(queryDefinition);

            List<Photo> photos = new();
            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();
                var photo = response.FirstOrDefault();
                if (photo is not null)
                {
                    photos.Add(photo);
                }
            }

            return new OkObjectResult(photos);
        }
    }
}
