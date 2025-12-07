using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Configuración del Mapa")]
    [SerializeField]
    private Vector3Int dimensiones = new(3, 3, 3);
    [SerializeField]
    private float grosorParedes = 0.1f;
    
    [Header("Referencias (Prefabs)")]
    [SerializeField]
    private GameObject prefabNodoVisual;
    [SerializeField]
    private GameObject prefabBorde;

    [Header("Contenedor Principal")]
    [SerializeField]
    private Transform contenedorMapa;

    [ContextMenu("Generar Mapa")]
    public void GenerarMapa()
    {
        // 1. Gestión del Contenedor Principal
        if (contenedorMapa == null)
        {
            // Si no existe, buscamos si hay uno en la escena por nombre
            GameObject objExistente = GameObject.Find("Map");
            
            if (objExistente != null)
            {
                contenedorMapa = objExistente.transform;
            }
            else
            {
                // Si no, lo creamos
                contenedorMapa = new GameObject("Map").transform;
            }
        }

        // 2. LIMPIEZA TOTAL: Borramos todo lo que haya dentro del contenedor principal
        // Usamos un while porque DestroyImmediate modifica la lista de hijos en tiempo real
        while (contenedorMapa.childCount > 0)
        {
            DestroyImmediate(contenedorMapa.GetChild(0).gameObject);
        }

        // 3. Crear los Sub-Contenedores
        GameObject containerNodosObj = new("Nodes");
        containerNodosObj.transform.SetParent(contenedorMapa);
        
        GameObject containerBordesObj = new("Borders");
        containerBordesObj.transform.SetParent(contenedorMapa);

        // 4. Generar usando los nuevos padres
        GenerarNodos(containerNodosObj.transform);
        GenerarBordes(containerBordesObj.transform);
        
        Debug.Log($"Mapa generado: {dimensiones.x}x{dimensiones.y}x{dimensiones.z}");
    }

    private void GenerarNodos(Transform padreNodos)
    {
        for (int x = 0; x < dimensiones.x; x++)
        {
            for (int y = 0; y < dimensiones.y; y++)
            {
                for (int z = 0; z < dimensiones.z; z++)
                {
                    Vector3 posicion = new Vector3(x, y, z);
                    GameObject nodo = Instantiate(prefabNodoVisual, posicion, Quaternion.identity);
                    
                    // Asignamos al sub-contenedor de nodos
                    nodo.transform.SetParent(padreNodos);
                    nodo.name = $"Node_{x}_{y}_{z}";
                }
            }
        }
    }

    private void GenerarBordes(Transform padreBordes)
    {
        Vector3 centro = new((dimensiones.x - 1) / 2f, (dimensiones.y - 1) / 2f, (dimensiones.z - 1) / 2f);
        
        float minX = -0.5f - grosorParedes/2;
        float maxX = dimensiones.x - 0.5f + grosorParedes/2;
        float minY = -0.5f - grosorParedes/2;
        float maxY = dimensiones.y - 0.5f + grosorParedes/2;
        float minZ = -0.5f - grosorParedes/2;
        float maxZ = dimensiones.z - 0.5f + grosorParedes/2;

        // Pasamos 'padreBordes' a la función CrearPared
        CrearPared(new Vector3(minX, centro.y, centro.z), new Vector3(grosorParedes, dimensiones.y, dimensiones.z), padreBordes, "Wall_Left");
        CrearPared(new Vector3(maxX, centro.y, centro.z), new Vector3(grosorParedes, dimensiones.y, dimensiones.z), padreBordes, "Wall_Right");
        CrearPared(new Vector3(centro.x, minY, centro.z), new Vector3(dimensiones.x, grosorParedes, dimensiones.z), padreBordes, "Wall_Down");
        CrearPared(new Vector3(centro.x, maxY, centro.z), new Vector3(dimensiones.x, grosorParedes, dimensiones.z), padreBordes, "Wall_Up");
        CrearPared(new Vector3(centro.x, centro.y, minZ), new Vector3(dimensiones.x, dimensiones.y, grosorParedes), padreBordes, "Wall_Front");
        CrearPared(new Vector3(centro.x, centro.y, maxZ), new Vector3(dimensiones.x, dimensiones.y, grosorParedes), padreBordes, "Wall_Back");
    }

    private void CrearPared(Vector3 pos, Vector3 escala, Transform padre, string nombre)
    {
        GameObject pared = Instantiate(prefabBorde, pos, Quaternion.identity);
        pared.transform.localScale = escala;
        pared.transform.SetParent(padre); // Asignamos al sub-contenedor de bordes
        pared.name = nombre;
    }
}