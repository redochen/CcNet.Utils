using System;
using System.Data;

namespace CcNet.Utils
{
    /// <summary>
    /// 列表列头显示
    /// </summary>
    public class GridColumnDisplay
    {
        public GridColumnDisplay() { }

        public GridColumnDisplay(DataColumn column)
        {
            if (column != null)
            {
                FieldName = column.ColumnName;
                Caption = column.ColumnName;
                DataType = column.DataType;
            }
        }

        public GridColumnDisplay(string field, string caption, int? width = null, object bindType = null)
        {
            FieldName = field;
            Caption = caption;
            Width = width ?? Width;
            BindType = bindType;
        }

        /// <summary>
        /// 绑定域
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 宽度：-1表示自动宽度；0表示不显示
        /// </summary>
        public int Width { get; set; } = -1;

        /// <summary>
        /// 绑定类型
        /// </summary>
        public object BindType { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type DataType { get; set; } = typeof(string);

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int? VisibleIndex { get; set; }

        /// <summary>
        /// 绝对顺序
        /// </summary>
        public int? AbsoluteIndex { get; set; }
    }
}