using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] private string message = "Você encontrou uma pista!";
    [SerializeField] private bool disableAfterFound = false;

    private bool found;

    void OnMouseDown()
    {
        if (found) return;

        Debug.Log(message);
        found = true;

        if (disableAfterFound)
            gameObject.SetActive(false);
    }
}