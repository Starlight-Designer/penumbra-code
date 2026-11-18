
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Itens do inventário")]
    public List<string> itens = new List<string>();

    [Header("Interface do inventário")]
    public GameObject painelInventario;
    public TextMeshProUGUI textoListaItens;

    private bool interfaceEncontrada = false;
    private bool inventarioAberto = false;

    private void Awake()
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

    private void OnEnable()
    {
        if (instance == this)
            SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private void Start()
    {
        EncontrarInterface();

        inventarioAberto = false;

        if (painelInventario != null)
            painelInventario.SetActive(false);

        AtualizarTextoInventario();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AlternarInventario();
        }
    }

    private void AlternarInventario()
    {
        if (!interfaceEncontrada || painelInventario == null)
            EncontrarInterface();

        if (painelInventario == null)
        {
            Debug.LogError("InventoryManager: Painel_Inventario não encontrado!");
            return;
        }

        // ABRIR INVENTÁRIO
        if (!inventarioAberto)
        {
            // Ativa os objetos pais do painel
            Transform pai = painelInventario.transform.parent;

            while (pai != null)
            {
                if (!pai.gameObject.activeSelf)
                    pai.gameObject.SetActive(true);

                pai = pai.parent;
            }

            painelInventario.SetActive(true);
            inventarioAberto = true;

            AtualizarTextoInventario();

            Debug.Log("Inventário aberto!");
        }
        // FECHAR INVENTÁRIO
        else
        {
            painelInventario.SetActive(false);
            inventarioAberto = false;

            Debug.Log("Inventário fechado!");
        }

        Debug.Log("Painel ativo na Hierarchy: " + painelInventario.activeSelf);
        Debug.Log("Painel visível na cena: " + painelInventario.activeInHierarchy);
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        painelInventario = null;
        textoListaItens = null;
        interfaceEncontrada = false;
        inventarioAberto = false;

        EncontrarInterface();
        AtualizarTextoInventario();

        if (painelInventario != null)
            painelInventario.SetActive(false);
    }

    private void EncontrarInterface()
    {
        painelInventario = null;
        textoListaItens = null;

        GameObject[] objetos = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in objetos)
        {
            if (obj.name == "Painel_Inventario" &&
                obj.scene == SceneManager.GetActiveScene())
            {
                painelInventario = obj;
                break;
            }
        }

        if (painelInventario != null)
        {
            Transform lista = painelInventario.transform.Find("Titulo_Inventario/Lista_Itens");

            if (lista != null)
                textoListaItens = lista.GetComponentInChildren<TextMeshProUGUI>(true);

            if (textoListaItens == null)
                textoListaItens = painelInventario.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        interfaceEncontrada = painelInventario != null;

        Debug.Log("Interface do inventário encontrada: " + interfaceEncontrada);
    }

    public void AdicionarItem(string nomeItem)
    {
        if (!itens.Contains(nomeItem))
        {
            itens.Add(nomeItem);
            Debug.Log(nomeItem + " foi adicionado ao inventário!");

            AtualizarTextoInventario();
        }
    }

    private void AtualizarTextoInventario()
    {
        if (textoListaItens == null)
            EncontrarInterface();

        if (textoListaItens == null)
            return;

        if (itens.Count == 0)
        {
            textoListaItens.text = "Nenhum item";
            return;
        }

        StringBuilder lista = new StringBuilder();

        foreach (string item in itens)
        {
            if (lista.Length > 0)
                lista.AppendLine();

            lista.Append("• ");
            lista.Append(item);
        }

        textoListaItens.text = lista.ToString();
    }
}
