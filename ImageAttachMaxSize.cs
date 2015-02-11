using Sitecore;
using Sitecore.Configuration;
using Sitecore.Exceptions;
using Sitecore.Pipelines.Attach;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sitecore.Validations.Attach
{
    public class ImageAttachMaxSize
    {
        public List<string> RestrictedExtensions { get; set; }

        public static long MaxImageSizeInDatabase
        {
            get
            {
                return Settings.GetLongSetting("Media.MaxImageSizeInDatabase", 524288000L);
            }
        }

        public ImageAttachMaxSize()
        {
            this.RestrictedExtensions = new List<string>();
        }

        public void Process(AttachArgs args)
        {
            if (!args.MediaItem.FileBased && (!this.IsRestrictedExtension(args.File.FileName) || args.File.InputStream.Length > ImageAttachMaxSize.MaxImageSizeInDatabase))
                throw new ClientAlertException(string.Format("The file is too big to be attached. The maximum size of a file that can be uploaded is {0}.", (object)MainUtil.FormatSize(ImageAttachMaxSize.MaxImageSizeInDatabase)));
        }

        private bool IsRestrictedExtension(string filename)
        {
            return this.RestrictedExtensions.Exists((Predicate<string>)(restrictedExtension => string.Equals(restrictedExtension, Path.GetExtension(filename), StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}
