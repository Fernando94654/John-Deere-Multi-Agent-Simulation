using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController controller;
    public FieldGridGenerator fieldGridGenerator;
    public WebSocketManager webSocketManager;

    // The engine leaves a bare headland per side; below this there is no crop left.
    private const int MinimumSide = 6;


    public TMP_InputField rowsInputField;
    public TMP_InputField columnsInputField;

    public TMP_Text timeText;

    private int rows,columns;
    private bool simulationStarted;
    public float timer = 0f;

    void Awake()
    {
        controller = this;
    }

    void Update()
    {
        

        if (simulationStarted)
        {
            timer += Time.deltaTime;
            timeText.text = "Tiempo: " + timer.ToString("F2") + "s";

            // Scripted agents live only in SampleScene, so they are optional here.
            if (Tractor.tractor != null)
            {
                Tractor.tractor.Move();
            }

            if (Harvester.harvester != null)
            {
                Harvester.harvester.Move();
            }
        }
    }

    public void OnGenerateButton()
    {
        if (!int.TryParse(rowsInputField.text, out rows) || !int.TryParse(columnsInputField.text, out columns))
        {
            Debug.LogWarning("Rows and columns must be whole numbers.");
            return;
        }

        if (rows < MinimumSide || columns < MinimumSide)
        {
            Debug.LogWarning("The field must be at least " + MinimumSide + "x" + MinimumSide + ".");
            return;
        }

        // The server builds the field from this size; Unity draws it on the first state.
        webSocketManager.SendConfiguration(rows, columns);
        StartSimulation();
    }

    void StartSimulation()
    {
        simulationStarted = true;
    }
}
