
using UnityEngine;

public class DetectorPastor : MonoBehaviour
{
    public PastorPerseguicao pastor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            if (pastor == null)
            {
                Debug.LogError("DetectorPastor: referência do Pastor não atribuída!");
                return;
            }

            Debug.Log("DetectorFrente detectou Alice!");
            pastor.IniciarAtaque();
        }
    }
}
