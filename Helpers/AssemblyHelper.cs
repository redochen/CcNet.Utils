using System;
using System.Linq;
using System.Reflection;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    /// <summary>
    ///程序集帮助类
    /// </summary>
    public class AssemblyHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        /// <summary>
        /// 程序集加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void AssemblyLoad(object sender, AssemblyLoadEventArgs args) =>
            AssemblyInitializer.InitAssembly(args.LoadedAssembly, silentMode: true);

        /// <summary>
        /// 程序集解析事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //获取加载失败的程序集名称
            var assemblyName = new AssemblyName(args.Name).Name;
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var assembly = loadedAssemblies.FirstOrDefault(a => a.GetName().Name == assemblyName);
            if (assembly != null)
            {
                return assembly;
            }

            //尝试从所有已加载的程序集资源中加载
            loadedAssemblies.ForEach(a => (assembly = LoadResourceAssembly(a, assemblyName)) != null);

            return assembly;
        }

        /// <summary>
        /// 加载资源中的程序集
        /// </summary>
        /// <param name="assembly">调用者程序集</param>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns></returns>
        public static Assembly LoadResourceAssembly(Assembly assembly, string assemblyName)
            => AssemblyInitializer.LoadResourceAssembly(assembly, assemblyName);
    }
}
