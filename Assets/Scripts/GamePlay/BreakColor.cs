using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BreakColor : BaseComponent
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Image img;
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private ColorType _color;
    public ColorType SetColor
    {
        set
        {
            string name = ColorManager.StringColor(value);
            img.sprite = ColorManager.FindSpriteWithColor(name, _sprites);
            _color = value;
        }
    }

    public ColorType GetColor
    {
        get
        {
            return _color;
        }
    }

    private void OnEnable()
    {
        _animator.enabled = false;
    }

    protected  override void LoadComponent()
    { 
        _animator = GetComponent<Animator>();
        img = this.GetComponent<Image>();
        _sprites = Resources.LoadAll<Sprite>("Sprites/ColorBreak").ToList();
        _animator.enabled = false;
    }
    public void PlayAnimation(ColorType type)
    {
        _animator.enabled = true;
        _animator.Play(ColorManager.StringColor(type));
        StartCoroutine(DesPawn());
    }

    IEnumerator DesPawn()
    {
        yield return new WaitForSeconds(0.45f);
        //_animator.Play("New State");
        _animator.enabled = false;
         Pooling.Despawn(this.gameObject);
    }
}
