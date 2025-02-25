using Chip8.UI;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Chip8.Engine;

public class CPU
{
    const bool DEBUG = true;

    public bool IsFileLoaded { get; private set; }

    // Graphics
    private Graphics graphics;

    // Memory
    private byte[] Memory;

    // Registers
    private ushort PC; // Program Counter
    private ushort I; // Used to point at memory locations

    // Variable registers - VF is used as a flag register (e.g. carry flag)
    private byte[] V;
    private byte VF { get => V[0xF]; set => V[0xF] = value; }

    private byte SP;

    // Timers
    private byte DelayTimer;
    private byte SoundTimer;

    // Stack
    private Stack<ushort> Stack;

    public CPU(Form1 form)
    {
        this.graphics = new Graphics(form);
        Stack = new Stack<ushort>();
        this.Memory = new byte[4096];
        this.PC = 0x200;
        this.V = new byte[16];
        this.SetFont();
    }

    public void LoadProgram(string path)
    {
        var fileContents = File.ReadAllBytes(path);
        
        for (int i = 0; i < fileContents.Length; i++)
        {
            this.Memory[i + 0x200] = fileContents[i];
        }

        this.IsFileLoaded = true;
    }

    public void Reset()
    {
        Stack = new Stack<ushort>();
        this.Memory = new byte[4096];
        this.PC = 0x200;
        this.V = new byte[16];
        this.SetFont();
    }

    public void RunProgram()
    {
        // We handle 2 bytes at a time by shifting the first 8 bits to the left,
        // and 'or'ing with the second byte to add set the last 8 bits.
        var opCode = (ushort)(this.Memory[this.PC] << 8 | this.Memory[this.PC + 1]);
        this.PC += 2;

        // Split out the nibbles before entering the switch. For the second and third
        // nibblewe have to shift the bits since we will have 0's after the required
        // bits.
        // e.g. Getting Y for OpCode: 1111 0101 *1001* 0101
        // We expect to get 1001, but after performing & we will have 1001 0000
        // So we shift 4 places to the right to get rid of those 0000's

        // Second Nibble (bits 5-8)
        byte X = (byte)((opCode & 0xF00) >> 8);
        // Third Nibble (bits 9-12)
        byte Y = (byte)((opCode & 0xF0) >> 4);
        // Fourth Nibble (bits 13-16)
        byte N = (byte)(opCode & 0xF);
        // Second byte (bits 9-16)
        byte NN = (byte)(opCode & 0xFF);
        // Second, Third and Fourth Nibbles
        ushort NNN = (ushort)(opCode & 0xFFF);

        // Debug
        if (DEBUG)
        {
            Console.WriteLine($"OpCode: {opCode:X2} | {Convert.ToString(opCode, 2)}");
        }

        // We 'and' this to get the first 4 bytes of the 16 bits, which is what
        // we initially compare to get the operation.
        switch (opCode & 0xF000)
        {
            case 0x0000 when opCode == 0x00E0:
                this.graphics.ClearScreen();
                break;
            case 0x0000:
                break;
            case 0x1000:
                this.PC = NNN;
                break;
            case 0x2000:
                this.SP++;
                this.Stack.Push(this.PC);
                this.PC = NNN;
                break;
            case 0x3000:
                if (V[X] == NN) this.PC += 2;
                break;
            case 0x4000:
                if (V[Y] != NN) this.PC += 2;
                break;
            case 0x5000:
                if (V[X] == V[Y]) this.PC += 2;
                break;
            case 0x6000:
                this.SetXNN(X, NN);
                break;
            case 0x7000:
                this.AddXNN(X, NN);
                break;
            case 0x8000 when (opCode & 0x000F) == 1:
                V[X] = (byte)(V[X] | V[Y]);
                break;
            case 0x8000 when (opCode & 0x000F) == 2:
                V[X] = (byte)(V[X] & V[Y]);
                break;
            case 0x8000 when (opCode & 0x000F) == 3:
                V[X] = (byte)(V[X] ^ V[Y]);
                break;
            case 0x8000 when (opCode & 0x000F) == 5:
                this.SubXY(X, Y);
                break;
            case 0x8000 when (opCode & 0x000F) == 6:
                VF = (byte)(V[X] & 0x01);
                V[X] >>= 2;
                break;
            case 0x8000 when (opCode & 0x00F) == 7:
                this.SubNXY(X, Y);
                break;
            case 0x8000 when (opCode & 0x00F) == 0xE:
                this.XYE(X, Y);
                break;
            case 0x9000:
                if (V[X] != V[Y]) this.PC += 2;
                break;
            case 0xA000:
                this.I = NNN;
                break;
            case 0xD000:
                this.DrawSprite(X, Y, N);
                break;
            case 0xF000 when (opCode & 0x00FF) == 0x1E:
                I += V[X];
                break;
            case 0xF000 when (opCode & 0x00FF) == 0x29:
                this.FX29(X);
                break;
            case 0xF000 when (opCode & 0x00FF) == 0x33:
                this.FX33(X);
                break;
            case 0xF000 when (opCode & 0x00FF) == 0x55:
                this.FX55(X);
                break;
            case 0xF000 when (opCode & 0x00FF) == 0x65:
                this.FX65(X);
                break;
            default:
                // Output the OpCode that isn't implemented
                Console.WriteLine($"Next OpCode: Switch: {(opCode & 0xF000):X2} | Raw Opcode: {opCode:X2} | Bin: {Convert.ToString(opCode, 2)}");
                throw new Exception($"Next OpCode: Switch: {(opCode & 0xF000):X2} | Raw Opcode: {opCode:X2} | Bin: {Convert.ToString(opCode, 2)}");
        }
    }

