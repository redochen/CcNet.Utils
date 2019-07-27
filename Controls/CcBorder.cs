using System;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义边框属性类
    /// </summary>
    [TypeConverter(typeof(CcBorderTypeConverter))]
    public class CcBorder
    {
        #region 属性
        [Description("获取或设置边框宽度"), RefreshProperties(RefreshProperties.All)]
        public int Size { get; set; }

        [Description("获取或设置边框开放的方向"), RefreshProperties(RefreshProperties.All)]
        public CcCtrlPosition OpenDirection { get; set; }

        [Description("获取或设置圆角半径"), RefreshProperties(RefreshProperties.All)]
        public int Radius { get; set; }

        [Description("获取或设置圆角类型"), RefreshProperties(RefreshProperties.All)]
        public CcCtrlCorner RadiusCorners { get; set; }
        #endregion

        /// <summary>
        /// 获取默认的边框属性
        /// </summary>
        /// <returns></returns>
        public static CcBorder DefaultBorder => new CcBorder
        {
            Size = 1,
            OpenDirection = CcCtrlPosition.None,
            Radius = 0,
            RadiusCorners = CcCtrlCorner.All,
        };
    }

    /// <summary>
    /// CcBorder类型转换器类
    /// </summary>
    public class CcBorderTypeConverter : CcTypeConverter
    {
        private const string KeySize = "Size:";
        private const string KeyDirection = "OpenDirection:";
        private const string KeyRadius = "Radius:";
        private const string KeyCorners = "RadiusCorners:";

        private TypeConverter PositionConverter = TypeDescriptor.GetConverter(typeof(CcCtrlPosition));
        private TypeConverter CornerConverter = TypeDescriptor.GetConverter(typeof(CcCtrlCorner));

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && !(value is CcBorder))
            {
                throw WrongTypeException;
            }

            var border = value as CcBorder;

            //转换成字符串
            if (destinationType == typeof(string))
            {
                if (null == border)
                {
                    return string.Empty;
                }

                var sb = new StringBuilder();

                if (border.Size > 0)
                {
                    sb.Append($"{KeySize}{IntConverter.ConvertToString(context, culture, border.Size)}", Separator);
                }

                if (border.Radius > 0)
                {
                    sb.Append($"{KeyRadius}{IntConverter.ConvertToString(context, culture, border.Radius)}", Separator);
                }

                if (border.OpenDirection != CcCtrlPosition.None)
                {
                    sb.Append($"{KeyDirection}{PositionConverter.ConvertToString(context, culture, border.OpenDirection)}", Separator);
                }

                if (border.RadiusCorners != CcCtrlCorner.None)
                {
                    sb.Append($"{KeyCorners}{CornerConverter.ConvertToString(context, culture, border.RadiusCorners)}", Separator);
                }

                return sb.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value)
            {
                return new CcBorder();
            }

            //从字符串解析
            if (value is string stringValue)
            {
                if (stringValue.Length <= 0)
                {
                    return new CcBorder();
                }

                int size = 0, radius = 0;
                CcCtrlPosition direction = CcCtrlPosition.None;
                CcCtrlCorner corners = CcCtrlCorner.None;

                stringValue.Split(Separator).ForEach(s =>
                {
                    if (s.StartsWith(KeySize))
                    {
                        size = (int)IntConverter.ConvertFromString(context, culture, s.Substring(KeySize.Length));
                    }
                    else if (s.StartsWith(KeyDirection))
                    {
                        direction = (CcCtrlPosition)PositionConverter.ConvertFromString(context, culture, s.Substring(KeyDirection.Length));
                    }
                    else if (s.StartsWith(KeyRadius))
                    {
                        radius = (int)IntConverter.ConvertFromString(context, culture, s.Substring(KeyRadius.Length));
                    }
                    else if (s.StartsWith(KeyCorners))
                    {
                        corners = (CcCtrlCorner)CornerConverter.ConvertFromString(context, culture, s.Substring(KeyCorners.Length));
                    }
                });

                return new CcBorder
                {
                    Size = size,
                    OpenDirection = direction,
                    Radius = radius,
                    RadiusCorners = corners
                };
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}