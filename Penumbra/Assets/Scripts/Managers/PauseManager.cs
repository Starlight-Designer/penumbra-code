
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
    }

    private GameObject ProcurarNaCena(string nome)
    {
        Transform[] todos = FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Transform t in todos)
        {
            if (t.name == nome)
                return t.gameObject;
        }

        Debug.LogWarning("PauseManager: não achei o objeto '" + nome + "' na cena.");
        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!jogoPausado)
            {
                PausarJogo();
            }
            else
            {
                RetomarJogo();
            }
        }
    }

    void PausarJogo()
    {
        if (painelPausa == null || painelObjetivos == null)
            return;

        jogoPausado = true;
        fechando = false;

        // Verifica se o vídeo estava sendo reproduzido
        videoEstavaTocandoAntesDaPausa =
            videoCutscene != null && videoCutscene.isPlaying;

        // Pausa o vídeo
        if (videoEstavaTocandoAntesDaPausa)
            videoCutscene.Pause();

        painelPausa.SetActive(true);
        painelObjetivos.SetActive(true);

        Time.timeScale = 0f;
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

        // Retoma o vídeo somente se ele estava tocando antes da pausa
        if (videoEstavaTocandoAntesDaPausa && videoCutscene != null)
            videoCutscene.Play();

        videoEstavaTocandoAntesDaPausa = false;
    }

    public void VoltarAoMenu()
    {
        Debug.Log("BOTÃO VOLTAR AO MENU FOI CLICADO!");

        GameObject jogador = GameObject.FindGameObjectWithTag("Player");

        if (jogador != null)
        {
            PlayerPrefs.SetString("CenaSalva", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetFloat("PosicaoX", jogador.transform.position.x);
            PlayerPrefs.SetFloat("PosicaoY", jogador.transform.position.y);
            PlayerPrefs.SetInt("TemPartidaSalva", 1);
            PlayerPrefs.SetInt("CarregarPosicaoSalva", 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Inicial");
    }
}
