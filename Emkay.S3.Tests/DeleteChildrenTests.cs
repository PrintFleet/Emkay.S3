using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Moq;
using NUnit.Framework;
#pragma warning disable 618

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class DeleteChildrenTests : S3TestsBase
    {
        [Test]
        public void should_call_DeleteBucket()
        {
            var deleteTask = new DeleteChildren(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
                Bucket = "my_bucket",
                Children = new[]
                {
                    "child1",
                    "child2/in/subdir/",
                },
            };

            Assert.That(deleteTask.Execute(), Is.True);

            // make sure S3 methods were invoked correctly
            Mock.Get(MockS3Client).Verify(x => x.DeleteObject("my_bucket", "child1"));
            Mock.Get(MockS3Client).Verify(x => x.DeleteObject("my_bucket", "child2/in/subdir/"));
        }
    }
}
