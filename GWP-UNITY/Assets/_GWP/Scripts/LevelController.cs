using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour, ISceneController<IInputManager>
{
    public Vector3Int mapSize = new Vector3Int(100, 100, 1);
    public Tilemap tilemap;
    public Tilemap previewTilemap;
    public CharacterMotor motorPrefab;

    public PrefabLibrary prefabLibrary;
    public CameraMover cameraMover;
    [System.NonSerialized] public LevelData levelData;
    public LevelSearch searchGraph;
    public LevelEditControl levelEditControl;

    private Queue<BuildTask> tileTask = new Queue<BuildTask>();
    private List<CharacterMotor> characterMotors = new List<CharacterMotor>();
    private GestureRecognizer gestureRecognizer;
    private PlayerCameraControl cameraControl;
    private IInputManager inputManager;

    public Vector3 CellToWorld(Vector3Int cell) => tilemap.GetCellCenterWorld(cell);

    public Vector3Int WorldToCell(Vector3 worldPosition) => tilemap.WorldToCell(worldPosition);

    public void Initialize(IInputManager inputManager)
    {
        this.inputManager = inputManager;

        // Generate empty level
        levelData = new LevelData(mapSize);
        levelData.Library = prefabLibrary;
        for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
        {
            for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
            {
                var floor = new Floor("Floor")
                {
                    gridPosition = new Vector3Int(x, y, 0)
                };
                levelData.PlaceFloor(floor);
            }
        }

        CreateCharacter();

        // Apply level to tilemap
        foreach (var floor in levelData.GetLevelEntities<Floor>())
        {
            tilemap.SetTile(floor.gridPosition,
                prefabLibrary.GetFloorInfo(floor.entityId).Prefab);
        }

        // Create Controls
        gestureRecognizer = new GestureRecognizer(inputManager);
        cameraControl = new PlayerCameraControl(cameraMover, gestureRecognizer);
        cameraMover.positionBounds = Rect.MinMaxRect(
            levelData.Bounds.xMin,
            levelData.Bounds.yMin,
            levelData.Bounds.xMax,
            levelData.Bounds.yMax);
        gestureRecognizer.TapCompleted += OnTapCompleted;

        levelEditControl = new LevelEditControl(gestureRecognizer, this);

        // Set up Pathfinding
        searchGraph = new LevelSearch(this);
    }

    public void SetItem(Item item)
    {
        bool willUpdateTilemap = true;
        if (levelData.GetEntity(item.gridPosition, out Item existing))
        {
            willUpdateTilemap = existing.entityId != item.entityId;
        }
        if (willUpdateTilemap) tilemap.SetTile(item.gridPosition, prefabLibrary.GetItemInfo(item.entityId).Prefab);
    }

    public void PreviewZone(RectInt rect)
    {
        ClearPreview();
        if (0 == rect.size.x && 0 == rect.size.y) return;
        for (int y = rect.yMin; y < rect.yMax + 1; y++)
        {
            for (int x = rect.xMin; x < rect.xMax + 1; x++)
            {
                previewTilemap.SetTile(new Vector3Int(x, y, 0), prefabLibrary.zoneTile);
            }
        }
    }

    public void ClearPreview() => previewTilemap.ClearAllTiles();

    public void EnqueueConstruct(RectInt rect)
    {
        if (0 == rect.size.x && 0 == rect.size.y) return;
        for (int y = rect.yMin; y < rect.yMax + 1; y++)
        {
            for (int x = rect.xMin; x < rect.xMax + 1; x++)
            {
                EnqueueConstruct(new Vector3Int(x, y, 0));
            }
        }
    }

    private void CreateCharacter()
    {
        var character = new Character(
            prefabLibrary.CharacterPrefabInfos[0].DisplayName,
            prefabLibrary.CharacterPrefabInfos[0].Id,
            prefabLibrary.CharacterPrefabInfos[0].Id + System.Guid.NewGuid().ToString()
        );
        levelData.characters.Add(character);

        var motor = Instantiate(motorPrefab);
        motor.Level = this;
        motor.Dijkstra = new Dijkstra<SearchNode<Vector3Int>, Vector3Int>(
            mapSize.x * mapSize.y, 4, coord => new SearchNode<Vector3Int>(coord));
        characterMotors.Add(motor);
    }

    private void OnTapCompleted(Pointer pointer)
    {
        if (inputManager.AltPointerId == pointer.pointerId) return;

        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(pointer.position);
        Vector3Int tileMousePosition = tilemap.WorldToCell(worldMousePosition);
        EnqueueConstruct(tileMousePosition);
    }

    private void EnqueueConstruct(Vector3Int position)
    {
        tilemap.SetTile(position, prefabLibrary.GetItemInfo("Wall_WIP").Prefab);
        tileTask.Enqueue(new BuildTask()
        {
            position = position,
            itemToBuild = "Wall"
        });
    }

    private void Start()
    {
        Initialize(_App.Instance.Input);
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (var motor in characterMotors)
        {
            if (0 == tileTask.Count) break;
            if (null != motor.Task) continue;
            motor.Task = tileTask.Dequeue();
        }

        foreach (var motor in characterMotors)
        {
            motor.Tick(Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (null != gestureRecognizer) gestureRecognizer.Dispose();
    }
}

public class BuildTask
{
    public Vector3Int position;
    public string itemToBuild;
}

[System.Serializable]
public class LevelEditControl
{
    private GestureRecognizer gestures;
    private LevelController level;
    private Pointer? startPointer;
    private IInputManager Input => _App.Instance.Input;

    public LevelEditControl(GestureRecognizer gestures, LevelController level)
    {
        this.gestures = gestures;
        this.level = level;
        Subscribe();
    }

    private void OnDragStarted(Pointer pointer)
    {
        if (Input.AltPointerId == pointer.pointerId) return;
        startPointer = pointer;
    }

    private void OnDragMoved(Pointer pointer)
    {
        if (!startPointer.HasValue) return;
        level.PreviewZone(GetRect(pointer));
    }

    private void OnDragCompleted(Pointer pointer)
    {
        if (!startPointer.HasValue) return;
        level.ClearPreview();
        level.EnqueueConstruct(GetRect(pointer));
        startPointer = null;
    }

    private void OnPointerPressed(Pointer pointer)
    {
        if (pointer.pointerId != Input.AltPointerId) return;

        // Cancel
        startPointer = null;
        level.ClearPreview();
    }

    private RectInt GetRect(Pointer pointer)
    {
        Vector3Int a = level.WorldToCell(Input.InteractionCam.ScreenToWorldPoint(startPointer.Value.position));
        Vector3Int b = level.WorldToCell(Input.InteractionCam.ScreenToWorldPoint(pointer.position));
        var size = (Vector2Int)(b - a);
        return new RectInt((Vector2Int)a, size);
    }

    private void Subscribe()
    {
        Input.PointerPressed += OnPointerPressed;
        gestures.DragStarted += OnDragStarted;
        gestures.DragMoved += OnDragMoved;
        gestures.DragCompleted += OnDragCompleted;
    }

    private void Unsubscribe()
    {
        gestures.DragStarted -= OnDragStarted;
        gestures.DragMoved -= OnDragMoved;
        gestures.DragCompleted -= OnDragCompleted;
    }
}