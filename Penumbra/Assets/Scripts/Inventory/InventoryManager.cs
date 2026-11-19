
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class InformacaoItemInventario
{
    public string nomeItem;

    [TextArea(2, 5)]
    public string descricao;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public static bool InventarioAberto { get; private set; }

    private const string CHAVE_INVENTARIO = "InventarioItens";

    private readonly string[] nomesFitas =
    {
        "Fita VHS",
        "Fita VHS 2",
        "Fita VHS 3"
    };

    [Header("Itens do inventário")]
    public List<string> itens = new List<string>();

    [Header("Interface do inventário")]
    public GameObject painelInventario;
    public TextMeshProUGUI textoDescricao;

    [Header("Ícones das fitas")]
    public GameObject[] iconesItens = new GameObject[3];

    [Header("Descrições dos itens")]
    public List<InformacaoItemInventario> informacoesItens =
        new List<InformacaoItemInventario>();

    private bool interfaceEncontrada = false;
    private bool inventarioAberto = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            CarregarInventario();
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
        InventarioAberto = false;

        if (painelInventario != null)
            painelInventario.SetActive(false);

        AtualizarInventario();
        EsconderDescricao();
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
            Debug.LogError(
                "InventoryManager: Painel_Inventario não encontrado!"
            );
            return;
        }

        if (!inventarioAberto)
        {
            Transform pai = painelInventario.transform.parent;

            while (pai != null)
            {
                if (!pai.gameObject.activeSelf)
                    pai.gameObject.SetActive(true);

                pai = pai.parent;
            }

            painelInventario.SetActive(true);
            inventarioAberto = true;
            InventarioAberto = true;

            AtualizarInventario();

            Debug.Log("Inventário aberto! Movimentação bloqueada.");
        }
        else
        {
            painelInventario.SetActive(false);
            inventarioAberto = false;
            InventarioAberto = false;

            Debug.Log("Inventário fechado! Movimentação liberada.");
        }
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        painelInventario = null;
        textoDescricao = null;
        iconesItens = new GameObject[3];

        interfaceEncontrada = false;
        inventarioAberto = false;
        InventarioAberto = false;

        EncontrarInterface();

        if (painelInventario != null)
            painelInventario.SetActive(false);

        AtualizarInventario();
        EsconderDescricao();
    }

    private void EncontrarInterface()
    {
        painelInventario = null;
        textoDescricao = null;
        iconesItens = new GameObject[3];

        GameObject[] objetos =
            Resources.FindObjectsOfTypeAll<GameObject>();

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
            Transform painel = painelInventario.transform;

            Transform descricao =
                EncontrarFilho(painel, "Texto_Descricao");

            if (descricao != null)
            {
                textoDescricao =
                    descricao.GetComponent<TextMeshProUGUI>();
            }

            iconesItens[0] =
                EncontrarFilho(painel, "Icone_Fita_1")?.gameObject;

            iconesItens[1] =
                EncontrarFilho(painel, "Icone_Fita_2")?.gameObject;

            iconesItens[2] =
                EncontrarFilho(painel, "Icone_Fita_3")?.gameObject;

            for (int i = 0; i < iconesItens.Length; i++)
            {
                Debug.Log(
                    "Ícone " + (i + 1) + " encontrado: " +
                    (iconesItens[i] != null)
                );
            }
        }

        interfaceEncontrada = painelInventario != null;

        Debug.Log(
            "Interface do inventário encontrada: " +
            interfaceEncontrada
        );
    }

    private Transform EncontrarFilho(Transform pai, string nome)
    {
        foreach (Transform filho in pai)
        {
            if (filho.name == nome)
                return filho;

            Transform resultado =
                EncontrarFilho(filho, nome);

            if (resultado != null)
                return resultado;
        }

        return null;
    }

    public void AdicionarItem(string nomeItem)
    {
        Debug.Log("===== ADICIONANDO ITEM AO INVENTÁRIO =====");
        Debug.Log("Nome recebido: " + nomeItem);

        if (System.Array.IndexOf(nomesFitas, nomeItem) < 0)
        {
            Debug.LogWarning(
                "Item não reconhecido pelo inventário: " + nomeItem
            );
            return;
        }

        if (itens.Contains(nomeItem))
        {
            Debug.Log("Esse item já está no inventário: " + nomeItem);
            return;
        }

        itens.Add(nomeItem);

        Debug.Log("Lista atual do inventário:");

        foreach (string item in itens)
        {
            Debug.Log("Item salvo: " + item);
        }

        SalvarInventario();
        AtualizarInventario();
    }

    private void SalvarInventario()
    {
        string dados = string.Join("\n", itens);

        PlayerPrefs.SetString(CHAVE_INVENTARIO, dados);
        PlayerPrefs.Save();

        Debug.Log("Inventário salvo!");
    }

    private void CarregarInventario()
    {
        itens.Clear();

        if (PlayerPrefs.HasKey(CHAVE_INVENTARIO))
        {
            string dados =
                PlayerPrefs.GetString(CHAVE_INVENTARIO, "");

            string[] itensSalvos = dados.Split('\n');

            foreach (string item in itensSalvos)
            {
                if (!string.IsNullOrEmpty(item) &&
                    System.Array.IndexOf(nomesFitas, item) >= 0 &&
                    !itens.Contains(item))
                {
                    itens.Add(item);
                }
            }
        }

        Debug.Log(
            "Inventário carregado. Quantidade de itens: " +
            itens.Count
        );

        foreach (string item in itens)
        {
            Debug.Log("Item carregado: " + item);
        }
    }

    public void LimparInventario()
    {
        itens.Clear();

        PlayerPrefs.DeleteKey(CHAVE_INVENTARIO);
        PlayerPrefs.Save();

        AtualizarInventario();
        EsconderDescricao();

        Debug.Log("Inventário limpo para uma nova partida!");
    }

    private void AtualizarInventario()
    {
        if (painelInventario == null || !interfaceEncontrada)
            EncontrarInterface();

        for (int i = 0; i < iconesItens.Length; i++)
        {
            if (iconesItens[i] == null)
            {
                Debug.LogWarning(
                    "InventoryManager: Ícone " + (i + 1) +
                    " não foi encontrado!"
                );
                continue;
            }

            bool temItem = itens.Contains(nomesFitas[i]);

            iconesItens[i].SetActive(temItem);

            Debug.Log(
                iconesItens[i].name +
                " | Item: " + nomesFitas[i] +
                " | Ativado: " + temItem
            );
        }
    }

    public void MostrarDescricao(int indiceSlot)
    {
        Debug.Log("===== TESTE DA DESCRIÇÃO =====");
        Debug.Log("Índice recebido: " + indiceSlot);

        if (textoDescricao == null ||
            indiceSlot < 0 ||
            indiceSlot >= nomesFitas.Length)
        {
            return;
        }

        string nomeItem = nomesFitas[indiceSlot].Trim();

        Debug.Log("Fita correspondente ao slot: " + nomeItem);

        if (!itens.Contains(nomeItem))
        {
            EsconderDescricao();
            return;
        }

        foreach (InformacaoItemInventario info in informacoesItens)
        {
            if (info.nomeItem.Trim() == nomeItem)
            {
                textoDescricao.text = info.descricao;

                Debug.Log(
                    "Descrição encontrada para: " + nomeItem
                );

                Debug.Log(
                    "Texto da descrição: " + info.descricao
                );

                return;
            }
        }

        textoDescricao.text = nomeItem;

        Debug.LogWarning(
            "Descrição não encontrada para: " + nomeItem
        );
    }

    public void EsconderDescricao()
    {
        if (textoDescricao != null)
            textoDescricao.text = "";
    }
}
