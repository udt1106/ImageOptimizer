using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.StringExtensions;
using System;
using System.Diagnostics;
using Sitecore.Validations.Compression.Jpeg;
using Sitecore.Validations.Compression.Png;

namespace Sitecore.Validations.Compression
{
    public class ImageCompression
    {
        public bool IsRestrictedExtension { get; set; }

        public void Process(GetMediaStreamPipelineArgs args)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull((object)args, "args");
            if (args.Options.Thumbnail)
                return;
            MediaStream outputStream = args.OutputStream;
            if (outputStream == null)
                return;
            if (!outputStream.AllowMemoryLoading)
            {
                Tracer.Error((object)"Could not resize image as it was larger than the maximum size allowed for memory processing. Media item: {0}", (object)outputStream.MediaItem.Path);
            }
            else
            {
                if (!args.MediaData.MimeType.StartsWith("image/", StringComparison.Ordinal))
                    return;
                string str = args.MediaData.Extension.ToLower();
                IImageOptimizer imageOptimizer = (IImageOptimizer)null;
                if (str.Equals("png"))
                    imageOptimizer = (IImageOptimizer)new PngOptimizer(outputStream.Stream);
                if (str.Equals("jpg") || str.Equals("jpeg"))
                    imageOptimizer = (IImageOptimizer)new JpegOptimizer(outputStream.Stream);
                if (imageOptimizer == null)
                    return;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                IOptimizerResult optimizerResult = imageOptimizer.Optimize();
                stopwatch.Stop();
                if (optimizerResult.Success)
                {
                    outputStream.Stream.Close();
                    Sitecore.Diagnostics.Log.Info(StringExtensions.StringExtensions.FormatWith("Dianoga: optimized {0}.{1} ({2} bytes) - saved {3} bytes / {4:p}. Optimized in {5}ms.", (object)args.OutputStream.MediaItem.MediaPath, (object)args.OutputStream.MediaItem.Extension, (object)optimizerResult.SizeAfter, (object)(optimizerResult.SizeBefore - optimizerResult.SizeAfter), (object)(float)(1.0 - (double)optimizerResult.SizeAfter / (double)optimizerResult.SizeBefore), (object)stopwatch.ElapsedMilliseconds), (object)this);
                    args.OutputStream = new MediaStream(optimizerResult.CreateResultStream(), outputStream.Extension, outputStream.MediaItem);
                }
                else
                    Sitecore.Diagnostics.Log.Error(StringExtensions.StringExtensions.FormatWith("Dianoga: unable to optimize {0} because {1}", (object)args.OutputStream.MediaItem.Name, (object)optimizerResult.ErrorMessage), (object)this);
            }
        }
    }
}