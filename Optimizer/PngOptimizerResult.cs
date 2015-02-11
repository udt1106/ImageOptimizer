﻿using System.IO;
using Sitecore.Validations.Compression;

namespace Sitecore.Validations.Compression.Png
{
    public class PngOptimizerResult : IOptimizerResult
    {
        public bool Success { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public int SizeBefore { get; internal set; }
        public int SizeAfter { get; internal set; }
        public byte[] OptimizedBytes { get; internal set; }
        public Stream CreateResultStream()
        {
            return (Stream)new MemoryStream(this.OptimizedBytes);
        }
    }
}