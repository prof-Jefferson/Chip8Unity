using UnityEngine;

public class Chip8Debugger : MonoBehaviour
{
    public Chip8Core chip8;

    void Start()
    {
        if (chip8 == null)
        {
            Debug.LogError("Chip8Core não atribuído ao debugger.");
            return;
        }

        // Simula o carregamento de dados
        chip8.memory[0x200] = 0x60;  // opcode parte 1
        chip8.memory[0x201] = 0x0A;  // opcode parte 2 (6XNN = set VX)

        chip8.V[0] = 42;
        chip8.I = 0x300;

        Debug.Log($"Teste: V0 = {chip8.V[0]}, I = {chip8.I}, PC = {chip8.PC:X}");
        Debug.Log($"Memória @ 0x200 = {chip8.memory[0x200]:X2} {chip8.memory[0x201]:X2}");
    }
}
