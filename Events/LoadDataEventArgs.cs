using System;

namespace CcNet.Utils.Events
{
    /// <summary>
    /// 数据加载事件代理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void LoadDataEventHandler(object sender, LoadDataEventArgs args);

    /// <summary>
    /// 数据加载完成回调
    /// </summary>
    /// <param name="total"></param>
    /// <param name="data"></param>
    public delegate void LoadDataCompleteCallback(int total, object data);

    /// <summary>
    /// 选择器事件参数类
    /// </summary>
    public class LoadDataEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="callback"></param>
        public LoadDataEventArgs(string filter, int pageIndex, int pageSize, LoadDataCompleteCallback callback)
            : base()
        {
            Filter = filter;
            PageIndex = pageIndex;
            PageSize = pageSize;
            OnCompletion = callback;
        }

        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string Filter { get; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 完成事件回调
        /// </summary>
        public LoadDataCompleteCallback OnCompletion;
    }
}