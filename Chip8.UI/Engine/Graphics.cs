using Chip8.Engine.Helpers;
using Chip8.UI;

namespace Chip8.Engine
{
    public class Graphics
    {
        const int WIDTH = 64;
        const int HEIGHT = 32;

        private DirectBitmap screenTexture;

        Form1 form;

        public Graphics(Form1 form)
        {
            this.form = form;
            this.screenTexture = new DirectBitmap(WIDTH, HEIGHT);
            this.form.GameBox.Image = this.screenTexture.Bitmap;
        }

        public bool UpdateScreen(ushort height, byte sprite, ushort x, ushort y)
        {
            var collision = false;

            // Draw for each pixel of height
            for (int i = 0; i < height; i++)
            {
                var Y = y + i;

                // Loop each bit in the sprite
                for (int b = 0; b < 8; b++)
                {
                    var currentScreenPixel = this.screenTexture.GetPixel(x, y);
                    var pixelValue = sprite & (0x80 >> b);
                    var X = x + b;

                    // If not 0, we flip the bit
                    if (pixelValue != 0)
                    {
                        if (currentScreenPixel.R == 0)
                        {
                            collision = true;
                        }

                        this.FlipPixel(X, Y, currentScreenPixel);
                    }
                }
            }

            if (collision)
            {
                // Redraw
            }

            this.form.GameBox.Invoke(new MethodInvoker(delegate()
            {
                this.form.GameBox.Image = screenTexture.Bitmap;
            }));

            return collision;
        }

        public void ClearScreen()
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    this.screenTexture.SetPixel(x, y, Color.Black);
                }
            }

            this.form.GameBox.Invoke(new MethodInvoker(delegate ()
            {
                this.form.GameBox.Image = screenTexture.Bitmap;
            }));
        }

        private void FlipPixel(int x, int y, Color currentPixel)
        {
            if (currentPixel.R == 0)
            {
                this.screenTexture.SetPixel(x, y, Color.White);
            }
            else
            {
                this.screenTexture.SetPixel(x, y, Color.Black);
            }
        }
    }
}
