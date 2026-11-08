
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider sliderVolumeGeral;
    public Slider sliderMusica;
    public Slider sliderCutscenes;

    private const string VOLUME_GERAL = "VolumeGeral";
    private const string VOLUME_MUSICA = "VolumeMusica";
    private const string VOLUME_CUTSCENES = "VolumeCutscenes";

    void Start()
    {
        sliderVolumeGeral.value = PlayerPrefs.GetFloat("VolumeGeral", 1f);
        sliderMusica.value = PlayerPrefs.GetFloat("VolumeMusica", 1f);
        sliderCutscenes.value = PlayerPrefs.GetFloat("VolumeCutscenes", 1f);

        AlterarVolumeGeral();
        AlterarVolumeMusica();
        AlterarVolumeCutscenes();
    }

    float ConverterVolume(float volume)
    {
        return volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
    }

    public void AlterarVolumeGeral()
    {
        mixer.SetFloat(VOLUME_GERAL, ConverterVolume(sliderVolumeGeral.value));
        PlayerPrefs.SetFloat("VolumeGeral", sliderVolumeGeral.value);
        PlayerPrefs.Save();
    }

    public void AlterarVolumeMusica()
    {
        mixer.SetFloat(VOLUME_MUSICA, ConverterVolume(sliderMusica.value));
        PlayerPrefs.SetFloat("VolumeMusica", sliderMusica.value);
        PlayerPrefs.Save();
    }

    public void AlterarVolumeCutscenes()
    {
        mixer.SetFloat(VOLUME_CUTSCENES, ConverterVolume(sliderCutscenes.value));
        PlayerPrefs.SetFloat("VolumeCutscenes", sliderCutscenes.value);
        PlayerPrefs.Save();
    }
}
