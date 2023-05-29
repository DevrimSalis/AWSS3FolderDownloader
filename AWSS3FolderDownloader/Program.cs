using Amazon;
using Amazon.S3;
using Amazon.S3.Model;


        string awsAccessKeyId = "YOUR_ACCESS_KEY";
        string awsSecretAccessKey = "YOUR_SECRET_KEY";
        string bucketName = "YOUR_BUCKET_NAME";
        string folderKey = "YOUR_FOLDER_KEY";
        string localPath = "YOUR_LOCAL_PATH";

        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName("eu-west-1"),
            ForcePathStyle = true,
            SignatureVersion = "2",
            ServiceURL = "https://s3.eu-west-1.amazonaws.com"
        };

        using var client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, config);

        var listObjectsRequest = new ListObjectsRequest
        {
            BucketName = bucketName,
            Prefix = folderKey
        };

        var listObjectsResponse = await client.ListObjectsAsync(listObjectsRequest);

        foreach (var s3Object in listObjectsResponse.S3Objects)
        {
            if (!s3Object.Key.EndsWith("/"))
            {
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = s3Object.Key
                };

                var getObjectResponse = await client.GetObjectAsync(getObjectRequest);

                var filePath = Path.Combine(localPath, s3Object.Key);
                var directoryPath = Path.GetDirectoryName(filePath);

                Directory.CreateDirectory(directoryPath);
                await using var fileStream = File.Create(filePath);
                await getObjectResponse.ResponseStream.CopyToAsync(fileStream);
            }
        }

        Console.WriteLine("Download Success");