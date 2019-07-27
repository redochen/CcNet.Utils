using System;

namespace CcNet.Utils.Events
{
    /// <summary>
    /// 分页器改变事件代理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">事件参数</param>
    public delegate void PageChangeEventHandler(object sender, PagerEventArgs args);

    /// <summary>
    /// 分页器改变事件完成回调
    /// </summary>
    /// <param name="totalCount">总项数</param>
    public delegate void PageChangeCompleteCallback(int totalCount);

    /// <summary>
    /// 分页器事件参数类
    /// </summary>
    public class PagerEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="callback"></param>
        public PagerEventArgs(int pageIndex, int pageSize, PageChangeCompleteCallback callback)
            : base()
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            OnCompletion = callback;
        }

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
        public PageChangeCompleteCallback OnCompletion;
    }
}