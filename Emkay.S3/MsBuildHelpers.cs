using System.Collections.Specialized;
using System.Linq;
using Microsoft.Build.Framework;

namespace Emkay.S3
{
    public class MsBuildHelpers
    {
        public static NameValueCollection GetCustomItemMetadata(ITaskItem taskItem) 
        {
            var nameValueCollection = new FriendlyNameValueCollection();

            foreach (string key in taskItem.CloneCustomMetadata().Keys)
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
                return "{"+string.Join("; ", this.Cast<string>().Select(key => string.Format("\"{0}\" = \"{1}\"", key, this[key])))+"}";
            }
        }


    }
}
