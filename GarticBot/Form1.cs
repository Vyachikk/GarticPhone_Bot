using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace GarticBot
{
    public partial class Form1 : Form
    {
        //эталонные значения при full hd и 100% масштабировании
        int brightness = 0;
        int pixelSize = 18;
        int brushX = 950;
        int brushY = 925;
        int palletteX = 490;
        int palletteY = 390;
        int canvasX = 640;
        int canvasY = 300;


        string radioButton_Tag = "RGB";


        public const int VK_SPACE = 0x20; 
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetAsyncKeyState(int vkey);


        MouseClick mc = new MouseClick();
        PrepareImage pi = new PrepareImage();
        Bitmap BitmapSource, BitmapResize, source;
        Bitmap GarticPalette = new Bitmap(@"Image\Gartic.png");


        private Thread thread;

        public Form1()
        {
            InitializeComponent();
            resolution_trackBar.Enabled = false;
            trackBar_Bright.Enabled = false;
            StartProgarm.Enabled = false;
            MethodInvoker mi = new MethodInvoker(StopKey);
            mi.BeginInvoke(null, null);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose an image";
            openFileDialog.Filter = "ALL|*.*|JPG|*.jpg|JPEG|*.jpeg|PNG|*.png|BMP|*.bmp|WEBP|*.webp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string FilePath = openFileDialog.FileName;
                BitmapSource = new Bitmap(FilePath);
                BitmapResize = new Bitmap(BitmapSource, Main_PictureBox.Width, Main_PictureBox.Height);

                Main_PictureBox.Image = BitmapResize;
                textBox_FilePath.Text = FilePath;
            }
            enableUI();
            UpdatePicture();
        }


        private void StartProgarm_Click(object sender, EventArgs e)
        {
            thread = new Thread(Painting);
            WindowState = FormWindowState.Minimized;
            setResolution();
            mc.btnSet_Click(1670, 400);
            thread.Start();
        }


        private void StopKey()
        {
            while (this.IsHandleCreated)
            {
                short keySPACE = GetAsyncKeyState(VK_SPACE);

                if (keySPACE != 0)
                    thread.Abort();            
            }
        }

        private void ColorModel_changer(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                radioButton_Tag = "RGB";
                radioButton2.Checked = false; 
            }

            else if (radioButton2.Checked == true)
            {
                radioButton_Tag = "XYZ";
                radioButton1.Checked = false;
            }

            UpdatePicture();
        }


        public void Painting()
        {
            SolidBrush PaletteColor = new SolidBrush(Color.Empty);
            SolidBrush ImageColor = new SolidBrush(Color.Empty);
            SolidBrush CurrentColor = new SolidBrush(Color.Empty);
            
            for (int i = 0; i < source.Width; i += pixelSize)
            {
                for (int j = 0; j < source.Height; j += pixelSize)
                {
                    ImageColor.Color = source.GetPixel(i, j);

                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 6; y++)
                        {
                            PaletteColor.Color = GarticPalette.GetPixel(x, y);

                            if (ImageColor.Color == GarticPalette.GetPixel(0, 1))
                                continue;

                            else if (PaletteColor.Color == CurrentColor.Color)
                            {
                                mc.btnfast_Click(i + canvasX, j + canvasY);
                            }

                            else if (PaletteColor.Color == ImageColor.Color)
                            {
                                mc.btnSet_Click((x + palletteX) + x * 50, (int)(y + palletteY + numericUpDown1.Value * 20 + y * 50));
                                mc.btnSet_Click(i + canvasX, j + canvasY);
                                CurrentColor.Color = PaletteColor.Color;
                            }
                        }
                    }
                }
            }
        }


        void UpdatePicture()
        {
            if (BitmapResize != null)
                source = pi.PixelImage(BitmapResize, pixelSize, brightness, radioButton_Tag);

            Main_PictureBox.Image = source;
        }

        private void setResolution() //Set brush size
        {
            mc.btnSet_Click(brushX - 65 * (resolution_trackBar.Value - 1), brushY);
        }

        private void getPixelSize(object sender, EventArgs e)
        {
            pixelSize = 18 - 4 * (resolution_trackBar.Value - 1);
            UpdatePicture();
        }

        private void getBrightnes(object sender, EventArgs e)
        {
            brightness = trackBar_Bright.Value;
            UpdatePicture();
        }

        private void enableUI()
        {
            resolution_trackBar.Enabled = true;
            trackBar_Bright.Enabled = true;
            StartProgarm.Enabled = true;
        }
    }
}
