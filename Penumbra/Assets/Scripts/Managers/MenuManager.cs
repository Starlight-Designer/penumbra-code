
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;
    public GameObject painelConfirmarReinicio;
    public UnityEngine.UI.Slider sliderVolume;

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
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.itens.Clear();
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        SceneManager.LoadScene("Cena_Teste");
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }
}
