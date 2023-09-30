using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloorplanDesigner
{
    public partial class Form1 : Form
    {
        private Dictionary<Control, Size> controlOriginalSizes = new Dictionary<Control, Size>();
        private Dictionary<Control, Point> controlOriginalPostions = new Dictionary<Control, Point>();
        private Dictionary<Control, float> controlOriginalTextSizes = new Dictionary<Control, float>();

        int width = 0;
        int height = 0;
        public Form1()
        {
            InitializeComponent();
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int desiredWidth = (int)(screenWidth * 0.6);
            int desiredHeight = (int)(desiredWidth * 0.7);
            this.SizeChanged += new EventHandler(Form1_SizeChanged);
            foreach (Control control in this.Controls)
            {
                controlOriginalSizes[control] = control.Size;
                controlOriginalPostions[control] = new Point(control.Location.X, control.Location.Y);
                if(control is Label)
                {
                    Label label = (Label)control;
                    controlOriginalTextSizes[control] = label.Font.Size;
                }
                else if(control is Button)
                {
                    Button button = (Button)control;
                    controlOriginalTextSizes[control] = button.Font.Size;
                }
            }
            width = this.ClientSize.Width;
            height = this.ClientSize.Height;
            //this.Width = desiredWidth;
            //this.Height = desiredHeight;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormWindowState prevState = this.WindowState;
            int prevWidth = this.Width;
            int prevHeight = this.Height;

            Form3 form3 = new Form3();
            this.Hide();


            form3.WindowState = prevState;
            form3.Width = prevWidth;
            form3.Height = prevHeight;
            form3.ShowDialog();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormWindowState prevState = this.WindowState;
            int prevWidth = this.Width;
            int prevHeight = this.Height;

            Form2 form2 = new Form2();
            this.Hide();


            form2.WindowState = prevState;
            form2.Width = prevWidth;
            form2.Height = prevHeight;
            form2.ShowDialog();
            this.Show();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {

            // 计算新的宽度和高度比例
            double widthRatio = (double)this.ClientSize.Width / width;
            double heightRatio = (double)this.ClientSize.Height / height;

            // 遍历窗体内的所有控件
            foreach (Control control in this.Controls)
            {
                
                // 获取控件的原始大小
                Size originalSize = controlOriginalSizes[control];
                Point originalPoint = controlOriginalPostions[control];
                // 根据比例计算新的宽度
                int newWidth = (int)(originalSize.Width * widthRatio);

                // 根据比例计算新的高度，这里假设保持宽高比例
                int newHeight = (int)(originalSize.Height * heightRatio);

                int newX = (int)(originalPoint.X * widthRatio);

                int newY = (int)(originalPoint.Y * heightRatio);

                float newFontSize;
                if(control is Label)
                {
                    Label label = (Label)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    label.Font = new Font(label.Font.FontFamily, newFontSize);
                }
                else if(control is Button)
                {
                    Button button = (Button)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    button.Font = new Font(button.Font.FontFamily, newFontSize);

                }

                // 设置控件的新大小
                control.Size = new Size(newWidth, newHeight);
                control.Location = new Point(newX, newY);

            }
        }
    }
}
