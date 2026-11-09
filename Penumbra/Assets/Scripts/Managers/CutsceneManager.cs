
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer reprodutorVideo;
    public GameObject textoPularCutscene;
    public GameObject painelPausaCutscene;

    private bool cutsceneFinalizada = false;
    private bool cutscenePausada = false;

    void Start()
    {
        textoPularCutscene.SetActive(false);
        painelPausaCutscene.SetActive(false);

        reprodutorVideo.loopPointReached += FinalizarCutscene;

        StartCoroutine(MostrarTextoPular());

        reprodutorVideo.Play();
    }

    void Update()
    {
        if (cutsceneFinalizada)
            return;

        // Pausar ou retomar com ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!cutscenePausada)
            {
                PausarCutscene();
            }
            else
            {
                ContinuarCutscene();
            }
        }

        // Pular com E, desde que não esteja pausada
        if (!cutscenePausada && Input.GetKeyDown(KeyCode.E))
        {
            FinalizarCutscene();
        }
    }

    void PausarCutscene()
    {
        cutscenePausada = true;
        reprodutorVideo.Pause();
        painelPausaCutscene.SetActive(true);
    }

    public void ContinuarCutscene()
    {
        cutscenePausada = false;
        painelPausaCutscene.SetActive(false);
        reprodutorVideo.Play();
    }

    public void PularCutscene()
    {
        FinalizarCutscene();
    }

    IEnumerator MostrarTextoPular()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (!cutsceneFinalizada)
        {
            textoPularCutscene.SetActive(true);
        }
    }

    void FinalizarCutscene(VideoPlayer video = null)
    {
        if (cutsceneFinalizada)
            return;

        cutsceneFinalizada = true;

        reprodutorVideo.Stop();
        textoPularCutscene.SetActive(false);
        painelPausaCutscene.SetActive(false);

        PlayerPrefs.SetInt("CutsceneInicialAssistida", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Cena_Teste");
    }

    void OnDestroy()
    {
        if (reprodutorVideo != null)
        {
            reprodutorVideo.loopPointReached -= FinalizarCutscene;
        }
    }
}
