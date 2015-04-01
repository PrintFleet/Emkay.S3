using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;

namespace Emkay.S3.Tests
{
    /// <summary>
    /// Fake instance of an ITaskItem
    /// </summary>
    internal class FakeFileItem : ITaskItem 
    {
        public FakeFileItem(string filename, NameValueCollection metadata = null)
        {
            ItemSpec = filename;
            FakeMetadata = metadata ?? new NameValueCollection();
            FakeMetadata["Identity"] = filename;
        }

        public NameValueCollection FakeMetadata { get; set; }

        public string GetMetadata(string metadataName)
        {
            return FakeMetadata[metadataName];
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            FakeMetadata[metadataName] = metadataValue;
        }

        public void RemoveMetadata(string metadataName)
        {
            FakeMetadata.Remove(metadataName);
        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            throw new System.NotImplementedException();
        }

        public IDictionary CloneCustomMetadata()
        {
            return FakeMetadata.Cast<string>().ToDictionary(key => key, key => FakeMetadata[key]);
        }

        public string ItemSpec { get; set; }

        public ICollection MetadataNames
        {
            get { return FakeMetadata.Keys; }
        }

        public int MetadataCount
        {
            get { return FakeMetadata.Count; }
        }
    }
}
