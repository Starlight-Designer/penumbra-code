
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nomeCena = "Casa";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (nomeCena == "Delegacia")
            {
                PlayerPrefs.SetInt("ObjetivoAtual", 1);
                PlayerPrefs.Save();
            }

            SceneManager.LoadScene(nomeCena);
        }
    }
}
