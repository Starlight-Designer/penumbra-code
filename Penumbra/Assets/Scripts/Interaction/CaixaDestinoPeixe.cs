
using UnityEngine;
using UnityEngine.EventSystems;

public class CaixaDestinoPeixe : MonoBehaviour, IDropHandler
{
    [Header("Espécie aceita")]
    public string especieAceita;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objetoArrastado = eventData.pointerDrag;

        if (objetoArrastado == null)
            return;

        PixeArrastavel peixe = objetoArrastado.GetComponent<PixeArrastavel>();

        if (peixe == null)
            return;

        if (peixe.especie == especieAceita)
        {
            peixe.AceitarNaCaixa(transform);

            Debug.Log("Peixe organizado corretamente!");
        }
        else
        {
            Debug.Log("Esse peixe não pertence a esta caixa!");
        }
    }
}
