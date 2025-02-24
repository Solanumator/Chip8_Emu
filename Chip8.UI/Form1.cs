namespace Chip8.UI
{
    using Chip8.Engine;

    public partial class Form1 : Form
    {
        public PictureBox GameBox => this.pictureBox1 as PictureBox;
        public Engine engine;

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.engine = new Engine(this);
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.engine.Start();
        }
    }
}
