using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleWin
{
    public partial class StartForm : Form
    {
        string FN;
        public StartForm()
        {
            InitializeComponent();
        }
        private void StartForm_Load(object sender, EventArgs e)
        {
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if(FN != null)
            {
                Image temp = Image.FromFile(FN);
                double resizer = 0.8 * Math.Min((double)Screen.PrimaryScreen.Bounds.Width/ (double)temp.Width, (double)Screen.PrimaryScreen.Bounds.Height/ (double)temp.Height);
                Image img = compressImage(FN, (int)(resizer * temp.Width), (int)(resizer * temp.Height), 100);
                PuzzleForm form1 = new PuzzleForm(FN, img);
                this.Hide();
                form1.Show();
            }
        }
        private Image compressImage(string file, int newWidth, int newHeight, int newQuality)
        {
            using (Image image = Image.FromFile(file))
            using (Image memImage = new Bitmap(image, newWidth, newHeight))
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                MemoryStream memStream = new MemoryStream();
                memImage.Save(memStream, myImageCodecInfo, myEncoderParameters);
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
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            FN = openFileDialog1.FileName;
            if (FN != "")
               btnStart.Enabled = true;
            else
                btnStart.Enabled = false;
        }
    }
}
