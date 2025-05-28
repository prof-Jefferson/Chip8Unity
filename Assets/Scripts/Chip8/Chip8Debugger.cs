using UnityEngine;

public class Chip8Debugger : MonoBehaviour
{
    public Chip8Core chip8;
    [Tooltip("Habilita a opção de debug para rodar instruções manuais")]
    public bool enableDebug = true;

    void Start()
    {
        if (!enableDebug) return;

        chip8.ClearMemory();

        // Mini-ROM escrita diretamente na memória
        chip8.memory[0x200] = 0x60; // V0 = 0x01
        chip8.memory[0x201] = 0x01;

        chip8.memory[0x202] = 0x61; // V1 = 0x05
        chip8.memory[0x203] = 0x05;

        chip8.memory[0x204] = 0x70; // V0 += 0x03
        chip8.memory[0x205] = 0x03;

        chip8.memory[0x206] = 0x12; // Jump para 0x202
        chip8.memory[0x207] = 0x02;

        // Rodar múltiplos ciclos
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"Ciclo {i + 1}");
            chip8.Cycle();
        }

        Debug.Log($"Resultado final: V0 = {chip8.V[0]}, V1 = {chip8.V[1]}, PC = {(int)chip8.PC:X}");
    }
}
