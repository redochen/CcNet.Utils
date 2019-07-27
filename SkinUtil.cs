using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace CcNet.Utils
{
    public class SkinUtil
    {
        /// <summary>
        /// 获取文本格式标识
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="rightToLeft"></param>
        /// <param name="singleLine">是否单行</param>
        /// <returns></returns>
        public static TextFormatFlags GetTextFormatFlags(HorizontalAlignment alignment, bool rightToLeft, bool singleLine)
        {
            var flags = GetTextFormatFlags(rightToLeft, singleLine);

            if (_HorizontalAlignmentFlags.ContainsKey(alignment))
            {
                flags |= _HorizontalAlignmentFlags[alignment];
            }

            return flags;
        }

        /// <summary>
        /// 获取文本格式标识
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="rightToLeft"></param>
        /// <param name="singleLine">是否单行</param>
        /// <returns></returns>
        public static TextFormatFlags GetTextFormatFlags(ContentAlignment alignment, bool rightToLeft, bool singleLine)
        {
            var flags = GetTextFormatFlags(rightToLeft, singleLine);

            if (_ContentAlignmentFlags.ContainsKey(alignment))
            {
                flags |= _ContentAlignmentFlags[alignment];
            }

            return flags;
        }

        /// <param name="rightToLeft"></param>
        /// <param name="singleLine">是否单行</param>
        static TextFormatFlags GetTextFormatFlags(bool rightToLeft, bool singleLine)
        {
            var flags = TextFormatFlags.WordBreak;

            if (singleLine)
            {
                flags |= TextFormatFlags.SingleLine;
            }

            if (rightToLeft)
            {
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }

            return flags;
        }

        /// <summary>
        /// HorizontalAlignment - TextFormatFlags 映射关系
        /// </summary>
        static readonly Dictionary<HorizontalAlignment, TextFormatFlags> _HorizontalAlignmentFlags
            = new Dictionary<HorizontalAlignment, TextFormatFlags>()
            {
                {HorizontalAlignment.Left, TextFormatFlags.VerticalCenter | TextFormatFlags.Left},
                {HorizontalAlignment.Center, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter},
                {HorizontalAlignment.Right, TextFormatFlags.VerticalCenter | TextFormatFlags.Right}
            };

        /// <summary>
        /// ContentAlignment - TextFormatFlags 映射关系
        /// </summary>
        static readonly Dictionary<ContentAlignment, TextFormatFlags> _ContentAlignmentFlags
            = new Dictionary<ContentAlignment, TextFormatFlags>()
            {
                {ContentAlignment.TopLeft, TextFormatFlags.Top | TextFormatFlags.Left},
                {ContentAlignment.TopCenter, TextFormatFlags.Top | TextFormatFlags.HorizontalCenter},
                {ContentAlignment.TopRight, TextFormatFlags.Top | TextFormatFlags.Right},
                {ContentAlignment.MiddleLeft, TextFormatFlags.VerticalCenter | TextFormatFlags.Left},
                {ContentAlignment.MiddleCenter, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter},
                {ContentAlignment.MiddleRight, TextFormatFlags.VerticalCenter | TextFormatFlags.Right},
                {ContentAlignment.BottomLeft, TextFormatFlags.Bottom | TextFormatFlags.Left},
                {ContentAlignment.BottomCenter, TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter},
                {ContentAlignment.BottomRight, TextFormatFlags.Bottom | TextFormatFlags.Right},
            };
    }
}
