using UnityEngine;

public static class CursorGlobal
{
    // Ponta do dedo na imagem de 64 px
    private static readonly Vector2 hotspot = new Vector2(5, 1);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Aplicar()
    {
        Texture2D cursor = Resources.Load<Texture2D>("cursor");

        if (cursor == null)
        {
            Debug.LogWarning("cursor não encontrado em Assets/Resources");
            return;
        }

        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}