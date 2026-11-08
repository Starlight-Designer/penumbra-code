
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;
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

    public void SairDoJogo()
    {
        Application.Quit();
    }
}
