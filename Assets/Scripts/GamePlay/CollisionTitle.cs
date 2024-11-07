using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CollisionTitle : BaseComponent
{
    private Cube cube;
    private Title currentTitle;
    private Tele currentTele;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        cube = GetComponent<Cube>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Title"))
        {
            currentTitle = other.GetComponent<Title>();

            if (currentTitle != null)
            {
                ProcessTitleInteraction(other);
            }
        }

        if (other.CompareTag("Teleport"))
        {
            AudioManager.Instance.PlaySFX(SoundEffect.Teleport);
            currentTele = other.GetComponent<Tele>();
            if (currentTitle != null)
            {
                ProcessTeleportInteraction(currentTele);
            }
        }
    }

    private void ProcessTitleInteraction(Collider2D other)
    {
        if (currentTitle.ColorType == ColorType.None)
        {
            SetTitleColor();
            if (currentTitle.IsExit)
            {
                StartCoroutine(SpawnLightEffect(other.transform.position,true));
                Debug.Log("WIN");
            }
        }

        if (currentTitle.ColorType == cube.colorBottom)
        {
            HandleMatchingColor(other);
        }
    }

    private void SetTitleColor()
    {
        currentTitle.SetSprite = ColorManager.FindSpriteWithColor(
            ColorManager.StringColor(cube.colorBottom) + "Color", 
            currentTitle.colors
        );
    }

    private void HandleMatchingColor(Collider2D other)
    {
        if (GameManager.Instance.CurrentLevel.condition)
        {
            if (currentTitle.ColorType == UIManager.Instance.CurrentList[0].GetColor)
            {
                UpdateGameStateAndUI(other);
            }
        }
        else
        {
            UpdateGameState(other);
        }
    }

    private void UpdateGameStateAndUI(Collider2D other)
    {
        UpdateGameState(other);
        var uiManager = UIManager.Instance;
        uiManager.CurrentList[0].PlayAnimation(currentTitle.ColorType);
        uiManager.CurrentList.RemoveAt(0);
    }

    private void UpdateGameState(Collider2D other)
    {
        GameManager.Instance.CountBlock--;
        if (GameManager.Instance.CountBlock <= 0)
        {
            EnableExit();
        }
        Pooling.Despawn(other.gameObject);
        GameManager.Instance.ListCanNotGo.Add(other.transform.position);
    }

    private void EnableExit()
    {
        var endTitle = GridManager.Instance.ListTitle[GameManager.Instance.CurrentLevel.endPoint];
        if (endTitle != null)
        {
            endTitle.IsExit = true;
        }
    }

    private IEnumerator SpawnLightEffect(Vector3 pos,bool isWin = false)
    {
        var obj = Pooling.Spawn(GameManager.Instance.PrefabUnLight, pos, quaternion.identity);
        GridManager.Instance.Player.IsMove = false;
        yield return new WaitForSeconds(0.5f);
        Pooling.Despawn(GridManager.Instance.Player.gameObject);
        Pooling.Despawn(obj.gameObject);
        if (isWin)
        {
            if (PlayerPrefs.GetInt("SaveByID") < 5)
            {
                Observer.Instance.PostEvent(EventID.FinishLevel);
            }
            else
            {
                Observer.Instance.PostEvent(EventID.FinishAll);
            }
        }
    }

    private void ProcessTeleportInteraction(Tele teleport)
    {
        StartCoroutine(SpawnLightEffect(teleport.transform.position));
        Observer.Instance.PostEvent(EventID.Teleport,teleport.EndPoint);
    }
}
