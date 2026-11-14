
using UnityEngine;

public class GerenciadorPuzzlePeixes : MonoBehaviour
{
    [Header("Popup do puzzle")]
    public GameObject painelPopupPeixes;

    [Header("Som do popup")]
    public AudioSource audioPopup;

    private int peixesOrganizados = 0;

    public void RegistrarPeixe()
    {
        peixesOrganizados++;

        Debug.Log("Peixes organizados: " + peixesOrganizados);

        if (peixesOrganizados == 10)
        {
            Debug.Log("BIANCA NÃO ESTÁ AQUI.");

            if (painelPopupPeixes != null)
            {
                painelPopupPeixes.SetActive(true);

                if (audioPopup != null)
                {
                    audioPopup.time = 0.3f;
                    audioPopup.Play();
                }
            }
        }
    }

    public void FecharPopup()
    {
        if (painelPopupPeixes != null)
        {
            painelPopupPeixes.SetActive(false);
        }
    }
}
