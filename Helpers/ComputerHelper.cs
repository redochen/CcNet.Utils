using System;
using System.IO;
using System.Diagnostics;
using System.Management;
using IWshRuntimeLibrary;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    public static class ComputerHelper
    {
        /// <summary>   
        /// 获取计算机名称
        /// </summary>   
        /// <returns></returns>   
        public static string GetComputerName()
        {
            try
            {
                return Environment.GetEnvironmentVariable("ComputerName");
            }
            catch (Exception)
            {
                return "未知";
            }
        }

        /// <summary>
        /// 获取处理器ID
        /// </summary>
        /// <returns></returns>
        public static string GetProcessorId() =>
            GetManagerObject("Win32_Processor", "ProcessorId")?.ToString();

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <param name="mcAddress">网卡地址</param>
        /// <returns></returns>
        public static string GetIpAddress(out string mcAddress)
        {
            string ipAddr = null;
            string macAddr = mcAddress = null;

            GetManagerObject("Win32_NetworkAdapterConfiguration", (mo) =>
            {
                if (!mo["IPEnabled"].ToBool())
                {
                    return false;
                }

                if (!(mo["IpAddress"] is Array array) || array.Length <= 0)
                {
                    return false;
                }

                macAddr = mo["MacAddress"].ToString();

                ipAddr = array.GetValue(0)?.ToString();
                if (ipAddr.Equals("0.0.0.0") || ipAddr.EndsWith(".1"))
                {
                    return false;
                }

                return true;
            });

            mcAddress = macAddr;

            return ipAddr;
        }

        /// <summary>
        /// 获取磁盘ID
        /// </summary>
        /// <returns></returns>
        public static string GetDistkID() =>
            GetManagerObject("Win32_DiskDrive", "Model")?.ToString();

        /// <summary>
        /// 操作系统的登录用户名
        /// </summary>
        /// <returns></returns>
        public static string GetUserName() =>
            GetManagerObject("Win32_ComputerSystem", "UserName")?.ToString();

        /// <summary>
        /// 获取PC类型
        /// </summary>
        /// <returns></returns>
        public static string GetSystemType() =>
            GetManagerObject("Win32_ComputerSystem", "SystemType")?.ToString();

        /// <summary>
        /// 获取物理内存
        /// </summary>
        /// <returns></returns>
        public static string GetTotalPhysicalMemory() =>
            GetManagerObject("Win32_ComputerSystem", "TotalPhysicalMemory")?.ToString();

        /// <summary>
        /// 获取管理对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        private static object GetManagerObject(string path, string propertyName)
        {
            object obj = null;
            GetManagerObject(path, (mo) => (obj = mo[propertyName]) != null);
            return obj;
        }

        /// <summary>
        /// 获取管理对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="getPropertyFunc">获取属性的方法。返回值：True=break；False=continue</param>
        private static void GetManagerObject(string path, Func<ManagementObject, bool> getPropertyFunc)
        {
            try
            {
                using (var mc = new ManagementClass(path))
                using (var collection = mc.GetInstances())
                {
                    collection.ForEach(getPropertyFunc);
                }
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标EXE路径</param>
        /// <param name="arguments">运行参数</param>
        /// <param name="workingDirectory">起始目录</param>
        /// <param name="shortcutPath">快捷方式路径</param>
        /// <param name="description">备注</param>
        /// <returns></returns>
        public static string CreateShortcut(string shortcutName, string targetPath, string arguments = null,
            string workingDirectory = null, string shortcutPath = null, string description = null)
        {
            try
            {
                if (!targetPath.IsValid())
                {
                    targetPath = Process.GetCurrentProcess().MainModule.FileName;
                }

                if (!shortcutName.IsValid())
                {
                    shortcutName = Path.GetFileName(targetPath);
                }

                var shortcutFile = Path.Combine(shortcutPath, $"{shortcutName}.lnk");
                if (System.IO.File.Exists(shortcutFile))
                {
                    return "快捷方式已存在";
                }

                var shell = new WshShell();
                var shortcut = (WshShortcut)shell.CreateShortcut(shortcutFile);

                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments.GetValue();
                shortcut.Description = description.GetValue();
                shortcut.WorkingDirectory = workingDirectory.GetValue(Environment.CurrentDirectory);
                shortcut.IconLocation = targetPath;
                //shortcut.Hotkey = "CTRL+SHIFT+";
                shortcut.WindowStyle = 1;

                shortcut.Save();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}