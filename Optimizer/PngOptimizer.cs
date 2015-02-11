using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Sitecore.Validations.Compression;

namespace Sitecore.Validations.Compression.Png
{
    public class PngOptimizer : IImageOptimizer
    {
        private readonly Stream _pngStream;

        public PngOptimizer(Stream pngStream)
        {
            this._pngStream = pngStream;
        }

        public IOptimizerResult Optimize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this._pngStream.CopyTo((Stream)memoryStream);
                byte[] image = memoryStream.ToArray();
                byte[] result = new byte[image.Length + 400000];
                int resultSize;
                bool flag = PngOptimizer.OptimizeBytes(image, image.Length, result, result.Length, out resultSize);
                PngOptimizerResult pngOptimizerResult = new PngOptimizerResult();
                pngOptimizerResult.Success = flag;
                pngOptimizerResult.SizeBefore = image.Length;
                pngOptimizerResult.SizeAfter = resultSize;
                pngOptimizerResult.OptimizedBytes = Enumerable.ToArray<byte>(Enumerable.Take<byte>((IEnumerable<byte>)result, resultSize));
                if (!pngOptimizerResult.Success)
                    pngOptimizerResult.ErrorMessage = PngOptimizer.GetLastErrorString();
                return (IOptimizerResult)pngOptimizerResult;
            }
        }

        [DllImport("../ImageCompressionTool/PNGOptimizer/PngOptimizerDll.dll", EntryPoint = "PO_OptimizeFileMem")]
        private static extern bool OptimizeBytes(byte[] image, int imageSize, [Out] byte[] result, int resultCapacity, out int resultSize);

        [DllImport("../ImageCompressionTool/PNGOptimizer/PngOptimizerDll.dll", EntryPoint = "PO_GetLastErrorString")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string GetLastErrorString();
    }
}