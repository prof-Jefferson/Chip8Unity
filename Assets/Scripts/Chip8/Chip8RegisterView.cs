using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chip8RegisterView : MonoBehaviour
{
    [SerializeField]
    private Chip8Core chip8;

    [SerializeField]
    private TextMeshProUGUI registerText;

    private readonly StringBuilder builder = new StringBuilder(256);

    void Awake()
    {
        if (chip8 == null)
            chip8 = FindFirstObjectByType<Chip8Core>();

        if (registerText == null)
            registerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (chip8 == null || registerText == null)
            return;

        registerText.text = BuildRegisterText();
    }

    private string BuildRegisterText()
    {
        builder.Clear();

        builder.Append("PC: ");
        builder.Append(chip8.PC.ToString("X4"));
        builder.Append(" | I: ");
        builder.Append(chip8.I.ToString("X4"));
        builder.Append(" | SP: ");
        builder.Append(chip8.SP.ToString("X2"));
        builder.Append(" | DT: ");
        builder.Append(chip8.delayTimer.ToString("X2"));
        builder.Append(" | ST: ");
        builder.Append(chip8.soundTimer.ToString("X2"));
        builder.AppendLine();

        for (int i = 0; i < chip8.V.Length; i++)
        {
            builder.Append('V');
            builder.Append(i.ToString("X1"));
            builder.Append(": ");
            builder.Append(chip8.V[i].ToString("X2"));

            if (i < chip8.V.Length - 1)
                builder.Append("  ");
        }

        return builder.ToString();
    }
}
