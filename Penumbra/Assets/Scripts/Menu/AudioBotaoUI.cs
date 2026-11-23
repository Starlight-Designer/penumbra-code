
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class AudioBotaoUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    [Header("Sons do botão")]
    public AudioClip somHover;
    public AudioClip somSelecao;

    [Header("Configurações")]
    [Range(0f, 1f)]
    public float volumeHover = 1f;

    [Range(0f, 1f)]
    public float volumeSelecao = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (somHover != null)
        {
            audioSource.PlayOneShot(somHover, volumeHover);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (somSelecao != null)
        {
            audioSource.PlayOneShot(somSelecao, volumeSelecao);
        }
    }
}
