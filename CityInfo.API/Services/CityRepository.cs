using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CityInfo.API.Controllers;
using CityInfo.API.Models;
using CityInfo.API.Services;
using CityInfo.API.Settings;
using Microsoft.Extensions.Options;

namespace CityInfo.API.Repositories
{

    public class CityRepository : ICityRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly ILogger<CityRepository> _logger;

        public CityRepository(IAmazonDynamoDB dynamoDB, IOptions<DatabaseSettings> databaseSettings, ILogger<CityRepository> logger)
        {
            _dynamoDb = dynamoDB;
            _databaseSettings = databaseSettings;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> CreateAsync(CityDto city)
        {
            //Need to print the output
            var citiesAsJson = JsonSerializer.Serialize(city);
            _logger.LogInformation($"STUART LOG: citiesAsJSON: {citiesAsJson}");
            var itemAsDocument = Document.FromJson(citiesAsJson);
            _logger.LogInformation($"STUART LOG: citiesAsJSON: {citiesAsJson}");
            var itemAsAttributes = itemAsDocument.ToAttributeMap();

            _logger.LogInformation($"STUART LOG: citiesAsJSON: {citiesAsJson}");

            var createItemRequest = new PutItemRequest
            {
                TableName = _databaseSettings.Value.TableName,
                Item = itemAsAttributes
            };

            var response = await _dynamoDb.PutItemAsync(createItemRequest);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }


        public async Task<CityDto?> GetAsync(string id)
        {
            _logger.LogInformation("STUART LOG: We in here mehn");
            var request = new GetItemRequest
            {
                TableName = _databaseSettings.Value.TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "pk", new AttributeValue { S = id.ToString() } },
                    { "sk", new AttributeValue {S = id.ToString() } }
                }
            };

            _logger.LogInformation("STUART LOG: About to send request");
            var response = await _dynamoDb.GetItemAsync(request);
            if(response.Item.Count == 0)
            {
                return null;
            }

            _logger.LogInformation("STUART LOG: Below response");

            var itemAsDocument = Document.FromAttributeMap(response.Item);
            _logger.LogInformation("STUART LOG: Ready to return");
            return JsonSerializer.Deserialize<CityDto?>(itemAsDocument.ToJson()); //Go away and understand the whole "itemAsDocument"
        }

        //public async Task<IEnumerable<CityDto>> GetAllAsync()
        //{

        //}

        public async Task<bool> UpdateAsync(CityDto city)
        {
            //Need to print the output
            var citiesAsJson = JsonSerializer.Serialize(city);
            var itemAsDocument = Document.FromJson(citiesAsJson);
            var itemAsAttributes = itemAsDocument.ToAttributeMap();

            var updateItemRequest = new PutItemRequest
            {
                TableName = _databaseSettings.Value.TableName,
                Item = itemAsAttributes
            };

            var response = await _dynamoDb.PutItemAsync(updateItemRequest);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var deleteItemrequest = new DeleteItemRequest
            {
                TableName = _databaseSettings.Value.TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "pk", new AttributeValue { S = id.ToString() } },
                    { "sk", new AttributeValue {S = id.ToString() } }
                }
            };

            var response = await _dynamoDb.DeleteItemAsync(deleteItemrequest);
            return response.HttpStatusCode == HttpStatusCode.OK;
            
        }
    }
}
