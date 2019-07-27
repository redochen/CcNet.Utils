using System.Drawing;
using System.Windows.Forms;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Forms
{
    public partial class ImageForm : Form
    {
        bool beginMove = false;//初始化鼠标位置
        int currentX = 0;
        int currentY = 0;

        public ImageForm()
        {
            InitializeComponent();
            this.SetTransparent(Color.Black);
            DoubleBuffered = true;//双缓存处理
        }

        private void ImageForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BeginMove();
            }
        }

        private void ImageForm_MouseMove(object sender, MouseEventArgs e)
        {
            OnMove();
        }

        private void ImageForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                EndMove();
            }
        }

        #region 移动控制
        /// <summary>
        /// 开始移动
        /// </summary>
        protected void BeginMove()
        {
            beginMove = true;
            currentX = MousePosition.X;//鼠标的x坐标为当前窗体左上角x坐标  
            currentY = MousePosition.Y;//鼠标的y坐标为当前窗体左上角y坐标  
        }

        /// <summary>
        /// 正在移动
        /// </summary>
        protected void OnMove()
        {
            if (beginMove)
            {
                this.Left += MousePosition.X - currentX;//根据鼠标x坐标确定窗体的左边坐标x  
                this.Top += MousePosition.Y - currentY;//根据鼠标的y坐标窗体的顶部，即Y坐标  
                currentX = MousePosition.X;
                currentY = MousePosition.Y;
            }
        }

        /// <summary>
        /// 结束移动
        /// </summary>
        protected void EndMove()
        {
            currentX = 0; //设置初始状态  
            currentY = 0;
            beginMove = false;
        }
        #endregion
    }
}