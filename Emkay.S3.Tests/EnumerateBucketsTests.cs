using NUnit.Framework;
using Moq;
#pragma warning disable 618

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class EnumerateBucketsTests : S3TestsBase
    {
        [Test]
        public void should_find_all_buckets()
        {
            Mock.Get(MockS3Client).Setup(x => x.EnumerateBuckets()).Returns(new[] {"bucket1", "bucket2", "bucket3"});

            var enumerateTask = new EnumerateBuckets(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
            };

            Assert.That(enumerateTask.Execute(), Is.True);

            Assert.That(enumerateTask.Buckets, Is.EquivalentTo(new[] { "bucket1", "bucket2", "bucket3" }));
        }
    }
}