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
        public Form1()
        {
            InitializeComponent();
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int desiredWidth = (int)(screenWidth * 0.6);
            int desiredHeight = (int)(desiredWidth * 0.7);
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
    }
}
