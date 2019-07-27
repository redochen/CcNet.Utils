using System.Configuration;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Helpers
{
    public class ConfigHelper
    {
        /// <summary>
        /// 从web.config中读取配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">获取失败时使用的默认值</param>
        /// <param name="configFile">EXE配置文件名称，为空时打开默认的配置文件</param>
        /// <returns></returns>
        public static string GetValue(string key, string defaultValue = "", string configFile = null)
        {
            if (null == key)
            {
                return defaultValue;
            }

            object value = null;


            if (configFile.IsValid())
            {
                var fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFile
                };

                var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                value = config.AppSettings.Settings[key]?.Value;
            }
            else
            {
                value = ConfigurationManager.AppSettings[key];
            }

            return value?.ToString() ?? defaultValue;
        }

        /// <summary>
        /// 获取Int32配置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">获取失败时使用的默认值</param>
        /// <param name="configFile">EXE配置文件名称，为空时打开默认的配置文件</param>
        /// <returns></returns>
        public static int GetInt32(string key, int defaultValue = 0, string configFile = null)
        {
            var value = GetValue(key, configFile: configFile);
            return value.ToInt32(defaultValue);
        }

        /// <summary>
        /// 获取Boolean配置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">获取失败时使用的默认值</param>
        /// <param name="configFile">EXE配置文件名称，为空时打开默认的配置文件</param>
        /// <returns></returns>
        public static bool GetBoolean(string key, bool defaultValue = false, string configFile = null)
        {
            var value = GetValue(key, configFile: configFile);
            return value.ToBoolean(defaultValue);
        }
    }
}