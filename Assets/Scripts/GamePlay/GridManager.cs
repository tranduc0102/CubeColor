using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridManager : Sington<GridManager>
{
    [SerializeField] private Camera mainCamera;
    private Dictionary<Vector2, Title> listTitle = new Dictionary<Vector2, Title>();
    private List<GameObject> obstaclesList = new List<GameObject>();
    private List<Tele> teleportList = new List<Tele>();
    private Cube player;

    public Dictionary<Vector2, Title> ListTitle => listTitle;
    public Cube Player
    {
        get => player;
        set => player = value;
    }

    private void OnEnable()
    {
        Observer.Instance.RegisterListener(EventID.NextLevel, _ => SpawnGrid());
        Observer.Instance.RegisterListener(EventID.Teleport, param =>
        {
            if (player.isActiveAndEnabled && player!=null)
            {
                Pooling.Despawn(player.gameObject);
            }
            StartCoroutine(SpawnPlayerWithLight((Vector2)param));
        });
    }

    protected override void Start()
    {
        base.Start();
        SpawnGrid();
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if(mainCamera!=null)return;
        mainCamera = FindObjectOfType<Camera>();
    }

    private void SetCameraPositionAndSize()
    {
        Vector2 gridCenter = new Vector2(GameManager.Instance.CurrentLevel.width, GameManager.Instance.CurrentLevel.height) / 2f - new Vector2(0.5f, 0.5f);
        mainCamera.transform.position = new Vector3(gridCenter.x, gridCenter.y, -10f);

        float aspectRatio = Screen.width / (float)Screen.height;
        float gridHeight = GameManager.Instance.CurrentLevel.height;
        float gridWidth = GameManager.Instance.CurrentLevel.width;

        mainCamera.orthographicSize = (gridWidth / aspectRatio > gridHeight) ? gridWidth / 4f + 0.5f : gridHeight / 2f + 0.5f;
    }

    private void SpawnGrid()
    {
        ClearGrid();
        Observer.Instance.PostEvent(EventID.CheckCondition);
        
        GenerateTitles();
        GenerateObstacles();
        GenerateTeleport();
        
        StartCoroutine(SpawnPlayerWithLight(GameManager.Instance.CurrentLevel.startPoint));

        SetCameraPositionAndSize();
    }

    private void GenerateTitles()
    {
        for (int i = 0; i < GameManager.Instance.CurrentLevel.width; i++)
        {
            for (int j = 0; j < GameManager.Instance.CurrentLevel.height; j++)
            {
                Vector2 position = new Vector2(i, j);

                if (!listTitle.ContainsKey(position) && !GameManager.Instance.ListCanNotGo.Contains(position))
                {
                    var title = Pooling.Spawn(GameManager.Instance.Data.prefab, new Vector3(i, j, 0f), quaternion.identity);
                    SetTitleColorIfMatching(title);
                    listTitle.Add(position, title);
                }
            }
        }
    }

    private void SetTitleColorIfMatching(Title title)
    {
        foreach (var block in GameManager.Instance.CurrentLevel.blockColors)
        {
            if ((Vector2)title.transform.position == block.pos)
            {
                title.ColorType = block.color;
            }
        }
    }

    private void GenerateObstacles()
    {
        if (GameManager.Instance.CurrentLevel.obstaclesList.Count > 0)
        {
            foreach (var obstacle in GameManager.Instance.CurrentLevel.obstaclesList)
            {
                var obstacleObject = Pooling.Spawn(obstacle.prefab, obstacle.pos, Quaternion.identity);
                obstaclesList.Add(obstacleObject);
            }
        }
    }

    private void GenerateTeleport()
    {
        if (GameManager.Instance.CurrentLevel.TeleportList.Count > 0)
        {
            foreach (var teleport in GameManager.Instance.CurrentLevel.TeleportList)
            {
                var teleportObject = Pooling.Spawn(GameManager.Instance.Data.teleport, teleport.startPos, Quaternion.identity);
                teleportObject.EndPoint = teleport.endPos;
                teleportList.Add(teleportObject);
            }
        }
    }
    private void ClearGrid()
    {
        ClearTitles();
        ClearObstacles();
        ClearTeleport();
    }

    private void ClearTitles()
    {
        foreach (var title in listTitle.Values)
        {
            if (title?.gameObject != null)
            {
                Pooling.Despawn(title.gameObject);
            }
        }
        listTitle.Clear();
    }

    private void ClearObstacles()
    {
        foreach (var obstacle in obstaclesList)
        {
            if (obstacle != null)
            {
                Pooling.Despawn(obstacle);
            }
        }
        obstaclesList.Clear();
    }

    private void ClearTeleport()
    {
        foreach (var tele in teleportList)
        {
            if (tele != null)
            {
                Pooling.Despawn(tele.gameObject);
            }
        }
        teleportList.Clear();
    }

    private IEnumerator SpawnPlayerWithLight(Vector2 pos)
    {
        var lightObject = Pooling.Spawn(GameManager.Instance.PrefabLight, pos + new Vector2(0f, 0.5f), quaternion.identity);
        
        yield return new WaitForSeconds(0.5f);
        
        player = Pooling.Spawn(GameManager.Instance.Player, pos, quaternion.identity);
        player.IsMove = false;
        yield return new WaitForSeconds(0.2f);
        Pooling.Despawn(lightObject.gameObject);
        player.IsMove = true;
    }
}
