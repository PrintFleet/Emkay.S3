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
                              string.Format("Publishing {0} files", SourceFiles.Length));

            Logger.LogMessage(MessageImportance.Normal,
                              string.Format("to S3 bucket {0}", Bucket));

            if (!string.IsNullOrEmpty(DestinationFolder))
                Logger.LogMessage(MessageImportance.Normal,
                                  string.Format("destination folder {0}", DestinationFolder));

            try
            {
                Client.EnsureBucketExists(Bucket);

                foreach (var fileItem in SourceFiles.Where(taskItem => taskItem != null
                && !string.IsNullOrEmpty(taskItem.GetMetadata("Identity"))))
                {
                    var info = new FileInfo(fileItem.GetMetadata("Identity"));
                    var headers = MsBuildHelpers.GetCustomItemMetadata(fileItem);

                    Logger.LogMessage(MessageImportance.Normal, string.Format("Copying file {0}", info.FullName));
                    Client.PutFile(Bucket, CreateRelativePath(DestinationFolder, info.Name), info.FullName, headers, PublicRead, TimeoutMilliseconds);
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