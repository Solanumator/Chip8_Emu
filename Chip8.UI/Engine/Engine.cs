using Chip8.UI;
using System.CodeDom.Compiler;

namespace Chip8.Engine;

public class Engine
{
    public CPU CPU { get; set; }
    
    private bool IsRunning = true;

    public Engine(Form1 form)
    {
        this.CPU = new CPU(form);
    }

    public void Start()
    {
        this.CPU.LoadProgram(@"C:\work\Emu\chip8_roms\roms\IBM Logo.ch8");

        // TODO: Implement this with interrupts and such
        Task t = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
    }

    private void Run()
    {
        while (this.IsRunning)
        {
            this.CPU.RunProgram();
        }
    }
}