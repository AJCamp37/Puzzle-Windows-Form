using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PuzzleWin
{
    public partial class PuzzleForm : Form
    {
        private Puzzle puzzle;
        
        private Image compressImage(string file, int newWidth, int newHeight, int newQuality)
        {
            using (Image image = Image.FromFile(file))
            using (Image memImage = new Bitmap(image, newWidth, newHeight))
            {
                ImageCodecInfo? myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                MemoryStream memStream = new MemoryStream();
                memImage.Save(memStream, myImageCodecInfo!, myEncoderParameters);
                Image newImage = Image.FromStream(memStream);
                ImageAttributes imageAttributes = new ImageAttributes();
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(newImage, new Rectangle(Point.Empty, newImage.Size), 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                return newImage;
            }
        }
        private static ImageCodecInfo? GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }
        public PuzzleForm(string name)
        {
            InitializeComponent();
            this.puzzle = new Puzzle(0.7, name);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            puzzle.SW.Start();
        }
        private void PuzzleForm_Paint(object sender, PaintEventArgs e)
        {
            puzzle.BMP = new Bitmap(this.Width, this.Height);
            puzzle.CANVAS = e.Graphics;
            puzzle.TRANSCAN = e.Graphics;
            puzzle.COLOR = Graphics.FromImage(puzzle.BMP);
            puzzle.COLOR.Clear(Color.Transparent);
            //GRAPH = e.Graphics;

            //DrawGraph();
            
            Rectangle rect = new Rectangle((int)puzzle.SIZE.X, (int)puzzle.SIZE.Y, (int)puzzle.SIZE.Width, (int)puzzle.SIZE.Height);
            if (puzzle.BG)
            {
                puzzle.TRANSCAN.DrawImage(puzzle.IMG, (int)puzzle.SIZE.X, (int)puzzle.SIZE.Y, (int)puzzle.SIZE.Width, (int)puzzle.SIZE.Height);
                Brush opq = new SolidBrush(Color.FromArgb(150, 255, 255, 255));
                puzzle.TRANSCAN.FillRectangle(opq, rect);
            }
            else
                puzzle.TRANSCAN.FillRectangle(new SolidBrush(Color.White), rect);
            puzzle.TRANSCAN.DrawRectangle(new Pen(Color.Black, 1F), (int)puzzle.SIZE.X - 1, (int)puzzle.SIZE.Y - 1, (int)puzzle.SIZE.Width + 1, (int)puzzle.SIZE.Height + 1);

            
            updateCanvas();

            if (isComplete())
            {
                puzzle.CANVAS.ResetClip();
                puzzle.CANVAS.DrawImage(puzzle.IMG, (int)puzzle.SIZE.X, (int)puzzle.SIZE.Y, (int)puzzle.SIZE.Width, (int)puzzle.SIZE.Height);
            }
        }
       /* public void DrawGraph()
        {
            int rows = this.Height;
            int cols = this.Width;

            for(int i = 0; i < rows; i += 5)
            {
                GRAPH.DrawLine(new Pen(Color.Black, 0.1F), new Point(i, 0), new Point(i, cols));
            }
            for(int j = 0; j < cols; j += 5)
            {
                GRAPH.DrawLine(new Pen(Color.Black, 0.1F), new Point(0, j), new Point(rows, j));
            }
        }
       */
        private void PuzzleForm_Load(object sender, EventArgs e)
        {

            Image temp = Image.FromFile(puzzle.FN);
            puzzle.SIZE.Rows = 2;
            puzzle.SIZE.Columns = 2;
            double resizer = puzzle.SCALER * Math.Min((double)this.Width / (double)temp.Width, (double)this.Height / (double)temp.Height);
            if(puzzle.SIZE.Rows % 2 == 0)
                puzzle.SIZE.Height = resizer * temp.Height;
            else
                puzzle.SIZE.Height = Math.Ceiling(resizer * temp.Height);
            if(puzzle.SIZE.Columns % 2 == 0)
                puzzle.SIZE.Width = resizer * temp.Width;
            else
                puzzle.SIZE.Width = Math.Ceiling(resizer * temp.Width);

            puzzle.SIZE.X = (double)this.Width / 2 - (double)puzzle.SIZE.Width / 2;
            puzzle.SIZE.Y = (double)this.Height / 2 - (double)puzzle.SIZE.Height / 2;

            puzzle.IMG = compressImage(puzzle.FN, (int)puzzle.SIZE.Width, (int)puzzle.SIZE.Height, 100);
            Console.WriteLine(puzzle.SIZE.Width + "x" + puzzle.SIZE.Height);
            Console.WriteLine("piece width: " + (puzzle.SIZE.Width / puzzle.SIZE.Columns) + " height: " + (puzzle.SIZE.Height / puzzle.SIZE.Rows));
        }
        public void updateCanvas()
        {
            if(puzzle.PIECES.Count == 0 && puzzle.COMPLETED_PIECES.Count == 0)
            {
                initializePieces();
                randomizePieces();
            }

            if(puzzle.COMPLETED_PIECES.Count != 0)
            {
                for (int i = 0; i < puzzle.COMPLETED_PIECES.Count; i++)
                    puzzle.COMPLETED_PIECES[i].draw(puzzle.CANVAS, puzzle.COLOR, puzzle.SIZE, puzzle.IMG);
            }

            for (int i = 0; i < puzzle.PIECES.Count; i++)
                puzzle.PIECES[i].draw(puzzle.CANVAS, puzzle.COLOR, puzzle.SIZE, puzzle.IMG);
        }
        public bool isComplete()
        {
            /*
            for(int i = 0; i < PIECES.Count; i++)
            {
                if (!PIECES[i].isCorrect)
                    return false;
            }
            */
            if (puzzle.PIECES.Count == 0)
                return true;
            else
                return false;
        }
        public void initializePieces()
        {
            double cols = puzzle.SIZE.Columns;
            int count = 0;
            Random rand = new Random();
            double sgn;
            List<Color> uniqueRandomColors = new List<Color>();

            for(int i = 0; i < puzzle.SIZE.Rows; i++)
            {
                for(int j = 0; j < puzzle.SIZE.Columns; j++)
                {
                    Color color = getRandomColor();
                    while (uniqueRandomColors.Contains(color))
                        color = getRandomColor();
                    puzzle.PIECES.Add(new Piece(i, j, puzzle.SIZE, color));
                }
            }

            for(int i = 0; i < puzzle.SIZE.Rows; i++)
            {
                for (int j = 0; j < puzzle.SIZE.Columns; j++)
                {
                    Piece piece = puzzle.PIECES[count];
                    puzzle.COLORDIC.Add(puzzle.PIECES[count].color, puzzle.PIECES[count]);
                    if (i == puzzle.SIZE.Rows - 1)
                        piece.Bottom = double.NaN;
                    else
                    {
                        sgn = (rand.NextDouble() - 0.5) < 0 ? -1 : 1;
                        piece.Bottom = sgn * (rand.NextDouble() * 0.4 + 0.3);
                    }

                    if (j == puzzle.SIZE.Columns - 1)
                        piece.Right = Double.NaN;
                    else
                    {
                        sgn = (rand.NextDouble() - 0.5) < 0 ? -1 : 1;
                        piece.Right = sgn * (rand.NextDouble() * 0.4 + 0.3);
                    }

                    if (i == 0)
                        piece.Top = Double.NaN;
                    else
                        piece.Top = -puzzle.PIECES[count - (int)puzzle.SIZE.Columns].Bottom;

                    if (j == 0)
                        piece.Left = Double.NaN;
                    else
                        piece.Left = -puzzle.PIECES[count - 1].Right;

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
            double tabWidth = Math.Min(puzzle.PIECES[0].Width, puzzle.PIECES[0].Height) * 0.2;
            Random rnd = new Random();
            for(int i = 0; i < puzzle.PIECES.Count; i++)
            {
                var loc = new {x = rnd.NextDouble() * (this.Width - puzzle.PIECES[i].Width),
                    y = rnd.NextDouble() * (this.Height - puzzle.PIECES[i].Height)};
                if(loc.x < puzzle.SIZE.X && loc.x > puzzle.SIZE.X - puzzle.PIECES[i].Width - tabWidth)
                    loc = new {x = loc.x - puzzle.PIECES[i].Width - tabWidth, y = loc.y};
                else if (loc.x > puzzle.SIZE.X && loc.x <= puzzle.SIZE.X + (puzzle.SIZE.Width / 2))
                    loc = new { x = loc.x - puzzle.PIECES[i].Width -  tabWidth - (puzzle.SIZE.Width / 2), y = loc.y };
                else if (loc.x > puzzle.SIZE.X + (puzzle.SIZE.Width / 2) && loc.x < puzzle.SIZE.X + puzzle.SIZE.Width)
                    loc = new {x = loc.x + puzzle.PIECES[i].Width + tabWidth, y = loc.y};
                else if(loc.x > puzzle.SIZE.X + puzzle.SIZE.Width && loc.x < puzzle.SIZE.X + puzzle.PIECES[i].Width + tabWidth)
                    loc = new { x = loc.x + (puzzle.PIECES[i].Width/2) + tabWidth, y = loc.y };

                puzzle.PIECES[i].X = loc.x;
                puzzle.PIECES[i].Y = loc.y;
                puzzle.PIECES[i].isCorrect = false;
            }
        }
        public void removePieces(Piece piece, List<Piece> visited)
        {
            if (visited.Contains(piece))
                return;
            visited.Add(piece);

            if(piece.leftPiece != null)
            {
                puzzle.PIECES.Remove(piece.leftPiece);
                puzzle.PIECES.Add(piece.leftPiece);
            }
            if (piece.rightPiece != null)
            {
                puzzle.PIECES.Remove(piece.rightPiece);
                puzzle.PIECES.Add(piece.rightPiece);
            }
            if (piece.topPiece != null)
            {
                puzzle.PIECES.Remove(piece.topPiece);
                puzzle.PIECES.Add(piece.topPiece);
            }
            if (piece.bottomPiece != null)
            {
                puzzle.PIECES.Remove(piece.bottomPiece);
                puzzle.PIECES.Add(piece.bottomPiece);
            }
            return;
        }
        private void PuzzleForm_MouseDown(object sender, MouseEventArgs e)
        {
            Color clicked = puzzle.BMP.GetPixel(e.X, e.Y);
            if (clicked.A == 0)
            {
                puzzle.SELECTED_PIECE = null;
                puzzle.PIECE_SELECTED = false;
            }
            else
            {
                puzzle.SELECTED_PIECE = puzzle.COLORDIC[clicked];
                if (!puzzle.SELECTED_PIECE.Moveable)
                {
                    puzzle.SELECTED_PIECE = null;
                    puzzle.PIECE_SELECTED = false;
                }
                else
                {
                    puzzle.PIECE_SELECTED = true;
                    //int i = PIECES.IndexOf(SELECTED_PIECE);
                    puzzle.PIECES.Remove(puzzle.SELECTED_PIECE);
                    puzzle.PIECES.Add(puzzle.SELECTED_PIECE);
                    removePieces(puzzle.SELECTED_PIECE, new List<Piece>());
                    puzzle.SELECTED_PIECE.OffsetX = e.X - puzzle.SELECTED_PIECE.X;
                    puzzle.SELECTED_PIECE.OffsetY = e.Y - puzzle.SELECTED_PIECE.Y;
                }
            }
            this.Invalidate();
        }
        private void PuzzleForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (puzzle.SELECTED_PIECE != null && puzzle.SELECTED_PIECE.close())
            {
                puzzle.SELECTED_PIECE.snap(puzzle.PIECES, puzzle.COMPLETED_PIECES);
                this.Invalidate();
                if (isComplete())
                {
                    this.Invalidate();
                    puzzle.SW.Stop();
                    label1.Text = "YOU WON!";
                    label1.Visible = true;
                    string eT = String.Format("{0:00}:{1:00}:{2:00}", puzzle.SW.Elapsed.Hours, puzzle.SW.Elapsed.Minutes, puzzle.SW.Elapsed.Seconds);
                    label2.Text = "Your time is: " + eT;
                    label2.Visible = true;
                }
            }
            else if(puzzle.SELECTED_PIECE != null)
            {
                if(puzzle.SELECTED_PIECE.LeftCheck.X >= 0 && puzzle.SELECTED_PIECE.LeftCheck.X <= this.Width && puzzle.SELECTED_PIECE.LeftCheck.Y >= 0 && puzzle.SELECTED_PIECE.LeftCheck.Y <= this.Height)
                {
                    Color tryLeft = puzzle.BMP.GetPixel(puzzle.SELECTED_PIECE.LeftCheck.X-1, puzzle.SELECTED_PIECE.LeftCheck.Y);
                    
                    if(tryLeft.A != 0)
                    {
                        Piece selectedLeft = puzzle.COLORDIC[tryLeft];
                        if(selectedLeft != puzzle.SELECTED_PIECE.leftPiece)
                        {
                            if(puzzle.SELECTED_PIECE.closePiece(selectedLeft, 'L', puzzle.COLORDIC))
                            {
                                puzzle.SELECTED_PIECE = null;
                                puzzle.PIECE_SELECTED = false;
                                this.Invalidate();
                                return;
                            }
                        }
                    }
    
                }
                if(puzzle.SELECTED_PIECE.RightCheck.X >= 0 && puzzle.SELECTED_PIECE.RightCheck.X <= this.Width && puzzle.SELECTED_PIECE.RightCheck.Y >= 0 && puzzle.SELECTED_PIECE.RightCheck.Y <= this.Height)
                {
                    Color tryRight = puzzle.BMP.GetPixel(puzzle.SELECTED_PIECE.RightCheck.X+1, puzzle.SELECTED_PIECE.RightCheck.Y);

                    if(tryRight.A != 0)
                    {
                        Piece selectedRight = puzzle.COLORDIC[tryRight];
                        if(selectedRight != puzzle.SELECTED_PIECE.rightPiece)
                        {
                            if(puzzle.SELECTED_PIECE.closePiece(selectedRight, 'R', puzzle.COLORDIC)){
                                puzzle.SELECTED_PIECE = null;
                                puzzle.PIECE_SELECTED = false;
                                this.Invalidate();
                                return;
                            }
                        }
                    }
                }
                
                if(puzzle.SELECTED_PIECE.TopCheck.X >= 0 && puzzle.SELECTED_PIECE.TopCheck.X <= this.Width && puzzle.SELECTED_PIECE.TopCheck.Y >= 0 && puzzle.SELECTED_PIECE.TopCheck.Y <= this.Height)
                {
                    Color tryTop = puzzle.BMP.GetPixel(puzzle.SELECTED_PIECE.TopCheck.X, puzzle.SELECTED_PIECE.TopCheck.Y-1);

                    if(tryTop.A != 0)
                    {
                        Piece selectedTop = puzzle.COLORDIC[tryTop];
                        if(selectedTop != puzzle.SELECTED_PIECE.topPiece)
                        {
                            if(puzzle.SELECTED_PIECE.closePiece(selectedTop, 'T', puzzle.COLORDIC))
                            {
                                puzzle.SELECTED_PIECE = null;
                                puzzle.PIECE_SELECTED = false;
                                this.Invalidate();
                                return;
                            }
                        }
                    }
                }

                if(puzzle.SELECTED_PIECE.BottomCheck.X >= 0 && puzzle.SELECTED_PIECE.BottomCheck.X <= this.Width && puzzle.SELECTED_PIECE.BottomCheck.Y >= 0 && puzzle.SELECTED_PIECE.BottomCheck.Y <= this.Height)
                {
                    Color tryBottom = puzzle.BMP.GetPixel(puzzle.SELECTED_PIECE.BottomCheck.X, puzzle.SELECTED_PIECE.BottomCheck.Y+1);

                    if(tryBottom.A != 0)
                    {
                        Piece selectedBottom = puzzle.COLORDIC[tryBottom];
                        if(selectedBottom != puzzle.SELECTED_PIECE.bottomPiece)
                        {
                            if(puzzle.SELECTED_PIECE.closePiece(selectedBottom, 'B', puzzle.COLORDIC))
                            {
                                puzzle.SELECTED_PIECE = null;
                                puzzle.PIECE_SELECTED = false;
                                this.Invalidate();
                                return;
                            }
                        }
                    }
                }
           }
            puzzle.SELECTED_PIECE = null;
            puzzle.PIECE_SELECTED = false;
        }
        private void PuzzleForm_MouseMove(object sender, MouseEventArgs e)
        {
            if(puzzle.PIECE_SELECTED)
            {
                //if you wanna move multiple pieces at once do it here
                puzzle.SELECTED_PIECE!.X = e.X - puzzle.SELECTED_PIECE.OffsetX;
                puzzle.SELECTED_PIECE.Y = e.Y - puzzle.SELECTED_PIECE.OffsetY;

                movePieces(puzzle.SELECTED_PIECE, new List<Piece>());

                this.Invalidate();
            }
        }
        private void movePieces(Piece piece, List<Piece> visited)
        {
            if (visited.Contains(piece))
                return;

            visited.Add(piece);

            if(piece.leftPiece != null)
            {
                piece.leftPiece.X = piece.X - piece.leftPiece.Width;
                piece.leftPiece.Y = piece.Y;
                movePieces(piece.leftPiece, visited);
            }

            if(piece.rightPiece != null)
            {
                piece.rightPiece.X = piece.X + piece.Width;
                piece.rightPiece.Y = piece.Y;
                movePieces(piece.rightPiece, visited);
            }
            
            if(piece.topPiece != null)
            {
                piece.topPiece.X = piece.X;
                piece.topPiece.Y = piece.Y - piece.topPiece.Height;
                movePieces(piece.topPiece, visited);
            }

            if(piece.bottomPiece != null)
            {
                piece.bottomPiece.X = piece.X;
                piece.bottomPiece.Y = piece.Y + piece.Height;
                movePieces(piece.bottomPiece, visited);
            }
            return;
        }
        private void Pause_OnClick(object sender, EventArgs e)
        {
            puzzle.SW.Stop();
            this.Hide();
            notifyIcon.Visible = true;
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon.Visible = false;
            puzzle.SW.Start();
        }
        private void ShowPic_OnClick(object sender, EventArgs e)
        {
            if (puzzle.BG)
            {
                menuShowPic.Text = "Show Picture";
            }
            else
                menuShowPic.Text = "Hide Picture";
            puzzle.BG = !puzzle.BG;
            this.Invalidate();
        }
        //remove
        private void menuComplete_Click(object sender, EventArgs e)
        {
            foreach(Piece piece in puzzle.PIECES)
            {
                piece.X = piece.Xcor;
                piece.Y = piece.Ycor;
            }
            this.Invalidate();
        }
    }
    public class Puzzle 
    {
        public Image? img;
        public Bitmap? bmp;
        public Graphics? _color;
        public Graphics? canvas;
        public Graphics? transcan;
        public SizeW size;
        public double scaler;
        public List<Piece> pieces;
        public List<Piece> completed;
        public Dictionary<Color, Piece> colordic;
        public bool selectedBool;
        public Piece? selected;
        public Stopwatch sw;
        public string fn;
        public bool bg;

        public Image IMG
        {
            get => img!;
            set { img = value; }
        }
        public Bitmap BMP
        {
            get => bmp!;
            set { bmp = value; }
        }
        public Graphics COLOR
        {
            get => _color!;
            set { _color = value; }
        }
        public Graphics CANVAS
        {
            get => canvas!;
            set { canvas = value; }
        }
        public Graphics TRANSCAN
        {
            get => transcan!;   
            set { transcan = value; }
        }
        public SizeW SIZE
        {
            get => size;
            set { size = value; }
        }
        public double SCALER
        {
            get => scaler;
            set { scaler = value; }
        }
        public List<Piece> PIECES
        {
            get => pieces;
            set { pieces = value; }
        }
        public List<Piece> COMPLETED_PIECES
        {
            get => completed;
            set { completed = value; }
        }
        public Dictionary<Color, Piece> COLORDIC
        {
            get => colordic;
            set { COLORDIC = value; }
        }
        public bool PIECE_SELECTED
        {
            get => selectedBool;
            set { selectedBool = value; }
        }
        public Piece? SELECTED_PIECE
        {
            get => selected;
            set { selected = value; }
        }
        public Stopwatch SW
        {
            get => sw;
            set { sw = value; }
        }
        public string FN
        {
            get => fn;
            set { fn = value; }
        }
        public bool BG
        {
            get => bg;
            set { bg = value; }
        }
        public Puzzle(double scaler, string name)
        {
            this.fn = name;
            this.scaler = scaler;
            this.size = new SizeW();
            this.pieces = new List<Piece>();
            this.colordic = new Dictionary<Color, Piece>();
            this.completed = new List<Piece>();
            this.sw = new Stopwatch();
            this.selectedBool = false;
            this.bg = false;
            this.img = null;
            this._color = null;
            this.transcan = null;
            this.canvas = null;
            this.bmp = null;
        }
    }
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
        private Piece? _leftPiece;
        private Piece? _rightPiece;
        private Piece? _bottomPiece;
        private Piece? _topPiece;
        private Point leftCheck; 
        private Point rightCheck; 
        private Point topCheck; 
        private Point bottomCheck; 

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
            this._topPiece = null;
            this._bottomPiece = null;
            this._leftPiece = null;
            this._rightPiece = null;
            this.leftCheck = new Point(-1, -1);
            this.rightCheck = new Point(-1, -1);
            this.topCheck = new Point(-1, -1);
            this.bottomCheck = new Point(-1, -1);
        }
        //remove 
        public double Xcor
        {
            get => xCorrect;
        }
        public double Ycor
        {
            get => yCorrect;
        }
        public Piece leftPiece
        {
            get => _leftPiece!;
            set { _leftPiece = value; } 
        }
        public Piece rightPiece 
        {
            get => _rightPiece!;
            set { _rightPiece= value; } 
        }
        public Piece bottomPiece 
        {
            get => _bottomPiece!;
            set { _bottomPiece= value; } 
        }
        public Piece topPiece 
        {
            get => _topPiece!;
            set { _topPiece = value; } 
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
        public Point LeftCheck
        {
            get => leftCheck;
            set { leftCheck = value; }
        }
        public Point RightCheck
        {
            get => rightCheck;
            set { rightCheck = value; }
        }
        public Point TopCheck
        {
            get => topCheck;
            set { topCheck = value; }
        }
        public Point BottomCheck
        {
            get => bottomCheck;
            set { bottomCheck = value; }
        }
        public static double distance(Coord p1, Coord p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        public void draw(Graphics CANVAS, Graphics COLOR, SizeW SIZE, Image IMG)
        {
            Pen pen = new Pen(Color.Transparent);
            Pen blackPen = new Pen(Color.Black, 2F);
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
                this.TopCheck = temp4;
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
                this.RightCheck = temp4;
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
                this.BottomCheck = temp4;
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
                path.AddLine(temp1, startP);
                this.LeftCheck = temp4;
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

            CANVAS.DrawPath(blackPen, path);

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
        private void snapAll( Piece piece, List<Piece> visited, List<Piece> PIECES, List<Piece> completed)
        {
            if (visited.Contains(piece))
                return;

            visited.Add(piece);
            completed.Add(piece);
            PIECES.Remove(piece);

            if(piece.leftPiece != null)
            {
                piece.leftPiece.X = piece.leftPiece.xCorrect;
                piece.leftPiece.Y = piece.leftPiece.yCorrect;
                piece.leftPiece.moveable = false;
                piece.leftPiece.correct = true;

                this.snapAll(piece.leftPiece, visited, PIECES, completed);
            }
            if(piece.rightPiece != null)
            {
                piece.rightPiece.X = piece.rightPiece.xCorrect;
                piece.rightPiece.Y = piece.rightPiece.yCorrect;
                piece.rightPiece.moveable = false;
                piece.rightPiece.correct = true;

                this.snapAll(piece.rightPiece, visited, PIECES, completed);
            }
            if(piece.topPiece != null)
            {
                piece.topPiece.X = piece.topPiece.xCorrect;
                piece.topPiece.Y = piece.topPiece.yCorrect;
                piece.topPiece.moveable = false;
                piece.topPiece.correct = true;

                this.snapAll(piece.topPiece, visited, PIECES, completed);
            }
            if(piece.bottomPiece != null)
            {
                piece.bottomPiece.X = piece.bottomPiece.xCorrect;
                piece.bottomPiece.Y = piece.bottomPiece.yCorrect;
                piece.bottomPiece.moveable = false;
                piece.bottomPiece.correct = true;
                
                this.snapAll(piece.bottomPiece, visited, PIECES, completed);
            }
            return;
        }
        public void snap(List<Piece> PIECES, List<Piece> completed)
        {
            this.X = this.xCorrect;
            this.Y = this.yCorrect;
            this.isCorrect = true;
            this.moveable = false;

            this.snapAll(this, new List<Piece>(), PIECES, completed);
        }
        public bool closePiece( Piece piece, char dir, Dictionary<Color, Piece> COLORDIC)
        {
            if(dir == 'L')
            {
                if (this.Left == -piece.Right && 
                   (this.LeftCheck.X == piece.RightCheck.X || this.LeftCheck.X == piece.RightCheck.X + 1 || this.LeftCheck.X == piece.RightCheck.X - 1) && 
                   (this.LeftCheck.Y == piece.RightCheck.Y || this.LeftCheck.Y == piece.RightCheck.Y + 1 || this.LeftCheck.Y == piece.RightCheck.Y - 1))
                {
                    this.connect(piece, COLORDIC);
                    return true;
                }
            }
            else if(dir == 'R')
            {
                if (this.Right == -piece.Left &&
                    (this.RightCheck.X == piece.LeftCheck.X || this.RightCheck.X == piece.LeftCheck.X + 1 || this.RightCheck.X == piece.LeftCheck.X - 1) && 
                    (this.RightCheck.X == piece.LeftCheck.X || this.RightCheck.X == piece.LeftCheck.X + 1 || this.RightCheck.X == piece.LeftCheck.X - 1))
                {
                    this.connect(piece, COLORDIC);
                    return true;
                }
            }
            else if(dir == 'T')
            {
                if (this.Top == -piece.Bottom && 
                    (this.TopCheck.X == piece.BottomCheck.X || this.TopCheck.X == piece.BottomCheck.X + 1 || this.TopCheck.X == piece.BottomCheck.X - 1) &&
                    (this.TopCheck.Y == piece.BottomCheck.Y || this.TopCheck.Y == piece.BottomCheck.Y + 1 || this.TopCheck.Y == piece.BottomCheck.Y - 1))
                {
                    this.connect(piece, COLORDIC);
                    return true;
                }
            }
            else
            {
                if (this.Bottom == -piece.Top && 
                    (this.BottomCheck.X == piece.TopCheck.X || this.BottomCheck.X == piece.TopCheck.X + 1 || this.BottomCheck.X == piece.TopCheck.X - 1) &&
                    (this.BottomCheck.Y == piece.TopCheck.Y || this.BottomCheck.Y == piece.TopCheck.Y + 1 || this.BottomCheck.Y == piece.TopCheck.Y - 1))
                {
                    this.connect(piece, COLORDIC);
                    return true;
                }
            }

            return false;
        }
        public void connectChild(Piece child, Piece piece, char dir, Dictionary<Color, Piece> COLORDIC)
        {
            if(dir == 'L')
            {
                piece.rightPiece = child;
                child.leftPiece = piece;
                piece.X = child.X - piece.Width;
                piece.Y = child.Y;
            }
        }
        private void updatePieces( Piece piece, List<Piece> visited)
        {
            if (visited.Contains(piece))
                return;

            visited.Add(piece);

            if(piece.leftPiece != null)
            {
                piece.leftPiece.X = piece.X - piece.leftPiece.Width;
                piece.leftPiece.Y = piece.Y;
                this.updatePieces(piece.leftPiece, visited);
            }

            if(piece.rightPiece != null)
            {
                piece.rightPiece.X = piece.X + piece.Width;
                piece.rightPiece.Y = piece.Y;
                this.updatePieces(piece.rightPiece, visited);
            }
            
            if(piece.topPiece != null)
            {
                piece.topPiece.X = piece.X;
                piece.topPiece.Y = piece.Y - piece.topPiece.Height;
                this.updatePieces(piece.topPiece, visited);
            }

            if(piece.bottomPiece != null)
            {
                piece.bottomPiece.X = piece.X;
                piece.bottomPiece.Y = piece.Y + piece.Height;
                this.updatePieces(piece.bottomPiece, visited);
            }
            return;
        }
        public void connect(Piece piece, Dictionary<Color, Piece> COLORDIC)
        {
            if (!piece.moveable)
                return;
            if (piece.Right == -this.Left)
            {
                piece.rightPiece = this;
                this.leftPiece = piece;
                piece.X = this.X - piece.Width;
                piece.Y = this.Y;
                this.updatePieces(this, new List<Piece>());
                //Console.WriteLine("connected piece: [" + this.rowIndex.ToString() + "," + this.columnIndex.ToString()  + "] to piece: [" + piece.rowIndex.ToString() + "," + piece.columnIndex.ToString() + "]");
            }
            else if(piece.Left == -this.Right)
            {
                piece.leftPiece = this;
                this.rightPiece = piece;
                piece.X = this.X + this.Width;
                piece.Y = this.Y;

                this.updatePieces(this, new List<Piece>());
            }
            else if(piece.Bottom == -this.Top)
            {
                piece.bottomPiece = this;
                this.topPiece = piece;
                piece.X = this.X;
                piece.Y = this.Y + this.Height;

                this.updatePieces(this, new List<Piece>());
            }
            else if(piece.Top == -this.Bottom)
            {
                piece.topPiece = this;
                this.bottomPiece = piece;
                piece.X = this.X;
                piece.Y = this.Y + piece.Height;

                this.updatePieces(this, new List<Piece>());
            }
            else
                return;
        }
    }
}