using System.Windows.Forms;

namespace CcNet.Utils.Forms
{
    public partial class MaskForm : Form
    {
        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public MaskForm()
        {
            InitializeComponent();
            CenterToParent();
        }
    }
}