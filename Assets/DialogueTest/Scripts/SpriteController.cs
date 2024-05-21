using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SpriteController : MonoBehaviour
{
    private SpriteSwitcher _switcher;
    private Animator _animator;
    private RectTransform _rect;

    private Queue<IEnumerator> _moveCoroutineQue = new Queue<IEnumerator>();
    private bool _isMoving = false;

    private RectTransform _childRectOne;
    private RectTransform _childRectTwo;
    
    private Image _childImageOne;
    private Image _childImageTwo;

    private void Awake()
    {
        _switcher = GetComponent<SpriteSwitcher>();
        _animator = GetComponent<Animator>();
        _rect = GetComponent<RectTransform>();

        _childRectOne = transform.GetChild(0).GetComponent<RectTransform>();
        _childRectTwo = transform.GetChild(1).GetComponent<RectTransform>();
        
        _childImageOne = transform.GetChild(0).GetComponent<Image>();
        _childImageTwo = transform.GetChild(1).GetComponent<Image>();
    }

    public void SetColorDark()
    {
        _childImageOne.color = Color.gray;
        _childImageTwo.color = Color.gray;
    }

    public void SetColorOrigin()
    {
        _childImageOne.color = Color.white;
        _childImageTwo.color = Color.white;
    }

    public void SetUp(Sprite sprite)
    {
        _switcher.SetImage(sprite);
    }

    public void Show(Vector2 coords)
    {
        _animator.SetTrigger("Show");
        _rect.localPosition = coords;
    }

    public void Hide() {
        _animator.SetTrigger("Hide");
    }

    public void Move(Vector2 coords, float speed)
    {
        _moveCoroutineQue.Enqueue(MoveCoroutine(coords, speed));
        if (!_isMoving)
        {
            StartCoroutine(ProcessCoroutineQueue());
        }
    }

    public void Scale(int width, int height, float speed)
    {
        StartCoroutine(ScaleCoroutine(new Vector2(width, height), speed));
    }

    public void SetFirst()
    {
        _rect.SetAsLastSibling();
    }

    IEnumerator ProcessCoroutineQueue()
    {
        _isMoving = true;
        
        while (_moveCoroutineQue.Count > 0)
        {
            yield return StartCoroutine(_moveCoroutineQue.Dequeue());
        }

        _isMoving = false;
    }

    IEnumerator MoveCoroutine(Vector2 coords, float speed)
    {
        while (_rect.localPosition.x != coords.x || _rect.localPosition.y != coords.y)
        {
            _rect.localPosition = Vector2.MoveTowards
                (_rect.localPosition,
                    coords,
                    speed * 1000f * Time.deltaTime);

            yield return new WaitForSeconds(0.01f);
        }
    }
    
    IEnumerator ScaleCoroutine(Vector2 scale, float speed)
    {
        while (_childRectOne.rect.width != scale.x || _childRectOne.rect.height != scale.y)
        {
            Vector2 curScale = new Vector2(_childRectOne.rect.width, _childRectTwo.rect.height);
            Vector2 scaleDir = new Vector2(scale.x, scale.y);

            Vector2 a = Vector2.Lerp(curScale, scaleDir, speed * Time.deltaTime);
            
            _childRectOne.sizeDelta = a;
            _childRectTwo.sizeDelta = a;

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SwitchSprite(Sprite sprite)
    {
        if (_switcher.GetImage() != sprite)
        {
            _switcher.SwitchImage(sprite);
        }
    }
}
