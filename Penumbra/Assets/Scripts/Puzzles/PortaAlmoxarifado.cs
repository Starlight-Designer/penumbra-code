
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortaAlmoxarifado : MonoBehaviour
{
    [Header("Configurações")]
    public string nomeCena = "Almoxarifado";

    private void OnMouseDown()
    {
        int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        // Só permite entrar quando o objetivo do almoxarifado estiver ativo
        if (objetivoAtual != 11)
        {
            Debug.Log("Ainda não é hora de investigar o almoxarifado.");
            return;
        }

        Debug.Log("Entrando no almoxarifado!");

        SceneManager.LoadScene(nomeCena);
    }
}
