
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

    [Header("Identificação das fitas")]
    public bool primeiraFitaVHS = false;
    public bool segundaFitaVHS = false;

    [Header("Cutscene da primeira fita")]
    public string nomeCenaCutsceneFita1 = "Cutscene_Fita_1";

    [Header("Cutscene da segunda fita")]
    public string nomeCenaCutsceneFita2 = "Cutscene_Fita_2";

    [Header("Transição da terceira fita")]
    public bool transportarParaIgreja = false;
    public string nomeCenaDestino = "Igreja_Porta";
    public FadePreto fadePreto;

    private bool itemColetado = false;

    private const string CHAVE_CENA_RETORNO_FITA1 =
        "CenaRetornoCutsceneFita1";

    private const string CHAVE_CENA_RETORNO_FITA2 =
        "CenaRetornoCutsceneFita2";

    private const string CHAVE_BLOQUEIO_FITA1 =
        "BloquearAlicePosFita1";

    private const string CHAVE_BLOQUEIO_FITA2 =
        "BloquearAlicePosFita2";

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

        if (segundaFitaVHS &&
            PlayerPrefs.GetInt("FitaVHS02Coletada", 0) == 1)
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

        // Bloqueia Alice imediatamente ao pegar a primeira fita.
        if (coletarItem && primeiraFitaVHS)
        {
            DialogueManager.DialogoGlobalAtivo = true;

            PlayerPrefs.SetInt(CHAVE_BLOQUEIO_FITA1, 1);
            PlayerPrefs.Save();
        }

        // Bloqueia Alice imediatamente ao pegar a segunda fita.
        if (coletarItem && segundaFitaVHS)
        {
            DialogueManager.DialogoGlobalAtivo = true;

            PlayerPrefs.SetInt(CHAVE_BLOQUEIO_FITA2, 1);
            PlayerPrefs.Save();
        }

        if (coletarItem)
        {
            InventoryManager.instance.AdicionarItem(nomeItem);

            if (primeiraFitaVHS)
            {
                PlayerPrefs.SetInt("FitaVHS01Coletada", 1);
                PlayerPrefs.Save();
            }

            if (segundaFitaVHS)
            {
                PlayerPrefs.SetInt("FitaVHS02Coletada", 1);
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

        // Inicia a cutscene depois da coleta da primeira fita.
        if (coletarItem && primeiraFitaVHS)
        {
            string cenaAtual = SceneManager.GetActiveScene().name;

            PlayerPrefs.SetString(
                CHAVE_CENA_RETORNO_FITA1,
                cenaAtual
            );

            PlayerPrefs.Save();

            SceneManager.LoadScene(nomeCenaCutsceneFita1);
            yield break;
        }

        // Inicia a cutscene depois da coleta da segunda fita.
        if (coletarItem && segundaFitaVHS)
        {
            string cenaAtual = SceneManager.GetActiveScene().name;

            PlayerPrefs.SetString(
                CHAVE_CENA_RETORNO_FITA2,
                cenaAtual
            );

            PlayerPrefs.Save();

            SceneManager.LoadScene(nomeCenaCutsceneFita2);
            yield break;
        }

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
