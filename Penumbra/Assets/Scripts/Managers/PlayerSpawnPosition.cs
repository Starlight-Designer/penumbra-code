using UnityEngine;

public class PlayerSpawnPosition : MonoBehaviour
{
    public Vector2 posicaoInicial;

    void Start()
    {
        transform.position = new Vector3(
            posicaoInicial.x,
            posicaoInicial.y,
            transform.position.z
        );
    }
}