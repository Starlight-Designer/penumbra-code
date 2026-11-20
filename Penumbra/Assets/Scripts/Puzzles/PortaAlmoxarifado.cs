
using UnityEngine;

public class PortaAlmoxarifado : MonoBehaviour
{
    [Header("Configurações")]
    public string nomeCena = "Almoxarifado";

    [Header("Transição")]
    public FadePreto fadePreto;

    private bool transicaoIniciada = false;

    private void OnMouseDown()
    {
        if (transicaoIniciada)
            return;

        int objetivoAtual = PlayerPrefs.GetInt("ObjetivoAtual", 0);

        // Só permite entrar quando o objetivo do almoxarifado estiver ativo.
        if (objetivoAtual != 11)
        {
            Debug.Log("Ainda não é hora de investigar o almoxarifado.");
            return;
        }

        Debug.Log("Entrando no almoxarifado!");

        transicaoIniciada = true;

        if (fadePreto != null)
        {
            fadePreto.EscurecerECarregar(nomeCena);
        }
        else
        {
            Debug.LogWarning("FadePreto não foi conectado! Carregando cena sem transição.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nomeCena);
        }
    }
}
