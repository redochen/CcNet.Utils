using System;
using System.Drawing;
using System.ComponentModel;

namespace CcNet.Utils.Controls
{
    /// <summary>
    /// CC类型转换器类
    /// </summary>
    public class CcTypeConverter : ExpandableObjectConverter
    {
        protected const char Separator = '|';
        protected static Exception WrongTypeException = new Exception("类型错误");

        protected TypeConverter FontConverter = TypeDescriptor.GetConverter(typeof(Font));
        protected TypeConverter ColorConverter = TypeDescriptor.GetConverter(typeof(Color));
        protected TypeConverter IntConverter = TypeDescriptor.GetConverter(typeof(int));
        protected TypeConverter FloatConverter = TypeDescriptor.GetConverter(typeof(float));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(CcAppearances) ||
                destinationType == typeof(CcAppearance) ||
                destinationType == typeof(CcBorder))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }
    }
}