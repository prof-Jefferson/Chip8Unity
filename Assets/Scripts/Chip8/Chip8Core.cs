using UnityEngine;

public class Chip8Core : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Display Controller")]
    private DisplayController displayController;

    [SerializeField]
    private Chip8Input inputHandler;


    // Memória de 4KB do CHIP-8
    public byte[] memory = new byte[4096];

    // Registradores V0 a VF (8 bits cada)
    public byte[] V = new byte[16];

    // Registrador de endereços e contador de programa
    public ushort I = 0;

    // Registradores de temporização
    public byte delayTimer = 0;
    public byte soundTimer = 0;

    public ushort PC = 0x200; // Início do programa

    // Pilha e ponteiro da pilha
    public ushort[] stack = new ushort[16];
    public byte SP = 0;

    // Ciclo de execução contínua
    public bool isRunning = false;
    public float cycleDelay = 0.1f; // tempo entre ciclos
    private float timer = 0f;

    void Awake()
    {
        displayController = FindObjectOfType<DisplayController>();
    }

    void Start()
    {
        Debug.Log("CHIP-8 inicializado.");
        ClearMemory();

        // Teste manual (opcional)
            // memory[0x200] = 0x60;
            // memory[0x201] = 0x2A;
            // Cycle(); // Executa uma vez

        // Log dos primeiros 16 bytes da memória da ROM
        for (int i = 0x200; i < 0x210; i++)
        {
            Debug.Log($"ROM[{i:X3}] = {memory[i]:X2}");
        }
    }

    void Update()
    {
        // 1. Verifica se a tecla de controle foi pressionada
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRunning = !isRunning;
           Debug.Log($"Execução {(isRunning ? "Iniciada" : "Pausada")}");
        }

        // 2. Se não estiver rodando, sai da função
        if (!isRunning) return;

        // 3. Executa o ciclo com delay
        timer += Time.deltaTime;
        if (timer >= cycleDelay)
        {
            Cycle();
            timer = 0f;
        }
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

        Debug.Log($"[Cycle End] PC = {PC:X4}");
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
                    if (displayController != null)
                        displayController.ClearScreen();
                    else
                        Debug.LogWarning("DisplayController não está conectado.");
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

                    case 0x4:
                        int sum = V[x] + V[y];
                        V[0xF] = (byte)(sum > 255 ? 1 : 0);
                        V[x] = (byte)(sum & 0xFF);
                        Debug.Log($"Executando 8XY4 – V[{x}] += V[{y}], carry: {V[0xF]}");
                        break;

                    case 0x5:
                        V[0xF] = (byte)(V[x] >= V[y] ? 1 : 0);
                        V[x] = (byte)(V[x] - V[y]);
                        Debug.Log($"Executando 8XY5 – V[{x}] -= V[{y}], borrow: {V[0xF]}");
                        break;

                    case 0x6:
                        V[0xF] = (byte)(V[x] & 0x1);
                        V[x] >>= 1;
                        Debug.Log($"Executando 8XY6 – V[{x}] >>= 1, VF = {V[0xF]}");
                        break;

                    case 0x7:
                        V[0xF] = (byte)(V[y] >= V[x] ? 1 : 0);
                        V[x] = (byte)(V[y] - V[x]);
                        Debug.Log($"Executando 8XY7 – V[{x}] = V[{y}] - V[{x}], VF = {V[0xF]}");
                        break;

                    case 0xE:
                        V[0xF] = (byte)((V[x] & 0x80) >> 7);
                        V[x] <<= 1;
                        Debug.Log($"Executando 8XYE – V[{x}] <<= 1, VF = {V[0xF]}");
                        break;   

                    default:
                        Debug.LogWarning($"Opcode 8XY? não implementado: {opcode:X4}");
                        break;
                }
                break;
            
            // ------------------------------
            // 0x9XY0: Pula próxima se VX != VY
            // ------------------------------
            case 0x9000:
                if (n == 0 && V[x] != V[y])
                {
                    Debug.Log($"Executando 9XY0 – V[{x}] != V[{y}] → pulando");
                    PC += 2;
                }
                else
                {
                    Debug.Log($"Executando 9XY0 – V[{x}] == V[{y}] → continua");
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

            // BNNN: Jump para NNN + V0
            // ------------------------------
            case 0xB000:
                PC = (ushort)(nnn + V[0]);
                Debug.Log($"Executando BNNN – Jump para {nnn:X3} + V[0] ({V[0]}) = {PC:X3}");
                return;

            // ------------------------------
            // 0xCXNN: VX = random() & NN
            // ------------------------------
            case 0xC000:
                byte randomValue = (byte)Random.Range(0, 256);
                V[x] = (byte)(randomValue & nn);
                Debug.Log($"Executando CXNN – V[{x}] = rand({randomValue}) & {nn} = {V[x]}");
                break;

            // ------------------------------
            // 0xDXYN: Desenha sprite em (VX, VY) com N bytes a partir de I
            // ------------------------------
            case 0xD000:
                if (displayController != null)
                {
                    bool collision = displayController.DrawSprite(V[x], V[y], memory, I, n, V);
                    V[0xF] = (byte)(collision ? 1 : 0);
                    Debug.Log($"Executando DXYN – Desenha sprite em ({V[x]},{V[y]}), altura: {n}, colisão: {collision}");
                }
                else
                {
                    Debug.LogWarning("DisplayController não está conectado.");
                }
                break;

            // ------------------------------
            // 0xEX9E / 0xEXA1: Input de tecla
            // ------------------------------
            case 0xE000:
                switch (nn)
                {
                    case 0x9E:
                        if (inputHandler != null && inputHandler.IsKeyPressed(V[x]))
                        {
                            Debug.Log($"Executando EX9E – Tecla V[{x}] ({V[x]}) pressionada → pulando");
                            PC += 2;
                        }
                        else
                        {
                            Debug.Log($"Executando EX9E – Tecla V[{x}] ({V[x]}) não pressionada");
                        }
                        break;

                    case 0xA1:
                        if (inputHandler != null && inputHandler.IsKeyReleased(V[x]))
                        {
                            Debug.Log($"Executando EXA1 – Tecla V[{x}] ({V[x]}) não pressionada → pulando");
                            PC += 2;
                        }
                        else
                        {
                            Debug.Log($"Executando EXA1 – Tecla V[{x}] ({V[x]}) pressionada");
                        }
                        break;

                    default:
                        Debug.LogWarning($"Opcode E??? desconhecido: {opcode:X4}");
                        break;
                }
                break;

            // ------------------------------
            // 0xFX**: Instruções diversas
            // ------------------------------
            case 0xF000:
                switch (nn)
                {
                    case 0x0A:
                        if (inputHandler != null)
                        {
                            int key = inputHandler.GetPressedKey();
                            if (key != -1)
                            {
                                V[x] = (byte)key;
                                Debug.Log($"Executando FX0A – Tecla {key} pressionada → V[{x}] = {key}");
                            }
                            else
                            {
                                // Não avança o PC, espera até uma tecla ser pressionada
                                PC -= 2;
                                Debug.Log("Executando FX0A – Aguardando tecla ser pressionada...");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("inputHandler não está atribuído para FX0A");
                        }
                        break;

                    case 0x07:
                        V[x] = delayTimer;
                        Debug.Log($"Executando FX07 – V[{x}] = delayTimer ({delayTimer})");
                        break;

                    case 0x15:
                        delayTimer = V[x];
                        Debug.Log($"Executando FX15 – delayTimer = V[{x}] ({V[x]})");
                        break;

                    case 0x18:
                        soundTimer = V[x];
                        Debug.Log($"Executando FX18 – soundTimer = V[{x}] ({V[x]})");
                        break;

                    case 0x1E:
                        I += V[x];
                        Debug.Log($"Executando FX1E – I += V[{x}] ({V[x]}), novo I = {I:X3}");
                        break;

                    case 0x29:
                        // Fontes ficam geralmente no início da memória (5 bytes por caractere)
                        I = (ushort)(V[x] * 5);
                        Debug.Log($"Executando FX29 – I = endereço da fonte de V[{x}] = {V[x]} → I = {I:X3}");
                        break;

                    case 0x33:
                        // Armazena o BCD de VX em I, I+1 e I+2
                        byte value = V[x];
                        memory[I] = (byte)(value / 100);
                        memory[I + 1] = (byte)((value / 10) % 10);
                        memory[I + 2] = (byte)(value % 10);
                        Debug.Log($"Executando FX33 – BCD de V[{x}] = {value} → [{memory[I]},{memory[I+1]},{memory[I+2]}]");
                        break;

                    case 0x55:
                        for (int i = 0; i <= x; i++)
                        {
                            memory[I + i] = V[i];
                        }
                        Debug.Log($"Executando FX55 – Armazenando V[0] até V[{x}] na memória a partir de I");
                        break;

                    case 0x65:
                        for (int i = 0; i <= x; i++)
                        {
                            V[i] = memory[I + i];
                        }
                        Debug.Log($"Executando FX65 – Lendo memória para V[0] até V[{x}] a partir de I");
                        break;

                    default:
                        Debug.LogWarning($"Opcode F??? desconhecido: {opcode:X4}");
                        break;
                }
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
