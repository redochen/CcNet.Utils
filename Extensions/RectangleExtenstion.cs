using System;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace CcNet.Utils.Extensions
{
    /// <summary>
    /// Rectangle扩展类
    /// </summary>
    public static class RectangleExtenstion
    {
        /// <summary>
        /// 根据普通矩形得到圆角矩形的路径
        /// </summary>
        /// <param name="rect">原始矩形</param>
        /// <param name="radius">半径</param>
        /// <param name="corners">圆角的方向</param>
        /// <returns>图形路径</returns>
        public static GraphicsPath GetRoundedRectPath(this Rectangle rect, int radius, CcCtrlCorner corners = CcCtrlCorner.All)
        {
            var diameter = 2 * radius;

            // 把圆角矩形分成八段直线、弧的组合，依次加到路径中  
            var path = new GraphicsPath();

            if ((corners & CcCtrlCorner.TopLeft) == CcCtrlCorner.TopLeft)
            {
                //左上角
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180F, 90F);
            }
            else
            {
                //顶部直线（从左往右画）
                path.AddLine(rect.X, rect.Y, rect.Right - diameter, rect.Y);
            }

            if ((corners & CcCtrlCorner.TopRight) == CcCtrlCorner.TopRight)
            {
                //右上角
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270F, 90F);
            }
            else
            {
                //右边直线（从上往下画）
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Bottom - diameter);
            }

            if ((corners & CcCtrlCorner.BottomRight) == CcCtrlCorner.BottomRight)
            {
                //右下角
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0F, 90F);
            }
            else
            {
                //底部直线（从右往左画）
                path.AddLine(rect.Right, rect.Bottom, rect.X + diameter, rect.Bottom);
            }

            if ((corners & CcCtrlCorner.BottomLeft) == CcCtrlCorner.BottomLeft)
            {
                //左下角
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90F, 90F);
            }
            else
            {
                //左边直线（从下往上）
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Y + diameter);
            }

            path.CloseAllFigures();

            return path;
        }

        /// <summary>
        /// 获取开放的线条
        /// </summary>
        /// <param name="rect">原始矩形</param>
        /// <param name="open">开放的方向</param>
        /// <param name="borderSize">边框大小</param>
        /// <param name="radius">圆角半径</param>
        /// <returns>Item1：起始点；Item2：终结点</returns>
        public static Tuple<Point, Point> GetOpenLine(this Rectangle rect, CcCtrlPosition open, int borderSize, int radius)
        {
            if (null == rect)
            {
                return null;
            }

            var delta = (radius > 0) ? Win32.SIZE_SYSTEM : 0;

            var leftX_H = rect.X + borderSize - (borderSize == 1 ? delta : 0);
            var leftX_V = rect.X + borderSize / 2;
            var rightX_H = rect.Right - borderSize - (borderSize == 1 ? 0 : delta);
            var rightX_V = rect.Right - borderSize / 2 - delta;
            var topY_H = rect.Y + borderSize / 2;
            var topY_V = rect.Y + borderSize - (borderSize == 1 ? delta : 0);
            var bottomY_H = rect.Bottom - borderSize / 2 - delta;
            var bottomY_V = rect.Bottom - borderSize - (borderSize == 1 ? 0 : delta);

            switch (open)
            {
                case CcCtrlPosition.Top:
                    return new Tuple<Point, Point>(new Point(leftX_H, topY_H), new Point(rightX_H, topY_H));
                case CcCtrlPosition.Left:
                    return new Tuple<Point, Point>(new Point(leftX_V, topY_V), new Point(leftX_V, bottomY_V));
                case CcCtrlPosition.Right:
                    return new Tuple<Point, Point>(new Point(rightX_V, topY_V), new Point(rightX_V, bottomY_V));
                case CcCtrlPosition.Bottom:
                    return new Tuple<Point, Point>(new Point(leftX_H, bottomY_H), new Point(rightX_H, bottomY_H));
                default:
                    return null;
            }
        }
    }
}
