
using UnityEngine;

public class MovimentoObstaculo : MonoBehaviour
{
    public float velocidade = 5f;
    public float limiteEsquerdo = -15f;

    private void Update()
    {
        transform.position += Vector3.left * velocidade * Time.deltaTime;

        if (transform.position.x <= limiteEsquerdo)
        {
            Destroy(gameObject);
        }
    }
}
