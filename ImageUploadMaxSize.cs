using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.Upload;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Sitecore.Validations.Upload
{
    public class ImageUploadMaxSize : UploadProcessor
    {
        public List<string> RestrictedExtensions { get; set; }

        public static long MaxImageSizeInDatabase
        {
            get
            {
                return Settings.GetLongSetting("Media.MaxImageSizeInDatabase", 524288000L);
            }
        }

        public ImageUploadMaxSize()
        {
            this.RestrictedExtensions = new List<string>();
        }

        public void Process(UploadArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            if (args.Destination == UploadDestination.File)
                return;
            foreach (string index in (NameObjectCollectionBase)args.Files)
            {
                HttpPostedFile httpPostedFile = args.Files[index];
                if (!string.IsNullOrEmpty(httpPostedFile.FileName) && this.IsRestrictedExtension(httpPostedFile.FileName) && (long)httpPostedFile.ContentLength > ImageUploadMaxSize.MaxImageSizeInDatabase)
                {
                    string str = (string)(object)(httpPostedFile.ContentLength / 1024) + (object)"KB";
                    args.ErrorText = string.Format("The image \"{0}\" is too big to be uploaded. The maximum size for uploading images is {1}.", (object)(httpPostedFile.FileName + " (" + str + ")"), (object)MainUtil.FormatSize(ImageUploadMaxSize.MaxImageSizeInDatabase));
                    Log.Warn(args.ErrorText, (object)this);
                    args.AbortPipeline();
                    break;
                }
            }
        }

        private bool IsRestrictedExtension(string filename)
        {
            return this.RestrictedExtensions.Exists((Predicate<string>)(restrictedExtension => string.Equals(restrictedExtension, Path.GetExtension(filename), StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}
