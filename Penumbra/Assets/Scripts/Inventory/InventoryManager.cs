using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<string> itens = new List<string>();

    void Awake()
    {
        instance = this;
    }

    public void AdicionarItem(string nomeItem)
    {
        if (!itens.Contains(nomeItem))
        {
            itens.Add(nomeItem);
            Debug.Log(nomeItem + " foi adicionado ao inventário!");
        }
    }
}