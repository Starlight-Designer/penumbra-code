
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ObjectInteraction : MonoBehaviour
{
    public TextMeshProUGUI textoInteracao;
    public string mensagem = "Você pegou a fita!";
    public float tempoNaTela = 3f;

    public string nomeItem = "Fita VHS";
    public bool coletarItem = true;

    [Header("Atualização de Objetivos")]
    public ObjectiveManager objectiveManager;
    public int objetivoParaConcluir = 2;

    [Header("Identificação da fita")]
    public bool primeiraFitaVHS = false;

    [Header("Transição da terceira fita")]
    public bool transportarParaIgreja = false;
    public string nomeCenaDestino = "Igreja_Porta";
    public FadePreto fadePreto;

    private bool itemColetado = false;

    void Start()
    {
        if (textoInteracao != null)
            textoInteracao.gameObject.SetActive(false);

        if (primeiraFitaVHS &&
            PlayerPrefs.GetInt("FitaVHS01Coletada", 0) == 1)
        {
            Destroy(gameObject);
            return;
        }

        if (coletarItem && InventoryManager.instance != null &&
            InventoryManager.instance.itens.Contains(nomeItem))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnMouseDown()
    {
        if (DialogueManager.DialogoGlobalAtivo)
            return;

        if (itemColetado)
            return;

        if (coletarItem && InventoryManager.instance == null)
        {
            Debug.LogError("INVENTÁRIO NÃO ENCONTRADO!");
            return;
        }

        itemColetado = true;

        if (coletarItem)
        {
            InventoryManager.instance.AdicionarItem(nomeItem);

            if (primeiraFitaVHS)
            {
                PlayerPrefs.SetInt("FitaVHS01Coletada", 1);
                PlayerPrefs.Save();
            }

            if (objectiveManager != null &&
                PlayerPrefs.GetInt("ObjetivoAtual", 0) == objetivoParaConcluir)
            {
                objectiveManager.ConcluirObjetivo();
            }
        }

        StartCoroutine(MostrarMensagem());
    }

    IEnumerator MostrarMensagem()
    {
        if (textoInteracao != null)
        {
            textoInteracao.text = mensagem;
            textoInteracao.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(tempoNaTela);

        if (textoInteracao != null)
            textoInteracao.gameObject.SetActive(false);

        if (coletarItem)
            Destroy(gameObject);

        if (transportarParaIgreja)
        {
            if (fadePreto != null)
            {
                fadePreto.EscurecerECarregar(nomeCenaDestino);
            }
            else
            {
                Debug.LogWarning("FadePreto não conectado! Carregando sem transição.");
                SceneManager.LoadScene(nomeCenaDestino);
            }
        }
    }
}
