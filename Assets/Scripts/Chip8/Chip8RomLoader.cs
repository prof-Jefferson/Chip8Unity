using System.IO;
using UnityEngine;

public class Chip8ROMLoader : MonoBehaviour
{
    private const int ProgramStartAddress = 0x200;

    public Chip8Core chip8;

    void Start()
    {
        if (chip8 == null)
            chip8 = FindFirstObjectByType<Chip8Core>();

        if (chip8 == null)
        {
            Debug.LogError("Chip8Core nao encontrado para carregar a ROM.");
            return;
        }

        string romFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
            "Chip8Unity", "roms");
        Directory.CreateDirectory(romFolder);

        string[] romFiles = Directory.GetFiles(romFolder, "*.ch8");
        if (romFiles.Length == 0)
        {
            Debug.LogWarning($"Nenhuma ROM encontrada em: {romFolder}");
            return;
        }

        string romPath = romFiles[0]; // Carrega a primeira ROM encontrada
        LoadROM(romPath);
    }

    public void LoadROM(string fullPath)
    {
        try
        {
            byte[] romData = File.ReadAllBytes(fullPath);

            if (romData.Length > chip8.memory.Length - ProgramStartAddress)
            {
                Debug.LogError($"ROM muito grande: {romData.Length} bytes. Maximo suportado: {chip8.memory.Length - ProgramStartAddress} bytes.");
                return;
            }

            for (int i = 0; i < romData.Length; i++)
                chip8.memory[ProgramStartAddress + i] = romData[i];

            Debug.Log($"ROM carregada: {Path.GetFileName(fullPath)} ({romData.Length} bytes)");
        }
        catch (IOException e)
        {
            Debug.LogError($"Erro ao carregar ROM: {e.Message}");
        }
    }
}
