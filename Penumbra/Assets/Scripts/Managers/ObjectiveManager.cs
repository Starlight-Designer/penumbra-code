
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI textoObjetivo1;
    public TextMeshProUGUI textoObjetivo2;

    private int objetivoAtual = 0;

    private string[] objetivos =
    {
        "Vá até a Delegacia.",
        "Fale com seu tio.",
        "Pegue a fita VHS.",
        "Vá até a Peixaria e converse com o pescador.",
        "Vá até a casa da mãe da Bianca.",
        "Converse com Dona Cida.",
        "Entre no quarto da Bianca e resolva o puzzle.",
        "Volte à sala e converse com Dona Cida.",
        "Receba a fita VHS de Dona Cida.",
        "Explore a floresta e siga em direção à igreja.",
        "Resolva o último puzzle para entrar na igreja."
    };

    void Start()
    {
        objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        Debug.Log("OBJETIVO ATUAL: " + objetivoAtual);

        AtualizarObjetivos();
    }

    public void ConcluirObjetivo()
    {
        objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        if (objetivoAtual < objetivos.Length)
        {
            objetivoAtual++;

            PlayerPrefs.SetInt("ObjetivoAtual", objetivoAtual);
            PlayerPrefs.Save();

            Debug.Log("OBJETIVO CONCLUÍDO! NOVO VALOR: " + objetivoAtual);

            AtualizarObjetivos();
        }
    }

    void AtualizarObjetivos()
    {
        objetivoAtual = Mathf.Clamp(objetivoAtual, 0, objetivos.Length);

        if (textoObjetivo1 == null || textoObjetivo2 == null)
        {
            Debug.LogError(
                "ObjectiveManager: conecte os campos Texto Objetivo 1 e Texto Objetivo 2 no Inspector!"
            );
            return;
        }

        if (objetivoAtual == 0)
        {
            textoObjetivo1.text = objetivos[0];
            textoObjetivo1.fontStyle = FontStyles.Normal;
            textoObjetivo1.color = Color.black;

            textoObjetivo2.text = objetivos[1];
            textoObjetivo2.fontStyle = FontStyles.Normal;
            textoObjetivo2.color = Color.black;
        }
        else
        {
            textoObjetivo1.text = objetivos[objetivoAtual - 1];
            textoObjetivo1.fontStyle = FontStyles.Strikethrough;
            textoObjetivo1.color = new Color(0f, 0f, 0f, 0.4f);

            if (objetivoAtual < objetivos.Length)
            {
                textoObjetivo2.text = objetivos[objetivoAtual];
                textoObjetivo2.fontStyle = FontStyles.Normal;
                textoObjetivo2.color = Color.black;
            }
            else
            {
                textoObjetivo2.text = "";
            }
        }
    }
}
