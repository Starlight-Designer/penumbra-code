
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectorHoverVHS : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public AnimacaoItemVHS animacaoVHS;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animacaoVHS != null)
            animacaoVHS.IniciarRotacao();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animacaoVHS != null)
            animacaoVHS.PararRotacao();
    }
}
