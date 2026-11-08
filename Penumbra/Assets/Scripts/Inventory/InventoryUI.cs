using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject painelInventario;
    public TextMeshProUGUI listaItens;

    void Start()
    {
        painelInventario.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            painelInventario.SetActive(!painelInventario.activeSelf);

            AtualizarInventario();
        }
    }

    void AtualizarInventario()
    {
        listaItens.text = "";

        foreach (string item in InventoryManager.instance.itens)
        {
            listaItens.text += item + "\n";
        }

        if (listaItens.text == "")
        {
            listaItens.text = "Nenhum item";
        }
    }
}