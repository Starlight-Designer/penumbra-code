
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
        "Encontre a primeira fita VHS.",
        "Vá até a Peixaria e converse com o pescador.",
        "Vá até a casa da mãe da Bianca.",
        "Resolva o puzzle e encontre a segunda fita VHS.",
        "Explore a floresta e siga em direção à igreja.",
        "Resolva o último puzzle para entrar na igreja."
    };

    void Start()
    {
        objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);
        AtualizarObjetivos();
    }

    public void ConcluirObjetivo()
    {
        if (objetivoAtual < objetivos.Length)
        {
            objetivoAtual++;

            PlayerPrefs.SetInt("ObjetivoAtual", objetivoAtual);
            PlayerPrefs.Save();

            AtualizarObjetivos();
        }
    }

    void AtualizarObjetivos()
    {
        // Primeiro espaço
        if (objetivoAtual == 0)
        {
            textoObjetivo1.text = objetivos[0];
            textoObjetivo1.fontStyle = FontStyles.Normal;
            textoObjetivo1.color = Color.white;
        }
        else
        {
            textoObjetivo1.text = objetivos[objetivoAtual - 1];
            textoObjetivo1.fontStyle = FontStyles.Strikethrough;
            textoObjetivo1.color = new Color(1f, 1f, 1f, 0.4f);
        }

        // Segundo espaço
        int indiceSegundoObjetivo = objetivoAtual == 0 ? 1 : objetivoAtual;

        if (indiceSegundoObjetivo < objetivos.Length)
        {
            textoObjetivo2.text = objetivos[indiceSegundoObjetivo];
            textoObjetivo2.fontStyle = FontStyles.Normal;
            textoObjetivo2.color = Color.white;
        }
        else
        {
            textoObjetivo2.text = "";
        }
    }
}
