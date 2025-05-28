using UnityEngine;

public class Chip8Core : MonoBehaviour
{
    public byte[] memory = new byte[4096];   // RAM do CHIP-8
    public byte[] V = new byte[16];          // Registradores V0 até VF
    public ushort I = 0;                     // Registrador de endereços (16 bits)
    public ushort PC = 0x200;                // Program Counter (início do código)

    public ushort[] stack = new ushort[16];  // Stack de 16 níveis
    public byte SP = 0;                      // Stack Pointer

    void Start()
    {
        Debug.Log("CHIP-8 inicializado.");
        ClearMemory();
    }

    public void ClearMemory()
    {
        for (int i = 0; i < memory.Length; i++)
            memory[i] = 0;

        for (int i = 0; i < V.Length; i++)
            V[i] = 0;

        I = 0;
        PC = 0x200;
        SP = 0;

        for (int i = 0; i < stack.Length; i++)
            stack[i] = 0;
    }
}
