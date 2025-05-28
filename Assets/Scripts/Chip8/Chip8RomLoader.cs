using System.IO;
using UnityEngine;

public class Chip8ROMLoader : MonoBehaviour
{
    public Chip8Core chip8;

    void Start()
    {
        string romFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Chip8Unity", "roms");
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
            for (int i = 0; i < romData.Length; i++)
                chip8.memory[0x200 + i] = romData[i];

            Debug.Log($"ROM carregada: {Path.GetFileName(fullPath)} ({romData.Length} bytes)");
        }
        catch (IOException e)
        {
            Debug.LogError($"Erro ao carregar ROM: {e.Message}");
        }
    }
}
