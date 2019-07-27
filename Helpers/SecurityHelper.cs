using System;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Forms;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    /// 安全帮助类
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// 以管理员身份运行
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>NULL:什么也不做; TRUE:可继续运行; FALSE:应退出程序</returns>
        public static bool? RunAsAdmin(string args)
        {
            //获得当前登录的Windows用户标示
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            //判断当前登录用户是否为管理员
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                return true;
            }

            var appName = Path.GetFileName(Application.ExecutablePath);
            var error = RunProgress(appName, Environment.CurrentDirectory, args);

            if (error.IsValid())
            {
                return null;
            }

            return false;
        }

        /// <summary>
        /// 以管理员身份运行程序
        /// </summary>
        /// <param name="appName">程序名称</param>
        /// <param name="appDirectory">程序目录</param>
        /// <param name="args">参数</param>
        /// <returns>错误信息</returns>
        public static string RunProgress(string appName, string appDirectory, string args = null)
        {
            var si = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = appDirectory,
                FileName = Path.Combine(appDirectory, appName),
                Arguments = args,
                Verb = "runas",
            };

            try
            {
                Process.Start(si);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 结束进程
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <returns></returns>
        public static bool KillProcess(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.IsEmpty())
            {
                return true;
            }

            processes.ForEach(p =>
            {
                p.Kill();
                p.WaitForExit();
            });

            return true;
        }
    }
}