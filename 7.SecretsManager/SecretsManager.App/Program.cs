﻿using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretsManagerClient = new AmazonSecretsManagerClient();

var listSecretVersionsRequest = new ListSecretVersionIdsRequest()
{
    SecretId = "ApiKey",
    IncludeDeprecated = true
};

var versionResponse = await secretsManagerClient.ListSecretVersionIdsAsync(listSecretVersionsRequest);

var request = new GetSecretValueRequest()
{
    SecretId = "ApiKey",
    //VersionStage = "AWSPREVIOUS"
   // VersionStage = "AWSCURRENT"
};

var response = await  secretsManagerClient.GetSecretValueAsync(request);

Console.WriteLine(response);

/*var describeSecretRequest = new DescribeSecretRequest()
{
    SecretId = "ApiKey"
};

var describeResponse = await  secretsManagerClient.DescribeSecretAsync(describeSecretRequest);
Console.WriteLine(describeResponse);*/