using UnityEngine;

public class DetectorPastor : MonoBehaviour
{
    public PastorPerseguicao pastor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pastor.IniciarAtaque();
        }
    }
}