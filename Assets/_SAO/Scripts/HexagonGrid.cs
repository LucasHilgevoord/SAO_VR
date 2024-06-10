using System.Collections;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public GameObject hexagonPrefab;
    public int gridRadius = 5;

    private float hexWidth;
    private float hexHeight;
    private float hexVerticalSpacing;
    private float hexHorizontalSpacing;

    void Start()
    {
        // Ensure hexWidth and hexHeight are calculated correctly if not set in the Inspector
        if (hexagonPrefab != null)
        {
            MeshRenderer hexRenderer = hexagonPrefab.GetComponent<MeshRenderer>();
            hexWidth = hexRenderer.bounds.size.x;
            hexHeight = hexRenderer.bounds.size.z;
            hexVerticalSpacing = hexHeight * 0.75f; // Vertical spacing adjusted for hexagon overlap
            hexHorizontalSpacing = hexWidth * 1.50f; // Horizontal spacing should be the full width of the hexagon
        }

        StartCoroutine(CreateHexagonGrid());
    }

    IEnumerator CreateHexagonGrid()
    {
        // Instantiate the center hexagon
        Instantiate(hexagonPrefab, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(0.2f);

        for (int ring = 1; ring <= gridRadius; ring++)
        {
            for (int i = 0; i < 6 * ring; i++)
            {
                Vector3 position = HexRingPosition(ring, i) + transform.position;
                Instantiate(hexagonPrefab, position, Quaternion.identity, transform);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    Vector3 HexRingPosition(int ring, int i)
    {
        // Calculate the hex position in the hexagonal coordinate system
        int sector = i / ring;
        int offset = i % ring;

        int q = 0;
        int r = 0;

        switch (sector)
        {
            case 0: // Right
                q = ring;
                r = -offset;
                break;
            case 1: // Top-Right
                q = ring - offset;
                r = -ring;
                break;
            case 2: // Top-Left
                q = -offset;
                r = -ring + offset;
                break;
            case 3: // Left
                q = -ring;
                r = offset;
                break;
            case 4: // Bottom-Left
                q = -ring + offset;
                r = ring;
                break;
            case 5: // Bottom-Right
                q = offset;
                r = ring - offset;
                break;
        }

        return HexToPosition(q, r);
    }

    Vector3 HexToPosition(int q, int r)
    {
        float x = hexHorizontalSpacing * (q + r / 2.0f);
        float z = hexVerticalSpacing * r;
        return new Vector3(x, 0, z);
    }
}
