using UnityEngine;

public class Chip8Core : MonoBehaviour
{
    public byte[] memory = new byte[4096];
    public byte[] V = new byte[16];
    public ushort I = 0;
    public ushort PC = 0x200;

    public ushort[] stack = new ushort[16];
    public byte SP = 0;

    void Start()
    {
        Debug.Log("CHIP-8 inicializado.");
        ClearMemory();

        // Exemplo: carrega instrução 6XNN (V0 = 42)
        memory[0x200] = 0x60; // 6X
        memory[0x201] = 0x2A; // NN = 0x2A = 42

        Cycle(); // Executa uma vez
    }

    public void ClearMemory()
    {
        for (int i = 0; i < memory.Length; i++) memory[i] = 0;
        for (int i = 0; i < V.Length; i++) V[i] = 0;
        I = 0;
        PC = 0x200;
        SP = 0;
        for (int i = 0; i < stack.Length; i++) stack[i] = 0;
    }

    public void Cycle()
    {
        ushort opcode = FetchOpcode();
        Debug.Log($"[Cycle] Opcode lido: {opcode:X4}");
        DecodeAndExecute(opcode);
    }

    public ushort FetchOpcode()
    {
        // Lê dois bytes consecutivos e forma um opcode de 16 bits
        byte highByte = memory[PC];
        byte lowByte = memory[PC + 1];
        return (ushort)((highByte << 8) | lowByte);
    }

    public void DecodeAndExecute(ushort opcode)
    {
        ushort nnn = (ushort)(opcode & 0x0FFF);
        byte  nn  = (byte)(opcode & 0x00FF);
        byte  n   = (byte)(opcode & 0x000F);
        byte  x   = (byte)((opcode & 0x0F00) >> 8);
        byte  y   = (byte)((opcode & 0x00F0) >> 4);

        switch (opcode & 0xF000)
        {
            case 0x0000:
                if (opcode == 0x00E0)
                {
                    Debug.Log("Executando 00E0 – Clear Screen");
                    // DisplayController.ClearScreen(); ← em breve
                }
                else if (opcode == 0x00EE)
                {
                    SP--;
                    PC = stack[SP];
                    Debug.Log("Executando 00EE – Return from subroutine");
                    return; // Não incrementar PC
                }
                break;

            case 0x1000:
                PC = nnn;
                Debug.Log($"Executando 1NNN – Jump para {nnn:X3}");
                return; // Não incrementar PC

            case 0x2000:
                stack[SP] = PC + 2;
                SP++;
                PC = nnn;
                Debug.Log($"Executando 2NNN – Chamada de sub-rotina para {nnn:X3}");
                return;

            case 0x6000:
                V[x] = nn;
                Debug.Log($"Executando 6XNN – V[{x}] = {nn}");
                break;

            case 0x7000:
                V[x] += nn;
                Debug.Log($"Executando 7XNN – V[{x}] += {nn}");
                break;

            case 0xA000:
                I = nnn;
                Debug.Log($"Executando ANNN – I = {nnn:X3}");
                break;

            default:
                Debug.LogWarning($"Opcode não implementado: {opcode:X4}");
                break;
        }

        PC += 2;
    }
}
