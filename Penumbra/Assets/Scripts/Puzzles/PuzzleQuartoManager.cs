
using UnityEngine;

public class PuzzleQuartoManager : MonoBehaviour
{
    [Header("Puzzles do quarto")]
    public PuzzleEstante puzzleEstante;
    public PuzzleArmario puzzleArmario;
    public PuzzleFotosFio puzzleFotosFio;

    [Header("Objetivos")]
    public ObjectiveManager objectiveManager;

    [Header("Cutscene")]
    public CutsceneQuartoManager cutsceneQuarto;

    private bool puzzleConcluido = false;

    void Update()
    {
        if (puzzleConcluido)
            return;

        if (puzzleEstante == null ||
            puzzleArmario == null ||
            puzzleFotosFio == null ||
            cutsceneQuarto == null)
            return;

        bool estantePronta = puzzleEstante.EstaCompleta();
        bool armarioPronto = puzzleArmario.EstaCompleto();
        bool fotosProntas = puzzleFotosFio.EstaCompleto();

        if (estantePronta && armarioPronto && fotosProntas)
        {
            puzzleConcluido = true;

            Debug.Log("PUZZLE DO QUARTO CONCLUÍDO!");

            // Conclui o objetivo 6
            if (objectiveManager != null &&
                PlayerPrefs.GetInt("ObjetivoAtual", 0) == 6)
            {
                objectiveManager.ConcluirObjetivo();
                Debug.Log("OBJETIVO DO QUARTO CONCLUÍDO!");
            }
            else
            {
                Debug.LogWarning("ObjectiveManager não encontrado ou objetivo atual diferente de 6.");
            }

            // Inicia a cutscene
            cutsceneQuarto.IniciarCutscene();
        }
    }
}
