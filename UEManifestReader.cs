using System;
using System.IO;
using System.Net;

namespace UEManifestReader
{
    public sealed class UEManifestReader
    {
        private readonly CustomManifestReadingSettings _readerSettings;
        private readonly Stream reader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public UEManifestReader(string file)
            : this(file, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readSettings"></param>
        public UEManifestReader(string file, CustomManifestReadingSettings readSettings)
            : this(file, readSettings, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        public UEManifestReader(string file, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading)
            : this(file, readSettings, writeOutputToFileWhileReading, JsonOutputFormat.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        /// <param name="outputFormat"></param>
        public UEManifestReader(string file, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, JsonOutputFormat outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFormat)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }

            reader = new BufferedStream(File.OpenRead(file));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public UEManifestReader(byte[] data)
            : this(data, false, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeDataToTempFile"></param>
        /// <param name="fileName"></param>
        public UEManifestReader(byte[] data, bool writeDataToTempFile, string fileName)
            : this(data, writeDataToTempFile, fileName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeDataToTempFile"></param>
        /// <param name="fileName"></param>
        /// <param name="readSettings"></param>
        public UEManifestReader(byte[] data, bool writeDataToTempFile, string fileName, CustomManifestReadingSettings readSettings)
            : this(data, writeDataToTempFile, fileName, readSettings, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeDataToTempFile"></param>
        /// <param name="fileName"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        public UEManifestReader(byte[] data, bool writeDataToTempFile, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading)
            : this(data, writeDataToTempFile, fileName, readSettings, writeOutputToFileWhileReading, JsonOutputFormat.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeDataToTempFile"></param>
        /// <param name="fileName"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        /// <param name="outputFormat"></param>
        public UEManifestReader(byte[] data, bool writeDataToTempFile, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, JsonOutputFormat outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFormat)
        {
            if (writeDataToTempFile)
            {
                File.WriteAllBytes(fileName, data);
                new UEManifestReader(fileName, readSettings, writeOutputToFileWhileReading, outputFormat);
            }
            reader = new MemoryStream(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public UEManifestReader(Stream stream)
            : this(stream, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="readSettings"></param>
        public UEManifestReader(Stream stream, CustomManifestReadingSettings readSettings)
            : this(stream, readSettings, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        public UEManifestReader(Stream stream, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading)
            : this(stream, readSettings, writeOutputToFileWhileReading, JsonOutputFormat.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        /// <param name="outputFormat"></param>
        public UEManifestReader(Stream stream, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, JsonOutputFormat outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFormat)
        {
            reader = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifestUrl"></param>
        /// <param name="manifestStorage"></param>
        /// <param name="fileName"></param>
        public UEManifestReader(Uri manifestUrl, OnlineManifestStorage manifestStorage, string fileName)
            : this(manifestUrl, manifestStorage, fileName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifestUrl"></param>
        /// <param name="manifestStorage"></param>
        /// <param name="fileName"></param>
        /// <param name="readSettings"></param>
        public UEManifestReader(Uri manifestUrl, OnlineManifestStorage manifestStorage, string fileName, CustomManifestReadingSettings readSettings)
            : this(manifestUrl, manifestStorage, fileName, readSettings, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifestUrl"></param>
        /// <param name="manifestStorage"></param>
        /// <param name="fileName"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        public UEManifestReader(Uri manifestUrl, OnlineManifestStorage manifestStorage, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading)
            : this(manifestUrl, manifestStorage, fileName, readSettings, writeOutputToFileWhileReading, JsonOutputFormat.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifestUrl"></param>
        /// <param name="manifestStorage"></param>
        /// <param name="fileName"></param>
        /// <param name="readSettings"></param>
        /// <param name="writeOutputToFileWhileReading"></param>
        /// <param name="outputFormat"></param>
        public UEManifestReader(Uri manifestUrl, OnlineManifestStorage manifestStorage, string fileName, CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, JsonOutputFormat outputFormat)
            : this(readSettings, writeOutputToFileWhileReading, outputFormat)
        {
            byte[] data = new WebClient().DownloadData(manifestUrl);
            switch(manifestStorage)
            {
                case OnlineManifestStorage.Disk:
                    File.WriteAllBytes(fileName, data);
                    new UEManifestReader(fileName, readSettings, writeOutputToFileWhileReading, outputFormat);
                    break;
                case OnlineManifestStorage.Memory:
                    reader = new MemoryStream(data);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private UEManifestReader(CustomManifestReadingSettings readSettings, bool writeOutputToFileWhileReading, JsonOutputFormat outputFormat)
        {
            _readerSettings = readSettings;

        }
    }
}