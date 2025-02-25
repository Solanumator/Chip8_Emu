namespace Chip8.UI
{
    using Chip8.Engine;
    using System.Drawing.Drawing2D;

    public partial class Form1 : Form
    {
        public PictureBox GameBox => this.pictureBox1 as PictureBox;
        public Engine engine;

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Paint += GameBox_Paint;
        }

        private void GameBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(this.pictureBox1.Image, new Rectangle(0, 0, this.GameBox.Width, this.GameBox.Height));
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.engine = new Engine(this);
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Chip8 Roms|*.ch8";
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.engine.LoadRom(openFileDialog1.FileName);
            }

            if (!this.engine.Start())
            {
                MessageBox.Show("No rom loaded", "Warning", MessageBoxButtons.OK);
            }
        }
    }
}
