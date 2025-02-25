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

    public void LoadRom(string path)
    {
        this.CPU.LoadProgram(path);
    }

    public bool Start()
    {
        if (!this.CPU.IsFileLoaded)
        {
            return false;
        }

        this.CPU.LoadProgram(@"C:\work\Emu\chip8_roms\roms\IBM Logo.ch8");

        // TODO: Implement this with interrupts and such
        Task t = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        return true;
    }

    private void Run()
    {
        while (this.IsRunning)
        {
            this.CPU.RunProgram();
        }
    }
}