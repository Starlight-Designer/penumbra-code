
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer reprodutorVideo;
    public GameObject textoPularCutscene;

    private bool cutsceneFinalizada = false;

    void Start()
    {
        textoPularCutscene.SetActive(false);

        reprodutorVideo.loopPointReached += FinalizarCutscene;

        StartCoroutine(MostrarTextoPular());

        reprodutorVideo.Play();
    }

    void Update()
    {
        if (!cutsceneFinalizada && Input.GetKeyDown(KeyCode.E))
        {
            FinalizarCutscene();
        }
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
