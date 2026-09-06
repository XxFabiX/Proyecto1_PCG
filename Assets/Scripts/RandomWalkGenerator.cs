using System.Collections.Generic;
using UnityEngine;

public class RandomWalkGenerator : MonoBehaviour
{
    [Header("Parámetros del Sendero Procedural")]

    public int maxSteps = 350;

    [Range(0f, 100f)]
    public float directionChangeProbability = 15f;

    [Tooltip("Grosor del camino. 1 = 3 vértices de ancho.")]
    public int pathWidth = 1;

    [Tooltip("Qué tanto se hunde el camino respecto al terreno original. Un valor de 0.005 crea una huella sutil sin romper la montaña.")]
    [Range(0f, 0.05f)]
    public float pathDepth = 0.005f;

    public HashSet<Vector2Int> pathPositions { get; private set; } = new HashSet<Vector2Int>();

    public void CarvePath(float[,] heights)
    {
        pathPositions.Clear();
        int resolution = heights.GetLength(0);

        Vector2Int currentPos = new Vector2Int(10, resolution / 2);
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        Vector2Int currentDir = Vector2Int.right;
        Vector2Int lastDir = currentDir;

        for (int i = 0; i < maxSteps; i++)
        {
            CarveArea(heights, currentPos, resolution);

            currentPos += currentDir;

            if (currentPos.x < 2 || currentPos.x >= resolution - 2 ||
                currentPos.y < 2 || currentPos.y >= resolution - 2)
            {
                break;
            }

            if (Random.Range(0f, 100f) < directionChangeProbability)
            {
                List<Vector2Int> validDirs = new List<Vector2Int>();

                foreach (Vector2Int dir in directions)
                {
                    if (dir != -lastDir)
                    {
                        validDirs.Add(dir);
                    }
                }

                currentDir = validDirs[Random.Range(0, validDirs.Count)];
                lastDir = currentDir;
            }
        }
    }

    private void CarveArea(float[,] heights, Vector2Int center, int resolution)
    {
        for (int x = center.x - pathWidth; x <= center.x + pathWidth; x++)
        {
            for (int y = center.y - pathWidth; y <= center.y + pathWidth; y++)
            {
                if (x >= 0 && x < resolution && y >= 0 && y < resolution)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    pathPositions.Add(pos);

                    // SOLUCIÓN AL HOYO: Ahora simplemente restamos un valor muy pequeño (pathDepth)
                    // a la altura que ya existe. Así el camino sube y baja con la montaña.
                    heights[y, x] = Mathf.Clamp01(heights[y, x] - pathDepth);
                }
            }
        }
    }
}