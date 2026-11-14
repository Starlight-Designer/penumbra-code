
using UnityEngine;

public class GerenciadorPuzzlePeixes : MonoBehaviour
{
    private int peixesOrganizados = 0;

    public void RegistrarPeixe()
    {
        peixesOrganizados++;

        Debug.Log("Peixes organizados: " + peixesOrganizados);

        if (peixesOrganizados == 10)
        {
            Debug.Log("BIANCA NÃO ESTÁ AQUI.");
        }
    }
}
