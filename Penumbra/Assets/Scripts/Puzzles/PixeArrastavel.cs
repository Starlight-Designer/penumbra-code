
using UnityEngine;
using UnityEngine.EventSystems;

public class PixeArrastavel : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Peixe")]
    public string especie;

    [Header("Áudios")]
    public AudioSource fonteAudio;
    public AudioClip somPegarPeixe;
    public AudioClip somColocarPeixe;

    private RectTransform rectTransform;
    private Vector3 posicaoOriginal;
    private Transform paiOriginal;
    private int ordemOriginal;
    private bool aceito = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (fonteAudio == null)
            fonteAudio = GetComponent<AudioSource>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posicaoOriginal = rectTransform.position;
        paiOriginal = transform.parent;
        ordemOriginal = transform.GetSiblingIndex();

        aceito = false;
        transform.SetAsLastSibling();

        // Toca o som de pegar o peixe.
        TocarSom(somPegarPeixe);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!aceito)
        {
            transform.SetParent(paiOriginal);
            rectTransform.position = posicaoOriginal;
            transform.SetSiblingIndex(ordemOriginal);
        }
    }

    public void AceitarNaCaixa(Transform caixa)
    {
        aceito = true;

        transform.SetParent(caixa);
        rectTransform.position = caixa.position;
        transform.SetSiblingIndex(1);

        // Toca o som de colocar o peixe na caixa.
        TocarSom(somColocarPeixe);
    }

    private void TocarSom(AudioClip som)
    {
        if (fonteAudio != null && som != null)
        {
            fonteAudio.PlayOneShot(som);
        }
    }
}
