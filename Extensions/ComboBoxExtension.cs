using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// ComboBox扩展类
    /// </summary>
    public static class ComboBoxExtension
    {
        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="object"></typeparam>
        /// <param name="cb"></param>
        /// <param name="items"></param>
        public static void BindData(this ComboBox cb, params object[] items)
            => cb.BindData(ComboxItem.FromValues(true, items));

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="object"></typeparam>
        /// <param name="cb"></param>
        /// <param name="items"></param>
        public static void BindData(this ComboBox cb, Dictionary<object, string> items)
            => cb.BindData(items?.Select(x => (ComboxItem)x));

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <typeparam name="object"></typeparam>
        /// <param name="cb"></param>
        /// <param name="items"></param>
        public static void BindData(this ComboBox cb, IEnumerable<ComboxItem> items)
        {
            if (null == cb || items.IsEmpty())
            {
                return;
            }

            var item = items.First();

            cb.DisplayMember = nameof(item.Text);
            cb.ValueMember = nameof(item.Value);
            cb.DataSource = items.ToList();
        }

        /// <summary>
        /// 获取所有项列表
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public static List<ComboxItem> GetItems(this ComboBox cb)
        {
            if (null == cb)
            {
                return null;
            }

            var items = new List<ComboxItem>();

            foreach (var item in cb.Items)
            {
                items.Add(item as ComboxItem);
            }

            return items;
        }

        /// <summary>
        /// 获取所选择的项
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public static ComboxItem GetSelectedItem(this ComboBox cb)
        {
            if (null == cb || null == cb.SelectedItem)
            {
                return null;
            }

            return cb.SelectedItem as ComboxItem;
        }

        /// <summary>
        /// 获取所选择的值
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public static object GetSelectedValue(this ComboBox cb)
            => cb.GetSelectedItem()?.Value;

        /// <summary>
        /// 获取所选择的文本
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public static string GetSelectedText(this ComboBox cb)
            => cb.GetSelectedItem()?.Text;

        /// <summary>
        /// 设置所选择的值
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="value"></param>
        public static void SetSelectedValue(this ComboBox cb, object value)
        {
            if (null == cb)
            {
                return;
            }

            if (cb.Items.IsEmptyEx())
            {
                return;
            }

            if (null == value)
            {
                cb.SelectedIndex = -1;
                return;
            }

            foreach (ComboxItem item in cb.Items)
            {
                if (null == item)
                {
                    continue;
                }

                if (value.GetType() != item.Value.GetType())
                {
                    value = Convert.ChangeType(value, item.Value.GetType());
                }

                if (value.Equals(item.Value))
                {
                    cb.SelectedItem = item;
                    return;
                }
            }

            cb.SelectedIndex = -1;
        }

        /// <summary>
        /// 设置自动调整高度
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="margin">边距</param>
        public static void AutoFitItemHeight(this ComboBox cb, int margin = 5)
        {
            if (null == cb)
            {
                return;
            }

            cb.DrawMode = DrawMode.OwnerDrawVariable;

            cb.DrawItem += (sender, e) =>
            {
                SolidBrush brush = null;

                try
                {
                    var cmb = sender as ComboBox;

                    brush = new SolidBrush(cmb.ForeColor);
                    Font ft = cmb.Font;

                    var text = cmb.GetItemText(cmb.Items[e.Index]);

                    // 计算字符串尺寸（以像素为单位）
                    var size = e.Graphics.MeasureString(text, cmb.Font);

                    // 水平居中
                    float left = 0;

                    //如果需要水平居中取消注释
                    //left = (float)(e.Bounds.Width - ss.Width) / 2; 
                    if (left < 0) left = 0f;

                    // 垂直居中
                    float top = (e.Bounds.Height - size.Height) / 2;
                    if (top <= 0) top = 0f;

                    // 输出
                    e.DrawBackground();

                    e.Graphics.DrawString(text, ft, brush, new RectangleF(
                        e.Bounds.X + left,    //设置X坐标偏移量
                        e.Bounds.Y + top,     //设置Y坐标偏移量
                        e.Bounds.Width, e.Bounds.Height), StringFormat.GenericDefault);

                    //e.Graphics.DrawString(cmb.GetItemText(cmb.Items[e.Index]), ft, myBrush, e.Bounds, StringFormat.GenericDefault);
                    //e.DrawFocusRectangle();
                }
                catch (Exception ex) { }
                finally
                {
                    brush?.Dispose();
                }
            };
        }
    }
}