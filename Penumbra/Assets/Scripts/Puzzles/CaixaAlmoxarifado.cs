
using UnityEngine;

public class CaixaAlmoxarifado : MonoBehaviour
{
    [Header("Pensamentos da Alice")]
    public GerenciadorPensamentos gerenciadorPensamentos;

    [TextArea(2, 4)]
    public string mensagem = "Apenas produtos de limpeza.";

    [Header("Configuração da descoberta")]
    public bool revelarFita = false;

    public GameObject fitaVHS;

    [Header("Rosto especial da Alice")]
    public bool usarRostoEspecial = false;
    public Sprite rostoBocaFechada;
    public Sprite rostoBocaAberta;

    private const string CHAVE_FITA = "FitaAlmoxarifadoRevelada";

    private bool fitaJaRevelada = false;

    void Start()
    {
        fitaJaRevelada =
            PlayerPrefs.GetInt(CHAVE_FITA, 0) == 1;

        // A fita fica escondida até ser descoberta
        if (fitaVHS != null)
        {
            fitaVHS.SetActive(fitaJaRevelada);
        }
    }

    private void OnMouseDown()
    {
        if (gerenciadorPensamentos == null)
        {
            Debug.LogWarning(
                "Conecte o GerenciadorPensamentos na caixa: " + gameObject.name
            );
            return;
        }

        // Se já revelou a fita, permite visualizar novamente o pensamento
        if (revelarFita && fitaJaRevelada)
        {
            MostrarMensagem(null);
            return;
        }

        if (revelarFita)
        {
            MostrarMensagem(RevelarFita);
        }
        else
        {
            MostrarMensagem(null);
        }
    }

    void MostrarMensagem(System.Action aoFechar)
    {
        if (usarRostoEspecial)
        {
            gerenciadorPensamentos.MostrarPensamento(
                mensagem,
                aoFechar,
                rostoBocaFechada,
                rostoBocaAberta
            );
        }
        else
        {
            gerenciadorPensamentos.MostrarPensamento(
                mensagem,
                aoFechar
            );
        }
    }

    void RevelarFita()
    {
        if (fitaJaRevelada)
            return;

        fitaJaRevelada = true;

        PlayerPrefs.SetInt(CHAVE_FITA, 1);
        PlayerPrefs.Save();

        if (fitaVHS != null)
        {
            fitaVHS.SetActive(true);
        }

        Debug.Log("A fita VHS foi revelada no Almoxarifado!");
    }
}
