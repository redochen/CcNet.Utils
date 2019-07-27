using System;
using System.Reflection;
using CcNet.Utils.Helpers;

namespace CcNet.Utils
{
    /// <summary>
    /// 程序集初始化器类
    /// 使用方法：请将此类的代码完整地复制到需要启用初始化的项目中（命令空间修改为对应的项目名称）
    /// </summary>
    internal class AssemblyInitializer
    {
        /// <summary>
        /// 约定的资源名
        /// </summary>
        public const string CONVENTION_RESOURCE_NAME = "References";

        /// <summary>
        /// 约定的初始化类名
        /// </summary>
        public const string CONVENTION_INIT_CLASS_NAME = "AssemblyInitializer";

        /// <summary>
        /// 约定的初始化方法名
        /// </summary>
        public const string CONVENTION_INIT_METHOD_NAME = "Initialize";

        /// <summary>
        /// 初始化方法
        /// </summary>
        public static void Initialize()
        {
            //非CcNet.Utils模块请注释本行
            AssemblyHelper.Initialize();
        }

        /// <summary>
        /// 加载资源中的程序集并初始化
        /// </summary>
        /// <param name="callingAssembly"></param>
        /// <param name="assemblyName"></param>
        /// <param name="silentMode"></param>
        private static void LoadAndInit(Assembly callingAssembly, string assemblyName, bool silentMode = true)
        {
            var assembly = LoadResourceAssembly(callingAssembly, assemblyName);
            if (null == assembly)
            {
                if (silentMode) return;
                throw new Exception($"加载程序集 {assemblyName} 失败");
            }

            InitAssembly(assembly, silentMode);
        }

        /// <summary>
        /// 初始化程序集（调用AssemblyInitializer.Initialize方法）
        /// </summary>
        /// <param name="assembly">要初始化的程序集</param>
        /// <param name="silentMode">静默模式（不提示错误）</param>
        public static void InitAssembly(Assembly assembly, bool silentMode = true)
        {
            if (null == assembly)
            {
                if (silentMode) return;
                throw new Exception("参数 assembly 不能为空");
            }

            var assemblyName = assembly.GetName().Name;
            var typeName = $"{assemblyName}.{CONVENTION_INIT_CLASS_NAME}";
            var type = assembly.GetType(typeName);
            if (null == type)
            {
                if (silentMode) return;
                throw new Exception($"获取类型 {typeName} 失败");
            }

            var method = type.GetMethod(CONVENTION_INIT_METHOD_NAME, BindingFlags.Public | BindingFlags.Static);
            if (null == method)
            {
                if (silentMode) return;
                throw new Exception($"获取方法 {CONVENTION_INIT_METHOD_NAME} 失败");
            }

            method.Invoke(null, null);
        }

        /// <summary>
        /// 加载资源中的程序集
        /// </summary>
        /// <param name="assembly">调用者程序集</param>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns></returns>
        public static Assembly LoadResourceAssembly(Assembly assembly, string assemblyName)
        {
            var resourceName = $"{assembly.GetName().Name}.{CONVENTION_RESOURCE_NAME}.{assemblyName}.dll";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (null == stream)
                {
                    return null;
                }

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return Assembly.Load(buffer);
            }
        }
    }
}