    private void FX33(ushort x)
    {
        // Split into 100's, 10's, 1 and store in relevant memory location
        this.Memory[I] = (byte)((V[x] / 100) % 10);
        this.Memory[I + 1] = (byte)((V[x] / 10) % 10);
        this.Memory[I + 2] = (byte)(V[x] % 10);
    }

    private void FX65(ushort x)
    {
        for (int i = 0; i <= x; i++)
        {
            V[i] = this.Memory[I + i];
        }
    }

    private void FX55(ushort x)
    {
        var initialAddress = I;
        for (int i = 0; i <= x; i++)
        {
            this.Memory[initialAddress + i] = V[i];
        }
    }

    private void FX29(ushort x)
    {
        I = (byte)(0x050 + (V[x] * 5));
    }

    private void XYE(ushort x, ushort y)
    {
        // Get most significant bit
        var msb = (V[x] & 0x80) >> 7;

        // Assign VF based on whether msb is 1 or 0
        VF = (byte)msb;
        
        // Multiply by 2 (shifting 1 place left achieves this)
        V[x] <<= 1;
    }

    private void SubNXY(ushort x, ushort y)
    {
        VF = (byte)(V[y] > V[x] ? 1 : 0);
        V[x] = (byte)(V[y] - V[x]);
    }

    private void SubXY(ushort x, ushort y)
    {
        this.VF = (byte)(V[x] > V[y] ? 1 : 0);
        V[x] = (byte)(V[x] - V[y]);
    }

    /// <summary>
    /// Display N byte sprite (starting at memory location I) at (X,Y).
    /// Sprites are XOR'd to screen, setting VF=1 if any pixels are erased, VF=0 otherwise.
    /// </summary>
    /// <param name="X">X Location.</param>
    /// <param name="Y">Y Location.</param>
    /// <param name="N">Height.</param>
    private void DrawSprite(byte X, byte Y, ushort N)
    {
        this.V[0xF] = 0;
        var spriteBytes = new ArraySegment<byte>(this.Memory, this.I, this.I + N);
        var collision = this.graphics.DrawSprite(N, spriteBytes.ToArray(), (ushort)(this.V[X] % 64), (ushort)(this.V[Y] % 32));
        this.V[0xF] = (byte)(collision ? 0 : 1);
    }
    public void AddXNN(byte X, byte NN) => this.V[X] += NN;
    
    public void SetXNN(ushort X, ushort NN) => this.V[X] = (byte)NN;

    private void SetFont()
    {
        byte[] font = {
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
            0xF0, 0x80, 0xF0, 0x80, 0x80, // F
        };

        for (int i = 0; i < font.Length; i++)
        {
            this.Memory[i + 0x050] = font[i];
        }
    }
}