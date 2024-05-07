using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpriteController : MonoBehaviour
{
    private SpriteSwitcher _switcher;
    private Animator _animator;
    private RectTransform _rect;

    private Queue<IEnumerator> _moveCoroutineQue = new Queue<IEnumerator>();
    private bool _isMoving = false;

    private void Awake()
    {
        _switcher = GetComponent<SpriteSwitcher>();
        _animator = GetComponent<Animator>();
        _rect = GetComponent<RectTransform>();
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

    public void SwitchSprite(Sprite sprite)
    {
        if (_switcher.GetImage() != sprite)
        {
            _switcher.SwitchImage(sprite);
        }
    }
}
