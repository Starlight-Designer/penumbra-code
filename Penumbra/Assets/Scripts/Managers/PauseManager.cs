
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

        // Identifica se o jogador está em uma cutscene.
        bool estaEmCutscene =
            videoCutscene != null ||
            cenaAtual.StartsWith("Cutscene_");

        if (estaEmCutscene)
        {
            // Salva o nome da cutscene para retomá-la pelo menu.
            PlayerPrefs.SetString(
                CHAVE_CUTSCENE_PENDENTE,
                cenaAtual
            );

            Debug.Log("Cutscene salva para retomada: " + cenaAtual);
        }
        else
        {
            // Mantém o salvamento normal da partida.
            GameObject jogador =
                GameObject.FindGameObjectWithTag("Player");

            if (jogador != null)
            {
                PlayerPrefs.SetString("CenaSalva", cenaAtual);

                PlayerPrefs.SetFloat(
                    "PosicaoX",
                    jogador.transform.position.x
                );

                PlayerPrefs.SetFloat(
                    "PosicaoY",
                    jogador.transform.position.y
                );

                PlayerPrefs.SetInt("TemPartidaSalva", 1);
                PlayerPrefs.SetInt("CarregarPosicaoSalva", 1);
            }
        }

        PlayerPrefs.Save();

        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu_Inicial");
    }
}
