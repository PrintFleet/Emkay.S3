using System.Collections.Specialized;
using NUnit.Framework;

namespace Emkay.S3.Tests
{
    [TestFixture]
    public class TaskItemExtensionTests
    {
        [Test]
        public void should_return_basic_metadata()
        {
            var item = new FakeFileItem(@"some\path\to\file.txt", new NameValueCollection()
            {
                {"RecursiveDir", @"path\to\"}, // simulates MSBuild Include="some\**\*.*" 
            });

            Assert.That(item.GetFullFilename(), Is.EqualTo("file.txt"));
            Assert.That(item.GetFullPath(), Is.EqualTo(@"Z:\fakepath\some\path\to\file.txt"));
            Assert.That(item.GetRecursiveDir(), Is.EqualTo(@"path\to\"));
        }

        [Test]
        public void should_return_s3_key_with_recursive()
        {
            var item = new FakeFileItem(@"some\path\to\file.txt", new NameValueCollection()
            {
                {"FullPath", @"c:\directory\some\path\to\file.txt"},
                {"RecursiveDir", @"path\to\"},
                {"Filename", "file"},
                {"Extension", ".txt"},
            });

            Assert.That(item.GetS3Key(@"base\folder"), Is.EqualTo(@"base/folder/path/to/file.txt"));
            Assert.That(item.GetS3Key(@"base\folder\"), Is.EqualTo(@"base/folder/path/to/file.txt"));
            Assert.That(item.GetS3Key(@"base/folder"), Is.EqualTo(@"base/folder/path/to/file.txt"));
            Assert.That(item.GetS3Key(@"base/folder/"), Is.EqualTo(@"base/folder/path/to/file.txt"));
            Assert.That(item.GetS3Key(@"base\folder", flattenFolders: true), Is.EqualTo(@"base/folder/file.txt"));
            Assert.That(item.GetS3Key(@"base/folder", flattenFolders: true), Is.EqualTo(@"base/folder/file.txt"));

            Assert.That(item.GetS3Key(string.Empty), Is.EqualTo(@"path/to/file.txt"));
            Assert.That(item.GetS3Key(null), Is.EqualTo(@"path/to/file.txt"));
        }


        [Test]
        public void should_return_s3_key_without_recursive()
        {
            var item = new FakeFileItem(@"some\path\to\file.txt", new NameValueCollection()
            {
                {"FullPath", @"c:\directory\some\path\to\file.txt"},
                {"Filename", "file"},
                {"Extension", ".txt"},
            });

            Assert.That(item.GetS3Key(@"base\folder"), Is.EqualTo(@"base/folder/some/path/to/file.txt"));
            Assert.That(item.GetS3Key(@"base\folder", flattenFolders: true), Is.EqualTo(@"base/folder/file.txt"));
        }
    }
}
