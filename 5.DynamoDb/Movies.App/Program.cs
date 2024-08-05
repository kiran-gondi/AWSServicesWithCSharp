using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

//await new DataSeeder().ImportDataAsync();

var dynamoDb = new AmazonDynamoDBClient();

var queryRequest = new QueryRequest
{
    TableName = "movies",
    IndexName = "year-rotten-index",
    KeyConditionExpression = "ReleaseYear = :v_Year and RottenTomatoesPercentage >= :v_Rotten",
    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
    {
        { ":v_Year", new AttributeValue { N = "2018" } },
        { ":v_Rotten", new AttributeValue { N = "88" } }
    }
};

var response = await dynamoDb.QueryAsync(queryRequest);

foreach (var itemAttributes in response.Items)
{
    var document = Document.FromAttributeMap(itemAttributes);
    var json = document.ToJsonPretty();
    Console.Write(json);
}
