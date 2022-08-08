using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace PuzzleWin
{
    public partial class PuzzleForm : Form
    {
        Image IMG;
        Bitmap BMP;
        Graphics COLOR;
        Graphics CANVAS;
        Graphics TRANSCAN;
        SizeW SIZE = new SizeW();
        double SCALER = 0.8;
        List<Piece> PIECES = new List<Piece>();
        Dictionary<Color, Piece> COLORDIC = new Dictionary<Color, Piece>();
        bool PIECE_SELECTED = false;
        Piece SELECTED_PIECE;
        Stopwatch SW = new Stopwatch();
        string FN;
        bool BG = false;
        
        public class SizeW
        {
            private double _width;
            private double _height;
            private double _x;
            private double _y;
            private double _columns;
            private double _rows;

            public double Width
            {
                get => _width;
                set { _width = value;}
            }
            public double Height
            {
                get => _height;
                set{ _height = value;}
            }
            public double X
            {
                get => _x;
                set{_x = value;}
            }
            public double Y
            {
                get => _y;
                set{_y = value;}
            }
            public double Columns
            {
                get => _columns;
                set { _columns = value; }
            }
            public double Rows
            {
                get => _rows;
                set { _rows = value; }  
            }
        }
        public class Coord
        {
            private double x;
            private double y;
            
            public double X
            {
                get => x;
                set { x = value; }
            }
            public double Y
            {
                get => y;
                set { y = value; }
            }
        }
        public class Piece
        {
            private int rowIndex;
            private int columnIndex;
            private double _width;
            private double _height;
            private double x;
            private double y;
            private double xCorrect;
            private double yCorrect;
            private bool correct;
            private double offsetX;
            private double offsetY;
            private double bottom;
            private double top;
            private double left;
            private double right;
            private Color _color;
            private bool moveable;

            public Piece(int rowIndex, int columnIndex, SizeW SIZE, Color color)
            {
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
                this._width = SIZE.Width / SIZE.Columns;
                this._height = SIZE.Height / SIZE.Rows;
                this.x = SIZE.X + this._width * this.columnIndex;
                this.y = SIZE.Y + this._height * this.rowIndex;
                this.xCorrect = this.x;
                this.yCorrect = this.y;
                this.correct = true;
                this.offsetX = 0;
                this.offsetY = 0;
                this._color = color;
                this.moveable = true;
            }
            public double Bottom
            {
                get => bottom;
                set { this.bottom = value; }
            }
            public double Top
            {
                get => top;
                set { this.top = value; }
            }
            public double Left
            {
                get => left;
                set { this.left = value; }
            }
            public double Right
            {
                get => right;
                set { this.right = value; }
            }
            public bool isCorrect
            {
                get => correct;
                set { this.correct = value; }
            }
            public double Width
            {
                get => _width;
                set { this._width = value; }
            }
            public double Height
            {
                get => _height;
                set { this._height = value; }
            }
            public double X
            {
                get => x;
                set { x = value; }
            }
            public double Y
            {
                get => y;
                set { this.y = value; }
            }
            public double OffsetX
            {
                get => offsetX;
                set { offsetX = value; }
            }
            public double OffsetY
            {
                get => offsetY;
                set { offsetY = value; }
            }
            public Color color
            {
                get => _color;
                set { _color = value; }
            }
            public bool Moveable{
                get => moveable;
                set { moveable = value;}
            }
            public void draw(Graphics CANVAS, Graphics COLOR, SizeW SIZE, Image IMG)
            {
                Pen pen = new Pen(Color.Transparent);
                Point startP = new Point((int)this.X, (int)this.Y);
                GraphicsPath path = new GraphicsPath();

                double sz = Math.Min(this.Width, this.Height);
                double neck = 0.1 * sz;
                double tabWidth = 0.2 * sz;
                double tabHeight = 0.2 * sz;
                

                double scaledTabHeight = Math.Min(IMG.Width / SIZE.Columns,
                    IMG.Height / SIZE.Rows) * tabHeight / sz;

                Rectangle rect = new Rectangle((int)(this.x - tabHeight), (int)(this.y - tabHeight), (int)(this.Width + tabHeight * 2) , (int)(this.Height + tabHeight * 2));

                Point temp1 = new Point();
                Point temp2 = new Point();
                Point temp3 = new Point();
                Point temp4 = new Point();

                if(!Double.IsNaN(this.Top))
                {
                    temp1.X = (int)(this.X + this.Width * Math.Abs(this.Top) - neck);
                    temp1.Y = (int)this.Y;

                    temp2.X = (int)(this.X + this.Width * Math.Abs(this.Top) - neck);
                    temp2.Y = (int)(this.Y - tabHeight * Math.Sign(this.Top) * 0.2);

                    temp3.X = (int)(this.X + this.Width * Math.Abs(this.Top) - tabWidth);
                    temp3.Y = (int)(this.Y - tabHeight * Math.Sign(this.Top));

                    temp4.X = (int)(this.X + this.Width * Math.Abs(this.Top));
                    temp4.Y = (int)(this.Y - tabHeight * Math.Sign(this.Top));

                    path.AddLine(startP, temp1);
                    path.AddBezier(temp1, temp2, temp3, temp4);

                    temp1.X = (int)(this.X + this.Width * Math.Abs(this.Top) + neck);
                    temp2.X = (int)(this.X + this.Width * Math.Abs(this.Top) + neck);
                    temp3.X = (int)(this.X + this.Width * Math.Abs(this.Top) + tabWidth);

                    path.AddBezier(temp4, temp3, temp2, temp1);
                }
                else
                    path.AddLine(startP, new Point((int)(this.X + this.Width), (int)this.Y));

                //to bottom right
                if (!Double.IsNaN(this.Right))
                {
                    temp1.X = (int)(this.X + this.Width);
                    temp1.Y = (int)(this.Y + this.Height * Math.Abs(this.Right) - neck);

                    temp2.X = (int)(this.X + this.Width - tabHeight * Math.Sign(this.Right) * 0.2);
                    temp2.Y = (int)(this.Y + this.Height * Math.Abs(this.Right) - neck);

                    temp3.X = (int)(this.X + this.Width - tabHeight * Math.Sign(this.Right));
                    temp3.Y = (int)(this.Y + this.Height * Math.Abs(this.Right) - tabWidth);

                    temp4.X = (int)(this.X + this.Width - tabHeight * Math.Sign(this.Right));
                    temp4.Y = (int)(this.Y + this.Height * Math.Abs(this.Right));

                    path.AddLine(new Point((int)(this.X + this.Width), (int)this.Y), temp1);
                    path.AddBezier(temp1, temp2, temp3, temp4);

                    temp1.Y = (int)(this.Y + this.Height * Math.Abs(this.Right) + neck);
                    temp2.Y = (int)(this.Y + this.Height * Math.Abs(this.Right) + neck);
                    temp3.Y = (int)(this.Y + this.Height * Math.Abs(this.Right) + tabWidth);

                    path.AddBezier(temp4, temp3, temp2, temp1);
                }
                else
                    path.AddLine(
                         new Point((int)(this.X + this.Width), (int)this.Y),
                         new Point((int)(this.X + this.Width), (int)(this.Y + this.Height)));

                //to bottom left
                if (!Double.IsNaN(this.Bottom))
                {
                    temp1.X = (int)(this.X + this.Width * Math.Abs(this.Bottom) + neck);
                    temp1.Y = (int)(this.Y + this.Height);

                    temp2.X = (int)(this.X + this.Width * Math.Abs(this.Bottom) + neck);
                    temp2.Y = (int)(this.Y + this.Height + tabHeight * Math.Sign(this.Bottom) * 0.2);

                    temp3.X = (int)(this.X + this.Width * Math.Abs(this.Bottom) + tabWidth);
                    temp3.Y = (int)(this.Y + this.Height + tabHeight * Math.Sign(this.Bottom));

                    temp4.X = (int)(this.X + this.Width * Math.Abs(this.Bottom));
                    temp4.Y = (int)(this.Y + this.Height + tabHeight * Math.Sign(this.Bottom));

                    path.AddLine(new Point((int)(this.X + this.Width), (int)(this.Y + this.Height)), temp1);
                    path.AddBezier(temp1, temp2, temp3, temp4);

                    temp1.X = (int)(this.X + this.Width * Math.Abs(this.Bottom) - neck);
                    temp2.X = (int)(this.X + this.Width * Math.Abs(this.Bottom) - neck);
                    temp3.X = (int)(this.X + this.Width * Math.Abs(this.Bottom) - tabWidth);

                    path.AddBezier(temp4, temp3, temp2, temp1);
                }
                else
                    path.AddLine(
                         new Point((int)(this.X + this.Width), (int)(this.Y + this.Height)),
                         new Point((int)(this.X), (int)(this.Y + this.Height)));

                //to top left
                if (!Double.IsNaN(this.Left))
                {
                    temp1.X = (int)(this.X);
                    temp1.Y = (int)(this.Y + this.Height * Math.Abs(this.Left) + neck);

                    temp2.X = (int)(this.X + tabHeight * Math.Sign(this.Left) * 0.2);
                    temp2.Y = (int)(this.Y + this.Height * Math.Abs(this.Left) + neck);

                    temp3.X = (int)(this.X + tabHeight * Math.Sign(this.Left));
                    temp3.Y = (int)(this.Y + this.Height * Math.Abs(this.Left) + tabWidth);

                    temp4.X = (int)(this.X + tabHeight * Math.Sign(this.Left));
                    temp4.Y = (int)(this.Y + this.Height * Math.Abs(this.Left));

                    path.AddLine(new Point((int)(this.X), (int)(this.Y + this.Height)), temp1);
                    path.AddBezier(temp1, temp2, temp3, temp4);

                    temp1.Y = (int)(this.Y + this.Height * Math.Abs(this.Left) - neck);
                    temp2.Y = (int)(this.Y + this.Height * Math.Abs(this.Left) - neck);
                    temp3.Y = (int)(this.Y + this.Height * Math.Abs(this.Left) - tabWidth);

                    path.AddBezier(temp4, temp3, temp2, temp1);
                }
                else
                    path.AddLine(
                         new Point((int)(this.X), (int)(this.Y + this.Height)),
                         new Point((int)this.X, (int)this.Y));

                Region region = new Region(path);
                CANVAS.DrawPath(pen, path);
                CANVAS.SetClip(region, CombineMode.Replace);

                CANVAS.DrawImage(IMG,
                    rect,
                    (int)(this.columnIndex * IMG.Width / SIZE.Columns - scaledTabHeight),
                    (int)(this.rowIndex * (int)IMG.Height / SIZE.Rows - scaledTabHeight),
                    (int)(IMG.Width / SIZE.Columns + scaledTabHeight * 2),
                    (int)(IMG.Height / SIZE.Rows + scaledTabHeight * 2), GraphicsUnit.Pixel);

                COLOR.DrawPath(pen, path);
                COLOR.SetClip(region, CombineMode.Replace);
                COLOR.FillPath(new SolidBrush(this.color), path);
                COLOR.FillRectangle(new SolidBrush(this.color), (int)(this.X - tabHeight), (int)(this.Y - tabHeight), (int)(this.Width + tabHeight * 2), (int)(this.Height * tabHeight * 2));
            }
            public bool close()
            {
                if (distance(new Coord{ X = this.X, Y = this.Y}, new Coord{ X = this.xCorrect, Y = this.yCorrect}) < this.Width/3)
                    return true;
                return false;
            }
            public void snap()
            {
                this.X = this.xCorrect;
                this.Y = this.yCorrect;
                this.isCorrect = true;
                this.moveable = false;
            }
        }

        public PuzzleForm(string name)
        {
            InitializeComponent();
            FN = name;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            SW.Start();
        }
        private void PuzzleForm_Paint(object sender, PaintEventArgs e)
        {
            BMP = new Bitmap(this.Width, this.Height);
            CANVAS = e.Graphics;
            TRANSCAN = e.Graphics;
            COLOR = Graphics.FromImage(BMP);
            COLOR.Clear(Color.Transparent);
            //IMG = Image.FromFile("D:\\Pictures\\Hololive\\Marine.jpeg");
            IMG = Image.FromFile(FN);

            double resizer = SCALER * Math.Min((double)this.Width / (double)IMG.Width, (double)this.Height / (double)IMG.Height);
            SIZE.Width = resizer * IMG.Width;
            SIZE.Height = resizer * IMG.Height;
            SIZE.X = (double)this.Width / 2 - (double)SIZE.Width / 2;
            SIZE.Y = (double)this.Height / 2 - (double)SIZE.Height / 2;

            Rectangle rect = new Rectangle((int)SIZE.X, (int)SIZE.Y, (int)SIZE.Width, (int)SIZE.Height);
            if (BG)
            {
                TRANSCAN.DrawImage(IMG, (int)SIZE.X, (int)SIZE.Y, (int)SIZE.Width, (int)SIZE.Height);
                Brush opq = new SolidBrush(Color.FromArgb(150, 255, 255, 255));
                TRANSCAN.FillRectangle(opq, rect);
            }
            else
                TRANSCAN.FillRectangle(new SolidBrush(Color.White), rect);
            TRANSCAN.DrawRectangle(new Pen(Color.Black), (int)SIZE.X - 1, (int)SIZE.Y - 1, (int)SIZE.Width + 2, (int)SIZE.Height + 2);

            SIZE.Rows = 6;
            SIZE.Columns = 4;
            
            updateCanvas();

            if (isComplete())
            {
                CANVAS.ResetClip();
                CANVAS.DrawImage(IMG, (int)SIZE.X, (int)SIZE.Y, (int)SIZE.Width, (int)SIZE.Height);
            }
        }
        private void PuzzleForm_Load(object sender, EventArgs e)
        {
        }
        public static double distance(Coord p1, Coord p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        public void updateCanvas()
        {
            if(PIECES.Count == 0)
            {
                initializePieces();
                randomizePieces();
            }

            for(int i = 0; i < PIECES.Count(); i++)
                PIECES[i].draw(CANVAS, COLOR, SIZE, IMG);
        }
        public bool isComplete()
        {
            for(int i = 0; i < PIECES.Count; i++)
            {
                if (!PIECES[i].isCorrect)
                    return false;
            }
            return true;
        }
        public void initializePieces()
        {
            double cols = SIZE.Columns;
            int count = 0;
            Random rand = new Random();
            double sgn;
            List<Color> uniqueRandomColors = new List<Color>();

            for(int i = 0; i < SIZE.Rows; i++)
            {
                for(int j = 0; j < SIZE.Columns; j++)
                {
                    Color color = getRandomColor();
                    while (uniqueRandomColors.Contains(color))
                        color = getRandomColor();
                    PIECES.Add(new Piece(i, j, SIZE, color));
                }
            }

            for(int i = 0; i < SIZE.Rows; i++)
            {
                for (int j = 0; j < SIZE.Columns; j++)
                {
                    Piece piece = PIECES[count];
                    COLORDIC.Add(PIECES[count].color, PIECES[count]);
                    if (i == SIZE.Rows - 1)
                        piece.Bottom = double.NaN;
                    else
                    {
                        sgn = (rand.NextDouble() - 0.5) < 0 ? -1 : 1;
                        piece.Bottom = sgn * (rand.NextDouble() * 0.4 + 0.3);
                    }

                    if (j == SIZE.Columns - 1)
                        piece.Right = Double.NaN;
                    else
                    {
                        sgn = (rand.NextDouble() - 0.5) < 0 ? -1 : 1;
                        piece.Right = sgn * (rand.NextDouble() * 0.4 + 0.3);
                    }

                    if (i == 0)
                        piece.Top = Double.NaN;
                    else
                        piece.Top = -PIECES[count - (int)SIZE.Columns].Bottom;

                    if (j == 0)
                        piece.Left = Double.NaN;
                    else
                        piece.Left = -PIECES[count - 1].Right;

                    sgn = (rand.NextDouble() - 0.5) < 0 ? -1 : 1;
                    count++;
                }
            }
        }
        public Color getRandomColor()
        {
            Random rand = new Random();
            byte red = (byte)Math.Floor(rand.NextDouble() * 255);
            byte green = (byte)Math.Floor(rand.NextDouble() * 255);
            byte blue = (byte)Math.Floor(rand.NextDouble() * 255);
            Color ans = new Color();
            ans = Color.FromArgb(255, red, green, blue);
            return ans;
        }
        public void randomizePieces()
        {
            double tabWidth = Math.Min(PIECES[0].Width, PIECES[0].Height) * 0.2;
            Random rnd = new Random();
            for(int i = 0; i < PIECES.Count; i++)
            {
                var loc = new {x = rnd.NextDouble() * (this.Width - PIECES[i].Width),
                    y = rnd.NextDouble() * (this.Height - PIECES[i].Height)};
                if(loc.x < SIZE.X && loc.x > SIZE.X - PIECES[i].Width - tabWidth)
                    loc = new {x = loc.x - PIECES[i].Width - tabWidth, y = loc.y};
                else if (loc.x > SIZE.X && loc.x <= SIZE.X + (SIZE.Width / 2))
                    loc = new { x = loc.x - PIECES[i].Width -  tabWidth - (SIZE.Width / 2), y = loc.y };
                else if (loc.x > SIZE.X + (SIZE.Width / 2) && loc.x < SIZE.X + SIZE.Width)
                    loc = new {x = loc.x + PIECES[i].Width + tabWidth, y = loc.y};
                else if(loc.x > SIZE.X + SIZE.Width && loc.x < SIZE.X + PIECES[i].Width + tabWidth)
                    loc = new { x = loc.x + (PIECES[i].Width/2) + tabWidth, y = loc.y };

                PIECES[i].X = loc.x;
                PIECES[i].Y = loc.y;
                PIECES[i].isCorrect = false;
            }
        }
        private void PuzzleForm_MouseDown(object sender, MouseEventArgs e)
        {
            Color clicked = BMP.GetPixel(e.X, e.Y);
            if (clicked.A == 0)
            {
                SELECTED_PIECE = null;
                PIECE_SELECTED = false;
            }
            else
            {
                SELECTED_PIECE = COLORDIC[clicked];
                if (!SELECTED_PIECE.Moveable)
                {
                    SELECTED_PIECE = null;
                    PIECE_SELECTED = false;
                }
                else
                {
                    PIECE_SELECTED = true;
                    int i = PIECES.IndexOf(SELECTED_PIECE);
                    PIECES.Remove(SELECTED_PIECE);
                    PIECES.Add(SELECTED_PIECE);
                    SELECTED_PIECE.OffsetX = e.X - SELECTED_PIECE.X;
                    SELECTED_PIECE.OffsetY = e.Y - SELECTED_PIECE.Y;
                }
            }
            this.Invalidate();
        }
        private void PuzzleForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (SELECTED_PIECE != null && SELECTED_PIECE.close())
            {
                SELECTED_PIECE.snap();
                this.Invalidate();
                if (isComplete())
                {
                    this.Invalidate();
                    SW.Stop();
                    label1.Text = "YOU WON!";
                    label1.Visible = true;
                    string eT = String.Format("{0:00}:{1:00}:{2:00}", SW.Elapsed.Hours, SW.Elapsed.Minutes, SW.Elapsed.Seconds);
                    label2.Text = "Your time is: " + eT;
                    label2.Visible = true;
                }
            }
            SELECTED_PIECE = null;
            PIECE_SELECTED = false;
        }
        private void PuzzleForm_MouseMove(object sender, MouseEventArgs e)
        {
            if(PIECE_SELECTED)
            {
                //if you wanna move multiple pieces at once do it here
                SELECTED_PIECE.X = e.X - SELECTED_PIECE.OffsetX;
                SELECTED_PIECE.Y = e.Y - SELECTED_PIECE.OffsetY;
                this.Invalidate();
            }
        }

        private void Pause_OnClick(object sender, EventArgs e)
        {
            SW.Stop();
            this.Hide();
            notifyIcon.Visible = true;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            SW.Start();
        }

        private void ShowPic_OnClick(object sender, EventArgs e)
        {
            if (BG)
            {
                menuShowPic.Text = "Show Picture";
            }
            else
                menuShowPic.Text = "Hide Picture";
            BG = !BG;
            this.Invalidate();
        }
    }
}