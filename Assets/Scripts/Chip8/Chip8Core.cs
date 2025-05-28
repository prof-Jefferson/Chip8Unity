using UnityEngine;

public class Chip8Core : MonoBehaviour
{
    // Memória de 4KB do CHIP-8
    public byte[] memory = new byte[4096];

    // Registradores V0 a VF (8 bits cada)
    public byte[] V = new byte[16];

    // Registrador de endereços e contador de programa
    public ushort I = 0;
    public ushort PC = 0x200; // Início do programa

    // Pilha e ponteiro da pilha
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
        for (int i = 0; i < stack.Length; i++) stack[i] = 0;

        I = 0;
        PC = 0x200;
        SP = 0;
    }

    public void Cycle()
    {
        ushort opcode = FetchOpcode();
        Debug.Log($"[Cycle] Opcode lido: {opcode:X4}");
        DecodeAndExecute(opcode);
    }

    public ushort FetchOpcode()
    {
        // Combina dois bytes da memória para formar o opcode de 16 bits
        byte highByte = memory[PC];
        byte lowByte = memory[PC + 1];
        return (ushort)((highByte << 8) | lowByte);
    }

    public void DecodeAndExecute(ushort opcode)
    {
        // Decodificação dos campos comuns
        ushort nnn = (ushort)(opcode & 0x0FFF);
        byte nn = (byte)(opcode & 0x00FF);
        byte n = (byte)(opcode & 0x000F);
        byte x = (byte)((opcode & 0x0F00) >> 8);
        byte y = (byte)((opcode & 0x00F0) >> 4);

        switch (opcode & 0xF000)
        {
            // ------------------------------
            // 0x0---: Instruções de sistema
            // ------------------------------
            case 0x0000:
                if (opcode == 0x00E0)
                {
                    Debug.Log("Executando 00E0 – Clear Screen");
                    // DisplayController.ClearScreen(); ← a ser implementado
                }
                else if (opcode == 0x00EE)
                {
                    SP--;
                    PC = stack[SP];
                    Debug.Log("Executando 00EE – Return from subroutine");
                    return; // Não incrementa PC
                }
                break;

            // ------------------------------
            // 0x1NNN: Jump para endereço NNN
            // ------------------------------
            case 0x1000:
                PC = nnn;
                Debug.Log($"Executando 1NNN – Jump para {nnn:X3}");
                return;

            // ------------------------------
            // 0x2NNN: Chamada de sub-rotina
            // ------------------------------
            case 0x2000:
                stack[SP] = (ushort)(PC + 2);
                SP++;
                PC = nnn;
                Debug.Log($"Executando 2NNN – Call sub-rotina {nnn:X3}");
                return;

            // ------------------------------
            // 0x3XNN: Pula próxima se VX == NN
            // ------------------------------
            case 0x3000:
                if (V[x] == nn)
                {
                    Debug.Log($"Executando 3XNN – V[{x}] == {nn} → pulando");
                    PC += 2;
                }
                else
                {
                    Debug.Log($"Executando 3XNN – V[{x}] != {nn} → continua");
                }
                break;

            // ------------------------------
            // 0x4XNN: Pula próxima se VX != NN
            // ------------------------------
            case 0x4000:
                if (V[x] != nn)
                {
                    Debug.Log($"Executando 4XNN – V[{x}] != {nn} → pulando");
                    PC += 2;
                }
                else
                {
                    Debug.Log($"Executando 4XNN – V[{x}] == {nn} → continua");
                }
                break;

            // ------------------------------
            // 0x5XY0: Pula próxima se VX == VY
            // ------------------------------
            case 0x5000:
                if (n == 0 && V[x] == V[y])
                {
                    Debug.Log($"Executando 5XY0 – V[{x}] == V[{y}] → pulando");
                    PC += 2;
                }
                else
                {
                    Debug.Log($"Executando 5XY0 – condição falsa → continua");
                }
                break;

            // ------------------------------
            // 0x6XNN: VX = NN
            // ------------------------------
            case 0x6000:
                V[x] = nn;
                Debug.Log($"Executando 6XNN – V[{x}] = {nn}");
                break;

            // ------------------------------
            // 0x7XNN: VX += NN
            // ------------------------------
            case 0x7000:
                V[x] += nn;
                Debug.Log($"Executando 7XNN – V[{x}] += {nn}");
                break;

            // ------------------------------
            // 0x8XY*: Operações entre registradores
            // ------------------------------
            case 0x8000:
                switch (opcode & 0x000F)
                {
                    case 0x0:
                        V[x] = V[y];
                        Debug.Log($"Executando 8XY0 – V[{x}] = V[{y}] → {V[y]}");
                        break;

                    case 0x1:
                        V[x] |= V[y];
                        Debug.Log($"Executando 8XY1 – V[{x}] |= V[{y}] → {V[x]}");
                        break;

                    case 0x2:
                        V[x] &= V[y];
                        Debug.Log($"Executando 8XY2 – V[{x}] &= V[{y}] → {V[x]}");
                        break;

                    case 0x3:
                        V[x] ^= V[y];
                        Debug.Log($"Executando 8XY3 – V[{x}] ^= V[{y}] → {V[x]}");
                        break;

                    default:
                        Debug.LogWarning($"Opcode 8XY? não implementado: {opcode:X4}");
                        break;
                }
                break;

            // ------------------------------
            // 0xANNN: I = NNN
            // ------------------------------
            case 0xA000:
                I = nnn;
                Debug.Log($"Executando ANNN – I = {nnn:X3}");
                break;

            // ------------------------------
            // Opcode desconhecido
            // ------------------------------
            default:
                Debug.LogWarning($"Opcode não implementado: {opcode:X4}");
                break;
        }

        // Avança para a próxima instrução (2 bytes)
        PC += 2;
    }
}
