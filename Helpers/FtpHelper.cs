using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// FTP上下文类
    /// </summary>
    public class FtpContext
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 远程目录
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// FTP帮助类
    /// </summary>
    public class FtpHelper
    {
        /// <summary>
        /// 连接上下文
        /// </summary>
        public FtpContext Context { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ctx"></param>
        public FtpHelper(FtpContext ctx)
        {
            Context = ctx;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host">FTP主机</param>
        /// <param name="folder">远程路径</param>
        /// <param name="uid">登录账号</param>
        /// <param name="pwd">登录密码</param>
        /// <param name="port">FTP端口(默认为21)</param>
        public FtpHelper(string host, string folder, string uid, string pwd, int port = 21)
        {
            Context = new FtpContext
            {
                Host = host,
                Port = port,
                Folder = folder,
                UserName = uid,
                Password = pwd
            };
        }

        #region 上传
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据对象</param>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns></returns>
        public string UploadData<T>(T data, string remoteFile, Action<int, int> updateProgress = null)
            => UploadString(data.ToJson(), remoteFile, updateProgress);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileContent">文件内容</param>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns></returns>
        public string UploadString(string fileContent, string remoteFile, Action<int, int> updateProgress = null)
        {
            MemoryStream memoryStream = null;

            try
            {
                if (!fileContent.IsValid())
                {
                    return "文件内容不能为空";
                }

                if (!remoteFile.IsValid())
                {
                    return "远程文件不能为空";
                }

                var data = fileContent.GetBytes();
                memoryStream = new MemoryStream(data);

                return UploadFile(memoryStream, fileContent.Length, remoteFile, updateProgress);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                memoryStream?.Close();
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="remoteFolder">远程目录</param>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns>错误信息</returns>
        public string UploadFile(string localFile, string remoteFolder = null, string remoteFileName = null, Action<int, int> updateProgress = null)
        {
            FileStream fileStream = null;

            try
            {
                if (!localFile.IsValid())
                {
                    return "本地文件不能为空";
                }

                var info = new FileInfo(localFile);
                if (null == info || !info.Exists)
                {
                    return "指定的文件不存在";
                }

                var remoteFile = Path.Combine(remoteFolder.GetValue(), remoteFileName.GetValue(info.Name));

                updateProgress?.Invoke((int)info.Length, 0);
                fileStream = info.OpenRead();

                return UploadFile(fileStream, info.Length, remoteFile, updateProgress);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                fileStream?.Close();
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="inputStream">输入流(由创建者释放)</param>
        /// <param name="inputSize">输入大小</param>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns></returns>
        public string UploadFile(Stream inputStream, long inputSize, string remoteFile, Action<int, int> updateProgress = null)
        {
            try
            {
                if (!remoteFile.IsValid())
                {
                    return "远程文件不能为空";
                }

                if (null == inputStream || !inputStream.CanRead)
                {
                    return "输入流对象为空或不可读";
                }

                updateProgress?.Invoke((int)inputSize, 0);

                int bufferSize = 4096; //缓存大小
                int readSize = 0; //本次下载大小
                long totalSize = 0; //已下载总大小
                var buffer = new byte[bufferSize];

                var error = DoRequest(WebRequestMethods.Ftp.UploadFile, remoteFile, KeepAlive: true,
                     preRequest: (request) => request.ContentLength = inputSize,
                     handleRequest: (stream) =>
                     {
                         while ((readSize = inputStream.Read(buffer, 0, bufferSize)) > 0)
                         {
                             stream.Write(buffer, 0, readSize);

                             totalSize += readSize;
                             updateProgress?.Invoke((int)inputSize, (int)totalSize);
                         }
                     });

                return error;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 下载
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="data">下载的内容</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns></returns>
        public string DownloadData<T>(string remoteFile, out T data, Action<int, int> updateProgress = null)
        {
            data = default(T);

            try
            {
                var error = DownloadString(remoteFile, out string json, updateProgress);
                if (error.IsValid())
                {
                    return error;
                }

                data = json.FromJson<T>();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="fileContent">下载的内容</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns></returns>
        public string DownloadString(string remoteFile, out string fileContent, Action<int, int> updateProgress = null)
        {
            fileContent = null;

            try
            {
                if (!remoteFile.IsValid())
                {
                    return "远程文件不能为空";
                }

                var tempFile = IoExtension.GetTempFile(Path.GetFileName(remoteFile));
                var error = DownloadFile(remoteFile, tempFile, updateProgress);
                if (error.IsValid())
                {
                    return error;
                }

                fileContent = tempFile.ReadFile();
                File.Delete(tempFile);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="localFile">本地文件</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns>错误信息</returns>
        public string DownloadFile(string remoteFile, string localFile, Action<int, int> updateProgress = null)
        {
            FileStream fileStream = null;

            try
            {
                if (!remoteFile.IsValid())
                {
                    return "远程文件不能为空";
                }

                if (!localFile.IsValid())
                {
                    return "本地文件不能为空";
                }

                localFile.EnsureDirectory(isFile: true);
                fileStream = new FileStream(localFile, FileMode.Create);

                return DownloadFile(remoteFile, fileStream, updateProgress);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                fileStream?.Close();
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="outputStream">输出流(由创建者释放)</param>
        /// <param name="updateProgress">更新进度方法(总大小/当前进度)</param>
        /// <returns>错误信息</returns>
        public string DownloadFile(string remoteFile, Stream outputStream, Action<int, int> updateProgress = null)
        {
            try
            {
                if (!remoteFile.IsValid())
                {
                    return "远程文件不能为空";
                }

                if (null == outputStream || !outputStream.CanWrite)
                {
                    return "输出流对象为空或不可写";
                }

                var fileSize = GetFileSize(remoteFile, out string error);
                if (error.IsValid() || fileSize <= 0)
                {
                    return error.GetValue("获取文件大小失败");
                }

                updateProgress?.Invoke((int)fileSize, 0);

                int bufferSize = 4096; //缓存大小
                int readSize = 0; //本次下载大小
                long totalSize = 0; //已下载总大小
                var buffer = new byte[bufferSize];

                error = DoRequest(WebRequestMethods.Ftp.DownloadFile, remoteFile, KeepAlive: false,
                    handleResponse: (response, stream, reader) =>
                    {
                        while ((readSize = stream.Read(buffer, 0, bufferSize)) > 0)
                        {
                            outputStream.Write(buffer, 0, readSize);

                            totalSize += readSize;
                            updateProgress?.Invoke((int)fileSize, (int)totalSize);
                        }
                    });

                return error;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public string DeleteFile(string remoteFile, out string error)
        {
            string result = null;

            error = DoRequest(WebRequestMethods.Ftp.DeleteFile, remoteFile,
                handleResponse: (response, stream, reader) => result = reader.ReadToEnd());

            return result;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folderName">文件夹名称</param> 
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public string CreateDirectory(string folderName, out string error)
        {
            string result = null;

            error = DoRequest(WebRequestMethods.Ftp.MakeDirectory, folderName,
                handleResponse: (response, stream, reader) => result = reader.ReadToEnd());

            return result;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="error">错误信息</param>
        /// <param name="errIfNotEmpty">目录非空时返回错误</param>
        /// <returns></returns>
        public string DeleteDirectory(string folderName, out string error, bool errIfNotEmpty = true)
        {
            var paths = GetFilesAndDirectories(out error, folderName);
            if (error.IsValid())
            {
                return string.Empty;
            }

            if (errIfNotEmpty)
            {
                if (!paths.IsEmpty())
                {
                    return "非空目录不可删除";
                }
            }
            else
            {

            }

            string result = null;

            error = DoRequest(WebRequestMethods.Ftp.RemoveDirectory, folderName,
                handleResponse: (response, stream, reader) => result = reader.ReadToEnd());

            return result;
        }

        /// <summary>
        /// 判断指定的子目录是否存在
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="error">错误信息</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        public bool DirectoryExists(string folderName, out string error, string ftpFolder = null)
        {
            var dirs = GetDirectories(out error, ftpFolder);
            if (dirs.IsEmpty())
            {
                return false;
            }

            return dirs.Exists(x => x.EqualsEx(folderName, ignoreCase: true));
        }

        /// <summary>
        /// 判断指定的文件是否存在
        /// </summary>
        /// <param name="filePath">远程文件</param>
        /// <param name="error">错误信息</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        public bool FileExists(string filePath, out string error, string ftpFolder = null)
        {
            var folder = Path.GetDirectoryName(filePath);
            folder = Path.Combine(ftpFolder.GetValue(), folder);
            var fileName = Path.GetFileName(filePath);

            var files = GetFiles(out error, mask: MASK_ALL, ftpFolder: folder);
            if (files.IsEmpty())
            {
                return false;
            }

            return files.Exists(x => x.EqualsEx(fileName, ignoreCase: true));
        }

        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹)
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        /// <returns></returns>
        public List<string> GetFilesAndDirectories(out string error, string ftpFolder = null)
        {
            var list = new List<string>();

            error = DoRequest(WebRequestMethods.Ftp.ListDirectoryDetails, ftpFolder,
                handleResponse: (response, stream, reader) =>
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
            );

            return list;
        }

        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <param name="mask">文件掩码</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        /// <returns></returns>
        public List<string> GetFiles(out string error, string mask = null, string ftpFolder = null)
        {
            var list = new List<string>();
            var filter = string.Empty;

            if (mask.IsValid() && !mask.Equals(MASK_ALL))
            {
                filter = mask.Substring(0, mask.IndexOf(Chars.星号));
            }

            error = DoRequest(WebRequestMethods.Ftp.ListDirectory, ftpFolder,
                handleResponse: (response, stream, reader) =>
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!filter.IsValid() || line.Substring(0, filter.Length) == filter)
                        {
                            list.Add(line);
                        }
                    }
                }
            );

            return list;
        }

        /// <summary>
        /// 获取当前目录下所有的文件夹列表(仅文件夹)
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        /// <returns></returns>
        public List<string> GetDirectories(out string error, string ftpFolder = null)
        {
            var paths = GetFilesAndDirectories(out error, ftpFolder);
            return ParseDirectories(paths);
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="newFile">新文件名</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public string RenameFile(string remoteFile, string newFile, out string error)
        {
            string result = null;

            error = DoRequest(WebRequestMethods.Ftp.Rename, remoteFile,
                preRequest: (request) => request.RenameTo = newFile,
                handleResponse: (response, stream, reader) => result = reader.ReadToEnd());

            return result;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="toDirectory">目标文件夹</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public string MovieFile(string remoteFile, string toDirectory, out string error)
            => RenameFile(remoteFile, toDirectory, out error);

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="remoteFile">远程文件</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public long GetFileSize(string remoteFile, out string error)
        {
            long fileSize = 0;

            error = DoRequest(WebRequestMethods.Ftp.GetFileSize, remoteFile,
                handleResponse: (response, stream, reader) => fileSize = response.ContentLength);

            return fileSize;
        }

        /// <summary>
        /// 解析目录列表
        /// </summary>
        /// <param name="paths">路径列表</param>
        /// <returns></returns>
        private List<string> ParseDirectories(List<string> paths)
        {
            if (paths.IsEmpty())
            {
                return null;
            }

            var dirs = new List<string>();

            foreach (string path in paths)
            {
                int dirPos = path.IndexOf(DIR_WIN);
                if (dirPos > 0)
                {
                    /*判断 Windows 风格*/
                    var dir = path.Substring(dirPos + 5).Trim();
                    dirs.Add(dir);
                }
                else if (path.UpperCase().StartsWith(DIR_UNIX))
                {
                    /*判断 Unix 风格*/
                    //string dir = str.Substring(54).Trim();
                    var date = RgxUnixDirDate.Match(path).Value;
                    var dir = path.Substring(path.IndexOf(date) + date.Length);
                    if (dir != DIR_CURRENT && dir != DIR_PARENT)
                    {
                        dirs.Add(dir.GetValue());
                    }
                }
            }

            return dirs;
        }

        /// <summary>
        /// 发送FTP请求
        /// </summary>
        /// <param name="method">操作方法</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        /// <param name="KeepAlive">是否保持长连接(批量上传时请传入True)</param>
        /// <param name="preRequest">请求之前的处理</param>
        /// <param name="handleRequest">处理请求流的方法（无须释放请求流）</param>
        /// <param name="handleResponse">处理响应的方法（无须释放响应流）</param>
        /// <returns></returns>
        private string DoRequest(string method, string ftpFolder = null, bool KeepAlive = false,
            Action<FtpWebRequest> preRequest = null, Action<Stream> handleRequest = null,
            Action<FtpWebResponse, Stream, StreamReader> handleResponse = null)
        {
            FtpWebResponse response = null;
            Stream reqStream = null, rspStram = null;

            try
            {
                var request = CreateRequest(method, ftpFolder, KeepAlive);

                //添加额外的请求参数
                preRequest?.Invoke(request);

                response = (FtpWebResponse)request.GetResponse();

                //处理请求流，写入数据
                if (handleRequest != null)
                {
                    reqStream = request.GetRequestStream();
                    handleRequest.Invoke(reqStream);
                }

                rspStram = response.GetResponseStream();

                if (handleResponse != null)
                {
                    using (var reader = new StreamReader(rspStram, Encoding.UTF8))
                    {
                        //执行具体的操作
                        handleResponse(response, rspStram, reader);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                reqStream?.Close();
                rspStram?.Close();
                response?.Close();
            }
        }

        /// <summary>
        /// 创建FTP请求
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        /// <param name="KeepAlive">是否保持长连接(批量上传时请传入True)</param>
        /// <returns></returns>
        private FtpWebRequest CreateRequest(string method, string ftpFolder = null, bool KeepAlive = false)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(GetFtpUri(ftpFolder));

            request.Credentials = new NetworkCredential(Context.UserName, Context.Password);
            request.UseBinary = true;
            //request.UsePassive = false;
            request.KeepAlive = KeepAlive;
            request.Method = method;
            request.ReadWriteTimeout = 10000;
            request.Timeout = 10000;

            return request;
        }

        /// <summary>
        /// 获取FTP URI
        /// </summary>
        /// <param name="ftpFolder">远程路径（默认为当前路径）</param>
        /// <returns></returns>
        private Uri GetFtpUri(string ftpFolder = null)
        {
            var sbUri = new StringBuilder(FTP_PROTOCOL);
            sbUri.Append(Context.Host);

            if (Context.Port > 0 && Context.Port < 65536 && Context.Port != 21)
            {
                sbUri.Append(Context.Port.ToString(), Chars.冒号);
            }

            if (Context.Folder.IsValid())
            {
                sbUri.Append(Context.Folder.Trim(Chars.正斜线), Chars.正斜线);
            }

            if (ftpFolder.IsValid())
            {
                sbUri.Append(ftpFolder.Trim(Chars.正斜线), Chars.正斜线);
            }
            else
            {
                sbUri.Append(Chars.正斜线);
            }

            return new Uri(sbUri.ToString());
        }

        private const string FTP_PROTOCOL = "ftp://";
        private const string MASK_ALL = "*.*";
        private const string DIR_WIN = "<DIR>";
        private const string DIR_UNIX = "D";
        private const string DIR_CURRENT = ".";
        private const string DIR_PARENT = "..";

        /// <summary>
        /// Feb 25 19:27
        /// </summary>
        private readonly static Regex RgxUnixDirDate = new Regex(@"\s\w{3}\s\d{2}\s\d{2}:\d{2}\s", RegexOptions.IgnoreCase);
    }
}