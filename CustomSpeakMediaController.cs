using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Controllers.Results;
using System.Web;
using System.IO;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Configuration;
using Sitecore.Resources.Media;
using Sitecore.Data.Managers;
using Sitecore.Controllers;
using Sitecore;

namespace Sitecore.Validations.Speak
{
    public class CustomSpeakMediaController : Controller
    {
        private string ParseDestinationUrl(string destinationUrl)
        {
            if (string.IsNullOrEmpty(destinationUrl))
            {
                return "/sitecore/media library/Upload/";
            }

            if (!destinationUrl.EndsWith("/"))
            {
                destinationUrl = destinationUrl + "/";
            }
            return destinationUrl;
        }

        public JsonResult Upload(string database, string destinationUrl)
        {
            List<UploadedFileItem> list = new List<UploadedFileItem>();
            SitecoreViewModelResult result = new SitecoreViewModelResult();

            foreach (string str in base.Request.Files)
            {
                HttpPostedFileBase file = base.Request.Files[str];

                if (file != null)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    fileNameWithoutExtension = ItemUtil.ProposeValidItemName(fileNameWithoutExtension, "default");
                    Database contentDatabase = Context.ContentDatabase;

                    if (!string.IsNullOrEmpty(database))
                    {
                        contentDatabase = Factory.GetDatabase(database);
                    }

                    if (contentDatabase == null)
                    {
                        contentDatabase = Context.ContentDatabase;
                    }

                    MediaCreatorOptions options = new MediaCreatorOptions
                    {
                        Database = contentDatabase,
                        FileBased = false,
                        IncludeExtensionInItemName = Settings.Media.IncludeExtensionsInItemNames,
                        KeepExisting = true,
                        Language = LanguageManager.DefaultLanguage,
                        Versioned = false,
                        Destination = this.ParseDestinationUrl(destinationUrl) + fileNameWithoutExtension
                    };

                    if (!ValidateFile(file, result))
                    {
                        return result;
                    }
                    Item innerItem = MediaManager.Creator.CreateFromStream(file.InputStream, "/upload/" + file.FileName, options);
                    MediaItem item = new MediaItem(innerItem);

                    string mediaUrl = MediaManager.GetMediaUrl(item);
                    list.Add(new UploadedFileItem(innerItem.Name, innerItem.ID.ToString(), innerItem.ID.ToShortID().ToString(), mediaUrl));
                }
            }

            ((dynamic)result.Result).uploadedFileItems = list;
            return result;
        }

        private static bool ValidateFile(HttpPostedFileBase file, SitecoreViewModelResult result)
        {

            List<ErrorItem> list = new List<ErrorItem>();
            int contentLength = file.ContentLength;
            bool flag = true;

            if (contentLength > CustomSpeakMediaController.MaxImageSizeInDatabase)
            {
                list.Add(new ErrorItem("size", contentLength.ToString(), string.Format(ClientHost.Globalization.Translate("The file \"{0} (size: {1} KB)\" exceeds the maximum size \"{2}\"."), file.FileName, (file.ContentLength / 1024f), (object)MainUtil.FormatSize(CustomSpeakMediaController.MaxImageSizeInDatabase))));
                flag = false;
            }

            if (!flag)
            {
                ((dynamic)result.Result).errorItems = list;
            }
            return flag;
        }

        public static long MaxImageSizeInDatabase
        {
            get
            {
                return Settings.GetLongSetting("Media.MaxImageSizeInDatabase", 524288000L);
            }
        }
    }
}