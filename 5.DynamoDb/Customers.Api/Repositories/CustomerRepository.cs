using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Customers.Api.Contracts.Data;
using Customers.Api.Database;
using Customers.Api.Domain;
using Dapper;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IAmazonDynamoDB _amazonDynamoDb;
    private readonly string _tableName = "customers";

    public CustomerRepository(IAmazonDynamoDB amazonDynamoDb)
    {
        _amazonDynamoDb = amazonDynamoDb;
    }

    public async Task<bool> CreateAsync(CustomerDto customer)
    {
        customer.UpdateAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();
        
        var createItemRequest = new PutItemRequest()
        {
            TableName = _tableName,
            Item = customerAsAttribute,
            ConditionExpression = "attribute_not_exits(pk) and attribute_not_exists(sk)"
        };

        var response = await _amazonDynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<CustomerDto?> GetAsync(Guid id)
    {
        var getItemRequest = new GetItemRequest()
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>()
            {
                {"pk", new AttributeValue(){ S = id.ToString() }}, 
                {"sk", new AttributeValue(){ S = id.ToString() }}, 
            }
        };

        var response = await _amazonDynamoDb.GetItemAsync(getItemRequest);
        if (response.Item.Count == 0)
            return null;

        var itemAsDocument = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email)
    {
        var getItemRequest = new QueryRequest()
        {
            TableName = _tableName,
            IndexName = "email-id-index",
            KeyConditionExpression = "Email = :v_Email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
            {
                {
                    ":v_Email", new AttributeValue(){S = email }
                }
            }
        };

        var response = await _amazonDynamoDb.QueryAsync(getItemRequest);
        if (response.Items.Count == 0)
            return null;

        var itemAsDocument = Document.FromAttributeMap(response.Items[0]);
        return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
    }
    
    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var scanRequest = new ScanRequest()
        {
            TableName = _tableName
        };

        var response = await _amazonDynamoDb.ScanAsync(scanRequest);
        return response.Items.Select(x =>
        {
            var json = Document.FromAttributeMap(x).ToString();
            return JsonSerializer.Deserialize<CustomerDto>(json);
        });
    }

    public async Task<bool> UpdateAsync(CustomerDto customer)
    {
        customer.UpdateAt = DateTime.UtcNow;
        var customerAsJson = JsonSerializer.Serialize(customer);
        var customerAsAttribute = Document.FromJson(customerAsJson).ToAttributeMap();
        
        var updateItemRequest = new PutItemRequest()
        {
            TableName = _tableName,
            Item = customerAsAttribute,
            ConditionExpression = "UpdatedAt < :requestStarted",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
            {
                {":requestStarted", new AttributeValue(){S= customer.Id.ToString() }}
            }
        };

        var response = await _amazonDynamoDb.PutItemAsync(updateItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleteItemRequest = new DeleteItemRequest()
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"pk", new AttributeValue(){S = id.ToString()}},
                {"sk", new AttributeValue(){S = id.ToString()}}
            }
        };

        var response = await _amazonDynamoDb.DeleteItemAsync(deleteItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    
}
