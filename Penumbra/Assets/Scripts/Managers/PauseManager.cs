using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PauseManager : MonoBehaviour
{
    public GameObject painelPausa;
    public GameObject painelObjetivos;

    [Header("Vídeo da cutscene (opcional)")]
    public VideoPlayer videoCutscene;

    [Header("Marque se esta cena é uma cutscene (ex.: cutscene final)")]
    public bool ehCutscene = false;

    private bool jogoPausado = false;
    private bool fechando = false;
    private bool videoEstavaTocandoAntesDaPausa = false;

    [Header("Nomes dos painéis na Hierarchy (usados se os campos acima estiverem vazios)")]
    public string nomePainelPausa = "Painel_Pausa";
    public string nomePainelObjetivos = "Painel_Objetivos";

    private AnimacaoQueda animacaoPausa;
    private AnimacaoZoom animacaoObjetivos;

    private ObjectiveManager objectiveManager;

    private const string CHAVE_CUTSCENE_PENDENTE =
        "CutscenePendenteMenu";

    private const string CHAVE_PUZZLE_PEIXES =
        "PuzzlePeixesEmAndamento";

    void Awake()
    {
        if (painelPausa == null)
            painelPausa = ProcurarNaCena(nomePainelPausa);

        if (painelObjetivos == null)
            painelObjetivos = ProcurarNaCena(nomePainelObjetivos);

        if (painelPausa != null)
            animacaoPausa = painelPausa.GetComponent<AnimacaoQueda>();

        if (painelObjetivos != null)
            animacaoObjetivos = painelObjetivos.GetComponent<AnimacaoZoom>();

        objectiveManager = FindFirstObjectByType<ObjectiveManager>();

        if (objectiveManager == null)
        {
            Debug.LogWarning(
                "PauseManager: ObjectiveManager não encontrado na cena."
            );
        }
    }

    private GameObject ProcurarNaCena(string nome)
    {
        Transform[] todos = FindObjectsByType<Transform>(
            FindObjectsInactive.Include
        );

        foreach (Transform t in todos)
        {
            if (t.name == nome)
                return t.gameObject;
        }

        Debug.LogWarning(
            "PauseManager: não achei o objeto '" + nome + "' na cena."
        );

        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!jogoPausado)
                PausarJogo();
            else
                RetomarJogo();
        }
    }

    void PausarJogo()
    {
        if (painelPausa == null || painelObjetivos == null)
            return;

        jogoPausado = true;
        fechando = false;

        videoEstavaTocandoAntesDaPausa =
            videoCutscene != null && videoCutscene.isPlaying;

        if (videoEstavaTocandoAntesDaPausa)
            videoCutscene.Pause();

        if (objectiveManager != null)
            objectiveManager.CancelarNotificacaoParaPausa();

        painelPausa.SetActive(true);
        painelObjetivos.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("Jogo pausado.");
    }

    public void RetomarJogo()
    {
        if (!jogoPausado || fechando)
            return;

        StartCoroutine(RetomarComAnimacoes());
    }

    private IEnumerator RetomarComAnimacoes()
    {
        fechando = true;

        if (animacaoPausa != null)
            animacaoPausa.Fechar();
        else
            painelPausa.SetActive(false);

        if (animacaoObjetivos != null)
            animacaoObjetivos.Fechar();
        else
            painelObjetivos.SetActive(false);

        yield return new WaitUntil(() =>
            !painelPausa.activeSelf &&
            !painelObjetivos.activeSelf
        );

        FinalizarRetomada();
    }

    private void FinalizarRetomada()
    {
        jogoPausado = false;
        fechando = false;

        painelPausa.SetActive(false);
        painelObjetivos.SetActive(false);

        Time.timeScale = 1f;

        if (videoEstavaTocandoAntesDaPausa && videoCutscene != null)
            videoCutscene.Play();

        videoEstavaTocandoAntesDaPausa = false;

        Debug.Log("Jogo retomado.");
    }

    public void VoltarAoMenu()
    {
        Debug.Log("BOTÃO VOLTAR AO MENU FOI CLICADO!");

        string cenaAtual = SceneManager.GetActiveScene().name;

        bool estaEmCutscene =
            ehCutscene ||
            videoCutscene != null ||
            cenaAtual.StartsWith("Cutscene_");

        if (estaEmCutscene)
        {
            PlayerPrefs.SetString(
                CHAVE_CUTSCENE_PENDENTE,
                cenaAtual
            );

            Debug.Log("Cutscene salva para retomada: " + cenaAtual);
        }
        else
        {
            // A cena é salva SEMPRE, com ou sem Player.
            PlayerPrefs.SetString("CenaSalva", cenaAtual);
            PlayerPrefs.SetInt("TemPartidaSalva", 1);

            GameObject jogador =
                GameObject.FindGameObjectWithTag("Player");

            if (jogador != null)
            {
                PlayerPrefs.SetFloat(
                    "PosicaoX",
                    jogador.transform.position.x
                );

                PlayerPrefs.SetFloat(
                    "PosicaoY",
                    jogador.transform.position.y
                );

                PlayerPrefs.SetInt("CarregarPosicaoSalva", 1);
            }
            else
            {
                // Sem Player: não reaproveita a posição de outra cena.
                PlayerPrefs.SetInt("CarregarPosicaoSalva", 0);
                PlayerPrefs.DeleteKey("PosicaoX");
                PlayerPrefs.DeleteKey("PosicaoY");

                Debug.LogWarning(
                    "Nenhum objeto com a tag Player em '" + cenaAtual +
                    "'. Cena salva sem posição."
                );
            }

            // Mantém o registro de que o puzzle dos peixes
            // deve ser retomado ao carregar novamente a Peixaria.
            if (cenaAtual == "Peixaria" &&
                PlayerPrefs.GetInt(CHAVE_PUZZLE_PEIXES, 0) == 1)
            {
                Debug.Log(
                    "Retomada do puzzle dos peixes registrada."
                );
            }
        }

        PlayerPrefs.Save();

        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu_Inicial");
    }
}