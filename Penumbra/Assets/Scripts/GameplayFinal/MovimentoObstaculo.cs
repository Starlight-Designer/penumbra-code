
using UnityEngine;

public class MovimentoObstaculo : MonoBehaviour
{
    [Header("Movimento normal")]
    public float velocidade = 8f;
    public float limiteEsquerdo = -15f;

    private bool foiArremessado = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (foiArremessado)
            return;

        transform.position += Vector3.left * velocidade * Time.deltaTime;

        if (transform.position.x <= limiteEsquerdo)
        {
            Destroy(gameObject);
        }
    }

    public void Arremessar()
    {
        foiArremessado = true;
    }
}
