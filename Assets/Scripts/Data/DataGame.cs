using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataGame",menuName = "Data/DataGame")]
public class DataGame : ScriptableObject
{
    public List<DataLevel> listLevels;
    public Title prefab;
    public Tele teleport;
}

[Serializable]
public class DataLevel
{
    public string name;
    public int width;
    public int height;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public List<BlockColor> blockColors;
    public List<Obstacles> obstaclesList;
    public List<Teleport> TeleportList;
    public List<Vector2> canNotGoes = new List<Vector2>();
    public bool condition;
}
[Serializable]
public class BlockColor
{
    public string name;
    public ColorType color;
    public Vector2 pos;
}

[Serializable]
public class Obstacles
{
    public string name;
    public GameObject prefab;
    public Vector2 pos;
}
[Serializable]
public class Teleport
{
    public string name;
    public Vector2 startPos;
    public Vector2 endPos;
}
