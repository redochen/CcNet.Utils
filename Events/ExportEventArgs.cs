using System;

namespace CcNet.Utils.Events
{
    /// <summary>
    /// 导出事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ExportEventHandler(object sender, ExportEventArgs e);

    /// <summary>
    /// 导出事件参数类
    /// </summary>
    public class ExportEventArgs : EventArgs
    {
        /// <summary>
        /// 空事件实例
        /// </summary>
        public new static readonly ExportEventArgs Empty = new ExportEventArgs(null, null);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public ExportEventArgs(string filePath, object data) : base()
        {
            FilePath = filePath;
            Data = data;
        }

        /// <summary>
        /// 导出的文件路径 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 导出的数据
        /// </summary>
        public object Data { get; set; }
    }
}