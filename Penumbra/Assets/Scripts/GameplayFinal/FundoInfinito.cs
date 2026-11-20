
using UnityEngine;

public class FundoInfinito : MonoBehaviour
{
    public Transform fundo1;
    public Transform fundo2;

    public float velocidade = 3f;

    private float largura;

    void Start()
    {
        SpriteRenderer sr = fundo1.GetComponent<SpriteRenderer>();

        largura = sr.bounds.size.x;

        // Mantém a posição original do fundo 1.
        // Apenas posiciona o fundo 2 ao lado dele.
        fundo2.position = new Vector3(
            fundo1.position.x + largura,
            fundo1.position.y,
            fundo1.position.z
        );
    }

    void Update()
    {
        fundo1.position += Vector3.left * velocidade * Time.deltaTime;
        fundo2.position += Vector3.left * velocidade * Time.deltaTime;

        if (fundo1.position.x <= -largura)
        {
            fundo1.position = new Vector3(
                fundo2.position.x + largura,
                fundo1.position.y,
                fundo1.position.z
            );
        }

        if (fundo2.position.x <= -largura)
        {
            fundo2.position = new Vector3(
                fundo1.position.x + largura,
                fundo2.position.y,
                fundo2.position.z
            );
        }
    }
}
