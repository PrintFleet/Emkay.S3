using System.Collections.Specialized;
using Microsoft.Build.Framework;
using Moq;
using NUnit.Framework;
#pragma warning disable 618

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class PublishFilesTests  : S3TestsBase 
    {
        [Test]
        public void should_call_PutFile()
        {
            var publishTask = new PublishFiles(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
                Bucket = "my_bucket",
                SourceFiles = new ITaskItem[]
                {
                    new FakeFileItem("test1.txt"),
                    new FakeFileItem("test2.json", new NameValueCollection()
                    {
                        {"Content-Type", "application/json"}
                    }),
                },
                DestinationFolder = "my/dest",
                PublicRead = true,
                TimeoutMilliseconds = 1424
            };

            Assert.That(publishTask.Execute(), Is.True);

            // make sure S3 methods were invoked correctly
            Mock.Get(MockS3Client).Verify(x => x.EnsureBucketExists("my_bucket"));
            Mock.Get(MockS3Client).Verify(x =>
                x.PutFile(
                    "my_bucket", "my/dest/test1.txt",
                    It.IsRegex(@"test1\.txt$"),
                    It.Is<NameValueCollection>(headers => headers["Content-Type"] == null),
                    true, 1424),
                Times.Once());
            Mock.Get(MockS3Client).Verify(x =>
                x.PutFile(
                    "my_bucket", "my/dest/test2.jsonx",
                    It.IsRegex(@"test2\.json"),
                    It.Is<NameValueCollection>(headers => headers["Content-Type"] == "application/json"),
                    true, 1424),
                Times.Once());

        }

    }
}
