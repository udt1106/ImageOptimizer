using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using Sitecore.StringExtensions;
using Sitecore.Validations.Compression;

namespace Sitecore.Validations.Compression.Jpeg
{
    public class JpegOptimizer : IImageOptimizer
    {
        private readonly Stream _jpegStream;

        protected virtual string ToolPath
        {
            get
            {
                return HostingEnvironment.MapPath("~/ImageCompressionTool/libjpeg/jpegtran.exe");
            }
        }

        protected virtual int ToolTimeout
        {
            get
            {
                return 4000;
            }
        }

        public JpegOptimizer(Stream jpegStream)
        {
            this._jpegStream = jpegStream;
        }

        public IOptimizerResult Optimize()
        {
            string tempFilePath = this.GetTempFilePath();
            using (FileStream fileStream = File.OpenWrite(tempFilePath))
                this._jpegStream.CopyTo((Stream)fileStream);
            JpegOptimizerResult jpegOptimizerResult = new JpegOptimizerResult();
            jpegOptimizerResult.SizeBefore = (int)new FileInfo(tempFilePath).Length;
            Process process = Process.Start(this.ToolPath, StringExtensions.StringExtensions.FormatWith(" -optimize -copy none -progressive -outfile \"{0}\" \"{0}\"", (object)tempFilePath));
            if (process != null && process.WaitForExit(this.ToolTimeout))
            {
                if (process.ExitCode != 0)
                {
                    jpegOptimizerResult.Success = false;
                    jpegOptimizerResult.ErrorMessage = "jpegtran exited with unexpected exit code " + (object)process.ExitCode;
                    return (IOptimizerResult)jpegOptimizerResult;
                }
                jpegOptimizerResult.Success = true;
                jpegOptimizerResult.SizeAfter = (int)new FileInfo(tempFilePath).Length;
                using (FileStream fileStream = File.OpenRead(tempFilePath))
                {
                    jpegOptimizerResult.ResultStream = (Stream)new MemoryStream();
                    fileStream.CopyTo(jpegOptimizerResult.ResultStream);
                }
                return (IOptimizerResult)jpegOptimizerResult;
            }
            jpegOptimizerResult.Success = false;
            jpegOptimizerResult.ErrorMessage = StringExtensions.StringExtensions.FormatWith("jpegtran took longer than {0} to execute, which we consider a failure.", (object)this.ToolTimeout);
            return (IOptimizerResult)jpegOptimizerResult;
        }

        protected virtual string GetTempFilePath()
        {
            return Path.GetTempFileName();
        }
    }
}