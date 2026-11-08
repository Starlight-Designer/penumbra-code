using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<string> itens = new List<string>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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