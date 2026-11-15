using UnityEngine;

public class AnimacaoIndicador : MonoBehaviour
{
    public float altura = 0.15f;
    public float velocidade = 2f;

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.localPosition;
    }

    void Update()
    {
        float movimento = Mathf.Sin(Time.time * velocidade) * altura;

        transform.localPosition = posicaoInicial +
            new Vector3(0f, movimento, 0f);
    }
}