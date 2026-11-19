
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
    }

    public void Jogar()
    {
        if (PlayerPrefs.GetInt("TemPartidaSalva", 0) == 1)
        {
            string cenaSalva = PlayerPrefs.GetString("CenaSalva", "Cena_Teste");
            SceneManager.LoadScene(cenaSalva);
        }
        else if (PlayerPrefs.GetInt("CutsceneInicialAssistida", 0) == 0)
        {
            IrParaCutscene();
        }
        else
        {
            SceneManager.LoadScene("Cena_Teste");
        }
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
        // Guardar as configurações de áudio
        float volumeGeral = PlayerPrefs.GetFloat("VolumeGeral", 1f);
        float volumeMusica = PlayerPrefs.GetFloat("VolumeMusica", 1f);
        float volumeCutscenes = PlayerPrefs.GetFloat("VolumeCutscenes", 1f);

        // Limpar o inventário
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.LimparInventario();
        }

        // Limpar o progresso do jogo
        PlayerPrefs.DeleteAll();

        // Restaurar as configurações de áudio
        PlayerPrefs.SetFloat("VolumeGeral", volumeGeral);
        PlayerPrefs.SetFloat("VolumeMusica", volumeMusica);
        PlayerPrefs.SetFloat("VolumeCutscenes", volumeCutscenes);

        PlayerPrefs.Save();

        Time.timeScale = 1f;

        // Iniciar uma nova partida
        IrParaCutscene();
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
