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
        public Form2()
        {
            InitializeComponent();
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int desiredWidth = (int)(screenWidth * 0.6);
            int desiredHeight = (int)(desiredWidth * 0.7);
            this.Width = desiredWidth;
            this.Height = desiredHeight;
            dataGridView1.CellContentClick += DeleteButton_Click;
            dataGridView2.CellContentClick += DeleteButton_Click;
            dataGridView3.CellContentClick += DeleteButton_Click;

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
    }
}
