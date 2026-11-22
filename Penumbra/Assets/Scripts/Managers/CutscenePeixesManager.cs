
using UnityEngine;
using UnityEngine.Video;

public class CutscenePeixesManager : MonoBehaviour
{
    [Header("Reprodução do vídeo")]
    public VideoPlayer reprodutorVideo;
    public GameObject imagemCutscene;

    [Header("Diálogo final")]
    public DialogueManager dialogoPescador;

    [Header("Popups do puzzle")]
    public GameObject painelPopupPeixes;

    [Header("Painel do puzzle")]
    public GameObject painelPuzzlePeixes;

    [Header("Personagens")]
    public GameObject player;
    public GameObject npcPeixaria;

    private bool cutsceneFinalizada = false;
    private bool cutsceneIniciada = false;
    private bool personagensOcultos = false;

    private Renderer[] renderizadoresPlayer;
    private Renderer[] renderizadoresNPC;

    private bool[] estadoPlayer;
    private bool[] estadoNPC;

    void Awake()
    {
        if (imagemCutscene != null)
            imagemCutscene.SetActive(false);

        if (reprodutorVideo != null)
            reprodutorVideo.loopPointReached += FinalizarCutscene;

        if (player != null)
        {
            renderizadoresPlayer =
                player.GetComponentsInChildren<Renderer>(true);

            estadoPlayer = new bool[renderizadoresPlayer.Length];

            for (int i = 0; i < renderizadoresPlayer.Length; i++)
                estadoPlayer[i] = renderizadoresPlayer[i].enabled;
        }

        if (npcPeixaria != null)
        {
            renderizadoresNPC =
                npcPeixaria.GetComponentsInChildren<Renderer>(true);

            estadoNPC = new bool[renderizadoresNPC.Length];

            for (int i = 0; i < renderizadoresNPC.Length; i++)
                estadoNPC[i] = renderizadoresNPC[i].enabled;
        }
    }

    void Update()
    {
        if (painelPuzzlePeixes != null &&
            painelPuzzlePeixes.activeInHierarchy &&
            !personagensOcultos)
        {
            OcultarPersonagens();
        }
    }

    void OcultarPersonagens()
    {
        personagensOcultos = true;

        if (renderizadoresPlayer != null)
        {
            foreach (Renderer r in renderizadoresPlayer)
            {
                if (r != null)
                    r.enabled = false;
            }
        }

        if (renderizadoresNPC != null)
        {
            foreach (Renderer r in renderizadoresNPC)
            {
                if (r != null)
                    r.enabled = false;
            }
        }
    }

    void MostrarPersonagens()
    {
        if (renderizadoresPlayer != null)
        {
            for (int i = 0; i < renderizadoresPlayer.Length; i++)
            {
                if (renderizadoresPlayer[i] != null)
                    renderizadoresPlayer[i].enabled = estadoPlayer[i];
            }
        }

        if (renderizadoresNPC != null)
        {
            for (int i = 0; i < renderizadoresNPC.Length; i++)
            {
                if (renderizadoresNPC[i] != null)
                    renderizadoresNPC[i].enabled = estadoNPC[i];
            }
        }

        personagensOcultos = false;
    }

    public void IniciarCutscene()
    {
        if (cutsceneIniciada || reprodutorVideo == null)
            return;

        cutsceneIniciada = true;

        DialogueManager.DialogoGlobalAtivo = true;

        if (painelPuzzlePeixes != null)
            painelPuzzlePeixes.SetActive(false);

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

        // Remove o popup original e todos os popups extras.
        if (painelPopupPeixes != null)
        {
            Transform pai = painelPopupPeixes.transform.parent;

            if (pai != null)
            {
                for (int i = pai.childCount - 1; i >= 0; i--)
                {
                    Transform filho = pai.GetChild(i);

                    if (filho == painelPopupPeixes.transform ||
                        filho.name.Contains(painelPopupPeixes.name))
                    {
                        filho.gameObject.SetActive(false);
                    }
                }
            }
        }

        // Marca o puzzle como concluído antes de iniciar o diálogo.
        PlayerPrefs.SetInt("PescadorPuzzleConcluido", 1);
        PlayerPrefs.Save();

        MostrarPersonagens();

        // Inicia automaticamente o diálogo final do pescador.
        if (dialogoPescador != null)
        {
            dialogoPescador.IniciarDialogoAutomatico();
        }
        else
        {
            Debug.LogError("DialogueManager do pescador não foi conectado!");
            DialogueManager.DialogoGlobalAtivo = false;
        }
    }

    void OnDestroy()
    {
        if (reprodutorVideo != null)
            reprodutorVideo.loopPointReached -= FinalizarCutscene;
    }
}
