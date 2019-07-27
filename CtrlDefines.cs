using System.ComponentModel;

namespace CcNet.Utils
{
    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum CcCtrlState
    {
        [Description("普通状态")]
        Normal = 0,

        [Description("悬停状态")]
        Hovered,

        [Description("聚焦状态")]
        Focused,

        [Description("按下状态")]
        Pressed,

        [Description("禁用状态")]
        Disabled,
    }

    /// <summary>
    /// 位置枚举
    /// </summary>
    public enum CcCtrlPosition
    {
        [Description("无")]
        None = 0,

        [Description("顶部")]
        Top,

        [Description("左边")]
        Left,

        [Description("右边")]
        Right,

        [Description("底部")]
        Bottom,
    }

    /// <summary>
    /// 拐角枚举
    /// </summary>
    public enum CcCtrlCorner
    {
        [Description("无")]
        None = 0,

        [Description("左上角")]
        TopLeft = 1,

        [Description("右上角")]
        TopRight = 2,

        [Description("左下角")]
        BottomLeft = 4,

        [Description("右下角")]
        BottomRight = 8,

        [Description("左上角和右上角")]
        Top = TopLeft | TopRight,

        [Description("左上角和左下角")]
        Left = TopLeft | BottomLeft,

        [Description("右上角和右下角")]
        Right = TopRight | BottomRight,

        [Description("左下角和右下角")]
        Bottom = BottomLeft | BottomRight,

        [Description("除了左上角以外")]
        ExceptTopLeft = Right | BottomLeft,

        [Description("除了左下角以外")]
        ExceptBottomLeft = Right | TopLeft,

        [Description("除了右上角以外")]
        ExceptTopRight = Left | BottomRight,

        [Description("除了右下角以外")]
        ExceptBottomRight = Left | TopRight,

        [Description("全部")]
        All = TopLeft | TopRight | BottomLeft | BottomRight,
    }

    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class CcCtrlEnumHelper
    {
        /// <summary>
        /// 获取圆角的位置
        /// </summary>
        /// <param name="openPosition">边框开放的位置</param>
        /// <returns></returns>
        public static CcCtrlCorner GetCorners(CcCtrlPosition openPosition)
        {
            switch (openPosition)
            {
                case CcCtrlPosition.Left:
                    return CcCtrlCorner.Right;
                case CcCtrlPosition.Top:
                    return CcCtrlCorner.Bottom;
                case CcCtrlPosition.Right:
                    return CcCtrlCorner.Left;
                case CcCtrlPosition.Bottom:
                    return CcCtrlCorner.Top;
                case CcCtrlPosition.None:
                default:
                    return CcCtrlCorner.All;
            }
        }
    }
}