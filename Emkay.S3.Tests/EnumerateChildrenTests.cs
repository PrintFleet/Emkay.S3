using NUnit.Framework;
using Moq;
#pragma warning disable 618

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class EnumerateChildrenTests : S3TestsBase
    {
        [Test]
        public void should_find_all_children()
        {
            Mock.Get(MockS3Client).Setup(x => x.EnumerateChildren("my_bucket",null))
                .Returns(new[] { "test1.txt", "dir2/test2.txt", "dir2/test3.txt" });

            var enumerateTask = new EnumerateChildren(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
                Bucket = "my_bucket",
            };

            Assert.That(enumerateTask.Execute(), Is.True);

            Assert.That(enumerateTask.Children, Is.EquivalentTo(new[] { "test1.txt", "dir2/test2.txt", "dir2/test3.txt" }));
        }

        [Test]
        public void should_find_all_children_with_empty_prefix()
        {
            Mock.Get(MockS3Client).Setup(x => x.EnumerateChildren("my_bucket", ""))
                .Returns(new[] { "test1.txt", "dir2/test2.txt", "dir2/test3.txt" });

            var enumerateTask = new EnumerateChildren(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
                Bucket = "my_bucket",
                Prefix = string.Empty
            };

            Assert.That(enumerateTask.Execute(), Is.True);

            Assert.That(enumerateTask.Children, Is.EquivalentTo(new[] { "test1.txt", "dir2/test2.txt", "dir2/test3.txt" }));
        }

        [Test]
        public void should_find_children_in_subdir()
        {
            Mock.Get(MockS3Client).Setup(x => x.EnumerateChildren("my_bucket", "dir2/"))
                .Returns(new[] { "dir2/test2.txt", "dir2/test3.txt" });

            var enumerateTask = new EnumerateChildren(MockS3ClientFactory, MockLogger)
            {
                Key = FakeAwsKey,
                Secret = FakeAwsSecret,
                Bucket = "my_bucket",
                Prefix = "dir2/"
            };

            Assert.That(enumerateTask.Execute(), Is.True);

            Assert.That(enumerateTask.Children, Is.EquivalentTo(new[] { "dir2/test2.txt", "dir2/test3.txt" }));
        }

    }
}
