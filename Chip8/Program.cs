
using Chip8.Engine;

using var graphics = new Graphics(640, 320, "Chip8");

var engine = new Engine();
engine.CPU.LoadProgram(@"C:\work\Emu\chip8_roms\roms\IBM Logo.ch8");
// graphics.Run(engine);

return;
var e = new Engine();
engine.CPU.LoadProgram(@"C:\work\Emu\chip8_roms\roms\IBM Logo.ch8");
engine.Run();