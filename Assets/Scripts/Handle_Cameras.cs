using NUnit.Framework;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Handle_Cameras : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public VehicleManager vehicleManager;
    public Button botonMenu;
    public Button prefab_boton;
    public GameObject Menu_Panel;
    public Transform Menu_Contenido;
    private List<Button> botones_creados = new List<Button>();

    public GameObject panelCamara;
    public RawImage displayCamara;
    private Camera activeCamera;

    public TMP_Text tituloCamara;
    public Button botonCerrar;

    void Awake()
    {

        botonMenu.onClick.AddListener(boton_menu);
        tituloCamara.text = "Selecciona un vehiculo";
    }
    public void boton_menu()
    {
        bool isOpen = !Menu_Panel.activeSelf;
        Menu_Panel.SetActive(isOpen);
        if (isOpen)
        {
            construir_menu();
        }

    }

    void crear_botones(string id, string label)
    {
        // recibir las dos cosas d eid y label 
        Button boton = Instantiate(prefab_boton);
        boton.gameObject.SetActive(true);

        boton.transform.SetParent(Menu_Contenido, false);

        TextMeshProUGUI texto_boton = boton.GetComponentInChildren<TextMeshProUGUI>();

        texto_boton.text = label + " " + id;

        boton.onClick.AddListener(() => seleccionar_vehiculo(id, label));
        botones_creados.Add(boton);

    }

    void limpiarMenu()
    {
        foreach (Button boton in botones_creados)
        {
            Destroy(boton.gameObject);
        }
        botones_creados.Clear();
    }
    void construir_menu()
    {
        limpiarMenu();
        // para cada uno de los ids del vehico tanto de tractor y harvester
        foreach (string id in vehicleManager.TractorIds)
        {
            crear_botones(id, "tractor");

        }
        foreach (string id in vehicleManager.HarvesterIds)
        {
            crear_botones(id, "harvester");

        }

    }
    void seleccionar_vehiculo(string id, string label)
    {
        GameObject vehiculo = vehicleManager.GetVehicleObject(id);

        if (vehiculo != null)
        {
            Camera camera = vehiculo.GetComponentInChildren<Camera>(true);
            camera.targetTexture = new RenderTexture(630, 432, 16);
            activeCamera = camera;
            if (displayCamara != null && panelCamara != null)
            {
                displayCamara.texture = camera.targetTexture;
                panelCamara.SetActive(true);
            }
        }
        tituloCamara.text = label + " " + id;
        Menu_Panel.SetActive(false);
    }

    public void cerrarCamaras()
    {
        seleccionar_vehiculo("C0", "");
        tituloCamara.text = "Selecciona un vehiculo";
    }


    void Update()
    {
        if (activeCamera != null)
        {
            activeCamera.Render();
        }
    }
}