﻿using Chip8.Engine.Helpers;
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

        /// <summary>
        /// Draws a sprite to the screen and updates the displayed image.
        /// </summary>
        /// <param name="height">The height of the sprite to create.</param>
        /// <param name="sprite">The bytes representing the sprite. This should equal height.</param>
        /// <param name="x">Start X position.</param>
        /// <param name="y">Start Y position.</param>
        /// <returns>Whether or not we've flipped any pixel which is 'on'.</returns>
        public bool DrawSprite(ushort height, byte[] sprite, ushort x, ushort y)
        {
            var desc = $"Drawing a sprite of height: {height}, sprite: {sprite}, at position {x},{y}";
            Console.WriteLine(desc);

            var collision = false;

            // For N rows
            for (int spriteY = 0; spriteY < height; spriteY++)
            {
                // Nth byte of sprite data
                var currentSprite = sprite[spriteY];

                // For each bit in the sprite
                for (int spriteX = 0; spriteX < 8; spriteX++)
                {
                    // If we hit the end of the screen, continue
                    // Its possible we'll let this wrap in future.
                    if (x + spriteX >= WIDTH)
                    {
                        continue;
                    }

                    // Current Pixel Color
                    var currentScreenPixel = this.screenTexture.GetPixel(x + spriteX, y + spriteY);

                    // Get single pixel value
                    var pixelValue = (currentSprite & (0x80 >> spriteX));
                
                    // Set pixel
                    if (pixelValue != 0)
                    {
                        if (currentScreenPixel.R == 0)
                        {
                            collision = true;
                        }

                        this.FlipPixel(x + spriteX, y + spriteY, currentScreenPixel);
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
