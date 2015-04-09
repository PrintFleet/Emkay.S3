using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;

namespace Emkay.S3
{
    public static class TaskItemExtensions
    {
        /// <summary>
        /// Creates the "key" of the object to be uploaded (directory path and target filename)
        /// </summary>
        /// <param name="item">The ITaskItem (MSBuild ItemGroup entry)</param>
        /// <param name="destinationBaseFolder">The root base folder all files should be stored underneath</param>
        /// <param name="flattenFolders">If all files should go directly into destinationBaseFolder, 
        /// regardless of their original path</param>
        /// <returns></returns>
        public static string GetS3Key(this ITaskItem item, string destinationBaseFolder, bool flattenFolders = false)
        {
            // convert to windows-style path (temporarily) so we can use Path.Combine()
            var destinationFolder = (destinationBaseFolder ?? String.Empty).Replace("/",@"\");
            
            // subfolder is either the recursive dir (if wildcard pattern was used), the original path, or nothing
            var destinationSubFolder = flattenFolders ? string.Empty :
                item.GetRecursiveDir() ?? item.GetOriginalSubpath() ?? string.Empty;

            return Path.Combine(destinationFolder, destinationSubFolder, item.GetFullFilename())
                .Replace(@"\", "/"); // convert to URL-style path
        }

        /// <summary>
        /// Contains the full local path of the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetFullPath(this ITaskItem item)
        {
            return item == null ? string.Empty : item.GetMetadata("FullPath");
        }

        /// <summary>
        /// Path of the file starting at wildcards in the search pattern. 
        /// 
        /// For example, if a file existed at Dir1\Dir2\Dir3\File.txt, and
        /// Include="Dir1\D*\**\file.txt" was used to find it, RecursiveDir would
        /// contain "Dir2\Dir3".
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetRecursiveDir(this ITaskItem item)
        {
            return item == null ? string.Empty : item.GetMetadata("RecursiveDir");
        }

        /// <summary>
        /// Filename of the file including extension (not including directory)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetFullFilename(this ITaskItem item)
        {
            return item == null ? string.Empty : item.GetMetadata("Filename") + item.GetMetadata("Extension");
        }
        
        /// <summary>
        /// Gets the original (relative) subpath of the file (not including filename or full local path)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetOriginalSubpath(this ITaskItem item)
        {
            return item == null ? string.Empty : Path.GetDirectoryName(item.ItemSpec);
        }
        

        public static DateTime? GetModifiedTime(this ITaskItem item)
        {
            return item == null ? (DateTime?)null : DateTime.Parse(item.GetMetadata("ModifiedTime"));
        }
        public static DateTime? GetCreatedTime(this ITaskItem item)
        {
            return item == null ? (DateTime?) null : DateTime.Parse(item.GetMetadata("CreatedTime"));
        }

        

        public static NameValueCollection GetCustomItemMetadata(this ITaskItem taskItem)
        {
            var nameValueCollection = new FriendlyNameValueCollection();

            foreach (string key in taskItem.MetadataNames.Cast<string>())
            {
                nameValueCollection.Add(key, taskItem.GetMetadata(key));
            }

            return nameValueCollection;
        }


        private class FriendlyNameValueCollection : NameValueCollection
        {
            /// <summary>
            /// Custom tostring override to be able to easily dump contents 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "{" +
                       string.Join("; ",
                           this.Cast<string>().Select(key => string.Format("\"{0}\" = \"{1}\"", key, this[key]))) + "}";
            }
        }
    }
}
