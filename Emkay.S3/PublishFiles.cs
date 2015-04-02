using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;

namespace Emkay.S3
{
    public class PublishFiles : S3Base
    {
        public PublishFiles() : base()
        { }

        [Obsolete("Only for test purpose!")]
        internal PublishFiles(IS3ClientFactory s3ClientFactory, ITaskLogger logger = null)
            : base(s3ClientFactory, logger)
        { }

        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        [Required]
        public string DestinationFolder { get; set; }

        public bool PublicRead { get; set; }
        
        public override bool Execute()
        {
            Logger.LogMessage(MessageImportance.Normal,
                              string.Format("Publishing {0} files to bucket {1} in region {2}", SourceFiles.Length, Bucket, Region));

            if (!string.IsNullOrEmpty(DestinationFolder))
                Logger.LogMessage(MessageImportance.Normal,
                                  string.Format("Destination folder {0}", DestinationFolder));

            try
            {
                Client.EnsureBucketExists(Bucket);

                foreach (var fileItem in SourceFiles.Where(taskItem => taskItem != null
                && !string.IsNullOrEmpty(taskItem.GetMetadata("Identity"))))
                {
                    var info = new FileInfo(fileItem.GetMetadata("Identity"));
                    var headers = MsBuildHelpers.GetCustomItemMetadata(fileItem);

                    var destinationFilename = CreateRelativePath(DestinationFolder, info.Name);
                    Logger.LogMessage(MessageImportance.Normal, string.Format("Copying file {0} to {1}", info.FullName, destinationFilename));
                    Client.PutFile(Bucket, destinationFilename, info.FullName, headers, PublicRead, TimeoutMilliseconds);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogMessage(MessageImportance.High,
                                  string.Format("Publishing folder has failed because of {0}", ex.Message));
                return false;
            }
        }


        protected static string CreateRelativePath(string folder, string name)
        {
            var destinationFolder = folder ?? String.Empty;

            // Append a folder seperator if a folder has been specified without one.
            if (!string.IsNullOrEmpty(destinationFolder) && !destinationFolder.EndsWith("/"))
            {
                destinationFolder += "/";
            }

            return destinationFolder + name;
        }
    }
}