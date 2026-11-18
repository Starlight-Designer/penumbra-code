
using UnityEngine;

public class PuzzleArmario : MonoBehaviour
{
    [Header("Componentes")]
    public Collider2D colisorArmario;
    public DestaqueSprite2D destaqueArmario;

    private int roupasGuardadas = 0;
    private bool armarioCompleto = false;

    public bool TentarGuardar(RoupaArrastavel roupa, Vector2 posicao)
    {
        if (armarioCompleto || roupa == null)
            return false;

        if (colisorArmario == null ||
            !colisorArmario.OverlapPoint(posicao))
        {
            return false;
        }

        roupasGuardadas++;

        roupa.ConfirmarEncaixe();

        Debug.Log("Roupa guardada! Total: " + roupasGuardadas);

        if (roupasGuardadas >= 4)
        {
            armarioCompleto = true;

            colisorArmario.enabled = false;

            if (destaqueArmario != null)
                destaqueArmario.Destacar(false);

            Debug.Log("Todas as roupas foram guardadas!");
        }

        return true;
    }

    public void Destacar(bool valor)
    {
        if (destaqueArmario != null && !armarioCompleto)
            destaqueArmario.Destacar(valor);
    }

    public bool EstaCompleto()
    {
        return armarioCompleto;
    }
}
