using UnityEngine;

public class Chip8Input : MonoBehaviour
{
    // Mapeamento das teclas físicas para as teclas do CHIP-8
    public KeyCode[] KeyMap = new KeyCode[16]
    {
        KeyCode.X,      // 0
        KeyCode.Alpha1, // 1
        KeyCode.Alpha2, // 2
        KeyCode.Alpha3, // 3
        KeyCode.Q,      // 4
        KeyCode.W,      // 5
        KeyCode.E,      // 6
        KeyCode.A,      // 7
        KeyCode.S,      // 8
        KeyCode.D,      // 9
        KeyCode.Z,      // A
        KeyCode.C,      // B
        KeyCode.Alpha4, // C
        KeyCode.R,      // D
        KeyCode.F,      // E
        KeyCode.V       // F
    };

    // Retorna true se a tecla correspondente ao valor estiver pressionada
    public bool IsKeyPressed(byte key)
    {
        if (key < 16)
        {
            return Input.GetKey(KeyMap[key]);
        }
        return false;
    }

    // Retorna true se a tecla NÃO estiver pressionada
    public bool IsKeyReleased(byte key)
    {
        if (key < 16)
        {
            return !Input.GetKey(KeyMap[key]);
        }
        return true;
    }

    // Retorna o índice da tecla pressionada (0 a F), ou -1 se nenhuma
    public int GetPressedKey()
    {
        for (int i = 0; i < KeyMap.Length; i++)
        {
            if (Input.GetKey(KeyMap[i]))
                return i;
        }
        return -1;
    }
}
