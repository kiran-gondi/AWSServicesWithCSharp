using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

var s3Client = new AmazonS3Client();

var getObjectRequest = new GetObjectRequest
{
    BucketName = "nickawscourse",
    Key = "files/movies.csv"
};

var response = await s3Client.GetObjectAsync(getObjectRequest);

using var memoryStream = new MemoryStream();
response.ResponseStream.CopyTo(memoryStream);

var text = Encoding.Default.GetString(memoryStream.ToArray());


Console.WriteLine(text);










/*
await using var inputStream = new FileStream("./movies.csv", FileMode.Open, FileAccess.Read);

var putObjectRequest = new PutObjectRequest
{
    BucketName = "nickawscourse",
    Key = "files/movies.csv",
    ContentType = "text/csv",
    InputStream = inputStream
};

await s3Client.PutObjectAsync(putObjectRequest);
*/
