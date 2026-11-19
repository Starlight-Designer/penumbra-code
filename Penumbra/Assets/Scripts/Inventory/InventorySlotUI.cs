
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventorySlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Identificação do espaço")]
    public int indiceSlot = 0;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = InventoryManager.instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(
            "Mouse entrou no slot: " + gameObject.name +
            " | Índice: " + indiceSlot
        );

        if (inventoryManager == null)
            inventoryManager = InventoryManager.instance;

        if (inventoryManager != null)
            inventoryManager.MostrarDescricao(indiceSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventoryManager != null)
            inventoryManager.EsconderDescricao();
    }
}
