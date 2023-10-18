using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FloorplanDesigner
{
    public partial class Form3 : Form
    {
        

        class Chip
        {
            int width;
            int height;
            Point point;
            Rectangle rectangle;
            List<Module> modules;
            public Chip(int width, int height, Point point)
            {
                this.width = width;
                this.height = height;
                modules = new List<Module>();
                this.point = point;
            }

            public Chip()
            {
                width = height = 0;
                point = new Point();
                rectangle = new Rectangle();
                modules = new List<Module>();
            }

            public Chip(Chip other)
            {
                // 將參數對象的屬性複製到新對象中
                this.width = other.width;
                this.height = other.height;
                this.point = other.point;
                this.rectangle = other.rectangle;
                this.modules = new List<Module>();
                for(int i =0;i<other.modules.Count;i++)
                    modules.Add(other.modules[i]);
                //this.modules = new List<Module>(other.modules);
                
            }

            public Chip clone()
            {
                Chip chip = new Chip();
                chip.width = this.width;
                chip.height = this.height;
                chip.point = new Point(this.point.X, this.point.Y);
                chip.rectangle = new Rectangle(this.rectangle.X ,this.rectangle.Y,this.rectangle.Width,this.rectangle.Height);
                return chip;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is Chip))
                {
                    return false;
                }

                Chip other = (Chip)obj;

                if (this.getPoint() == other.getPoint())
                    return true;
                else
                    return false;
            }

            public void setRectangle(int x, int y, int width, int height)
            {
                rectangle = new Rectangle(x, y, width, height);
            }

            public int getWidth()
            {
                return width;
            }

            public int getHeight()
            {
                return height;
            }

            public void addModule(Module module)
            {
                if (!modules.Contains(module))
                    modules.Add(module);
            }

            public void reMoveModule(Module module)
            {
                modules.Remove(module);
            }

            public void removeAllModule()
            {
                modules.Clear();
            }

            //回傳屬於這個chip且enable的modules
            public List<Module> getEnableModules()
            {
                List<Module> m = new List<Module>();
                for (int i = 0; i < modules.Count; i++)
                {
                    if (modules[i].getEnable())
                        m.Add(modules[i]);
                }
                return m;
            }

            public List<Module> getModules()
            {
                return modules;
            }

            public Rectangle getRectangle()
            {
                return rectangle;
            }

            public Point getPoint()
            {
                return point;
            }
        }
        class Module
        {
            private string name;
            protected Boolean enable = true;
            protected Color color;
            protected PointF center;
            protected List<KeyValuePair<Module, int>> connectedModules;
            protected GraphicsPath gp;
            protected List<Chip> chips;
            protected bool enableConnection = false;
            public Module(string name)
            {
                this.name = name;
                chips = new List<Chip>();
                connectedModules = new List<KeyValuePair<Module, int>>();
            }

            public Module(Module other)
            {
                // 將參數對象的屬性複製到新對象中
                this.name = other.name;
                this.enable = other.enable;
                this.color = other.color;
                this.center = other.center;

                // 創建新的connectedModules列表，並複製連接模塊的資訊
                this.connectedModules = new List<KeyValuePair<Module, int>>(other.connectedModules);

                // 創建新的GraphicsPath，並複製內容
                this.gp = (GraphicsPath)other.gp.Clone();

                // 創建新的chips列表，並複製Chip
                this.chips = new List<Chip>();
                for (int i = 0; i < other.chips.Count; i++)
                    this.chips.Add(other.chips[i].clone());
                //this.chips = new List<Chip>(other.chips);
                
            }

            public void setChips(List<Chip>otherChips)
            {
                chips = new List<Chip> (otherChips);
            }

            public Module()
            {
                chips = new List<Chip>();
                connectedModules = new List<KeyValuePair<Module, int>>();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is Module))
                {
                    return false;
                }
                Module module = (Module)obj;
                if (module.getName() == this.getName())
                    return true;
                else
                    return false;
            }

            public void addConnectedModule(Module module, int nets)
            {
                connectedModules.Add(new KeyValuePair<Module, int>(module, nets));
            }
            public string getName()
            {
                return name;
            }

            public List<KeyValuePair<Module, int>> getConnectedModules()
            {
                return connectedModules;
            }

            public PointF getCenter()
            {
                return center;
            }

            public GraphicsPath GetGraphicsPath()
            {
                return gp;
            }

            public void SetEnable(bool enable)
            {
                this.enable = enable;
            }

            public Boolean getEnable()
            {
                return enable;
            }

            public void setColor(Color c)
            {
                color = c;
            }

            public Color getColor()
            {
                return color;
            }

            public void addChip(Chip chip)
            {
                if (!chips.Contains(chip))
                    chips.Add(chip);
            }

            public void reMoveAllChip()
            {
                chips.Clear();
            }

            public void reMoveChip(Chip chip)
            {
                for(int i =0;i<chips.Count;i++)
                {
                    if (chips[i].getPoint().X == chip.getPoint().X && chips[i].getPoint().Y == chip.getPoint().Y)
                        chips.Remove(chips[i]);
                }

                //chips.Remove(chip);
            }
            public List<Chip> getChips()
            {
                return chips;
            }

            public bool getEnableConnection()
            {
                return enableConnection;
            }

            public void setEnableConnection(bool enable)
            {
                enableConnection = enable;
            }
        }
        class Softmodule : Module
        {
            int minArea;
            float area;
            List<Point> corners;
            Rectangle boundingRect;
            double aspectRatio;
            double retangleRatio;

            public Softmodule(string name, int minArea)
            : base(name)
            {
                this.minArea = minArea;
            }

            public void setCorner(List<Point> points)
            {
                corners = new List<Point>(points);
            }

            public void setGraphicsPath(List<List<Chip>> chips, int chipUnitSize)
            {
                gp = new GraphicsPath();
                List<Point> points = new List<Point>();
                for (int i = 0; i < corners.Count; i++)
                {
                    int x = chips[0][0].getRectangle().X + corners[i].X * chipUnitSize;
                    int y = (chips[0][0].getRectangle().Y + chipUnitSize) - corners[i].Y * chipUnitSize;
                    points.Add(new Point(x, y));
                }
                for (int i = 1; i < points.Count; i++)
                    gp.AddLine(points[i - 1], points[i]);
            }

            public List<Point> getCorners()
            {
                return corners;
            }

            public void calculateArea()
            {
                area = chips.Count;
                /*
                //將最後一個座標加入以便計算面積
                corners.Add(corners[0]);

                //初始化面積
                area = 0;

                //利用外積求面積
                for (int i = 0; i < corners.Count - 1; i++)
                {
                    Point p1 = corners[i];
                    Point p2 = corners[i + 1];
                    area += p1.X * p2.Y - p2.X * p1.Y;
                }
                area /= 2;
                area = Math.Abs(area);

                //移除最後一個座標
                corners.RemoveAt(corners.Count - 1);*/
            }

            public void findBoundingRect()
            {
                //當大小為零時
                if (area == 0)
                {
                    center = new PointF(0, 0);
                    aspectRatio = 0;
                    retangleRatio = 0;
                    return;
                }
                Point minPoint = new Point(int.MaxValue, int.MaxValue);
                Point maxPoint = new Point(int.MinValue, int.MinValue);
                //找到左下角和右下角

                for (int i = 0; i < chips.Count; i++)
                {
                    Point point = chips[i].getPoint();
                    if (minPoint.X > point.X)
                        minPoint.X = point.X;
                    if (minPoint.Y > point.Y)
                        minPoint.Y = point.Y;
                    if (maxPoint.X < point.X + 1)
                        maxPoint.X = point.X + 1;
                    if (maxPoint.Y < point.Y + 1)
                        maxPoint.Y = point.Y + 1;
                }

                //建立包含的最小矩形
                boundingRect = new Rectangle(minPoint.X, minPoint.Y, (maxPoint.X - minPoint.X), (maxPoint.Y - minPoint.Y));
                //建立中心點
                center = new PointF((minPoint.X + maxPoint.X) / 2f, (minPoint.Y + maxPoint.Y) / 2f);
                //根據最小矩形找到長寬比
                aspectRatio = Math.Round((float)boundingRect.Height / (float)boundingRect.Width, 2);
                //根據最小矩形的面積跟soft module的面積來建立面積比
                retangleRatio = Math.Round((float)area / ((float)(maxPoint.X - minPoint.X) * (maxPoint.Y - minPoint.Y)), 2);
            }
            public int getMinArea()
            {
                return minArea;
            }

            public float getArea()
            {
                return chips.Count;
            }

            public double getAspectRatio()
            {
                return aspectRatio;
            }

            public double getRectangleRatio()
            {
                return retangleRatio;
            }

            public bool checkMinArea()
            {
                if (area >= minArea)
                    return true;
                else
                    return false;
            }

            public bool checkAspectRatio()
            {
                if (aspectRatio <= 2 && aspectRatio >= 0.5)
                    return true;
                else
                    return false;
            }

            public bool checkRectangleRatio()
            {
                if (retangleRatio <= 1 && retangleRatio >= 0.8)
                    return true;
                else
                    return false;
            }
        }

        class Fixedmodule : Module
        {
            Point point;
            int width;
            int height;

            public Fixedmodule(string name, Point point, int width, int height)
            : base(name)
            {
                this.point = point;
                this.width = width;
                this.height = height;
                this.color = Color.Black;
                center = new PointF(point.X + (float)width / 2, point.Y + (float)height / 2);
            }

            public void setGraphicsPath(List<List<Chip>> chips, int chipUnitSize)
            {
                gp = new GraphicsPath();
                int x = chips[0][0].getRectangle().X + point.X * chipUnitSize;
                // units[0][0].Y為其左上角座標，而(0,0)為最左下角的座標，故要再加上一個chipUnitSize
                // 且rectangle的起始座標是左上角，因此要再扣掉其高度，因為座標是右下角
                int y = (chips[0][0].getRectangle().Y + chipUnitSize) - (point.Y + getHeight()) * chipUnitSize;
                Rectangle rectangle = new Rectangle(x, y, getWidth() * chipUnitSize, getHeight() * chipUnitSize);
                gp.AddRectangle(rectangle);
            }

            public int getWidth()
            {
                return width;
            }

            public int getHeight()
            {
                return height;
            }

            public Point getPoint()
            {
                return point;
            }
        }

        //全域變數
        private List<List<Chip>> chips;
        private List<Module> modules;
        private List<Softmodule> softmodules;
        private List<Fixedmodule> fixedmodules;
        private int chipUnitSize;
        private float HPWL;
        private double total;
        private string inputFilePath;
        private string outputFilePath;
        private int width,height;
        private Stack<(List<List<List<string>>> ChipsContainModuleNames, List<List<Point>> ModulesContainChipsPoints)> undoStack;
        private Stack<(List<List<List<string>>> ChipsContainModuleNames, List<List<Point>> ModulesContainChipsPoints)> redoStack;
        bool isModify = false;

        //編輯module變數
        private Point mouseDownLocation; // 滑鼠點擊的位置
        private Point mouseMoveLocation;
        private Rectangle currentRectangle; // 目前顯示的矩形
        private Module currentModule;
        private bool middleClick = false;

        public Form3()
        {
            InitializeComponent();
            readFile();
            this.Resize += formResize;
            //儲存檔案ToolStripMenuItem.Enabled = false;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows[0].Selected = false;
            dataGridView2.Rows[0].Selected = false;
        }


        //--------------------------------------
        //讀檔

        public void readFile()
        {
            isModify = false;
            undoStack = new Stack<(List<List<List<string>>> , List<List<Point>>)>();
            redoStack = new Stack<(List<List<List<string>>>, List<List<Point>>)>();
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            //讀取輸入檔
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                softmodules = new List<Softmodule>();
                fixedmodules = new List<Fixedmodule>();
                using (StreamReader reader = new StreamReader(openFileDialog1.FileName))
                {
                    inputFilePath = openFileDialog1.FileName;
                    if(!readInput(reader)) return;
                }
            }
            else
                return;

            //建立List of module
            modules = new List<Module>(softmodules);
            modules.AddRange(fixedmodules);
            toolStripDropDownButton2.Enabled = true;
            //建立image
            Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
            //設定module
            setModule();
            setChip();
            for (int i = 0; i < softmodules.Count; i++)
            {
                softmodules[i].calculateArea();
                softmodules[i].findBoundingRect();
            }

            draw();
            displaySoftModuleInfo();
            displayConnectionInfo();
            displayErrorMessage();
            currentModule = null;
            panel2.BackColor = Color.Transparent;
        }
        private void 開啟設定檔案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isModify)
            {
                DialogResult result = MessageBox.Show("您有未儲存的變更，是否要儲存？", "提醒", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    if (outputFilePath != null)
                        儲存檔案ToolStripMenuItem_Click(sender, e);
                    else
                        另存布局檔案ToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    // 取消
                    return;
                }
            }
            readFile();
        }

        public void readTempFile(string filePath)
        {
            isModify = false;
            undoStack = new Stack<(List<List<List<string>>>, List<List<Point>>)>();
            redoStack = new Stack<(List<List<List<string>>>, List<List<Point>>)>();
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            //讀取輸入檔
            using (StreamReader reader = new StreamReader(filePath))
            {
                if(!readOutput(reader)) return;
            }
            resetChipList();
            //建立List of module
            modules = new List<Module>(softmodules);
            modules.AddRange(fixedmodules);
            toolStripDropDownButton2.Enabled = true;
            //建立image
            Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
            //設定module
            setModule();
            setChip();
            for (int i = 0; i < softmodules.Count; i++)
            {
                softmodules[i].calculateArea();
                softmodules[i].findBoundingRect();
            }

            draw();
            displaySoftModuleInfo();
            displayConnectionInfo();
            displayErrorMessage();
            currentModule = null;
            panel2.BackColor = Color.Transparent;
        }

        private void 匯入檔案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isModify)
            {
                DialogResult result = MessageBox.Show("您有未儲存的變更，是否要儲存？", "提醒", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    if (outputFilePath != null)
                        儲存檔案ToolStripMenuItem_Click(sender, e);
                    else
                        另存布局檔案ToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    // 取消
                    return;
                }
            }
            isModify = false;
            undoStack = new Stack<(List<List<List<string>>>, List<List<Point>>)>();
            redoStack = new Stack<(List<List<List<string>>>, List<List<Point>>)>();
            toolStripButton2.Enabled = false;
            toolStripButton3.Enabled = false;
            //讀取輸入檔
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader reader = new StreamReader(openFileDialog1.FileName))
                {
                    outputFilePath = openFileDialog1.FileName;
                    if (!readOutput(reader)) return;
                }
            }
            else
                return;
            resetChipList();
            //建立List of module
            modules = new List<Module>(softmodules);
            modules.AddRange(fixedmodules);
            toolStripDropDownButton2.Enabled = true;
            //建立image
            Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
            //設定module
            setModule();
            setChip();
            for (int i = 0; i < softmodules.Count; i++)
            {
                softmodules[i].calculateArea();
                softmodules[i].findBoundingRect();
            }

            draw();
            displaySoftModuleInfo();
            displayConnectionInfo();
            displayErrorMessage();
            currentModule = null;
            儲存檔案ToolStripMenuItem.Enabled = true;
            panel2.BackColor = Color.Transparent;
        }

        

        //讀取輸入檔
        private bool readInput(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "CHIP":
                        width = int.Parse(tokens[1]);
                        height = int.Parse(tokens[2]);
                        chipUnitSize = pictureBox1.Height / Math.Max(height + 1, width + 1);
                        if(width >50 || height > 50)
                        {
                            MessageBox.Show("設定檔格式寬高超過限制", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        createChipList(width, height);
                        break;
                    case "SOFTMODULE":
                        int softModuleCount = int.Parse(tokens[1]);
                        readInputSoftModule(reader, softModuleCount);
                        break;
                    case "FIXEDMODULE":
                        int fixedModuleCount = int.Parse(tokens[1]);
                        readInputFixedModule(reader, fixedModuleCount);
                        break;
                    case "CONNECTION":
                        int numConnection = int.Parse(tokens[1]);
                        readInputConnected(reader, numConnection);
                        break;
                    default:
                        MessageBox.Show("設定檔格式錯誤", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                }
            }
            return true;
        }

        //讀取輸出檔
        private bool readOutput(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "HPWL":
                        HPWL = float.Parse(tokens[1]);
                        break;
                    case "SOFTMODULE":
                        int softModuleCount = int.Parse(tokens[1]);
                        if (softModuleCount != softmodules.Count || !readOutputSoftModule(reader, softModuleCount))
                        {
                            MessageBox.Show("設定檔與佈局檔不相符!!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        break;
                    default:
                        MessageBox.Show("佈局檔格式錯誤!!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                }
            }
            return true;
        }

        //根據讀入的input檔來建立soft module的List
        private void readInputSoftModule(StreamReader reader, int softModuleCount)
        {
            for (int i = 0; i < softModuleCount; i++)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split(' ');
                Softmodule softmodule = new Softmodule(tokens[0], int.Parse(tokens[1]));
                List<Point> corners = new List<Point>();
                softmodule.setCorner(corners);
                softmodules.Add(softmodule);
            }
        }

        //根據讀入的input來建立fixed module的List
        private void readInputFixedModule(StreamReader reader, int fixedModuleCount)
        {
            for (int i = 0; i < fixedModuleCount; i++)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split(' ');
                Point point = new Point(int.Parse(tokens[1]), int.Parse(tokens[2]));
                Fixedmodule fixedmodule = new Fixedmodule(tokens[0], point, int.Parse(tokens[3]), int.Parse(tokens[4]));
                fixedmodules.Add(fixedmodule);
            }
        }

        //根據讀入的input來建立module之間的連線數量
        private void readInputConnected(StreamReader reader, int numConnection)
        {
            for (int i = 0; i < numConnection; i++)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split(' ');
                Module module = findModuleByName(tokens[0]);
                module.addConnectedModule(findModuleByName(tokens[1]), int.Parse(tokens[2]));
            }
        }

        //根據讀入的input，來建立soft module的List中的conrner的位置
        private bool readOutputSoftModule(StreamReader reader, int softModuleCount)
        {
            for (int i = 0; i < softModuleCount; i++)
            {
                string line = reader.ReadLine();
                string[] tokens = line.Split(' ');
                Softmodule softmodule = (Softmodule)findModuleByName(tokens[0]);
                if (softmodule == null)
                    return false;
                int numberOfCorners = int.Parse(tokens[1]);
                List<Point> corners = new List<Point>();
                for (int j = 0; j < numberOfCorners; j++)
                {
                    line = reader.ReadLine();
                    tokens = line.Split(' ');
                    Point point = new Point(int.Parse(tokens[0]), int.Parse(tokens[1]));
                    corners.Add(point);
                }
                softmodule.setEnableConnection(false);
                softmodule.SetEnable(true);
                softmodule.setCorner(corners);

            }
            return true;
        }

        //根據module的名稱回傳對應的module
        private Module findModuleByName(string name)
        {
            for (int i = 0; i < fixedmodules.Count; i++)
            {
                if (fixedmodules[i].getName() == name)
                    return fixedmodules[i];
            }
            for (int i = 0; i < softmodules.Count; i++)
            {
                if (softmodules[i].getName() == name)
                    return softmodules[i];
            }
            return null;
        }
        //讀檔
        //-----------------------------------

        //-----------------------------------------
        //設定Moudle
        
        private void formResize(object sender, EventArgs e)
        {
            resizeChipUnitSize(chips);

            Stack<(List<List<List<string>>> ChipsContainModuleNames, List<List<Point>> ModulesContainChipsPoints)> TempUndoStack =
                new Stack<(List<List<List<string>>>, List<List<Point>>)>(undoStack);
            Stack<(List<List<List<string>>> ChipsContainModuleNames, List<List<Point>> ModulesContainChipsPoints)> TempedoStack = 
                new Stack<(List<List<List<string>>>, List<List<Point>>)>(redoStack);
            
            draw();
        }

        private void resizeChipUnitSize(List<List<Chip>> chips)
        {
            if(pictureBox1.Width > 0 && pictureBox1.Height > 0)
            {
                textBox1.Width = pictureBox1.Width;
                chipUnitSize = pictureBox1.Height / Math.Min(height + 2, width + 2);
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Chip chip = chips[i][j];
                        int x = (i) * chipUnitSize + (pictureBox1.Width - chipUnitSize * width) / 2;
                        int y = (height - j) * chipUnitSize;
                        chip.setRectangle(x, y, chipUnitSize, chipUnitSize);
                    }
                }
                Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = img;
            }
            
        }

        //根據傳入的數值回傳對應數量元素的List of color 
        private List<Color> generateColor(int n)
        {
            List<Color> rainbowColors = new List<Color>() {
            Color.Yellow, Color.Green, Color.DeepSkyBlue,
            Color.Indigo, Color.Violet, Color.Pink, Color.CornflowerBlue, Color.DarkOrchid };
            List<Color> colors = new List<Color>();
            Random rand = new Random(1234);
            for (int i = 0; i < n; i++)
            {
                if (i < rainbowColors.Count)
                {
                    colors.Add(rainbowColors[i]);
                }
                else
                {
                    Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                    while (colors.Contains(color))
                    {
                        color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                    }
                    colors.Add(color);
                }
            }
            return colors;
        }
        //reset List of chip
        private void resetChipList()
        {
            for(int i =0;i<chips.Count;i++)
            {
                List<Chip> temp = new List<Chip>(chips.Count);
                for (int j = 0; j < chips[i].Count;j++)
                {
                    Chip chip = new Chip(chipUnitSize, chipUnitSize, new Point(i, j));
                    int x = (i) * chipUnitSize + (pictureBox1.Width - chipUnitSize * width) / 2;
                    int y = (height - j) * chipUnitSize;
                    chip.setRectangle(x, y, chipUnitSize, chipUnitSize);
                    temp.Add(chip);
                }
                chips[i] = temp;
            }
        }


        //建立List of chip
        private void createChipList(int width, int height)
        {
            chips = new List<List<Chip>>();
            for (int i = 0; i < width; i++)
            {
                List<Chip> temp = new List<Chip>(width);
                for (int j = 0; j < height; j++)
                {
                    Chip chip = new Chip(chipUnitSize, chipUnitSize, new Point(i, j));
                    int x = (i) * chipUnitSize + (pictureBox1.Width - chipUnitSize * width) / 2;
                    int y = (height - j) * chipUnitSize;
                    chip.setRectangle(x, y, chipUnitSize, chipUnitSize);
                    temp.Add(chip);
                }
                chips.Add(temp);
            }
           
        }

        //設定下拉式選單、module顏色、module的graphicspath
        private void setModule()
        {
            toolStripDropDownButton2.DropDownItems.Clear();
            ToolStripMenuItem item1 = new ToolStripMenuItem("取消全選");
            item1.Click += new EventHandler(enableAllModule);
            toolStripDropDownButton2.DropDownItems.Add(item1);
            //建立modules的下拉式選單並綁定Event
            for (int i = 0; i < modules.Count; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(modules[i].getName());
                item.CheckOnClick = true;
                item.Checked = true;
                item.CheckedChanged += new EventHandler(enableModule);
                toolStripDropDownButton2.DropDownItems.Add(item);
            }

            List<Color> colors = generateColor(modules.Count);
            //設定modules的顏色
            for (int i = 0; i < softmodules.Count; i++)
            {
                softmodules[i].setColor(colors[i]);
            }

            for (int i = 0; i < fixedmodules.Count; i++)
            {
                fixedmodules[i].setColor(Color.Black);
            }

            //設定module的GraphicsPath
            for (int i = 0; i < softmodules.Count; i++)
                softmodules[i].setGraphicsPath(chips, chipUnitSize);
            for (int i = 0; i < fixedmodules.Count; i++)
                fixedmodules[i].setGraphicsPath(chips, chipUnitSize);

            //設定module的combo box
            comboBox1.Items.Clear();
            for (int i = 0; i < softmodules.Count; i++)
                comboBox1.Items.Add(softmodules[i].getName());
            comboBox1.SelectedIndexChanged += editModule;


            //建立modules的下拉式選單並綁定Event
            toolStripDropDownButton3.DropDownItems.Clear();
            item1 = new ToolStripMenuItem("全選");
            item1.Click += new EventHandler(enableAllConnection);
            toolStripDropDownButton3.DropDownItems.Add(item1);
            for (int i = 0; i < modules.Count; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(modules[i].getName());
                item.CheckOnClick = true;
                item.Checked = false;
                item.CheckedChanged += new EventHandler(enableConnection);
                toolStripDropDownButton3.DropDownItems.Add(item);
            }
        }

        private void enableConnection(object sender, EventArgs e)
        {
            // 取得觸發事件的 ToolStripMenuItem
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            string name = item.Text;
            Module module = findModuleByName(name);

            // 根據 Checked 狀態設定
            if (item.Checked)
            {
                module.setEnableConnection(true);
            }
            else
            {
                module.setEnableConnection(false);
            }
            draw();
        }


        private void enableAllConnection(object sender, EventArgs e)
        {
            // 取得觸發事件的 ToolStripMenuItem
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;

            if(clickedItem.Text == "全選")
            {
                // 迭代所有的下拉菜單項目，除了點選的那個
                foreach (ToolStripMenuItem item in toolStripDropDownButton3.DropDownItems)
                {
                    if (item != clickedItem)
                    {
                        item.Checked = true; // 設置其他項目為未選中
                    }
                }
                clickedItem.Text = "取消全選";
            }
            else
            {
                // 迭代所有的下拉菜單項目，除了點選的那個
                foreach (ToolStripMenuItem item in toolStripDropDownButton3.DropDownItems)
                {
                    if (item != clickedItem)
                    {
                        item.Checked = false; // 設置其他項目為未選中
                    }
                }
                clickedItem.Text = "全選";
            }
        }

        private void disableAllConnection(object sender, EventArgs e)
        {
            // 取得觸發事件的 ToolStripMenuItem
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            for (int i = 1; i < item.DropDownItems.Count; i++)
            {
                ToolStripMenuItem it = (ToolStripMenuItem)item.DropDownItems[i];
                it.Checked = false;
            }
        }

        //選擇編輯module所觸發的事件
        private void editModule(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string moduleName = (String)comboBox.SelectedItem;
            currentModule = (Softmodule)findModuleByName(moduleName);
            if (currentModule != null)
                panel2.BackColor = currentModule.getColor();
            else
                panel2.BackColor = Color.Transparent;
        }

        //啟用所有module所觸發事件
        private void enableAllModule(object sender, EventArgs e)
        {
            // 取得觸發事件的 ToolStripMenuItem
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;

            if (clickedItem.Text == "全選")
            {
                // 迭代所有的下拉菜單項目，除了點選的那個
                foreach (ToolStripMenuItem item in toolStripDropDownButton2.DropDownItems)
                {
                    if (item != clickedItem)
                    {
                        item.Checked = true; // 設置其他項目為未選中
                    }
                }
                clickedItem.Text = "取消全選";
            }
            else
            {
                // 迭代所有的下拉菜單項目，除了點選的那個
                foreach (ToolStripMenuItem item in toolStripDropDownButton2.DropDownItems)
                {
                    if (item != clickedItem)
                    {
                        item.Checked = false; // 設置其他項目為未選中
                    }
                }
                clickedItem.Text = "全選";
            }
        }

        //啟用module所觸發事件
        private void enableModule(object sender, EventArgs e)
        {
            // 取得觸發事件的 ToolStripMenuItem
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            string name = item.Text;
            Module module = findModuleByName(name);

            // 根據 Checked 狀態設定
            if (item.Checked)
            {
                module.SetEnable(true);
            }
            else
            {
                module.SetEnable(false);
            }
            draw();
        }

        //設定chip所包含的module
        private void setChip()
        {
            for (int i = 0; i < modules.Count; i++)
                modules[i].reMoveAllChip();

            Graphics g = Graphics.FromImage(pictureBox1.Image);
            for (int i = 0; i < modules.Count; i++)
            {
                GraphicsPath gp1 = modules[i].GetGraphicsPath();
                Region r1 = new Region(gp1);
                for (int j = 0; j < chips.Count; j++)
                {
                    for (int k = 0; k < chips[j].Count; k++)
                    {
                        Region r2 = r1.Clone();
                        GraphicsPath gp2 = new GraphicsPath();
                        gp2.AddRectangle(chips[j][k].getRectangle());
                        Region r3 = new Region(gp2);
                        r2.Intersect(r3);
                        if (!r2.IsEmpty(g))
                        {
                            chips[j][k].addModule(modules[i]);
                            modules[i].addChip(chips[j][k].clone());
                        }
                    }
                }
            }
        }

        //設定Moudle
        //------------------------------------------

        //------------------------------------------
        //繪製module

        private void draw()
        {
            resizeChipUnitSize(chips);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            drawSoftModule(g);
            drawChip(g);
            drawFixedModule(g);
            //drawSoftModuleColor(g);
            drawModuleCenter(g);
            drawConnections(g);
            儲存圖片ToolStripMenuItem.Enabled = true;
            pictureBox1.Refresh();
        }
        //畫出網格
        private void drawChip(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 1);
            for (int i = 0; i < chips.Count; i++)
            {
                for (int j = 0; j < chips[i].Count; j++)
                {
                    g.DrawRectangle(pen, chips[i][j].getRectangle());
                }
            }
            //pictureBox1.Refresh();
        }

        //畫出Fixed Module
        private void drawFixedModule(Graphics g)
        {
            StringFormat stringFormat = new StringFormat();
            // 設定文字置中
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < fixedmodules.Count; i++)
            {
                if (fixedmodules[i].getEnable())
                {
                    Point point = fixedmodules[i].getPoint();
                    int x = chips[0][0].getRectangle().X + point.X * chipUnitSize;
                    // units[0][0].Y為其左上角座標，而(0,0)為最左下角的座標，故要再加上一個chipUnitSize
                    // 且rectangle的起始座標是左上角，因此要再扣掉其高度，因為座標是右下角
                    int y = (chips[0][0].getRectangle().Y + chipUnitSize) - (point.Y + fixedmodules[i].getHeight()) * chipUnitSize;
                    Rectangle rectangle = new Rectangle(x, y, fixedmodules[i].getWidth() * chipUnitSize, fixedmodules[i].getHeight() * chipUnitSize);

                    // 計算字體大小，讓fixed module名字能完整顯示在rectangle內
                    Font font = new Font("Arial", 1, FontStyle.Bold);
                    while (g.MeasureString(fixedmodules[i].getName(), font).Width < rectangle.Width && g.MeasureString(fixedmodules[i].getName(), font).Height < rectangle.Height)
                    {
                        font = new Font("Arial", font.Size + 1, FontStyle.Bold);
                    }
                    font = new Font("Arial", font.Size - 1, FontStyle.Bold);

                    //g.FillRectangle(new SolidBrush(Color.Black), rectangle);
                    g.DrawString(fixedmodules[i].getName(), font, new SolidBrush(Color.White), rectangle, stringFormat);
                }
            }
            //pictureBox1.Refresh();
        }

        //畫出softmodule
        private void drawSoftModule(Graphics g)
        {
            for (int i = 0; i < chips.Count; i++)
            {
                for (int j = 0; j < chips[i].Count; j++)
                {
                    List<Module> modules = chips[i][j].getEnableModules();
                    Color color;

                    if (modules.Count == 1)
                    {
                        color = modules[0].getColor();
                        g.FillRectangle(new SolidBrush(color), chips[i][j].getRectangle());
                    }
                    else if (modules.Count > 1)
                    {
                        for (int k = 0; k < modules.Count; k++)
                        {
                            color = modules[k].getColor();
                            color = Color.FromArgb(128, color.R, color.G, color.B);
                            Brush brush = new SolidBrush(color);
                            g.FillRectangle(brush, chips[i][j].getRectangle());
                        }
                    }
                }
            }
            //pictureBox1.Refresh();
        }

        //畫出module的顏色資訊
        private void drawSoftModuleColor(Graphics g)
        {
            Font titleFont = new Font("Arial", chipUnitSize / 3, FontStyle.Bold);
            Font textFont = new Font("Arial", chipUnitSize / 4);
            Rectangle rectangle = new Rectangle();
            rectangle.Width = rectangle.Height = chipUnitSize / 3;
            int x = chipUnitSize * (chips.Count + 2);
            int y = chipUnitSize * 1;

            x -= chipUnitSize / 2;
            g.DrawString("Soft Module:", titleFont, Brushes.Black, x, y);
            y += (chipUnitSize / 4);

            x += chipUnitSize / 2;
            for (int i = 0; i < softmodules.Count; i++)
            {
                y += chipUnitSize / 2;
                rectangle.X = x;
                rectangle.Y = y;
                g.FillRectangle(new SolidBrush(softmodules[i].getColor()), rectangle);
                g.DrawRectangle(new Pen(Color.Black, 2), rectangle);
                g.DrawString(softmodules[i].getName(), textFont, Brushes.Black, x + chipUnitSize / 2, y);
            }
            pictureBox1.Refresh();
        }

        //畫出module質心
        private void drawModuleCenter(Graphics g)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].getEnable())
                {
                    PointF point = modules[i].getCenter();
                    if (point.X == 0 && point.Y == 0)
                        continue;
                    float centerX = chips[0][0].getRectangle().X + point.X * chipUnitSize;
                    float centerY = (chips[0][0].getRectangle().Y + chipUnitSize) - point.Y * chipUnitSize;
                    RectangleF rect = new RectangleF(centerX - chipUnitSize / 8, centerY - chipUnitSize / 8, chipUnitSize / 4, chipUnitSize / 4);
                    Color color = modules[i].getColor();
                    g.DrawEllipse(new Pen(Color.Black, 5), rect);
                    if (modules[i].getColor() == Color.Black)
                        g.FillEllipse(new SolidBrush(Color.White), rect);
                    else
                        g.FillEllipse(new SolidBrush(modules[i].getColor()), rect);
                }
            }
            //pictureBox1.Refresh();
        }

        
        //繪製連線情況
        private void drawConnections(Graphics g)
        {
            int minConnection = int.MaxValue;
            int maxConnection = int.MinValue;

            // 尋找最小和最大連線數量
            for (int i = 0; i < modules.Count; i++)
            {
                List<KeyValuePair<Module, int>> keys = modules[i].getConnectedModules();
                for (int j = 0; j < keys.Count; j++)
                {
                    int connectionCount = keys[j].Value;
                    if (connectionCount < minConnection)
                        minConnection = connectionCount;
                    if (connectionCount > maxConnection)
                        maxConnection = connectionCount;
                }
            }

            //降低線段鋸齒狀情況
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int bottomLeftX = chips[0][0].getRectangle().X;
            int bottomLeftY = chips[0][0].getRectangle().Y + chips[0][0].getRectangle().Height;
            for (int i = 0; i < modules.Count; i++)
            {
                if (!modules[i].getEnableConnection() || modules[i].getChips().Count == 0)
                    continue;
                List<KeyValuePair<Module, int>> keys = modules[i].getConnectedModules();
                for (int j = 0; j < keys.Count; j++)
                {
                    if (!keys[j].Key.getEnableConnection() || keys[j].Key.getChips().Count == 0)
                        continue;
                    int connectionCount = keys[j].Value;

                    // 根據連線數量取得漸變顏色
                    Color lineColor = GetGradientColor(connectionCount, minConnection, maxConnection);

                    Pen pen = new Pen(lineColor, 3);
                    PointF[] points = new PointF[2];
                    points[0] = new PointF(bottomLeftX + modules[i].getCenter().X * chipUnitSize, bottomLeftY - modules[i].getCenter().Y * chipUnitSize);
                    points[1] = new PointF(bottomLeftX + keys[j].Key.getCenter().X * chipUnitSize, bottomLeftY - keys[j].Key.getCenter().Y * chipUnitSize);
                    g.DrawCurve(pen, points);
                }
            }
            pictureBox1.Refresh();
        }
        
        // 根據連線數量取得漸變顏色
        private Color GetGradientColor(int value, int minValue, int maxValue)
        {
            // 自訂起始顏色和結束顏色
            Color startColor = Color.Orange;
            Color endColor = Color.Blue;

            // 根據連線數量在起始顏色和結束顏色之間進行插值
            float ratio = (float)(value - minValue + 1) / (maxValue - minValue + 1);
            int red = (int)(startColor.R + (endColor.R - startColor.R) * ratio);
            int green = (int)(startColor.G + (endColor.G - startColor.G) * ratio);
            int blue = (int)(startColor.B + (endColor.B - startColor.B) * ratio);

            return Color.FromArgb(red, green, blue);
        }

        //繪製module
        //------------------------------------------

        //-------------------------------------------
        //顯示module資訊

        //顯示soft module的資訊
        private void displaySoftModuleInfo()
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < softmodules.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = softmodules[i].getName();
                row.Cells[1].Style.BackColor = softmodules[i].getColor();
                row.Cells[2].Value = softmodules[i].getArea();
                row.Cells[3].Value = "(" + softmodules[i].getCenter().X.ToString() + ", "
                    + softmodules[i].getCenter().Y.ToString() + ")";
                row.Cells[4].Value = softmodules[i].getAspectRatio();
                row.Cells[5].Value = softmodules[i].getRectangleRatio();
                dataGridView1.Rows.Add(row);

                if (!softmodules[i].checkMinArea())
                {
                    dataGridView1.Rows[i].Cells[1].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold); ;
                    dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                }
                if (!softmodules[i].checkAspectRatio())
                {
                    dataGridView1.Rows[i].Cells[3].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold); ;
                    dataGridView1.Rows[i].Cells[3].Style.ForeColor = Color.Red;
                }
                if (!softmodules[i].checkRectangleRatio())
                {
                    dataGridView1.Rows[i].Cells[4].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold); ;
                    dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                }
            }
        }

        //顯示連線情形
        private void displayConnectionInfo()
        {
            total = 0;
            int numRow = 0;
            dataGridView2.Rows.Clear();
            for (int i = 0; i < modules.Count; i++)
            {
                List<KeyValuePair<Module, int>> keys = modules[i].getConnectedModules();
                for (int j = 0; j < keys.Count; j++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    double distance = Math.Round(Math.Abs(modules[i].getCenter().X - keys[j].Key.getCenter().X) + Math.Abs(modules[i].getCenter().Y - keys[j].Key.getCenter().Y), 1);
                    row.CreateCells(dataGridView2);
                    numRow++;
                    for (int k = 0; k < 4; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                row.Cells[0].Value = modules[i].getName() + "\r\n" + keys[j].Key.getName();
                                break;
                            case 1:
                                row.Cells[1].Value = keys[j].Value;
                                break;
                            case 2:
                                row.Cells[2].Value = distance;
                                break;
                            case 3:
                                double value = distance * keys[j].Value;
                                total += value;
                                row.Cells[3].Value = value;
                                break;
                        }
                    }
                    dataGridView2.Rows.Add(row);
                }
            }

            DataGridViewRow lastRow = new DataGridViewRow();
            lastRow.CreateCells(dataGridView2);
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                    lastRow.Cells[i].Value = "總和";
                else if (i == 3)
                    lastRow.Cells[i].Value = total;
                else
                    lastRow.Cells[i].Value = "";
            }
            dataGridView2.Rows.Add(lastRow);
            
            if(HPWL !=0)
            {
                if (total > HPWL)
                {
                    dataGridView2.Rows[numRow].Cells[3].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold); ;
                    dataGridView2.Rows[numRow].Cells[3].Style.ForeColor = Color.Red;
                }
                else if (total < HPWL)
                {
                    dataGridView2.Rows[numRow].Cells[3].Style.Font = new Font(dataGridView1.Font, FontStyle.Bold); ;
                    dataGridView2.Rows[numRow].Cells[3].Style.ForeColor = Color.Green;
                }
            }
        }

        //顯示錯誤資訊
        private void displayErrorMessage()
        {
            textBox1.Visible = false;
            儲存錯誤ToolStripMenuItem.Enabled = false;
            string errorMessage = "錯誤清單:";
            int errorNum = 0;
            for (int i = 0; i < softmodules.Count; i++)
            {
                if (!softmodules[i].checkMinArea())
                    errorMessage += "\r\n" + (++errorNum).ToString() + ". " + softmodules[i].getName() +
                        "的面積為" + softmodules[i].getArea().ToString() + "，小於最小面積"
                        + softmodules[i].getMinArea();

                if (!softmodules[i].checkAspectRatio())
                    errorMessage += "\r\n" + (++errorNum).ToString() + ". " + softmodules[i].getName() +
                        "的長寬比例為" + softmodules[i].getAspectRatio().ToString() + "，不介於0.5-2之間";

                if (!softmodules[i].checkRectangleRatio())
                    errorMessage += "\r\n" + (++errorNum).ToString() + ". " + softmodules[i].getName() +
                        "的矩形比例為" + (softmodules[i].getRectangleRatio() * 100).ToString() + "%，不介於80%-100%之間";
            }

            for (int i = 0; i < chips.Count; i++)
            {
                for (int j = 0; j < chips[i].Count; j++)
                {
                    if (chips[i][j].getModules().Count > 1)
                    {
                        errorMessage += "\r\n" + (++errorNum).ToString() + ". ";
                        for (int k = 0; k < chips[i][j].getModules().Count; k++)
                        {
                            if (k != chips[i][j].getModules().Count - 1)
                            {
                                errorMessage += chips[i][j].getModules()[k].getName() + "、";
                            }
                            else
                                errorMessage += chips[i][j].getModules()[k].getName() + "重疊於(" + i.ToString() + ", " + j.ToString() + ")";
                        }
                    }
                }
            }

            /*
            if (total != HPWL)
                errorMessage += "\r\n" + (++errorNum).ToString() + ". 半周長導線為" + HPWL.ToString() +
                    "正確應該為" + total.ToString();
            */

            if (errorNum != 0)
            {
                儲存錯誤ToolStripMenuItem.Enabled = true;
                textBox1.Visible = true;
                textBox1.Text = errorMessage;
                if(pictureBox1.Location.Y + pictureBox1.Height > textBox1.Location.Y)
                {
                    pictureBox1.Height -= textBox1.Height;
                    Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    pictureBox1.Image = img;
                    draw();
                }
            }
            else
            {
                textBox1.Visible = false;
                if (pictureBox1.Location.Y + pictureBox1.Height < textBox1.Location.Y)
                {
                    pictureBox1.Height += textBox1.Height;
                    Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    pictureBox1.Image = img;
                    draw();
                }
            }
        }
        //顯示module資訊
        //------------------------------------------- 

        //------------------------------------------
        //儲存檔案
        private void 儲存圖片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "PNG file|*.png";
            saveFileDialog1.FileName = "image.png";
            if (pictureBox1.Image != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    String output = saveFileDialog1.FileName;
                    pictureBox1.Image.Save(output);
                }
            }
        }

        private void 儲存錯誤ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog1.FileName = "errorMessage.txt";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    sw.Write(textBox1.Text);
                }
            }
        }

        
        private int findModuleIndex(Module m)
        {
            for(int i=0;i<modules.Count;i++)
            {
                if (modules[i].getName() == m.getName())
                    return i;
            }
            return -1;
        }

        // Find all modules' rectangle range and return List<List<int>> -> [[Xmin, Ymin, Xmax, Ymax], ...]
        private List<List<int>> FindAllRectRange(List<List<int>> table)
        {
            List<List<int>> rangeData = new List<List<int>>();
            for (int m = 0; m < modules.Count; ++m) rangeData.Add(FindRange(m, table));
            return rangeData;
        }

        // Find single module rectangle range
        private List<int> FindRange(int mIndex, List<List<int>> table)
        {
            List<int> data = new List<int>();
            int Xmin = table[0].Count; int Ymin = table.Count;
            int Xmax = 0; int Ymax = 0;

            for (int i = 0; i < table.Count; ++i)
            {
                for (int j = 0; j < table[i].Count; ++j)
                {
                    if (table[i][j] == mIndex)
                    {
                        if (Xmin > j) Xmin = j;
                        if (Ymin > i) Ymin = i;
                        if (Xmax < j) Xmax = j;
                        if (Ymax < i) Ymax = i;
                    }
                }
            }
            data.Add(Xmin); data.Add(Ymin); data.Add(Xmax); data.Add(Ymax);

            return data;
        }


        private void 儲存檔案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i =0;i< softmodules.Count;i++)
            {
                List<Point> points = new List<Point>();
                for (int j = 0; j < softmodules[i].getChips().Count; j++)
                    points.Add(softmodules[i].getChips()[j].getPoint());
                if(HasMultipleAreas(points))
                {
                    MessageBox.Show(softmodules[i].getName() + " 存在兩個以上的區塊錯誤", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (outputFilePath == null)
            {
                另存布局檔案ToolStripMenuItem_Click(sender, e);
                return;
            }
            StreamWriter writeFile = new StreamWriter(outputFilePath);

            writeFile.WriteLine($"HPWL {total}");
            writeFile.WriteLine($"SOFTMODULE {softmodules.Count}");
            List<List<int>> Points = new List<List<int>>();
            for (int i = 0; i < softmodules.Count; ++i)
            {
                List<List<int>> table = new List<List<int>>();
                for (int j = 0; j < chips[0].Count; ++j)
                {
                    List<int> line = new List<int>();
                    for (int k = 0; k < chips.Count; ++k)
                    {
                        if (chips[k][j].getModules().Count == 0)
                            line.Add(-1);
                        else
                        {
                            for (int z = 0; z < chips[k][j].getModules().Count; z++)
                            {
                                if (chips[k][j].getModules()[z].getName() == softmodules[i].getName())
                                {
                                    line.Add(i);
                                    break;
                                }
                                if (z == chips[k][j].getModules().Count - 1)
                                    line.Add(findModuleIndex(chips[k][j].getModules()[0]));
                            }
                        }
                    }
                    table.Add(line); // 使用Add添加行
                }
                List<List<int>> rectangleData = FindAllRectRange(table);

                Points.Clear();
                if (softmodules[i].getChips().Count == 0)
                    writeFile.WriteLine($"{softmodules[i].getName()} {0}");
                else
                {
                    Points = NineBlocksScanning(i, FindRange(i, table), table);
                    writeFile.WriteLine($"{softmodules[i].getName()} {Points.Count}");
                    for (int j = 0; j < Points.Count; ++j)
                        writeFile.WriteLine($"{Points[j][0]} {Points[j][1]}");
                }
            }

            writeFile.Close();
            isModify = false;
        }

        // 判斷是否存在兩個以上的區域
        bool HasMultipleAreas(List<Point> points)
        {
            HashSet<Point> visited = new HashSet<Point>();
            int areaCount = 0;

            foreach (var point in points)
            {
                if (!visited.Contains(point))
                {
                    DepthFirstSearch(points, point, visited);
                    areaCount++;
                }
            }

            return areaCount >= 2;
        }

        // 使用深度優先搜索 (DFS) 來訪問區域中的點
        void DepthFirstSearch(List<Point> points, Point currentPoint, HashSet<Point> visited)
        {
            visited.Add(currentPoint);

            // 定義上、下、左、右四個方向
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newX = currentPoint.X + dx[i];
                int newY = currentPoint.Y + dy[i];
                Point neighbor = new Point(newX, newY);

                if (points.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    DepthFirstSearch(points, neighbor, visited);
                }
            }
        }


        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isModify)
            {
                DialogResult result = MessageBox.Show("您有未儲存的變更，是否要儲存？", "提醒", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    if (outputFilePath != null)
                        儲存檔案ToolStripMenuItem_Click(sender, e);
                    else
                        另存布局檔案ToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    // 取消窗口關閉操作
                    e.Cancel = true;
                }
            }
        }

        private void 另存布局檔案ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < softmodules.Count; i++)
            {
                List<Point> points = new List<Point>();
                for (int j = 0; j < softmodules[i].getChips().Count; j++)
                    points.Add(softmodules[i].getChips()[j].getPoint());
                if (HasMultipleAreas(points))
                {
                    MessageBox.Show(softmodules[i].getName() + " 存在兩個以上的區塊錯誤", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog1.FileName = "output.txt";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "")
                return;
            StreamWriter writeFile = new StreamWriter(saveFileDialog1.FileName);

            writeFile.WriteLine($"HPWL {total}");
            writeFile.WriteLine($"SOFTMODULE {softmodules.Count}");
            List<List<int>> Points = new List<List<int>>();
            for (int i = 0; i < softmodules.Count; ++i)
            {
                List<List<int>> table = new List<List<int>>();
                for (int j = 0; j < chips[0].Count; ++j)
                {
                    List<int> line = new List<int>();
                    for (int k = 0; k < chips.Count; ++k)
                    {
                        if (chips[k][j].getModules().Count == 0)
                            line.Add(-1);
                        else
                        {
                            for(int z = 0;z< chips[k][j].getModules().Count;z++)
                            {
                                if (chips[k][j].getModules()[z].getName() == softmodules[i].getName())
                                {
                                    line.Add(i);
                                    break;
                                }
                                if(z == chips[k][j].getModules().Count - 1)
                                    line.Add(findModuleIndex(chips[k][j].getModules()[0]));
                            }
                        }
                    }
                    table.Add(line); // 使用Add添加行
                }
                List<List<int>> rectangleData = FindAllRectRange(table);

                Points.Clear();
                if (softmodules[i].getChips().Count == 0)
                    writeFile.WriteLine($"{softmodules[i].getName()} {0}");
                else
                {
                    Points = NineBlocksScanning(i, FindRange(i, table), table);
                    writeFile.WriteLine($"{softmodules[i].getName()} {Points.Count}");
                    for (int j = 0; j < Points.Count; ++j)
                        writeFile.WriteLine($"{Points[j][0]} {Points[j][1]}");
                }
            }

            writeFile.Close();
            isModify = false;
        }

        // Find data if it is existed in container
        private bool isExistedList(List<List<int>> container, List<int> target)
        {
            for (int i = 0; i < container.Count; ++i)
            {
                if (container[i][0] == target[0] && container[i][1] == target[1] && container[i][2] == target[2]) return true;
            }
            return false;
        }

        private List<List<int>> NineBlocksScanning(int mIndex, List<int> moduleRect, List<List<int>> table)
        {
            List<List<int>> buffer = new List<List<int>>();
            bool isFirst = true;

            // Scanning
            for (int j = moduleRect[0]; j <= moduleRect[2]; ++j)
            {
                isFirst = true;
                for (int i = moduleRect[1]; i <= moduleRect[3]; ++i)
                {
                    if (table[i][j] == mIndex)
                    {
                        // normal version
                        if (i + 1 == table.Count || table[i + 1][j] != mIndex)
                        {
                            // test p
                            if (j == 0 || table[i][j - 1] != mIndex)
                            {
                                List<int> p = new List<int> { j, i + 1, 0 };
                                if (!isExistedList(buffer, p)) buffer.Add(p);
                            }
                            else
                            {
                                if (i + 1 != table.Count && table[i + 1][j - 1] == mIndex)
                                {
                                    List<int> p = new List<int> { j, i + 1, 1 };
                                    if (!isExistedList(buffer, p)) buffer.Add(p);
                                }
                            }

                            // test q
                            if (j + 1 == table[i].Count || table[i][j + 1] != mIndex)
                            {
                                List<int> q = new List<int> { j + 1, i + 1, 0 };
                                if (!isExistedList(buffer, q)) buffer.Add(q);
                            }
                            else
                            {
                                if (i + 1 != table.Count && table[i + 1][j + 1] == mIndex)
                                {
                                    List<int> q = new List<int> { j + 1, i + 1, 1 };
                                    if (!isExistedList(buffer, q)) buffer.Add(q);
                                }
                            }

                            // test r
                            if (j != 0 && !isFirst)
                            {
                                if (i != 0)
                                {
                                    if ((table[i][j - 1] == mIndex) ^ (table[i - 1][j - 1] == mIndex))
                                    {
                                        List<int> r = new List<int> { j, i, 1 };
                                        if (!isExistedList(buffer, r)) buffer.Add(r);
                                    }
                                }
                                else if (table[i][j - 1] == mIndex)
                                {
                                    List<int> r = new List<int> { j, i, 1 };
                                    if (!isExistedList(buffer, r)) buffer.Add(r);
                                }
                            }

                            // test s
                            if (j + 1 != table[i].Count && !isFirst)
                            {
                                if (i != 0)
                                {
                                    if ((table[i][j + 1] == mIndex) ^ (table[i - 1][j + 1] == mIndex))
                                    {
                                        List<int> s = new List<int> { j + 1, i, 1 };
                                        if (!isExistedList(buffer, s)) buffer.Add(s);
                                    }
                                }
                                else if (table[i][j + 1] == mIndex)
                                {
                                    List<int> s = new List<int> { j + 1, i, 1 };
                                    if (!isExistedList(buffer, s)) buffer.Add(s);
                                }
                            }
                        }

                        // bottom version
                        if (isFirst)
                        {
                            // test bottom-left
                            if (j == 0 || table[i][j - 1] != mIndex)
                            {
                                List<int> p = new List<int> { j, i, 0 };
                                if (!isExistedList(buffer, p)) buffer.Add(p);
                            }
                            else
                            {
                                if (i != 0 && table[i - 1][j - 1] == mIndex)
                                {
                                    List<int> p = new List<int> { j, i, 1 };
                                    if (!isExistedList(buffer, p)) buffer.Add(p);
                                }
                            }

                            // test bottom-right
                            if (j + 1 == table[i].Count || table[i][j + 1] != mIndex)
                            {
                                List<int> q = new List<int> { j + 1, i, 0 };
                                if (!isExistedList(buffer, q)) buffer.Add(q);
                            }
                            else
                            {
                                if (i != 0 && table[i - 1][j + 1] == mIndex)
                                {
                                    List<int> q = new List<int> { j + 1, i, 1 };
                                    if (!isExistedList(buffer, q)) buffer.Add(q);
                                }
                            }

                            isFirst = false;
                        }
                    }

                    if (table[i][j] != mIndex) isFirst = true;
                }
            }

            // Sort all point
            List<List<int>> results = new List<List<int>>();
            List<int> nowPoint = buffer[0];
            List<int> nextPoint = buffer[0];
            int nowDirectionIndex = 0;

            for (int i = 1; i < buffer.Count; ++i)
            {
                if (buffer[i][0] < nowPoint[0]) nowPoint = buffer[i];
                else if (buffer[i][0] == nowPoint[0] && buffer[i][1] < nowPoint[1]) nowPoint = buffer[i];
            }
            results.Add(nowPoint);
            buffer.Remove(nowPoint);

            while (buffer.Count > 0)
            {
                switch (nowDirectionIndex)
                {
                    case 0:
                        int d0 = 2147483647;
                        for (int i = 0; i < buffer.Count; ++i)
                            if (buffer[i][0] == nowPoint[0] && buffer[i][1] > nowPoint[1] && buffer[i][1] - nowPoint[1] < d0)
                            {
                                nextPoint = buffer[i];
                                d0 = buffer[i][1] - nowPoint[1];
                            }
                        break;

                    case 1:
                        int d1 = 2147483647;
                        for (int i = 0; i < buffer.Count; ++i)
                            if (buffer[i][1] == nowPoint[1] && buffer[i][0] > nowPoint[0] && buffer[i][0] - nowPoint[0] < d1)
                            {
                                nextPoint = buffer[i];
                                d1 = buffer[i][0] - nowPoint[0];
                            }
                        break;

                    case 2:
                        int d2 = 2147483647;
                        for (int i = 0; i < buffer.Count; ++i)
                            if (buffer[i][0] == nowPoint[0] && nowPoint[1] > buffer[i][1] && nowPoint[1] - buffer[i][1] < d2)
                            {
                                nextPoint = buffer[i];
                                d2 = nowPoint[1] - buffer[i][1];
                            }
                        break;

                    case 3:
                        int d3 = 2147483647;
                        for (int i = 0; i < buffer.Count; ++i)
                            if (buffer[i][1] == nowPoint[1] && nowPoint[0] > buffer[i][0] && nowPoint[0] - buffer[i][0] < d3)
                            {
                                nextPoint = buffer[i];
                                d3 = nowPoint[0] - buffer[i][0];
                            }
                        break;

                    default:
                        break;
                }

                // Change direction
                if (nextPoint[2] == 1) nowDirectionIndex += 3;
                else ++nowDirectionIndex;
                nowDirectionIndex %= 4;

                // Save point
                if (nextPoint != nowPoint)
                {
                    results.Add(nextPoint);
                    buffer.Remove(nextPoint);
                    nowPoint = nextPoint;
                }

            }

            // Return
            return results;
        }

        //儲存檔案
        //--------------------------------------------

        //--------------------------------------------
        //編輯module

        //滑鼠按下時的事件
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            mouseDownLocation = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                currentRectangle = new Rectangle(e.Location, new Size(0, 0));
            }
            else if (e.Button == MouseButtons.Right)
            {
                currentRectangle = new Rectangle(e.Location, new Size(0, 0));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                middleClick = true;
                for (int i = 0; i < chips.Count; i++)
                {
                    for (int j = 0; j < chips[i].Count; j++)
                    {
                        if (chips[i][j].getRectangle().Contains(mouseDownLocation))
                        {
                            if (chips[i][j].getEnableModules().Count > 1)
                            {
                                //comboBox1.SelectedIndex = -1;
                                //currentModule = null;
                                return;
                            }
                            else if (chips[i][j].getEnableModules().Count == 1)
                            {

                                for (int k = 0; k < softmodules.Count; k++)
                                {
                                    if (softmodules[k] == chips[i][j].getEnableModules()[0])
                                    {
                                        comboBox1.SelectedIndex = k;
                                        return;
                                    }
                                }
                            }
                        }

                    }
                }
                //到此處代表沒有找到
                comboBox1.SelectedIndex = -1;
                currentModule = null;
            }
        }

        //滑鼠移動時的事件
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            mouseMoveLocation = e.Location;
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int x = Math.Min(e.X, mouseDownLocation.X);
                int y = Math.Min(e.Y, mouseDownLocation.Y);
                int width = Math.Abs(e.X - mouseDownLocation.X);
                int height = Math.Abs(e.Y - mouseDownLocation.Y);

                currentRectangle = new Rectangle(x, y, width, height);
                pictureBox1.Invalidate();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                pictureBox1.Invalidate();
            }

        }

        //滑鼠放開時的事件
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            pushStack(undoStack);
            redoStack.Clear();
            toolStripButton2.Enabled = true;
            toolStripButton3.Enabled = false;
            isModify = false;
            List<List<Chip>> originalChips = new List < List < Chip >>();

            for(int i =0;i<chips.Count;i++)
            {
                List<Chip> chipList = new List<Chip>();
                for(int j = 0; j < chips[i].Count;j++)
                {
                    chipList.Add(new Chip(chips[i][j]));
                }
                originalChips.Add(chipList);
            }

            List<Module> originalModule = new List<Module> ();
            for(int i =0;i<modules.Count;i++)
            {
                Module tempModule = new Module(modules[i]);
                originalModule.Add(tempModule); 
            }

            if (e.Button == MouseButtons.Left && currentModule != null)
            {
                for (int i = 0; i < chips.Count; i++)
                {
                    for (int j = 0; j < chips[i].Count; j++)
                    {
                        if (chips[i][j].getRectangle().IntersectsWith(currentRectangle))
                        {
                            chips[i][j].addModule(currentModule);
                            currentModule.addChip(chips[i][j].clone());
                            isModify = true;
                        }
                    }
                }
            }


            if (e.Button == MouseButtons.Right && currentModule != null)
            {
                for (int i = 0; i < chips.Count; i++)
                {
                    for (int j = 0; j < chips[i].Count; j++)
                    {
                        if (chips[i][j].getRectangle().IntersectsWith(currentRectangle))
                        {
                            chips[i][j].reMoveModule(currentModule);
                            currentModule.reMoveChip(chips[i][j]);
                            isModify = true;
                        }
                    }
                }
            }


            if (middleClick == true && currentModule != null)
            {
                middleClick = false;
                moveSoftModule();
                isModify = true;
            }
            /*
            if(isModify)
            {
                pushStack(undoStack);
                redoStack.Clear();
                toolStripButton2.Enabled = true;
                toolStripButton3.Enabled = false;
            }*/

            currentRectangle = new Rectangle(e.Location, new Size(0, 0));
            pictureBox1.Invalidate();
            updateModule();
        }

        private void pushStack(Stack<(List<List<List<string>>> ChipsContainModuleNames, List<List<Point>> ModulesContainChipsPoints)> stack)
        {
            List<List<List<string>>> chipsContainModuleNames = new List<List<List<string>>>();
            List<List<Point>> moduleContainChipPoints = new List<List<Point>>();
            for (int i = 0; i < chips.Count; i++)
            {
                List<List<string>> tempList = new List<List<string>>();
                for (int j = 0; j < chips[i].Count; j++)
                {
                    List<string> nameList = new List<string>();
                    for (int k = 0; k < chips[i][j].getModules().Count; k++)
                        nameList.Add(chips[i][j].getModules()[k].getName());
                    tempList.Add(nameList);
                }
                chipsContainModuleNames.Add(tempList);
            }

            for(int i =0;i<modules.Count;i++)
            {
                List<Point> pointList = new List<Point>();
                for (int j = 0; j < modules[i].getChips().Count; j++)
                    pointList.Add(modules[i].getChips()[j].getPoint());
                moduleContainChipPoints.Add(pointList);
            }
            var currentState = (chipsContainModuleNames, moduleContainChipPoints);
            stack.Push(currentState);
        }

        private void popStack(Stack<(List<List<List<string>>> ChipsContainModuleNames, List<List<Point>> ModulesContainChipsPoints)> stack)
        {
            var currentState = stack.Pop();
            List<List<List<string>>> chipsContainModuleNames = currentState.ChipsContainModuleNames;
            List<List<Point>> moduleContainChipPoints = currentState.ModulesContainChipsPoints;
            for (int i = 0; i < chipsContainModuleNames.Count; i++)
            {
                for (int j = 0; j < chipsContainModuleNames[i].Count; j++)
                {
                    chips[i][j].removeAllModule();
                    for (int k = 0; k < chipsContainModuleNames[i][j].Count; k++)
                        chips[i][j].addModule(findModuleByName(chipsContainModuleNames[i][j][k]));
                }
            }
            
            for (int i = 0; i < moduleContainChipPoints.Count; i++)
            {
                modules[i].reMoveAllChip();
                for (int j = 0; j < moduleContainChipPoints[i].Count; j++)
                {
                    for (int k = 0; k < chips.Count; k++)
                    {
                        for (int z = 0; z < chips[k].Count; z++)
                        {
                            if (chips[k][z].getPoint().X == moduleContainChipPoints[i][j].X && chips[k][z].getPoint().Y == moduleContainChipPoints[i][j].Y)
                                modules[i].addChip(chips[k][z].clone());
                        }
                    }
                }
            }
        }

        //繪製移動後的softmodule
        private void moveSoftModule()
        {
            //按下所在的位置、移動結束時所在的位置
            int downX = -1, downY = -1, moveX = -1, moveY = -1;
            for (int i = 0; i < chips.Count; i++)
            {
                for (int j = 0; j < chips[i].Count; j++)
                {
                    if (chips[i][j].getRectangle().Contains(mouseDownLocation))
                    {
                        downX = i;
                        downY = j;
                    }
                    if (chips[i][j].getRectangle().Contains(mouseMoveLocation))
                    {
                        moveX = i;
                        moveY = j;
                    }
                }
            }

            if ((downX == -1 && downY == -1) || (moveX == -1 && moveY == -1))
                return;

            //位置偏移量
            int deltaX = moveX - downX;
            int deltaY = moveY - downY;
            List<Chip> tempChips = new List<Chip>(currentModule.getChips());
            List<Chip> moduleChips = new List<Chip>();
            for(int i =0;i< tempChips.Count;i++)
            {
                moduleChips.Add(chips[tempChips[i].getPoint().X][tempChips[i].getPoint().Y]);
            }

            //判斷是否所有包含的chip皆在範圍內
            for (int i = 0; i < moduleChips.Count; i++)
            {
                Point point = moduleChips[i].getPoint();
                if (point.X + deltaX >= chips.Count || point.X + deltaX < 0)
                    return;
                if (point.Y + deltaY >= chips[0].Count || point.Y + deltaY < 0)
                    return;
            }

            //移除原本所包含的chip
            for (int i = 0; i < moduleChips.Count; i++)
                currentModule.reMoveChip(moduleChips[i]);
            //移除原本chip所包含的此module
            for (int i = 0; i < chips.Count; i++)
            {
                for (int j = 0; j < chips[i].Count; j++)
                    chips[i][j].reMoveModule(currentModule);
            }
            //添加移動後的chip
            for (int i = 0; i < moduleChips.Count; i++)
                currentModule.addChip(chips[moduleChips[i].getPoint().X + deltaX][moduleChips[i].getPoint().Y + deltaY].clone());
            tempChips = new List<Chip>(currentModule.getChips());
            moduleChips = new List<Chip>();
            for (int i = 0; i < tempChips.Count; i++)
            {
                moduleChips.Add(chips[tempChips[i].getPoint().X][tempChips[i].getPoint().Y]);
            }
            //更新位置後的chip上添加此module
            for (int i = 0; i < moduleChips.Count; i++)
                moduleChips[i].addModule(currentModule);
            updateModule();
        }

        //畫出跟隨中鍵移動的softmodule
        private void drawMoveSoftModule(PaintEventArgs e)
        {
            //偏移量
            float deltaX = mouseMoveLocation.X - mouseDownLocation.X;
        
            float deltaY = mouseMoveLocation.Y - mouseDownLocation.Y;
            for (int i = 0; i < chips.Count; i++)
            {
                for (int j = 0; j < chips[i].Count; j++)
                {
                    List<Module> modules = chips[i][j].getEnableModules();
                    for (int k = 0; k < modules.Count; k++)
                    {
                        if (modules[k] == currentModule)
                        {
                            Color color = modules[k].getColor();
                            color = Color.FromArgb(160, color.R, color.G, color.B);
                            Brush brush = new SolidBrush(color);
                            RectangleF rectangle = chips[i][j].getRectangle();
                            rectangle.X += deltaX;
                            rectangle.Y += deltaY;
                            e.Graphics.FillRectangle(brush, rectangle);
                        }
                    }
                }
            }
        }

        //更新編輯後module的資訊
        private void updateModule()
        {
            for (int i = 0; i < softmodules.Count; i++)
            {
                softmodules[i].calculateArea();
                softmodules[i].findBoundingRect();
            }
            draw();
            displaySoftModuleInfo();
            displayConnectionInfo();
            displayErrorMessage();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (currentRectangle != null)
            {
                Pen pen = new Pen(Color.Black, 1);
                pen.DashStyle = DashStyle.Dash;
                e.Graphics.DrawRectangle(pen, currentRectangle); // 畫出矩形
                if (middleClick == true)
                    drawMoveSoftModule(e);
            }

        }




        //編輯module
        //--------------------------------------------

        //--------------------------------------------
        
        //其他功能
        
        //自動放置module
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (isModify)
            {
                DialogResult result = MessageBox.Show("您有未儲存的變更，是否要儲存？", "提醒", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    if (outputFilePath != null)
                        儲存檔案ToolStripMenuItem_Click(sender, e);
                    else
                        另存布局檔案ToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    // 取消窗口關閉操作
                    return;
                }
            }

            // 獲取當前應用程序的目錄
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // 要執行的程式名稱
            string executableName = ".\\placement.exe";

            if (File.Exists(executableName))
            {
                // 輸入檔案路徑和輸出檔案路徑
                string tempOutputFilePath = ".\\tempOutput.txt";

                string command = $"{executableName} {inputFilePath} {tempOutputFilePath}";

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = currentDirectory, // 設定工作目錄為當前目錄
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.StartInfo = startInfo;
                process.Start();

                // 寫入命令到命令行
                process.StandardInput.WriteLine(command);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                // 等待命令執行完成
                process.WaitForExit();

                // 讀取命令輸出
                //string output = process.StandardOutput.ReadToEnd();

                // 顯示輸出或進行其他處理
                //MessageBox.Show(output);

                process.Close();
                readTempFile(tempOutputFilePath);
                File.Delete(tempOutputFilePath);
            }
            else
            {
                // 未找到placement.exe，执行相应的处理逻辑
                MessageBox.Show("無法開啟placement.exe", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //redo
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                // 將當前狀態壓入undoStack
                pushStack(undoStack);
                // 恢復下一步狀態
                popStack(redoStack);
                updateModule();
                if (redoStack.Count == 0)
                    toolStripButton3.Enabled = false;
                if(undoStack.Count > 0)
                    toolStripButton2.Enabled = true;
            }
        }


        //undo
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                // 將當前狀態壓入redoStack
                pushStack(redoStack);
                // 恢復上一步狀態
                popStack(undoStack);

                updateModule();

                toolStripButton3.Enabled = true;
                if (undoStack.Count == 0)
                    toolStripButton2.Enabled = false;
                if(redoStack.Count > 0)
                    toolStripButton3.Enabled = true;
            }

        }
        //其他功能
        //--------------------------------------------
    }
}
