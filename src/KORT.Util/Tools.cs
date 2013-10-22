using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;

namespace KORT.Util
{
    public class Tools
    {
        private static MD5CryptoServiceProvider m = new MD5CryptoServiceProvider();
        public static string GetMD5(byte[] data)
        {
            lock (m)
            {
                byte[] s = m.ComputeHash(data);
                return BitConverter.ToString(s).Replace("-", "");
            }
        }
        public static string GetMD5(string path)
        {
            lock (m)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] s = m.ComputeHash(fs);
                    return BitConverter.ToString(s).Replace("-", "");
                }
            }
        }

        public static string CryptoString(string value)
        {
            byte[] data = Encoding.Default.GetBytes(value);
            string base64 = Convert.ToBase64String(data);
            data = Encoding.Default.GetBytes(base64);

            lock (m)
            {
                byte[] s = m.ComputeHash(data);
                return BitConverter.ToString(s).Replace("-", "");
            }
        }

        private static ImageCodecInfo GetImageCodecInfo(string value)
        {
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象.
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals(value))
                {
                    ICI = arrayICI[x];
                    //设置JPEG编码
                    break;
                }
            }
            return ICI;
        }
        private static EncoderParameters GetEncoderParameter(long quality)
        {
            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            return encoderParams;
        }

        public static string ImageToBase64(Image myImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                myImage.Save(ms, GetImageCodecInfo("JPEG"), GetEncoderParameter(95));
                byte[] imageByteArray = ms.ToArray();

                return Convert.ToBase64String(imageByteArray);
            }
        }
        public static Image Base64ToImage(string imageString)
        {
            byte[] imageByteArray = Convert.FromBase64String(imageString);

            MemoryStream ms = new MemoryStream(imageByteArray, 0, imageByteArray.Length);
            ms.Write(imageByteArray, 0, imageByteArray.Length);

            return Image.FromStream(ms, true);
        }


        /// <summary>
        /// GZip压缩函数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GZipCompress(byte[] data)
        {
            return data;
            /*
            using (var stream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    gZipStream.Write(data, 0, data.Length);
                    gZipStream.Close();
                }
                return stream.ToArray();
            }
            */
        }

        public static void GZipDecompress(Stream source, Stream dest)
        {
            byte[] buf = new byte[1024];
            int len;
            while ((len = source.Read(buf, 0, buf.Length)) > 0)
            {
                dest.Write(buf, 0, len);
            }
            return;

            /*
            using (GZipStream zipStream = new GZipStream(source, CompressionMode.Decompress, true))
            {
                byte[] buf = new byte[1024];
                int len;
                while ((len = zipStream.Read(buf, 0, buf.Length)) > 0)
                {
                    dest.Write(buf, 0, len);
                }
            }
            */
        }

        /// <summary>
        /// 从已压缩的字节数组生成原始字节数组。
        /// </summary>
        /// <param name="bytesToDecompress">已压缩的字节数组。</param>
        /// <returns>返回原始字节数组。</returns>
        public static byte[] GZipDecompress(byte[] bytesToDecompress)
        {
            byte[] writeData = new byte[4096];
            Stream s2 = new GZipInputStream(new MemoryStream(bytesToDecompress));
            MemoryStream outStream = new MemoryStream();
            while (true)
            {
                int size = s2.Read(writeData, 0, writeData.Length);
                if (size > 0)
                {
                    outStream.Write(writeData, 0, size);
                }
                else
                {
                    break;
                }
            }
            s2.Close();
            byte[] outArr = outStream.ToArray();
            outStream.Close();
            return outArr;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationZipFilePath"></param>
        public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                sourceFilePath += System.IO.Path.DirectorySeparatorChar;

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            zipStream.SetLevel(6);  // 压缩级别 0-9
            CreateZipFiles(sourceFilePath, zipStream);

            zipStream.Finish();
            zipStream.Close();
        }

        /// <summary>
        /// 递归压缩文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
        /// <param name="staticFile"></param>
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream);
                }

                else                                            //如果是文件，开始压缩
                {
                    FileStream fileStream = File.OpenRead(file);

                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(sourceFilePath.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);

                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static void UnZip(string sourceZipFile, string destinationUnZipPath)
        {
            if (string.IsNullOrEmpty(sourceZipFile)
                || string.IsNullOrEmpty(destinationUnZipPath)
                || !File.Exists(sourceZipFile))
                return;

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(sourceZipFile)))
            {
                if (!Directory.Exists(destinationUnZipPath))
                {
                    System.Diagnostics.Debug.WriteLine("Create Dir:" + destinationUnZipPath);
                    Directory.CreateDirectory(destinationUnZipPath);
                }

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (fileName == String.Empty) continue;
                    if (theEntry.CompressedSize == 0) continue;//如果文件的壓縮後的大小為0那麼說明這個文件是空的因此不需要進行讀出寫入

                    //解压文件到指定的目录
                    string path = Path.Combine(destinationUnZipPath, theEntry.Name);
                    string dir = Path.GetDirectoryName(path);
                    if (string.IsNullOrEmpty(dir)) continue;
                    if (!Directory.Exists(dir))//建立下面的目录和子目录
                    {
                        System.Diagnostics.Debug.WriteLine("Create Dir:" + dir);
                        Directory.CreateDirectory(dir);
                    }
                    if (File.Exists(path)) File.Delete(path);
                    System.Diagnostics.Debug.WriteLine("Create File:" + path);
                    FileStream streamWriter = File.Create(path);
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        int size = s.Read(data, 0, data.Length);
                        if (size > 0)
                            streamWriter.Write(data, 0, size);
                        else break;
                    }
                    streamWriter.Close();
                }
                s.Close();
            }
        }
    }
}
