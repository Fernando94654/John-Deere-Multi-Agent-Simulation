using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController controller;
    public FieldGridGenerator fieldGridGenerator;
    

    public TMP_InputField rowsInputField;
    public TMP_InputField columnsInputField;

    public TMP_Text timeText;

    private int rows,columns;
    private bool simulacionIniciada;
    public float timer = 0f;

    void Awake()
    {
        controller = this;
    }

    void Update()
    {
        

        if (simulacionIniciada)
        {
            timer += Time.deltaTime;
        timeText.text = "Tiempo: " + timer.ToString("F2") + "s";
            Tractor.tractor.Move();
            Harvester.harvester.Move();
        }
    }

    public void OnGenerateButton()
    {
        Debug.Log("Generando campo con " + rowsInputField.text + " filas y " + columnsInputField.text + " columnas.");
    rows = int.Parse(rowsInputField.text);
    columns = int.Parse(columnsInputField.text);

    fieldGridGenerator.GenerateGrid(rows, columns);
    Spawner.spawner.SpawnObstacles(fieldGridGenerator.fields);
    iniciarSimulacion();
    }

    void iniciarSimulacion()
    {
        simulacionIniciada = true;
    }
}
