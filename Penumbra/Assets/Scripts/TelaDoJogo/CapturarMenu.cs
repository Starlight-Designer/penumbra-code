using UnityEngine;

public class CapturarMenu : MonoBehaviour
{
    void Start() { Invoke(nameof(Tirar), 1f); }

    void Tirar()
    {
        ScreenCapture.CaptureScreenshot("menu_captura.png");
        Debug.Log("Foto salva na pasta do projeto");
    }
}