using System;
using System.Drawing;
using System.Windows.Forms;
using CcNet.Utils.Extensions;

namespace CcNet.Utils.Forms
{
    public class OpacityForm : Form
    {
        private MaskForm maskForm = new MaskForm();//透明窗体不穿透鼠标

        public OpacityForm()
        {
            CenterToParent();
            InitForm();
        }

        private void InitForm()
        {
            //WindowState = FormWindowState.Maximized;//本窗体最大化
            this.SetTransparent(Color.Black);
            DoubleBuffered = true;//双缓存处理

            //maskForm.WindowState = FormWindowState.Maximized,//最大化
            this.SetTransparent(opacity: 0.6);
            maskForm.TopMost = true; //让不穿透鼠标透明窗体画板为最上层
            maskForm.DoubleBuffered = true; //双缓存处理

            if (!DesignMode)
            {
                maskForm.Show(this);//显示
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            maskForm.Width = this.Width;
            maskForm.Height = this.Height;
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            maskForm.Location = this.Location;
        }
    }
}