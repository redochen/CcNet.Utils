using System.Threading.Tasks;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Task扩展类
    /// </summary>
    public static class TaskExtension
    {
        /// <summary>
        /// 任务是否已停止（完成、取消、出错等）
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool IsDone(this Task task)
        {
            if (null == task)
            {
                return false;
            }

            return (task.IsCompleted || task.IsCanceled || task.IsFaulted);
        }
    }
}
