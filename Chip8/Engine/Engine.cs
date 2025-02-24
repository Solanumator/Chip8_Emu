namespace Chip8.Engine;

public class Engine
{
    public CPU CPU { get; set; }

    public Engine()
    {
        this.CPU = new CPU();
    }

    public void Run()
    {
        this.CPU.RunProgram();
    }
}