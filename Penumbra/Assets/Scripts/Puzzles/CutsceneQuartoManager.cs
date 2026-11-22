
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneQuartoManager : MonoBehaviour
{
    [Header("Reprodução do vídeo")]
    public VideoPlayer reprodutorVideo;
    public GameObject imagemCutscene;

    [Header("Painel do puzzle")]
    public GameObject painelPuzzleQuarto;

    private bool cutsceneIniciada = false;
    private bool cutsceneFinalizada = false;

    void Awake()
    {
        if (imagemCutscene != null)
            imagemCutscene.SetActive(false);

        if (reprodutorVideo != null)
            reprodutorVideo.loopPointReached += FinalizarCutscene;
    }

    public void IniciarCutscene()
    {
        if (cutsceneIniciada || reprodutorVideo == null)
            return;

        cutsceneIniciada = true;

        DialogueManager.DialogoGlobalAtivo = true;

        if (painelPuzzleQuarto != null)
            painelPuzzleQuarto.SetActive(false);

        if (imagemCutscene != null)
        {
            imagemCutscene.SetActive(true);
            imagemCutscene.transform.SetAsLastSibling();
        }

        reprodutorVideo.Play();
    }

    void FinalizarCutscene(VideoPlayer video = null)
    {
        if (cutsceneFinalizada || !cutsceneIniciada)
            return;

        cutsceneFinalizada = true;

        reprodutorVideo.Stop();

        if (imagemCutscene != null)
            imagemCutscene.SetActive(false);

        // Registra que Alice terminou o puzzle do quarto.
        PlayerPrefs.SetInt("PuzzleQuartoConcluido", 1);
        PlayerPrefs.SetInt("IniciarDialogoDonaCida", 1);
        PlayerPrefs.Save();

        DialogueManager.DialogoGlobalAtivo = false;

        SceneManager.LoadScene("Casa_Mae_Bianca");
    }

    void OnDestroy()
    {
        if (reprodutorVideo != null)
            reprodutorVideo.loopPointReached -= FinalizarCutscene;
    }
}
