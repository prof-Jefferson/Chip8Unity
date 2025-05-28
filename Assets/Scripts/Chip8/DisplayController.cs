using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public GameObject pixelOnPrefab;
    public GameObject pixelOffPrefab;

    private const int width = 64;
    private const int height = 32;

    private GameObject[,] pixelGrid = new GameObject[width, height];

    [Range(0f, 0.2f)]
    public float pixelSpacing = 0.015f; // Espaço entre os pixels

    void Start()
    {
        GenerateDisplay();
        ClearScreen();
    }

    /// <summary>
    /// Gera os 2048 pixels na tela usando pixelOffPrefab.
    /// </summary>
    void GenerateDisplay()
    {
        float pixelSize = 0.08f; // Tamanho dos pixels
        float spacing = pixelSize + pixelSpacing;

        float totalWidth = spacing * width;
        float totalHeight = spacing * height;

        float startX = -totalWidth / 2f + spacing / 2f;
        float startY = totalHeight / 2f - spacing / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(
                    startX + x * spacing,
                    startY - y * spacing
                );

                GameObject pixel = Instantiate(pixelOffPrefab, position, Quaternion.identity, this.transform);
                pixel.transform.localScale = Vector3.one * pixelSize;
                pixel.name = $"Pixel_{x}_{y}";
                pixelGrid[x, y] = pixel;
            }
        }
    }

    /// <summary>
    /// Define o estado de um pixel (aceso ou apagado).
    /// </summary>
    public void SetPixel(int x, int y, bool state)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;

        // Salva posição antes de destruir
        Vector3 pos = pixelGrid[x, y].transform.position;

        Destroy(pixelGrid[x, y]);

        GameObject newPixel = Instantiate(
            state ? pixelOnPrefab : pixelOffPrefab,
            pos,
            Quaternion.identity,
            this.transform
        );

        newPixel.transform.localScale = Vector3.one * 0.08f;
        newPixel.name = $"Pixel_{x}_{y}";
        pixelGrid[x, y] = newPixel;
    }

    /// <summary>
    /// Apaga todos os pixels da tela.
    /// </summary>
    public void ClearScreen()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                SetPixel(x, y, false);
    }

    /// <summary>
    /// Método futuro para desenhar sprites na tela.
    /// </summary>
    public void DrawSprite(int x, int y, byte[] memory, ushort I, byte height)
    {
        // Ainda será implementado — corresponde ao opcode DXYN
    }
}
