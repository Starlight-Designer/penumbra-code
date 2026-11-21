
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

    private bool iniciandoJogo = false;

    private void Start()
    {
        // Verifica se o jogador acabou de voltar da cutscene final.
        if (PlayerPrefs.GetInt(CHAVE_REINICIAR_APOS_FINAL, 0) == 1)
        {
            ApagarProgresso();
            Debug.Log("Progresso reiniciado após a cutscene final!");
        }

        // Recupera o volume geral salvo.
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

        // Diminui a música antes de sair do menu.
        if (musicaMenu != null)
        {
            musicaMenu.FadeOut();

            yield return new WaitForSecondsRealtime(
                musicaMenu.duracaoFade
            );
        }

        // Se existe uma partida salva, continua de onde parou.
        if (PlayerPrefs.GetInt("TemPartidaSalva", 0) == 1)
        {
            string cenaSalva = PlayerPrefs.GetString(
                "CenaSalva",
                "Cena_Teste"
            );

            SceneManager.LoadScene(cenaSalva);
            yield break;
        }

        // Se ainda não assistiu à abertura, reproduz a cutscene.
        if (PlayerPrefs.GetInt("CutsceneInicialAssistida", 0) == 0)
        {
            IrParaCutscene();
            yield break;
        }

        // Se já assistiu à abertura, entra diretamente no jogo.
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

        // Diminui a música antes de abrir a cutscene inicial.
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
        // Guardar todas as configurações de áudio.
        float volumeGeral = PlayerPrefs.GetFloat("VolumeGeral", 1f);
        float volumeMusica = PlayerPrefs.GetFloat("VolumeMusica", 1f);
        float volumeCutscenes = PlayerPrefs.GetFloat("VolumeCutscenes", 1f);

        // Limpar o inventário.
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.LimparInventario();
        }

        // Apagar os dados do jogo.
        PlayerPrefs.DeleteAll();

        // Restaurar as configurações de áudio.
        PlayerPrefs.SetFloat("VolumeGeral", volumeGeral);
        PlayerPrefs.SetFloat("VolumeMusica", volumeMusica);
        PlayerPrefs.SetFloat("VolumeCutscenes", volumeCutscenes);

        // Restaurar o volume geral aplicado.
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
