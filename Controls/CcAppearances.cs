using System;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// 自定义外观属性集类
    /// </summary>
    [TypeConverter(typeof(CcAppearancesTypeConverter))]
    public class CcAppearances
    {
        #region 属性
        [Description("获取或设置外观属性"), RefreshProperties(RefreshProperties.All)]
        public CcAppearance Appearance { get; set; }

        [Description("获取或禁用时设置外观属性"), RefreshProperties(RefreshProperties.All)]
        public CcAppearance DisabledAppearance { get; set; }

        [Description("获取或设置鼠标悬停时的外观属性"), RefreshProperties(RefreshProperties.All)]
        public CcAppearance HoveredAppearance { get; set; }

        [Description("获取或设置获得聚焦时的外观属性"), RefreshProperties(RefreshProperties.All)]
        public CcAppearance FocusedAppearance { get; set; }

        [Description("获取或设置鼠标按下时的外观属性"), RefreshProperties(RefreshProperties.All)]
        public CcAppearance PressedAppearance { get; set; }
        #endregion

        /// <summary>
        /// 获取默认的外观
        /// </summary>
        public static CcAppearances DefaultAppearances => new CcAppearances
        {
            Appearance = CcAppearance.DefaultAppearance,
            DisabledAppearance = CcAppearance.EmptyAppearance,
            FocusedAppearance = CcAppearance.EmptyAppearance,
            HoveredAppearance = CcAppearance.EmptyAppearance,
            PressedAppearance = CcAppearance.EmptyAppearance,
        };
    }

    /// <summary>
    /// CcAppearances类型转换器类
    /// </summary>
    public class CcAppearancesTypeConverter : CcTypeConverter
    {
        private const string KeyNormal = "Normal:";
        private const string KeyDisabled = "Disabled:";
        private const string KeyHovered = "Hovered:";
        private const string KeyFocused = "Focused:";
        private const string KeyPressed = "Pressed:";

        private TypeConverter AppearanceConverter = TypeDescriptor.GetConverter(typeof(CcAppearance));

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && !(value is CcAppearances))
            {
                throw WrongTypeException;
            }

            var appearances = value as CcAppearances;

            //转换成字符串
            if (destinationType == typeof(string))
            {
                if (null == appearances)
                {
                    return string.Empty;
                }

                var sb = new StringBuilder();

                if (appearances.Appearance != null)
                {
                    sb.Append($"{KeyNormal}{AppearanceConverter.ConvertToString(context, culture, appearances.Appearance)}", Separator);
                }

                if (appearances.DisabledAppearance != null)
                {
                    sb.Append($"{KeyDisabled}{AppearanceConverter.ConvertToString(context, culture, appearances.DisabledAppearance)}", Separator);
                }

                if (appearances.HoveredAppearance != null)
                {
                    sb.Append($"{KeyHovered}{AppearanceConverter.ConvertToString(context, culture, appearances.HoveredAppearance)}", Separator);
                }

                if (appearances.FocusedAppearance != null)
                {
                    sb.Append($"{KeyFocused}{AppearanceConverter.ConvertToString(context, culture, appearances.FocusedAppearance)}", Separator);
                }

                if (appearances.PressedAppearance != null)
                {
                    sb.Append($"{KeyPressed}{AppearanceConverter.ConvertToString(context, culture, appearances.PressedAppearance)}", Separator);
                }

                return sb.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (null == value)
            {
                return new CcAppearances();
            }

            //从字符串解析
            if (value is string stringValue)
            {
                if (stringValue.Length <= 0)
                {
                    return new CcAppearances();
                }

                CcAppearance normal = null;
                CcAppearance disabled = null;
                CcAppearance hovered = null;
                CcAppearance focused = null;
                CcAppearance pressed = null;

                stringValue.Split(Separator).ForEach(s =>
                {
                    if (s.StartsWith(KeyNormal))
                    {
                        normal = (CcAppearance)AppearanceConverter.ConvertFromString(context, culture, s.Substring(KeyNormal.Length));
                    }
                    else if (s.StartsWith(KeyDisabled))
                    {
                        disabled = (CcAppearance)AppearanceConverter.ConvertFromString(context, culture, s.Substring(KeyDisabled.Length));
                    }
                    else if (s.StartsWith(KeyHovered))
                    {
                        hovered = (CcAppearance)AppearanceConverter.ConvertFromString(context, culture, s.Substring(KeyHovered.Length));
                    }
                    else if (s.StartsWith(KeyFocused))
                    {
                        focused = (CcAppearance)AppearanceConverter.ConvertFromString(context, culture, s.Substring(KeyFocused.Length));
                    }
                    else if (s.StartsWith(KeyPressed))
                    {
                        pressed = (CcAppearance)AppearanceConverter.ConvertFromString(context, culture, s.Substring(KeyPressed.Length));
                    }
                });

                return new CcAppearances
                {
                    Appearance = normal,
                    DisabledAppearance = disabled,
                    FocusedAppearance = focused,
                    HoveredAppearance = hovered,
                    PressedAppearance = pressed,
                };
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}