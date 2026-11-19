
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimacaoItemVHS : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Rotação")]
    public float velocidadeRotacao = 90f;
    public float velocidadeRetorno = 8f;

    [Header("Flutuação")]
    public float alturaFlutuacao = 0.15f;
    public float velocidadeFlutuacao = 2f;

    [Header("Modo de animação")]
    public bool girarSomenteComMouse = false;
    public bool flutuar = true;

    [Header("Objeto visual")]
    public Transform objetoVisual;

    private Vector3 posicaoInicial;
    private Quaternion rotacaoOriginal;

    private bool mouseSobreIcone = false;

    void Start()
    {
        posicaoInicial = transform.localPosition;

        if (objetoVisual == null)
            objetoVisual = transform;

        rotacaoOriginal = objetoVisual.localRotation;
    }

    void Update()
    {
        // Rotação durante a inspeção
        if (!girarSomenteComMouse || mouseSobreIcone)
        {
            objetoVisual.Rotate(
                0f,
                velocidadeRotacao * Time.deltaTime,
                0f,
                Space.Self
            );
        }
        else
        {
            // Retorna à rotação original quando o mouse sai
            objetoVisual.localRotation = Quaternion.Slerp(
                objetoVisual.localRotation,
                rotacaoOriginal,
                velocidadeRetorno * Time.deltaTime
            );
        }

        // Flutuação
        if (flutuar)
        {
            float movimento =
                Mathf.Sin(Time.time * velocidadeFlutuacao)
                * alturaFlutuacao;

            transform.localPosition = posicaoInicial +
                new Vector3(0f, movimento, 0f);
        }
    }

    public void IniciarRotacao()
    {
        mouseSobreIcone = true;
    }

    public void PararRotacao()
    {
        mouseSobreIcone = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseSobreIcone = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseSobreIcone = false;
    }
}
