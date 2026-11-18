
using UnityEngine;

public class PuzzleEstante : MonoBehaviour
{
    [Header("Imagens da estante")]
    public Sprite estanteMetade;
    public Sprite estanteCheia;

    [Header("Componentes")]
    public SpriteRenderer imagemEstante;
    public Collider2D colisorEstante;
    public DestaqueSprite2D destaqueEstante;

    private int pilhasOrganizadas = 0;
    private bool estanteCompleta = false;

    public bool TentarEncaixar(ObjetoLivroArrastavel livro, Vector2 posicao)
    {
        if (estanteCompleta || livro == null)
            return false;

        if (colisorEstante == null ||
            !colisorEstante.OverlapPoint(posicao))
        {
            return false;
        }

        pilhasOrganizadas++;

        livro.ConfirmarEncaixe();

        if (pilhasOrganizadas == 1)
        {
            imagemEstante.sprite = estanteMetade;
            Debug.Log("Primeira pilha de livros organizada!");
        }
        else if (pilhasOrganizadas >= 2)
        {
            imagemEstante.sprite = estanteCheia;
            estanteCompleta = true;

            colisorEstante.enabled = false;

            if (destaqueEstante != null)
                destaqueEstante.Destacar(false);

            Debug.Log("Estante completamente organizada!");
        }

        return true;
    }

    public void Destacar(bool valor)
    {
        if (destaqueEstante != null && !estanteCompleta)
            destaqueEstante.Destacar(valor);
    }

    public bool EstaCompleta()
    {
        return estanteCompleta;
    }
}
