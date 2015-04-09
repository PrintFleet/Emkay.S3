using System;
using Amazon.EC2.Model;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Emkay.S3
{
    public abstract class S3Base : Task
    {
        private readonly Lazy<ITaskLogger> _logger;
        private readonly Lazy<IS3Client> _client;

        public const int DefaultRequestTimeout = 300000; // 5 min default timeout
        public const string DefaultRegion = "us-east-1";

        protected S3Base(IS3ClientFactory s3ClientFactory = null,
                         ITaskLogger logger = null)
        {
            TimeoutMilliseconds = DefaultRequestTimeout;
            Region = DefaultRegion;
            _client = new Lazy<IS3Client>(() =>
            {
                return (s3ClientFactory ?? new S3ClientFactory()).Create(Key, Secret, Region);
            });
            _logger = new Lazy<ITaskLogger>(() => logger ?? new MsBuildTaskLogger(base.Log));
        }

        public ITaskLogger Logger
        {
            get { return _logger.Value; }
        }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Secret { get; set; }

        public string Region { get; set; }
        
        public int TimeoutMilliseconds { get; set; }

        public IS3Client Client
        {
            get { return _client.Value; }
        }

    }
}