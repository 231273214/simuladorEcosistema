using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulacion : MonoBehaviour
{
    // Parámetros iniciales
    public int Pt = 200; // Cantidad de plantas
    public int Ht = 40;  // Número de herbívoros
    public int Ct = 8;   // Número de carnívoros

    // Tasas y parámetros del ecosistema
    public float g = 0.3f;   // Tasa de crecimiento de plantas
    public float Gh = 0.05f; // Consumo de plantas por cada herbívoro
    public float eh = 0.02f; // Eficiencia de conversión de plantas en herbívoros
    public float mr = 0.1f;  // Mortalidad natural de herbívoros
    public float Cc = 0.02f; // Consumo de herbívoros por cada carnívoro
    public float ec = 0.01f; // Eficiencia de conversión de herbívoros en carnívoros
    public float mc = 0.1f;  // Mortalidad natural de carnívoros

    // Intervalo de tiempo para la simulación
    public float simulationInterval = 1.0f;
    private float timer = 0f;

    // Historiales 
    private List<int> plantHistory = new List<int>();
    private List<int> herbivoreHistory = new List<int>();
    private List<int> carnivoreHistory = new List<int>();

    // Visualización en pantalla
    public GameObject plantPrefab;
    public GameObject herbivorePrefab;
    public GameObject carnivorePrefab;

    public Vector2 simulationArea = new Vector2(10f, 10f);
    public float plantSize = 0.3f;
    public float herbivoreSize = 0.5f;
    public float carnivoreSize = 0.7f;

    private List<GameObject> plantVisuals = new List<GameObject>();
    private List<GameObject> herbivoreVisuals = new List<GameObject>();
    private List<GameObject> carnivoreVisuals = new List<GameObject>();

    void Start()
    {
        // Inicializar historiales
        plantHistory.Add(Pt);
        herbivoreHistory.Add(Ht);
        carnivoreHistory.Add(Ct);

        // Inicializar visualización en pantalla
        DrawSimulation2D();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= simulationInterval)
        {
            Simulate();
            timer = 0f;
        }
    }

    void Simulate()
    {
        // Calcular nuevas poblaciones
        int newPt = CalculateNewPlants();
        int newHt = CalculateNewHerbivores();
        int newCt = CalculateNewCarnivores();

        // Para asegurar que la población no sea negativa
        Pt = Mathf.Max(0, newPt);
        Ht = Mathf.Max(0, newHt);
        Ct = Mathf.Max(0, newCt);

        // Guardar en historial
        plantHistory.Add(Pt);
        herbivoreHistory.Add(Ht);
        carnivoreHistory.Add(Ct);

        // Actualizar visualización en pantalla
        DrawSimulation2D();

        // Mostrar resultados en consola
        Debug.Log($"Tiempo: {Time.time:F2}s - Plantas: {Pt}, Herbívoros: {Ht}, Carnívoros: {Ct}");
    }

    int CalculateNewPlants()
    {
        // Pt+1 = Pt + g·Pt - (Gh · Ht)
        float result = Pt + (g * Pt) - (Gh * Ht);
        return Mathf.RoundToInt(result);
    }

    int CalculateNewHerbivores()
    {
        // Ht+1 = Ht + (eh * Gh * Ht) - (mr * Ht) - (Cc * Ct)
        float result = Ht + (eh * Gh * Ht) - (mr * Ht) - (Cc * Ct);
        return Mathf.RoundToInt(result);
    }

    int CalculateNewCarnivores()
    {
        // Ct+1 = Ct + (ec * Cc * Ct) - (mc * Ct)
        float result = Ct + (ec * Cc * Ct) - (mc * Ct);
        return Mathf.RoundToInt(result);
    }

    // Visualización de las especies
    void DrawSimulation2D()
    {
        // Limpiar visualizaciones anteriores
        ClearVisualization();

        // Dibujar plantas
        int visualPlantCount = Mathf.Clamp(Pt / 10, 1, 100);
        for (int i = 0; i < visualPlantCount; i++)
        {
            Vector2 position = GetRandomPositionInArea();
            CreatePlantVisual(position);
        }

        // Dibujar herbívoros
        int visualHerbivoreCount = Mathf.Clamp(Ht, 1, 50);
        for (int i = 0; i < visualHerbivoreCount; i++)
        {
            Vector2 position = GetRandomPositionInArea();
            CreateHerbivoreVisual(position);
        }

        // Dibujar carnívoros
        int visualCarnivoreCount = Mathf.Clamp(Ct, 1, 20);
        for (int i = 0; i < visualCarnivoreCount; i++)
        {
            Vector2 position = GetRandomPositionInArea();
            CreateCarnivoreVisual(position);
        }
    }

    GameObject CreatePlantVisual(Vector2 position)
    {
        if (plantPrefab == null) return null;
        GameObject plant = Instantiate(plantPrefab, position, Quaternion.identity, transform);
        plant.transform.localScale = Vector3.one * plantSize;
        plantVisuals.Add(plant);
        return plant;
    }

    GameObject CreateHerbivoreVisual(Vector2 position)
    {
        if (herbivorePrefab == null) return null;
        GameObject herbivore = Instantiate(herbivorePrefab, position, Quaternion.identity, transform);
        herbivore.transform.localScale = Vector3.one * herbivoreSize;
        herbivoreVisuals.Add(herbivore);
        return herbivore;
    }

    GameObject CreateCarnivoreVisual(Vector2 position)
    {
        if (carnivorePrefab == null) return null;
        GameObject carnivore = Instantiate(carnivorePrefab, position, Quaternion.identity, transform);
        carnivore.transform.localScale = Vector3.one * carnivoreSize;
        carnivoreVisuals.Add(carnivore);
        return carnivore;
    }

    Vector2 GetRandomPositionInArea()
    {
        float x = Random.Range(-simulationArea.x / 2f, simulationArea.x / 2f);
        float y = Random.Range(-simulationArea.y / 2f, simulationArea.y / 2f);
        return new Vector2(x, y);
    }

    // Para limpiar la escena 
    void ClearVisualization()
    {
        // Destruir todas las plantas
        foreach (GameObject plant in plantVisuals)
        {
            if (plant != null) Destroy(plant);
        }
        plantVisuals.Clear();

        // Destruir todos los herbívoros
        foreach (GameObject herbivore in herbivoreVisuals)
        {
            if (herbivore != null) Destroy(herbivore);
        }
        herbivoreVisuals.Clear();

        // Destruir todos los carnívoros
        foreach (GameObject carnivore in carnivoreVisuals)
        {
            if (carnivore != null) Destroy(carnivore);
        }
        carnivoreVisuals.Clear();
    }

    // Métodos para acceder a los datos desde la UI
    public int GetCurrentPlants() { return Pt; }
    public int GetCurrentHerbivores() { return Ht; }
    public int GetCurrentCarnivores() { return Ct; }

    public List<int> GetPlantHistory() { return plantHistory; }
    public List<int> GetHerbivoreHistory() { return herbivoreHistory; }
    public List<int> GetCarnivoreHistory() { return carnivoreHistory; }
}
