
using UnityEngine;
using UnityEngine.EventSystems;

public class PixeArrastavel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string especie;

    private RectTransform rectTransform;
    private Vector3 posicaoOriginal;
    private Transform paiOriginal;
    private int ordemOriginal;
    private bool aceito = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posicaoOriginal = rectTransform.position;
        paiOriginal = transform.parent;
        ordemOriginal = transform.GetSiblingIndex();

        aceito = false;
        transform.SetAsLastSibling();
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
    }
}
