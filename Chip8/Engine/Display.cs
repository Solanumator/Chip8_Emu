namespace Chip8.Engine;

public class Display
{
    public bool[,] pixels;

    public Display()
    {
        pixels = new bool[32, 64];
    }
}
