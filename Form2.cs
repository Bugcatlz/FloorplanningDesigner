using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloorplanDesigner
{
    public partial class Form2 : Form
    {
        private Dictionary<Control, Size> controlOriginalSizes = new Dictionary<Control, Size>();
        private Dictionary<Control, Point> controlOriginalPostions = new Dictionary<Control, Point>();
        private Dictionary<Control, float> controlOriginalTextSizes = new Dictionary<Control, float>();
        private Dictionary<DataGridViewColumn, int> controlOriginalColumnWidths = new Dictionary<DataGridViewColumn, int>();
        private Dictionary<DataGridViewRow, int> controlOriginalRowHeights = new Dictionary<DataGridViewRow, int>();



        int width = 0;
        int height = 0;
        public Form2()
        {
            InitializeComponent();
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int desiredWidth = (int)(screenWidth * 0.6);
            int desiredHeight = (int)(desiredWidth * 0.7);
            //this.Width = desiredWidth;
            //this.Height = desiredHeight;
            dataGridView1.CellContentClick += DeleteButton_Click;
            dataGridView2.CellContentClick += DeleteButton_Click;
            dataGridView3.CellContentClick += DeleteButton_Click;
            this.SizeChanged += new EventHandler(Form2_SizeChanged);
            SaveOriginalProperties(this);

            width = this.ClientSize.Width;
            height = this.ClientSize.Height;
        }
        private void SaveOriginalProperties(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                controlOriginalSizes[control] = control.Size;
                controlOriginalPostions[control] = control.Location;

                if (control is Label)
                {
                    Label label = (Label)control;
                    controlOriginalTextSizes[control] = label.Font.Size;
                }
                else if (control is Button)
                {
                    Button button = (Button)control;
                    controlOriginalTextSizes[control] = button.Font.Size;
                }
                else if(control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    controlOriginalTextSizes[control] = textBox.Font.Size;
                }
                else if(control is DataGridView)
                {
                    DataGridView dataGridView = (DataGridView)control;
                    controlOriginalTextSizes[control] = dataGridView.Font.Size;

                    // 保存列宽度
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        controlOriginalColumnWidths[column] = column.Width;
                    }

                    // 保存行高度
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        controlOriginalRowHeights[row] = row.Height;
                    }
                }

                // 如果控件是容器控件，递归保存其内部控件的属性
                if (control is Panel)
                {
                    SaveOriginalProperties(control);
                }
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView.ColumnCount - 1)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                dataGridView.Rows.Remove(row);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            string name = textBox10.Text;
            int MinArea = int.Parse((textBox9).Text);
            row.CreateCells(dataGridView1);
            row.Cells[0].Value = name;
            row.Cells[1].Value = MinArea;
            DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();
            buttonCell.Value = "✕";
            buttonCell.Tag = row;
            // 創建一個CellStyle並設置文字置中
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cellStyle.Font = new Font("Arial", 15);
            // 將CellStyle設置給DataGridViewButtonCell
            buttonCell.Style = cellStyle;

            row.Cells[2] = buttonCell;
            dataGridView1.Rows.Add(row);
            textBox10.Text = "";
            textBox9.Text = "";
            dataGridView1.ClearSelection();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            string name = textBox1.Text;
            int x = int.Parse((textBox2).Text);
            int y = int.Parse((textBox3).Text);
            int w = int.Parse((textBox4).Text);
            int h = int.Parse((textBox5).Text);

            row.CreateCells(dataGridView2);
            row.Cells[0].Value = name;
            row.Cells[1].Value = x;
            row.Cells[2].Value = y;
            row.Cells[3].Value = w;
            row.Cells[4].Value = h;
            DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();
            buttonCell.Value = "✕";
            buttonCell.Tag = row;
            // 創建一個CellStyle並設置文字置中
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cellStyle.Font = new Font("Arial", 15);
            // 將CellStyle設置給DataGridViewButtonCell
            buttonCell.Style = cellStyle;
            row.Cells[5] = buttonCell;
            dataGridView2.Rows.Add(row);
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            dataGridView2.ClearSelection();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            string name1 = textBox7.Text;
            string name2 = textBox6.Text;
            int connection = int.Parse((textBox8).Text);

            row.CreateCells(dataGridView3);
            row.Cells[0].Value = name1;
            row.Cells[1].Value = name2;
            row.Cells[2].Value = connection;
            DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();
            buttonCell.Value = "✕";
            buttonCell.Tag = row;
            // 創建一個CellStyle並設置文字置中
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cellStyle.Font = new Font("Arial", 15);
            // 將CellStyle設置給DataGridViewButtonCell
            buttonCell.Style = cellStyle;
            row.Cells[3] = buttonCell;
            dataGridView3.Rows.Add(row);
            textBox7.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
            dataGridView3.ClearSelection();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string output = "CHIP " + textBox13.Text  + " "+ textBox12.Text + "\n";
            int softModuleCount = dataGridView1.RowCount - 1;
            output += "SOFTMODULE " + softModuleCount.ToString() + "\n";
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow) // 跳过新行
                {
                    string name = row.Cells[0].Value.ToString();
                    string minArea =  row.Cells[1].Value.ToString();
                    output += name + " " + minArea + "\n";
                }
            }
            int fixedModuleCount = dataGridView2.RowCount - 1;
            output += "FIXEDMODULE " + softModuleCount.ToString() + "\n";
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow) // 跳过新行
                {
                    string name = row.Cells[0].Value.ToString();
                    string x = row.Cells[1].Value.ToString();
                    string y = row.Cells[2].Value.ToString();
                    string w = row.Cells[3].Value.ToString();
                    string h = row.Cells[4].Value.ToString();
                    output += name + " " + x + " " + y + " " + w + " " + h +"\n";
                }
            }
            int ConnectionCount = dataGridView3.RowCount;
            output += "CONNECTION " + ConnectionCount.ToString() + "\n";
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                if (!row.IsNewRow) // 跳过新行
                {
                    string name1 = row.Cells[0].Value.ToString();
                    string name2 = row.Cells[1].Value.ToString();
                    string conn = row.Cells[2].Value.ToString();
                    output += name1 + " " + name2 + " " + conn + "\n";
                }
            }
            output += '\n';
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog1.FileName = "input.txt";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    sw.Write(output);
                }
            }
            this.Close();
        }


        private void Form2_SizeChanged(object sender, EventArgs e)
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
                if (control is Label)
                {
                    Label label = (Label)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    label.Font = new Font(label.Font.FontFamily, newFontSize);
                }
                else if (control is Button)
                {
                    Button button = (Button)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    button.Font = new Font(button.Font.FontFamily, newFontSize);

                }

                // 设置控件的新大小
                control.Size = new Size(newWidth, newHeight);
                control.Location = new Point(newX, newY);
                if (control is Panel)
                {
                    resizeControl(control);
                }
            }
        }
    
        private void resizeControl(Control parentControl)
        {
            // 计算新的宽度和高度比例
            double widthRatio = (double)this.ClientSize.Width / width;
            double heightRatio = (double)this.ClientSize.Height / height;

            // 遍历窗体内的所有控件
            foreach (Control control in parentControl.Controls)
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
                if (control is Label)
                {
                    Label label = (Label)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    label.Font = new Font(label.Font.FontFamily, newFontSize);
                }
                else if (control is Button)
                {
                    Button button = (Button)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    button.Font = new Font(button.Font.FontFamily, newFontSize);

                }
                else if(control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    textBox.Font = new Font(textBox.Font.FontFamily, newFontSize);
                }
                else if(control is DataGridView)
                {
                    DataGridView dataGridView = (DataGridView)control;
                    newFontSize = controlOriginalTextSizes[control] * (float)widthRatio;
                    dataGridView.Font = new Font(dataGridView.Font.FontFamily, newFontSize);
                    // 调整列宽度
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        if (controlOriginalColumnWidths.TryGetValue(column, out int originalWidth))
                        {
                            column.Width = (int)(originalWidth * widthRatio);
                        }
                    }

                    // 调整行高度
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if (controlOriginalRowHeights.TryGetValue(row, out int originalHeight))
                        {
                            row.Height = (int)(originalHeight * heightRatio);
                        }
                    }
                }
                if (control is Panel)
                {
                    resizeControl(control);
                }
                // 设置控件的新大小
                control.Size = new Size(newWidth, newHeight);
                control.Location = new Point(newX, newY);
            }
        }
    }
}
