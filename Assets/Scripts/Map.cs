using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public static Map Instance;

    public int sizeX, sizeY;
    public Tilemap tilemap;
    public Transform smallUnitsParent, unitsCellsParent, surfacesCellsParent, projectileCellsParent, wallsParent;
    public List<Cell> allCells;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Wall wallPrefab;
    private List<Transform> _cellsParents;
    //private List<Cell[,]> _layers;
    public Dictionary<MapLayer, Cell[,]> layers;
    private const int _layersCount = 4;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        InitCellsParents();
        InitCells();
        InitLayers();
        InitWalls();
        InitCellsMonitoringOnUnitsLayer();
    }

    public void InitCellsParents()
    {
        _cellsParents = new List<Transform>();
        _cellsParents.Add(smallUnitsParent);
        _cellsParents.Add(unitsCellsParent);
        _cellsParents.Add(surfacesCellsParent);
        _cellsParents.Add(projectileCellsParent);
    }

    public void InitCells()
    {
        BoundsInt bounds = tilemap.cellBounds;
        sizeX = bounds.size.x;
        sizeY = bounds.size.y;
        layers = new Dictionary<MapLayer, Cell[,]>();

        for (int k = 0; k < _layersCount; k++)
        {
            layers.Add((MapLayer)k, new Cell[sizeX, sizeY]);

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (tilemap.GetTile(new Vector3Int(i, j, 0)) != null)
                    {
                        var instantiatedCell = Instantiate(cellPrefab, tilemap.CellToWorld(new Vector3Int(i, j, 0)) + Vector3.up * 0.5f, Quaternion.identity);
                        layers[(MapLayer)k][i, j] = instantiatedCell;
                        instantiatedCell.transform.SetParent(_cellsParents[k]);
                        instantiatedCell.coordinates = new Vector2Int(i, j);
                        instantiatedCell.mapLayer = (MapLayer)k;
                    }
                }
            }
        }

        allCells = new List<Cell>();

        foreach (var layer in layers)
        {
            foreach (var cell in layer.Value)
            {
                allCells.Add(cell);
            }
        }
    }

    public void InitWalls()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if ((i == 0 || j == 0 || i == sizeX - 1 || j == sizeY - 1))
                {
                    var wall = Instantiate(wallPrefab, wallsParent);
                    layers[MapLayer.DefaultUnit][i, j].Content = wall;
                    wall.transform.position = layers[MapLayer.DefaultUnit][i, j].coordinates.ToVector3();
                }
            }
        }
    }

    public void InitLayers()
    {
        /*smallUnitsLayer = _layers[MapLayer.SmallUnit];
        unitsLayer = _layers[MapLayer.DefaultUnit];
        surfacesLayer = _layers[MapLayer.Surface];*/
    }

    public void InitCellsMonitoringOnUnitsLayer()
    {
        for (int i = 0; i < layers[0].GetLength(0); i++)
        {
            for (int j = 0; j < layers[0].GetLength(1); j++)
            {
                if (tilemap.GetTile(new Vector3Int(i, j, 0)) != null)
                {
                    var activateTriggerInProjectiles = layers[MapLayer.Projectile][i, j].GetComponent<ActivateTriggerOnUnitsLayerCellFilled>();
                    var activateTriggerInSmallUnits = layers[MapLayer.SmallUnit][i, j].GetComponent<ActivateTriggerOnUnitsLayerCellFilled>();
                    var activateTriggerInSurfaces = layers[MapLayer.Surface][i, j].GetComponent<ActivateTriggerOnUnitsLayerCellFilled>();

                    layers[MapLayer.DefaultUnit][i, j].OnBecameFull.AddListener(activateTriggerInProjectiles.TriggerOnBecameFull);
                    layers[MapLayer.DefaultUnit][i, j].OnBecameEmpty.AddListener(activateTriggerInProjectiles.TriggerOnBecameEmpty);
                    layers[MapLayer.DefaultUnit][i, j].OnBecameFull.AddListener(activateTriggerInSurfaces.TriggerOnBecameFull);
                    layers[MapLayer.DefaultUnit][i, j].OnBecameEmpty.AddListener(activateTriggerInSurfaces.TriggerOnBecameEmpty);
                    
                    layers[MapLayer.SmallUnit][i, j].OnBecameFull.AddListener(activateTriggerInProjectiles.TriggerOnBecameFull);
                    layers[MapLayer.SmallUnit][i, j].OnBecameEmpty.AddListener(activateTriggerInProjectiles.TriggerOnBecameEmpty);
                    layers[MapLayer.SmallUnit][i, j].OnBecameFull.AddListener(activateTriggerInSurfaces.TriggerOnBecameFull);
                    layers[MapLayer.SmallUnit][i, j].OnBecameEmpty.AddListener(activateTriggerInSurfaces.TriggerOnBecameEmpty);
                    
                    layers[MapLayer.DefaultUnit][i, j].OnBecameFull.AddListener(activateTriggerInSmallUnits.TriggerOnBecameFull);
                    layers[MapLayer.DefaultUnit][i, j].OnBecameEmpty.AddListener(activateTriggerInSmallUnits.TriggerOnBecameEmpty);
                }
            }
        }
    }

    public Cell FindNeighborhoodForCell(Cell startCell, Vector2Int direction)
    {
        return GetLayerByCell(startCell)[startCell.coordinates.x + direction.x, startCell.coordinates.y + direction.y];
    }

    public Cell[,] GetLayerByCell(Cell cell)
    {
        return layers[cell.mapLayer];
    }

    public Cell[,] GetLayerByType(MapLayer mapLayer)
    {
        return layers[mapLayer];
    }

    public Cell GetUnitsLayerCellByCoordinates(Vector2Int coordinates) 
    {
        return layers[MapLayer.DefaultUnit][coordinates.x, coordinates.y];
    }

    public List<Cell> GetCellsColumn(Cell cell) => GetCellsColumn(cell.coordinates);
    public List<Cell> GetCellsColumn(Vector2Int coordinates)
    {
        List<Cell> cells = new List<Cell>();
        foreach (var layer in layers)
        {
            cells.Add(layer.Value[coordinates.x, coordinates.y]);
        }

        return cells;
    }

    public List<Cell> GetCellsColumnIgnoreSurfaces(Vector2Int coordinates)
    {
        var temp = GetCellsColumn(coordinates);
        temp.Remove(layers[MapLayer.Surface][coordinates.x, coordinates.y]);
        return temp;
    }

    public Cell GetCell(Vector2Int coordinates, MapLayer mapLayer)
    {
        return layers[mapLayer][coordinates.x, coordinates.y];
    }
}
