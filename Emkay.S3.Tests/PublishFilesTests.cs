using System.Collections.Specialized;
using Microsoft.Build.Framework;
using Moq;
using NUnit.Framework;
#pragma warning disable 618

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class PublishFilesTests 
    {
        [Test]
        public void PublishFiles_should_call_PutFile()
        {
            var s3Client = Mock.Of<IS3Client>();
            var s3Factory = Mock.Of<IS3ClientFactory>(
                x=> x.Create("my_aws_key", "my_aws_secret") == s3Client);
            var logger = Mock.Of<ITaskLogger>();

            var publishTask = new PublishFiles(s3Factory, logger: logger)
            {
                Bucket = "my_bucket",
                Key = "my_aws_key",
                Secret = "my_aws_secret",
                SourceFiles = new ITaskItem[]
                {
                    new FakeFileItem("test1.txt"),
                    new FakeFileItem("test2.txt"),
                },
                DestinationFolder = "my/dest",
                PublicRead = true,
                TimeoutMilliseconds = 1424
            };

            Assert.That(publishTask.Execute(), Is.True);

            // make sure S3 methods were invoked correctly
            Mock.Get(s3Client).Verify(x => x.EnsureBucketExists("my_bucket"));
            Mock.Get(s3Client).Verify(x => x.PutFile("my_bucket", "my/dest/test1.txt", It.IsRegex(@"test1\.txt$"), It.IsAny<NameValueCollection>(), true, 1424), Times.Once());
            Mock.Get(s3Client).Verify(x => x.PutFile("my_bucket", "my/dest/test2.txt", It.IsRegex(@"test2\.txt$"), It.IsAny<NameValueCollection>(), true, 1424), Times.Once());
        }

    }
}
