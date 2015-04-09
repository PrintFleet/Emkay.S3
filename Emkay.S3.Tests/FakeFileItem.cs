using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
        public FakeFileItem(string filepath, NameValueCollection metadata = null)
        {
            ItemSpec = filepath;
            FakeMetadata = new NameValueCollection();
            // set default metadata
            FakeMetadata["Identity"] = filepath;
            FakeMetadata["FullPath"] = @"Z:\fakepath\" + filepath;
            FakeMetadata["RootDir"] = @"Z:\";
            FakeMetadata["RelativeDir"] = Path.GetDirectoryName(filepath) + @"\"; // path specified in the Include attribute, up to the final backslash (\)
            FakeMetadata["Directory"] = @"fakepath\" + FakeMetadata["RelativeDir"]; // directory of the item, without the root directory.
            FakeMetadata["Filename"] = Path.GetFileNameWithoutExtension(filepath);
            FakeMetadata["Extension"] = Path.GetExtension(filepath);
            //TODO if needed: ModifiedTime, CreatedTime, AccessedTime
                            
            // passed metadata overrides 
            if (metadata != null)
            {
                foreach (var key in metadata.AllKeys)
                {
                    FakeMetadata[key] = metadata[key];
                }    
            }
            
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
