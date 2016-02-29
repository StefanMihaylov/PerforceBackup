﻿namespace PerforceBackup.Engine.Arhivators
{
    using System;
    using System.IO;
    using System.Reflection;
    using PerforceBackup.Engine.Interfaces;
    using SevenZip;

    public class SevenZipSharp : IArhivator
    {
        private SevenZipCompressor compressor;

        public const string DefaultArhiveType = "7z";

        public SevenZipSharp()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string processorBits = Environment.Is64BitProcess ? "x64" : "x86";
            var path = Path.Combine(currentDirectory, "7z.dll");
            SevenZipBase.SetLibraryPath(path);

            this.compressor = new SevenZipCompressor();
            this.compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
            this.compressor.CompressionMethod = CompressionMethod.Lzma;
            this.compressor.CompressionLevel = CompressionLevel.Normal;
        }

        public string Arhive(string sourceName, string arhivePath, string arhiveName)
        {
            return this.Arhive(sourceName, arhivePath, arhiveName, DefaultArhiveType);
        }

        public string Arhive(string sourceName, string arhivePath, string arhiveName, string arhiveType)
        {
            var arhiveFullPath = Path.Combine(arhivePath, string.Format("{0}.{1}", arhiveName, arhiveType));
            return Arhive(sourceName, arhiveFullPath);
        }

        public string Arhive(string sourceName, string arhiveFullPath)
        {
            if (File.Exists(arhiveFullPath))
            {
                this.compressor.CompressionMode = CompressionMode.Append;
            }
            else
            {
                this.compressor.CompressionMode = CompressionMode.Create;
            }  

            FileAttributes attr = File.GetAttributes(sourceName);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                this.compressor.CompressDirectory(sourceName, arhiveFullPath, true);
            }
            else
            {
                this.compressor.CompressFiles(arhiveFullPath, sourceName);
            }

            return arhiveFullPath;
        }
    }
}