using Moq;
using NUnit.Framework;
#pragma warning disable 618

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class DeleteBucketTests : S3TestsBase
    {
        [Test]
        public void should_call_DeleteBucket()
        {
            var deleteTask = new DeleteBucket(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
                Bucket = "bucket_to_delete"
            };

            Assert.That(deleteTask.Execute(), Is.True);

            // make sure S3 methods were invoked correctly
            Mock.Get(MockS3Client).Verify(x => x.DeleteBucket("bucket_to_delete"));
        }
    }
}