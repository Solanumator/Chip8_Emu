public namespace Chip8.Engine;

public class CPU
{
    // Memory
    private byte[] Memory;

    // Registers
    private ushort PC; // Program Counter
    private ushort I; // Used to point at memory locations

    // Variable registers - VF is used as a flag register (e.g. carry flag)
    private byte V0, V1, V2, V3, V4, V5, V6, V7, V8, V9, VA, VB, VC, VD, VE, VF;

    // Stack
    private Stack<ushort> Stack;

    public CPU()
    {
        this.Memory = byte[4096];
        this.SetFont();
    }

    public void LoadProgram()
    {
        throw new NotImplementedException("This should load the program into memory address 0x200");
    }

    private void SetFont()
    {
        byte[] font = new [
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        ];

        for (int i = 0; i < font.Length(); i++)
        {
            this.Memory[i + 0x050] = font[i];
        }
    }
}