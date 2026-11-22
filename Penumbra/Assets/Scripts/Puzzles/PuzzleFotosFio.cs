using UnityEngine;

public class PuzzleFotosFio : MonoBehaviour
{
    [Header("Posições das fotos no fio")]
    public Transform[] posicoesFotos = new Transform[4];

    [Header("Componentes")]
    public Collider2D colisorFio;
    public DestaqueSprite2D destaqueFio;

    [Header("Áudio do encaixe")]
    public AudioSource fonteAudio;
    public AudioClip somEncaixarFoto;

    private int fotosPenduradas = 0;
    private bool fioCompleto = false;

    void Awake()
    {
        if (fonteAudio == null)
            fonteAudio = GetComponent<AudioSource>();
    }

    public bool TentarPendurar(FotoArrastavel foto, Vector2 posicao)
    {
        if (fioCompleto || foto == null)
            return false;

        if (colisorFio == null ||
            !colisorFio.OverlapPoint(posicao))
            return false;

        int indice = foto.ObterIndice();

        if (indice < 0 || indice >= posicoesFotos.Length)
            return false;

        if (posicoesFotos[indice] == null)
        {
            Debug.LogWarning("Defina a posição da foto " + (indice + 1) + " no Inspector.");
            return false;
        }

        foto.ConfirmarEncaixe(posicoesFotos[indice]);
        TocarSom(somEncaixarFoto);

        fotosPenduradas++;

        Debug.Log("Foto pendurada! Total: " + fotosPenduradas);

        if (fotosPenduradas >= 4)
        {
            fioCompleto = true;

            if (colisorFio != null)
                colisorFio.enabled = false;

            if (destaqueFio != null)
                destaqueFio.Destacar(false);

            Debug.Log("Todas as fotos foram penduradas!");
        }

        return true;
    }

    void TocarSom(AudioClip som)
    {
        if (som == null)
            return;

        if (fonteAudio != null)
            fonteAudio.PlayOneShot(som);
        else
            AudioSource.PlayClipAtPoint(som, transform.position);
    }

    public void Destacar(bool valor)
    {
        if (destaqueFio != null && !fioCompleto)
            destaqueFio.Destacar(valor);
    }

    public bool EstaCompleto()
    {
        return fioCompleto;
    }
}
