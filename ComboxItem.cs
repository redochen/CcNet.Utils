using System.Collections.Generic;
using System.Linq;
using CcNet.Utils.Extensions;

namespace CcNet.Utils
{
    /// <summary>
    /// 下拉框item对象
    /// </summary>
    public class ComboxItem
    {
        /// <summary>
        /// 绑定的值
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 显示的文本
        /// </summary>
        public string Text { get; private set; }

        public ComboxItem(object value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        /// <summary>
        /// 赋值操作符
        /// </summary>
        /// <param name="pair"></param>
        public static implicit operator ComboxItem(KeyValuePair<object, string> pair)
            => new ComboxItem(pair.Key, pair.Value);

        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="item"></param>
        public static implicit operator KeyValuePair<object, string>(ComboxItem item)
            => new KeyValuePair<object, string>(item.Value, item.Text);

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// 判断是否匹配(Value相关判断；Text包含判断)
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalMatchText">文本全匹配或包括匹配</param>
        /// <returns></returns>
        public bool IsMatch(ComboxItem filter, bool totalMatchText)
        {
            if (null == filter)
            {
                return false;
            }

            if (filter.Value != null)
            {
                return Value.Equals(filter.Value);
            }

            if (filter.Text.IsValid())
            {
                return totalMatchText ? Text.Equals(filter.Text) : Text.Contains(filter.Text);
            }

            return false;
        }

        /// <summary>
        /// 根据值列表获取ComboxItem列表
        /// </summary>
        /// <param name="setTextAsValue">是否将文本设置为值</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<ComboxItem> FromValues(bool setTextAsValue = true, params object[] values)
        {
            if (values.IsEmpty())
            {
                return null;
            }

            return values.Select(v => new ComboxItem(v, setTextAsValue ?
                v?.ToString() ?? string.Empty : string.Empty)).ToList();
        }

        /// <summary>
        /// 根据文本列表获取ComboxItem列表
        /// </summary>
        /// <param name="setValueAsText">是否将值设置为文本</param>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static List<ComboxItem> FromTexts(bool setValueAsText = true, params string[] texts)
        {
            if (texts.IsEmpty())
            {
                return null;
            }

            return texts.Select(t => new ComboxItem(setValueAsText ? t : null, t)).ToList();
        }
    }
}