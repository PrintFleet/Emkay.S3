using Moq;
using NUnit.Framework;

namespace Emkay.S3.Tests
{
    public abstract class S3TestsBase
    {
        public const string FakeAwsKey = "my_aws_key";
        public const string FakeAwsSecret = "my_aws_secret";
        public const string FakeAwsRegion = "us-east-1";

        public IS3Client MockS3Client { get; set; }
        public IS3ClientFactory MockS3ClientFactory { get; set; }
        public ITaskLogger MockLogger { get; set; }

        [SetUp]
        public void CreateMocks()
        {
            MockS3Client = Mock.Of<IS3Client>();
            MockS3ClientFactory = Mock.Of<IS3ClientFactory>(
                x => x.Create(FakeAwsKey, FakeAwsSecret, FakeAwsRegion) == MockS3Client);
            MockLogger = Mock.Of<ITaskLogger>();
        }
    }
}