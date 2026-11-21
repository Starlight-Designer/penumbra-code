
using UnityEngine;

public class PadreOpicional : MonoBehaviour
{
    private const string CHAVE_NPC_LIBERADO = "NPCOpcionalLiberado";
    private const string CHAVE_NPC_DESAPARECEU = "NPCOpcionalDesapareceu";

    void Start()
    {
        bool liberado = PlayerPrefs.GetInt(CHAVE_NPC_LIBERADO, 0) == 1;
        bool desapareceu = PlayerPrefs.GetInt(CHAVE_NPC_DESAPARECEU, 0) == 1;

        if (!liberado || desapareceu)
        {
            gameObject.SetActive(false);
            return;
        }

        Debug.Log("Padre opcional apareceu na Cena_Teste!");
    }

    // Chamado quando Alice entrar na casa da Bianca.
    public static void RegistrarEntradaNaCasaDaBianca()
    {
        PlayerPrefs.SetInt(CHAVE_NPC_DESAPARECEU, 1);
        PlayerPrefs.Save();

        Debug.Log("Padre opcional desapareceu permanentemente.");
    }
}
