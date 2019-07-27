using System.Collections.Generic;
using System.Linq;
using CcNet.Utils.Extensions;

namespace CcNet.Utils
{
    /// <summary>
    /// 下拉框item对象
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ComboxItem<TValue>
    {
        /// <summary>
        /// 绑定的值
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// 显示的文本
        /// </summary>
        public string Text { get; private set; }

        public ComboxItem(TValue value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        /// <summary>
        /// 赋值操作符
        /// </summary>
        /// <param name="pair"></param>
        public static implicit operator ComboxItem<TValue>(KeyValuePair<TValue, string> pair)
            => new ComboxItem<TValue>(pair.Key, pair.Value);

        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <param name="item"></param>
        public static implicit operator KeyValuePair<TValue, string>(ComboxItem<TValue> item)
            => new KeyValuePair<TValue, string>(item.Value, item.Text);

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
        public bool IsMatch(ComboxItem<TValue> filter, bool totalMatchText)
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
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<ComboxItem<TValue>> FromValues(params TValue[] values)
        {
            if (values.IsEmpty())
            {
                return null;
            }

            return values.Select(v => new ComboxItem<TValue>(v, string.Empty)).ToList();
        }

        /// <summary>
        /// 根据文本列表获取ComboxItem列表
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static List<ComboxItem<TValue>> FromTexts(params string[] texts)
        {
            if (texts.IsEmpty())
            {
                return null;
            }

            return texts.Select(t => new ComboxItem<TValue>(default(TValue), t)).ToList();
        }
    }
}