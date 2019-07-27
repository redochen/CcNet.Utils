using System;

namespace CcNet.Utils.Events
{
    /// <summary>
    /// 通用事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CommonEventHandler(object sender, CommonEventArgs e);

    /// <summary>
    /// 通用事件参数类
    /// </summary>
    public class CommonEventArgs : EventArgs
    {
        /// <summary>
        /// 空事件实例
        /// </summary>
        public new static readonly CommonEventArgs Empty = new CommonEventArgs(null);

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="data"></param>
        public CommonEventArgs(object data) : base()
        {
            Data = data;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool Handled { get; set; }
    }
}