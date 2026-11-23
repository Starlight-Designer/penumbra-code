
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;
    public GameObject painelConfirmarReinicio;
    public UnityEngine.UI.Slider sliderVolume;

    [Header("Transição")]
    public FadePreto fade;

    [Header("Música do menu")]
    public MusicaMenu musicaMenu;

    private const string CHAVE_REINICIAR_APOS_FINAL =
        "ReiniciarAoVoltarMenu";

    private const string CHAVE_RETORNAR_FLORESTA =
        "RetornarFlorestaAposDerrota";

    private const string CHAVE_CUTSCENE_PENDENTE =
        "CutscenePendenteMenu";

    private bool iniciandoJogo = false;

    private void Start()
    {
        if (PlayerPrefs.GetInt(CHAVE_REINICIAR_APOS_FINAL, 0) == 1)
        {
            ApagarProgresso();
            Debug.Log("Progresso reiniciado após a cutscene final!");
        }

        float volumeGeral = PlayerPrefs.GetFloat("VolumeGeral", 1f);
        AudioListener.volume = volumeGeral;

        if (sliderVolume != null)
            sliderVolume.value = volumeGeral;
    }

    public void AbrirConfiguracoes()
    {
        painelConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        painelConfiguracoes.SetActive(false);
    }

    public void AlterarVolume()
    {
        AudioListener.volume = sliderVolume.value;

        PlayerPrefs.SetFloat("VolumeGeral", sliderVolume.value);
        PlayerPrefs.Save();
    }

    public void Jogar()
    {
        if (iniciandoJogo)
            return;

        StartCoroutine(JogarComFade());
    }

    private IEnumerator JogarComFade()
    {
        iniciandoJogo = true;

        if (musicaMenu != null)
        {
            musicaMenu.FadeOut();

            yield return new WaitForSecondsRealtime(
                musicaMenu.duracaoFade
            );
        }

        // PRIORIDADE 1: Retomar uma cutscene interrompida.
        if (PlayerPrefs.HasKey(CHAVE_CUTSCENE_PENDENTE))
        {
            string cutsceneSalva = PlayerPrefs.GetString(
                CHAVE_CUTSCENE_PENDENTE
            );

            // Apaga o registro para não repetir a cutscene
            // depois que ela for concluída normalmente.
            PlayerPrefs.DeleteKey(CHAVE_CUTSCENE_PENDENTE);
            PlayerPrefs.Save();

            Time.timeScale = 1f;

            if (fade != null)
                fade.EscurecerECarregar(cutsceneSalva);
            else
                SceneManager.LoadScene(cutsceneSalva);

            yield break;
        }

        // PRIORIDADE 2: Retornar à floresta após a derrota.
        if (PlayerPrefs.GetInt(CHAVE_RETORNAR_FLORESTA, 0) == 1)
        {
            PlayerPrefs.DeleteKey(CHAVE_RETORNAR_FLORESTA);
            PlayerPrefs.Save();

            Time.timeScale = 1f;

            if (fade != null)
                fade.EscurecerECarregar("Floresta_Fuga");
            else
                SceneManager.LoadScene("Floresta_Fuga");

            yield break;
        }

        // PRIORIDADE 3: Continuar uma partida salva.
        if (PlayerPrefs.GetInt("TemPartidaSalva", 0) == 1)
        {
            string cenaSalva = PlayerPrefs.GetString(
                "CenaSalva",
                "Cena_Teste"
            );

            if (fade != null)
                fade.EscurecerECarregar(cenaSalva);
            else
                SceneManager.LoadScene(cenaSalva);

            yield break;
        }

        // PRIORIDADE 4: Cutscene inicial.
        if (PlayerPrefs.GetInt("CutsceneInicialAssistida", 0) == 0)
        {
            IrParaCutscene();
            yield break;
        }

        // PRIORIDADE 5: Entrar diretamente no jogo.
        if (fade != null)
            fade.EscurecerECarregar("Cena_Teste");
        else
            SceneManager.LoadScene("Cena_Teste");
    }

    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
    }

    public void AbrirConfirmacaoReinicio()
    {
        painelConfirmarReinicio.SetActive(true);
    }

    public void CancelarReinicio()
    {
        painelConfirmarReinicio.SetActive(false);
    }

    public void ConfirmarReinicio()
    {
        if (iniciandoJogo)
            return;

        ApagarProgresso();

        Time.timeScale = 1f;

        StartCoroutine(ReiniciarComFade());
    }

    private IEnumerator ReiniciarComFade()
    {
        iniciandoJogo = true;

        if (musicaMenu != null)
        {
            musicaMenu.FadeOut();

            yield return new WaitForSecondsRealtime(
                musicaMenu.duracaoFade
            );
        }

        IrParaCutscene();
    }

    private void ApagarProgresso()
    {
        float volumeGeral = PlayerPrefs.GetFloat("VolumeGeral", 1f);
        float volumeMusica = PlayerPrefs.GetFloat("VolumeMusica", 1f);
        float volumeCutscenes = PlayerPrefs.GetFloat("VolumeCutscenes", 1f);

        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.LimparInventario();
        }

        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetFloat("VolumeGeral", volumeGeral);
        PlayerPrefs.SetFloat("VolumeMusica", volumeMusica);
        PlayerPrefs.SetFloat("VolumeCutscenes", volumeCutscenes);

        AudioListener.volume = volumeGeral;

        PlayerPrefs.Save();

        Time.timeScale = 1f;
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }

    private void IrParaCutscene()
    {
        if (fade != null)
            fade.EscurecerECarregar("Cutscene_Inicial");
        else
            SceneManager.LoadScene("Cutscene_Inicial");
    }
}
