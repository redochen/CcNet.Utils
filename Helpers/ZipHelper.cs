using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// 压缩帮助类
    /// </summary>
    public static class ZipHelper
    {
        private const string ExtZip = ".zip";

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="folderName">要压缩的文件夹</param>
        /// <param name="zipFileName">压缩文件名称</param>
        /// <param name="compressionLevel">压缩率0（无压缩）9（压缩率最高）</param>
        /// <returns>错误信息</returns>
        public static string ZipFolder(string folderName, ref string zipFileName, int compressionLevel = 9)
        {
            if (!folderName.IsValid())
            {
                return "文件夹名称不能为空";
            }

            if (!Directory.Exists(folderName))
            {
                return "指定的文件夹不存在";
            }

            var files = folderName.GetFiles(out string error, searchSubFolderFiles: true);
            if (error.IsValid())
            {
                return error;
            }

            if (!zipFileName.IsValid())
            {
                zipFileName = $"{folderName}{ExtZip}";
            }
            else if (!zipFileName.EndsWith(ExtZip, StringComparison.OrdinalIgnoreCase))
            {
                zipFileName += ExtZip;
            }

            using (var fileStream = File.Create(zipFileName))
            using (var zipStream = new ZipOutputStream(fileStream))
            {
                zipStream.SetLevel(compressionLevel);

                var crc32 = new Crc32();

                files.ForEach(fi =>
                {
                    using (var reader = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var buffer = new byte[fi.Length];
                        reader.Read(buffer, 0, buffer.Length);

                        var entry = new ZipEntry(Path.GetFileName(fi.Name))
                        {
                            DateTime = fi.LastWriteTime,
                            Size = reader.Length
                        };

                        crc32.Reset();
                        crc32.Update(buffer);
                        entry.Crc = crc32.Value;

                        zipStream.PutNextEntry(entry);
                        zipStream.Write(buffer, 0, buffer.Length);
                    }
                });
            }

            return string.Empty;
        }

        /// <summary>  
        /// 功能：解压zip格式的文件。  
        /// </summary>  
        /// <param name="zipFilePath">压缩文件路径</param>  
        /// <param name="folderName">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
        /// <returns>错误信息</returns>  
        public static string UnZip(string zipFilePath, ref string folderName)
        {
            if (!zipFilePath.IsValid())
            {
                return "压缩文件不能为空";
            }

            if (!File.Exists(zipFilePath))
            {
                return "压缩文件不存在";
            }

            if (!folderName.IsValid())
            {
                folderName = zipFilePath.PathRemoveFileExtension(out string error);
            }

            folderName.EnsureDirectory(isFile: false);

            using (var fileStream = File.OpenRead(zipFilePath))
            using (var zipStream = new ZipInputStream(fileStream))
            {
                ZipEntry ze = null;
                while ((ze = zipStream.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(ze.Name);
                    string fileName = Path.GetFileName(ze.Name);

                    if (directoryName.IsValid())
                    {
                        Directory.CreateDirectory(Path.Combine(folderName, directoryName));
                    }

                    if (!fileName.IsValid())
                    {
                        continue;
                    }

                    using (var writer = File.Create(Path.Combine(folderName, ze.Name)))
                    {
                        int size = 0;
                        byte[] data = new byte[2048];
                        while ((size = zipStream.Read(data, 0, data.Length)) > 0)
                        {
                            writer.Write(data, 0, size);
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="filePath">被压缩的文件名称(包含文件路径)，文件的全路径</param>
        /// <param name="zipedFileName">压缩后的文件名称(包含文件路径)，保存的文件名称</param>
        /// <param name="compressionLevel">压缩率0（无压缩）到 9（压缩率最高）</param>
        public static void ZipFile(string filePath, ref string zipedFileName, int compressionLevel = 9)
        {
            // 如果文件没有找到，则报错 
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("文件：" + filePath + "没有找到！");
            }
            // 如果压缩后名字为空就默认使用源文件名称作为压缩文件名称
            if (string.IsNullOrEmpty(zipedFileName))
            {
                string oldValue = Path.GetFileName(filePath);
                if (oldValue != null)
                {
                    zipedFileName = filePath.Replace(oldValue, "") + Path.GetFileNameWithoutExtension(filePath) + ".zip";
                }
            }
            // 如果压缩后的文件名称后缀名不是zip，就是加上zip，防止是一个乱码文件
            if (Path.GetExtension(zipedFileName) != ".zip")
            {
                zipedFileName = zipedFileName + ".zip";
            }
            // 如果指定位置目录不存在，创建该目录  C:\Users\yhl\Desktop\大汉三通
            string zipedDir = zipedFileName.Substring(0, zipedFileName.LastIndexOf("\\", StringComparison.Ordinal));
            if (!Directory.Exists(zipedDir))
            {
                Directory.CreateDirectory(zipedDir);
            }
            // 被压缩文件名称
            string filename = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var streamToZip = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var zipFile = File.Create(zipedFileName);
            var zipStream = new ZipOutputStream(zipFile);
            var zipEntry = new ZipEntry(filename);
            zipStream.PutNextEntry(zipEntry);
            zipStream.SetLevel(compressionLevel);
            var buffer = new byte[2048];
            Int32 size = streamToZip.Read(buffer, 0, buffer.Length);
            zipStream.Write(buffer, 0, size);
            try
            {
                while (size < streamToZip.Length)
                {
                    int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            finally
            {
                zipStream.Finish();
                zipStream.Close();
                streamToZip.Close();
            }
        }

        /// <summary> 
        /// 压缩单个文件 
        /// </summary> 
        /// <param name="fileToZip">要进行压缩的文件名，全路径</param> 
        /// <param name="zipedFile">压缩后生成的压缩文件名,全路径</param> 
        public static void ZipFile(string fileToZip, ref string zipedFile)
        {
            // 如果文件没有找到，则报错 
            if (!File.Exists(fileToZip))
            {
                throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }
            using (FileStream fileStream = File.OpenRead(fileToZip))
            {
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();
                using (FileStream zipFile = File.Create(zipedFile))
                {
                    using (ZipOutputStream zipOutputStream = new ZipOutputStream(zipFile))
                    {
                        // string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        string fileName = Path.GetFileName(fileToZip);
                        var zipEntry = new ZipEntry(fileName)
                        {
                            DateTime = DateTime.Now,
                            IsUnicodeText = true
                        };
                        zipOutputStream.PutNextEntry(zipEntry);
                        zipOutputStream.SetLevel(5);
                        zipOutputStream.Write(buffer, 0, buffer.Length);
                        zipOutputStream.Finish();
                        zipOutputStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 压缩多个目录或文件
        /// </summary>
        /// <param name="folderOrFileList">待压缩的文件夹或者文件，全路径格式,是一个集合</param>
        /// <param name="zipedFile">压缩后的文件名，全路径格式</param>
        /// <param name="password">压宿密码</param>
        /// <returns></returns>
        public static bool ZipManyFilesOrDictorys(IEnumerable<string> folderOrFileList, ref string zipedFile, string password)
        {
            bool res = true;
            using (var s = new ZipOutputStream(File.Create(zipedFile)))
            {
                s.SetLevel(6);
                if (!string.IsNullOrEmpty(password))
                {
                    s.Password = password;
                }
                foreach (string fileOrDir in folderOrFileList)
                {
                    //是文件夹
                    if (Directory.Exists(fileOrDir))
                    {
                        res = ZipFileDictory(fileOrDir, s, "");
                    }
                    else
                    {
                        //文件
                        res = ZipFileWithStream(fileOrDir, s);
                    }
                }
                s.Finish();
                s.Close();
                return res;
            }
        }

        /// <summary>
        /// 带压缩流压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        private static bool ZipFileWithStream(string fileToZip, ZipOutputStream zipStream)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }
            //FileStream fs = null;
            FileStream zipFile = null;
            ZipEntry zipEntry = null;
            bool res = true;
            try
            {
                zipFile = File.OpenRead(fileToZip);
                byte[] buffer = new byte[zipFile.Length];
                zipFile.Read(buffer, 0, buffer.Length);
                zipFile.Close();
                zipEntry = new ZipEntry(Path.GetFileName(fileToZip));
                zipStream.PutNextEntry(zipEntry);
                zipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (zipEntry != null)
                {
                }

                if (zipFile != null)
                {
                    zipFile.Close();
                }
                GC.Collect();
                GC.Collect(1);
            }
            return res;
        }

        /// <summary>
        /// 递归压缩文件夹方法
        /// </summary>
        /// <param name="folderToZip"></param>
        /// <param name="s"></param>
        /// <param name="parentFolderName"></param>
        private static bool ZipFileDictory(string folderToZip, ZipOutputStream s, string parentFolderName)
        {
            bool res = true;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                //创建当前文件夹
                entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/")); //加上 “/” 才会当成是文件夹创建
                s.PutNextEntry(entry);
                s.Flush();
                //先压缩文件，再递归压缩文件夹
                var filenames = Directory.GetFiles(folderToZip);
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (entry != null)
                {
                }
                GC.Collect();
                GC.Collect(1);
            }
            var folders = Directory.GetDirectories(folderToZip);
            foreach (string folder in folders)
            {
                if (!ZipFileDictory(folder, s, Path.Combine(parentFolderName, Path.GetFileName(folderToZip))))
                {
                    return false;
                }
            }
            return res;
        }
    }
}