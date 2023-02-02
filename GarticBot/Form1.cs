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

        [DllImport("KeyPressDLL.dll")]
        static public extern bool IsKeyPress(int key);


        MouseClick mc = new MouseClick();
        PrepareImage pi = new PrepareImage();
        Bitmap BitmapSource, BitmapResize, source;
        Bitmap GarticPalette = new Bitmap(@"Image\Gartic.png");


        private Thread thread;
        private Thread whileThread;

        public Form1()
        {
            InitializeComponent();
            resolution_trackBar.Enabled = false;
            trackBar_Bright.Enabled = false;
            StartProgarm.Enabled = false;
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
            whileThread = new Thread(StopKey);
            WindowState = FormWindowState.Minimized;
            mc.btnSet_Click(1670, 550);
            thread.Start();
            whileThread.Start();
        }

        private void StopKey()
        {
            while (true)
            {
                bool flag = IsKeyPress(27);

                if (flag)
                {
                    flag = false;
                    thread.Abort();
                    whileThread.Abort();
                }
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

        //Функция рисования

        public void Painting()
        {
            int x = 0, y = 0;
            SolidBrush PaletteColor = new SolidBrush(Color.Empty);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 6; j++)
                {     
                    mc.btnSet_Click((i * 50 + palletteX), (int)(j * 50 + palletteY + numericUpDown1.Value * 20));
                    PaletteColor.Color = GarticPalette.GetPixel(i, j);

                    for (x = 0; x < source.Width; x += pixelSize)
                        for (y = 0; y < source.Height; y += pixelSize)
                        {
                            if (source.GetPixel(x, y) == PaletteColor.Color)
                            {
                                Thread.Sleep(1);
                                mc.btnDown_Click(x + canvasX, y + canvasY);
                                mc.btnUp_Click(x + canvasX + pixelSize, y + canvasY + pixelSize);
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
