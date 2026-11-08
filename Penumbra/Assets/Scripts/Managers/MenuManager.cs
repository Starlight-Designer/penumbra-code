using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject painelConfiguracoes;
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
}