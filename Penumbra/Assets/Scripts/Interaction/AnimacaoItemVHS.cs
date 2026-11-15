using UnityEngine;

public class AnimacaoItemVHS : MonoBehaviour
{
    [Header("Rotação")]
    public float velocidadeRotacao = 90f;

    [Header("Flutuação")]
    public float alturaFlutuacao = 0.15f;
    public float velocidadeFlutuacao = 2f;

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.localPosition;
    }

    void Update()
    {
        // Gira a fita continuamente
        transform.Rotate(0f, velocidadeRotacao * Time.deltaTime, 0f);

        // Faz a fita flutuar
        float movimento = Mathf.Sin(Time.time * velocidadeFlutuacao)
                          * alturaFlutuacao;

        transform.localPosition = posicaoInicial +
            new Vector3(0f, movimento, 0f);
    }
}