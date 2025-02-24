using Chip8.Exceptions;
using SDL2;
using System.Data.SqlTypes;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Chip8.Engine;

public class Graphics_Old
{
    private const int PIXEL_WIDTH = 64;
    private const int PIXEL_HEIGHT = 32;
    private const int SCALE = 10;

    public nint Renderer { get; set; }
    public nint Window { get; set; }

    public bool[,] pixels;

    public Graphics_Old()
    {
        this.pixels = new bool[32, 64];

        // Init
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            throw new GraphicsException($"Failed to initialize SDL. {SDL.SDL_GetError()}");
        }

        // Window
        var window = SDL.SDL_CreateWindow(
            "Chip8 Emulator",
            SDL.SDL_WINDOWPOS_UNDEFINED,
            SDL.SDL_WINDOWPOS_UNDEFINED,
            PIXEL_WIDTH * SCALE,
            PIXEL_HEIGHT * SCALE,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (window == IntPtr.Zero)
        {
            throw new GraphicsException($"Failed to create window with SDL. {SDL.SDL_GetError()}");
        }

        // Create HW renderer
        var renderer = SDL.SDL_CreateRenderer(window,
            -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
            SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

        if (renderer == IntPtr.Zero)
        {
            throw new GraphicsException($"Failed to create rendered with SDL. {SDL.SDL_GetError()}");
        }

        // Initilizes SDL_image for use with png files.
        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
        {
            Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
        }

        // Sets the color that the screen will be cleared with.
        if (SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255) < 0)
        {
            Console.WriteLine($"There was an issue with setting the render draw color. {SDL.SDL_GetError()}");
        }

        // Clears the current render surface.
        if (SDL.SDL_RenderClear(renderer) < 0)
        {
            Console.WriteLine($"There was an issue with clearing the render surface. {SDL.SDL_GetError()}");
        }

        SDL.SDL_SetRenderDrawColor(this.Renderer, 255, 255, 255, 255);

        return;

        var running = true;

        // Main loop for the program
        while (running)
        {
            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                }
            }

            // Sets the color that the screen will be cleared with.
            if (SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255) < 0)
            {
                Console.WriteLine($"There was an issue with setting the render draw color. {SDL.SDL_GetError()}");
            }

            // Clears the current render surface.
            if (SDL.SDL_RenderClear(renderer) < 0)
            {
                Console.WriteLine($"There was an issue with clearing the render surface. {SDL.SDL_GetError()}");
            }

            // Switches out the currently presented render surface with the one we just did work on.
            SDL.SDL_RenderPresent(renderer);
        }

        // Clean up the resources that were created.
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }

    /// <summary>
    /// For each row in 
    /// <param name="height">Height in pixels to draw the sprite.</param>
    /// <param name="sprite"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public unsafe bool DrawSprite(ushort height, byte sprite, ushort x, ushort y)
    {
        var collision = false;

        // Draw for each pixel of height
        for (int i = 0; i < height; i++)
        {
            var Y = y + i;

            // Loop each bit in the sprite
            for (int b = 0; b < 8; b++)
            {
                var pixelValue = sprite & (0x80 >> b);
                var X = x + b;

                // If not 0, we flip the bit
                if (pixelValue != 0)
                {
                    if (this.pixels[Y,X] == true)
                    {
                        collision = true;
                    }

                    this.pixels[Y, X] = !this.pixels[Y, X];
                }
            }
        }

        // TODO: Code above can be integrated into this for setting pixels
        // Texture
        nint windowTexture = SDL.SDL_CreateTexture(
            this.Renderer, 
            SDL.SDL_PIXELFORMAT_RGBA8888,
            (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
            PIXEL_WIDTH * SCALE,
            PIXEL_HEIGHT * SCALE);

        var rect = new SDL.SDL_Rect();
        rect.x = 0;
        rect.y = 0;
        rect.w = PIXEL_WIDTH;
        rect.h = PIXEL_HEIGHT;

        SDL.SDL_LockTexture(
            windowTexture,
            ref rect,
            out var texturePixels,
            out int pitch);

        var pixelFormat = new SDL.SDL_PixelFormat();
        pixelFormat.format = SDL.SDL_PIXELFORMAT_RGBA8888;

        // White
        var white = SDL.SDL_MapRGB(
            Unsafe.As<SDL.SDL_PixelFormat, nint>(ref pixelFormat),
            0xFF,
            0xFF,
            0xFF);

        var black = SDL.SDL_MapRGB(
            Unsafe.As<SDL.SDL_PixelFormat, nint>(ref pixelFormat),
            0x0,
            0x0,
            0x0);

        uint* pix = (uint*)texturePixels;

        for (int tY = 0; tY < PIXEL_HEIGHT; tY++)
        {
            for (int tX = 0; tX < PIXEL_WIDTH; tX++)
            {
                UInt32 pos = (uint)(tY * (pitch / sizeof(uint)) + tX);
                if (this.pixels[tY,tX] == true)
                {
                    pix[pos] = white;
                }
                else
                {
                    pix[pos] = black;
                }
            }
        }

        nint pixNint = new nint(pix);
        SDL.SDL_UpdateTexture(
            windowTexture,
            ref rect,
            pixNint,
            pitch);
        
        SDL.SDL_RenderClear(this.Renderer);
        SDL.SDL_RenderCopy(this.Renderer, windowTexture, ref rect, ref rect);
        SDL.SDL_RenderPresent(this.Renderer);

        return collision;
    }

    public void Draw()
    {
        // TODO: This should set all the pixels before rendering..
        throw new NotImplementedException("Set the pixels first");

        SDL.SDL_RenderDrawPoint(this.Renderer, 1, 1);
    }

    public void ClearScreen()
    {
        // Sets the color that the screen will be cleared with.
        if (SDL.SDL_SetRenderDrawColor(this.Renderer, 0, 0, 0, 255) < 0)
        {
            Console.WriteLine($"There was an issue with setting the render draw color. {SDL.SDL_GetError()}");
        }

        // Clears the current render surface.
        if (SDL.SDL_RenderClear(this.Renderer) < 0)
        {
            Console.WriteLine($"There was an issue with clearing the render surface. {SDL.SDL_GetError()}");
        }

        SDL.SDL_SetRenderDrawColor(this.Renderer, 255, 255, 255, 255);
    }

    public void Destroy()
    {
        // Clean up the resources that were created.
        SDL.SDL_DestroyRenderer(this.Renderer);
        SDL.SDL_DestroyWindow(this.Window);
        SDL.SDL_Quit();
    }
}
