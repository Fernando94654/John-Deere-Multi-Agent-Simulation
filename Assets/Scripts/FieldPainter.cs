using UnityEngine;

// Colours each cell by its server-side state: gold for standing crop, soil for cut.
public class FieldPainter : MonoBehaviour
{
    public Color cropColor = new Color(0.85f, 0.72f, 0.24f);
    public Color harvestedColor = new Color(0.45f, 0.33f, 0.20f);

    // The values the engine puts in crop.cells.
    private const int Obstacle = -1;
    private const int Harvested = 0;

    // A property block recolours without cloning a material per cell, as .material would.
    private MaterialPropertyBlock block;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    // Skips unchanged cells: only a few are cut per tick.
    private int[] painted;

    public void Paint(GameObject[] fields, int[] cells)
    {
        if (fields == null || cells == null)
        {
            return;
        }

        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }

        // A resized grid invalidates the cache, which would compare against the old field.
        if (painted == null || painted.Length != cells.Length)
        {
            painted = new int[cells.Length];
            for (int i = 0; i < painted.Length; i++)
            {
                painted[i] = int.MinValue;
            }
        }

        int count = Mathf.Min(fields.Length, cells.Length);

        for (int i = 0; i < count; i++)
        {
            if (painted[i] == cells[i])
            {
                continue;
            }

            painted[i] = cells[i];

            // Rock cells already carry an obstacle prefab; recolouring would hide it.
            if (cells[i] == Obstacle || fields[i] == null)
            {
                continue;
            }

            Renderer renderer = fields[i].GetComponent<Renderer>();

            if (renderer == null)
            {
                continue;
            }

            renderer.GetPropertyBlock(block);
            block.SetColor(BaseColorId, cells[i] == Harvested ? harvestedColor : cropColor);
            renderer.SetPropertyBlock(block);
        }
    }
}
