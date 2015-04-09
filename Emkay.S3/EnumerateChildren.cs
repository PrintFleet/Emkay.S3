﻿using System;
using Microsoft.Build.Framework;

namespace Emkay.S3
{
    public class EnumerateChildren : S3Base
    {
        public EnumerateChildren() : base()
        { }

        [Obsolete("Only for test purpose!")]
        internal EnumerateChildren(IS3ClientFactory s3ClientFactory, ITaskLogger logger = null)
            : base(s3ClientFactory, logger)
        { }


        [Required]
        public string Bucket { get; set; }

        [Output]
        public string[] Children { get; private set; }

        public string Prefix { get; set; }

        public override bool Execute()
        {
            Logger.LogMessage(MessageImportance.Normal,
                              string.Format("Enumerating bucket {0}", Bucket));

            try
            {
                Enumerate();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogMessage(MessageImportance.High,
                                  string.Format("Enumerating bucket has failed because of {0}", ex.Message));
                return false;
            }
        }

        private void Enumerate()
        {
            Children = Client.EnumerateChildren(Bucket, Prefix);
        }
    }
}