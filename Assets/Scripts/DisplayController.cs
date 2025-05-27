using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public GameObject pixelOnPrefab;
    public GameObject pixelOffPrefab;

    private const int width = 64;
    private const int height = 32;
    private GameObject[,] pixelGrid = new GameObject[width, height];

    public float pixelSpacing = 0.1f; // Espaçamento opcional entre pixels

    void Start()
    {
        GenerateDisplay();

        // TESTE EDUCACIONAL APENAS PARA MOSTRAR O FUNCIONAMENTO DO SCRIPT
        SetPixel(0, 0, true); // Acende pixel no canto superior esquerdo
        SetPixel(10, 10, true); // Outro pixel aceso

    }

    void GenerateDisplay()
    {
        float pixelSize = 0.08f;   // tamanho do pixel
        float pixelGap = 0.015f;   // espaço entre pixels

        float spacing = pixelSize + pixelGap;

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

    public void SetPixel(int x, int y, bool state)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;

        Destroy(pixelGrid[x, y]);

        GameObject newPixel = Instantiate(
            state ? pixelOnPrefab : pixelOffPrefab,
            pixelGrid[x, y].transform.position,
            Quaternion.identity,
            this.transform
        );

        newPixel.name = $"Pixel_{x}_{y}";
        pixelGrid[x, y] = newPixel;
    }

    public void ClearScreen()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                SetPixel(x, y, false);
    }
}
