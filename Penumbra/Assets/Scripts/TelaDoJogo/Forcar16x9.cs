using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Forcar16x9 : MonoBehaviour
{
    private const float ProporcaoAlvo = 16f / 9f;
    private int ultimaLargura, ultimaAltura;

    void Start() { Ajustar(); }

    void Update()
    {
        if (Screen.width != ultimaLargura || Screen.height != ultimaAltura)
            Ajustar();
    }

    void Ajustar()
    {
        ultimaLargura = Screen.width;
        ultimaAltura = Screen.height;

        float proporcaoTela = (float)Screen.width / Screen.height;
        float escala = proporcaoTela / ProporcaoAlvo;
        Camera cam = GetComponent<Camera>();

        if (escala < 1f)
            cam.rect = new Rect(0f, (1f - escala) / 2f, 1f, escala);
        else
        {
            float largura = 1f / escala;
            cam.rect = new Rect((1f - largura) / 2f, 0f, largura, 1f);
        }
    }
}