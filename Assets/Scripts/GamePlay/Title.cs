using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Title : BaseComponent
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private ColorType _color;
    private bool isBlockColor;
    private bool isExit;
    public List<Sprite> colors;
    
    public ColorType ColorType
    {
        set
        {
            isBlockColor = true;
            _color = value;
            sr.sprite = ColorManager.FindSpriteWithColor(ColorManager.StringColor(_color)+"ColorRequest", colors);
        }
        get
        {
            return _color;
        }
    }

    public Sprite SetSprite
    {
        set
        {
            if (!isBlockColor) sr.sprite = value;
        }
    }

    public bool IsExit
    {
        set
        {
            sr.sprite = ColorManager.FindSpriteWithColor("ExitColor", colors);
            isExit = value;
        } 
        get
        {
            return isExit;
        } 
    }

    private void OnEnable()
    {
        ResetStatus();
    }

    private void ResetStatus()
    {
        isExit = false;
        _color = ColorType.None;
        isBlockColor = false;
        sr.sprite = colors[6];
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        if (sr != null) return;
        sr = GetComponent<SpriteRenderer>();
        colors = Resources.LoadAll<Sprite>("Sprites/TitleColor").ToList();
    }
    
}
