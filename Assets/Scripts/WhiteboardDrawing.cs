using UnityEngine;
using System.Collections.Generic; // Add this using directive

public class WhiteboardDrawing : MonoBehaviour
{
    public RenderTexture whiteboardTexture;
    public float drawingDuration = 5f;
    public float lineWidth = 0.05f; // Adjust the line width here

    private LineRenderer currentLine;
    private Material drawMaterial;
    private Material eraseMaterial;
    private float drawingTime;
    private Vector3? lastDrawnPosition;
    private bool isDrawing = false;

    void Start()
    {
        drawMaterial = new Material(Shader.Find("Sprites/Default"));
        drawMaterial.mainTexture = whiteboardTexture;
        drawMaterial.color = Color.white;

        eraseMaterial = new Material(Shader.Find("Sprites/Default"));
        eraseMaterial.mainTexture = whiteboardTexture;
        eraseMaterial.color = Color.clear; // Use a transparent color for erasing


        drawingTime = Time.time;
        isDrawing = false; // Add this line to set isDrawing to false initially
    }

    bool isErasing = false; // Add a flag to track erasing state
    List<int> erasedIndices = new List<int>(); // Track the erased point indices

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            StartDrawing();
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            ContinueDrawing();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            StopDrawing();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isErasing = true;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            isErasing = false;
            erasedIndices.Clear();
        }

        if (isErasing)
        {
            Erase();
        }
    }

    void StartDrawing()
    {
        RaycastHit hit;
        if (RaycastToCollider(out hit))
        {
            CreateNewLine(drawMaterial); // Create a new line every time you click
            isDrawing = true;
            AddPoint(hit.point);
        }
    }




    void ContinueDrawing()
    {
        if (isDrawing && currentLine != null) // Check if we are already drawing
        {
            RaycastHit hit;
            if (RaycastToCollider(out hit))
            {
                AddPoint(hit.point);
            }
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
        lastDrawnPosition = null;
    }


    void Erase()
    {
        RaycastHit hit;
        if (RaycastToCollider(out hit) && currentLine != null)
        {
            Vector3 cursorPos = hit.point;
            int linePositionCount = currentLine.positionCount;

            if (linePositionCount > 1)
            {
                for (int i = 0; i < linePositionCount; i++)
                {
                    if (!erasedIndices.Contains(i) && Vector3.Distance(currentLine.GetPosition(i), cursorPos) < 0.1f)
                    {
                        erasedIndices.Add(i);
                    }
                }

                if (erasedIndices.Count > 0)
                {
                    List<Vector3> updatedPositions = new List<Vector3>();
                    for (int i = 0; i < linePositionCount; i++)
                    {
                        if (!erasedIndices.Contains(i))
                        {
                            updatedPositions.Add(currentLine.GetPosition(i));
                        }
                    }

                    currentLine.positionCount = updatedPositions.Count;
                    for (int i = 0; i < updatedPositions.Count; i++)
                    {
                        currentLine.SetPosition(i, updatedPositions[i]);
                    }

                    if (currentLine.positionCount < 2)
                    {
                        Destroy(currentLine.gameObject);
                        currentLine = null;
                    }
                }
            }
        }
    }

    void CreateNewLine(Material material)
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(transform); // Attach the line to the script's GameObject
        currentLine = lineObject.AddComponent<LineRenderer>();
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.material = material;
        currentLine.positionCount = 0;
        drawingTime = Time.time;
        lastDrawnPosition = null;
    }

    void AddPoint(Vector3 point)
    {
        if (!lastDrawnPosition.HasValue || Vector3.Distance(lastDrawnPosition.Value, point) > 0.01f)
        {
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, point);
            lastDrawnPosition = point;
        }
    }

    bool RaycastToCollider(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit) && hit.collider == GetComponent<Collider>();
    }
}
