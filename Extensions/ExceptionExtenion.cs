using System;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Exception扩展类
    /// </summary>
    public static class ExceptionExtenion
    {
        /// <summary>
        /// 是否为特定的异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSpecialExcetion(this Exception ex, Type type)
        {
            if (null == ex)
            {
                return false;
            }

            if (ex.GetType().Equals(type))
            {
                return true;
            }

            return ex.InnerException.IsSpecialExcetion(type);
        }
    }
}