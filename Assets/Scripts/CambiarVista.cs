using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Teclas 1/2/3: manda esa camara al panel grande
public class CambiarVista : MonoBehaviour
{
    public RawImage[] vistas;
    public Text[] etiquetas;
    public RenderTexture[] texturas;
    public string[] titulos;

    private int[] orden = { 0, 1, 2 };

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) Agrandar(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) Agrandar(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) Agrandar(2);
    }

    void Agrandar(int camara)
    {
        for (int i = 0; i < orden.Length; i++)
        {
            if (orden[i] != camara) continue;

            orden[i] = orden[0];
            orden[0] = camara;

            for (int j = 0; j < vistas.Length; j++)
            {
                vistas[j].texture = texturas[orden[j]];
                etiquetas[j].text = titulos[orden[j]];
            }
            return;
        }
    }
}
