
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PixeArrastavel : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Peixe")]
    public string especie;

    [Tooltip("Identificador único para salvar este peixe. Use um diferente em cada peixe.")]
    public string idPersistencia;

    [Header("Áudios")]
    public AudioSource fonteAudio;
    public AudioClip somPegarPeixe;
    public AudioClip somColocarPeixe;

    private RectTransform rectTransform;
    private Vector3 posicaoOriginal;
    private Transform paiOriginal;
    private int ordemOriginal;

    private bool aceito = false;
    private bool estavaAceitoAntesDoArraste = false;

    private string ChaveBase
    {
        get
        {
            string identificador = string.IsNullOrEmpty(idPersistencia)
                ? gameObject.name
                : idPersistencia;

            return "PeixeSalvo_" +
                   UnityEngine.SceneManagement.SceneManager
                       .GetActiveScene().name + "_" +
                   identificador;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (fonteAudio == null)
            fonteAudio = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        // Espera um frame para que os objetos da cena sejam ativados.
        yield return null;

        CarregarEstadoSalvo();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posicaoOriginal = rectTransform.position;
        paiOriginal = transform.parent;
        ordemOriginal = transform.GetSiblingIndex();

        estavaAceitoAntesDoArraste = aceito;
        aceito = false;

        transform.SetAsLastSibling();

        TocarSom(somPegarPeixe);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!aceito)
        {
            transform.SetParent(paiOriginal);
            rectTransform.position = posicaoOriginal;
            transform.SetSiblingIndex(ordemOriginal);

            // Se o peixe já estava em uma caixa antes do arraste,
            // mantém o estado salvo caso o jogador não o coloque em outra.
            if (estavaAceitoAntesDoArraste)
            {
                aceito = true;
                SalvarEstadoAceito();
            }
        }
    }

    public void AceitarNaCaixa(Transform caixa)
    {
        aceito = true;

        transform.SetParent(caixa);
        rectTransform.position = caixa.position;
        transform.SetSiblingIndex(1);

        SalvarEstadoAceito();

        TocarSom(somColocarPeixe);
    }

    private void SalvarEstadoAceito()
    {
        if (!aceito || transform.parent == null)
            return;

        PlayerPrefs.SetInt(ChaveBase + "_Aceito", 1);

        PlayerPrefs.SetString(
            ChaveBase + "_CaminhoCaixa",
            CriarCaminho(transform.parent)
        );

        PlayerPrefs.SetInt(
            ChaveBase + "_Ordem",
            transform.GetSiblingIndex()
        );

        PlayerPrefs.SetFloat(
            ChaveBase + "_LocalX",
            rectTransform.localPosition.x
        );

        PlayerPrefs.SetFloat(
            ChaveBase + "_LocalY",
            rectTransform.localPosition.y
        );

        PlayerPrefs.SetFloat(
            ChaveBase + "_LocalZ",
            rectTransform.localPosition.z
        );

        PlayerPrefs.Save();
    }

    private void CarregarEstadoSalvo()
    {
        if (PlayerPrefs.GetInt(ChaveBase + "_Aceito", 0) != 1)
            return;

        string caminho = PlayerPrefs.GetString(
            ChaveBase + "_CaminhoCaixa",
            ""
        );

        if (string.IsNullOrEmpty(caminho))
            return;

        GameObject objetoCaixa = GameObject.Find(caminho);

        if (objetoCaixa == null)
        {
            Debug.LogWarning(
                "Não foi possível encontrar a caixa salva do peixe: " +
                gameObject.name + ". Caminho: " + caminho
            );

            return;
        }

        Transform caixa = objetoCaixa.transform;

        transform.SetParent(caixa);

        rectTransform.localPosition = new Vector3(
            PlayerPrefs.GetFloat(ChaveBase + "_LocalX", 0f),
            PlayerPrefs.GetFloat(ChaveBase + "_LocalY", 0f),
            PlayerPrefs.GetFloat(ChaveBase + "_LocalZ", 0f)
        );

        int ordem = PlayerPrefs.GetInt(
            ChaveBase + "_Ordem",
            1
        );

        transform.SetSiblingIndex(ordem);

        aceito = true;

        Debug.Log(
            "Peixe restaurado: " + gameObject.name
        );
    }

    private string CriarCaminho(Transform alvo)
    {
        string caminho = alvo.name;

        Transform atual = alvo.parent;

        while (atual != null)
        {
            caminho = atual.name + "/" + caminho;
            atual = atual.parent;
        }

        return caminho;
    }

    public void LimparEstadoSalvo()
    {
        PlayerPrefs.DeleteKey(ChaveBase + "_Aceito");
        PlayerPrefs.DeleteKey(ChaveBase + "_CaminhoCaixa");
        PlayerPrefs.DeleteKey(ChaveBase + "_Ordem");
        PlayerPrefs.DeleteKey(ChaveBase + "_LocalX");
        PlayerPrefs.DeleteKey(ChaveBase + "_LocalY");
        PlayerPrefs.DeleteKey(ChaveBase + "_LocalZ");

        aceito = false;
    }

    private void TocarSom(AudioClip som)
    {
        if (fonteAudio != null && som != null)
        {
            fonteAudio.PlayOneShot(som);
        }
    }
}
