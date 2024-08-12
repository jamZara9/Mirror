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
    // 스프라이트별 로직 처리에 필요한 변수.
    private SpriteSwitcher _switcher;
    private Animator _animator;
    private RectTransform _rect;

    // 처리가 겹쳤을 때, 이를 저장하는 Queue.
    private Queue<IEnumerator> _moveCoroutineQue = new Queue<IEnumerator>();
    private Queue<IEnumerator> _scaleCoroutineQue = new Queue<IEnumerator>();
    private bool _isMoving = false;
    private bool _isScaling = false;

    // 하위 오브젝트의 정보.
    private RectTransform _childRectOne;
    private RectTransform _childRectTwo;
    private Image _childImageOne;
    private Image _childImageTwo;

    private void Awake()
    {
        // 각 변수에 컴포넌트 들고와 저장.
        _switcher = GetComponent<SpriteSwitcher>();
        _animator = GetComponent<Animator>();
        _rect = GetComponent<RectTransform>();

        _childRectOne = transform.GetChild(0).GetComponent<RectTransform>();
        _childRectTwo = transform.GetChild(1).GetComponent<RectTransform>();
        
        _childImageOne = transform.GetChild(0).GetComponent<Image>();
        _childImageTwo = transform.GetChild(1).GetComponent<Image>();
    }

    /// <summary>
    /// 스프라이트가 어두워지는 함수.
    /// </summary>
    public void SetColorDark()
    {
        _childImageOne.color = Color.gray;
        _childImageTwo.color = Color.gray;
    }

    /// <summary>
    /// 스프라이트가 밝아지는 함수.
    /// </summary>
    public void SetColorOrigin()
    {
        _childImageOne.color = Color.white;
        _childImageTwo.color = Color.white;
    }
    
    /// <summary>
    /// 자신 포함 모든 스프라이트가 밝아지는 함수.
    /// </summary>
    /// <param name="sprites">스프라이트 저장 딕셔너리</param>
    public void AllBright(Dictionary<Speaker, SpriteController> sprites)
    {
        foreach (var sprite in sprites)
        {
            sprite.Value.SetColorOrigin();
        }
    }

    /// <summary>
    /// 자신 포함 모든 스프라이트가 어두워지는 함수.
    /// </summary>
    /// <param name="sprites">스프라이트 저장 딕셔너리</param>
    public void AllDark(Dictionary<Speaker, SpriteController> sprites)
    {
        foreach (var sprite in sprites)
        {
            sprite.Value.SetColorDark();
        }
    }

    /// <summary>
    /// 스프라이트 처음 이미지 세팅.
    /// </summary>
    /// <param name="sprite">세팅할 이미지</param>
    public void SetUp(Sprite sprite)
    {
        _switcher.SetImage(sprite);
    }

    /// <summary>
    /// 스프라이트 생성 애니메이션.
    /// </summary>
    /// <param name="coords">생성 위치</param>
    public void Show(Vector2 coords)
    {
        _animator.SetTrigger("Show");
        _rect.localPosition = coords;
    }

    /// <summary>
    /// 스프라이트 숨기기.
    /// </summary>
    public void Hide() {
        _animator.SetTrigger("Hide");
    }

    /// <summary>
    /// 스프라이트 이동.
    /// </summary>
    /// <param name="coords">목적지</param>
    /// <param name="speed">이동 소요 시간</param>
    public void Move(Vector2 coords, float speed)
    {
        _moveCoroutineQue.Enqueue(MoveCoroutine(coords, speed));
        if (!_isMoving)
        {
            StartCoroutine(ProcessMoveQueue());
        }
    }

    /// <summary>
    /// 스프라이트 크기 변환
    /// </summary>
    /// <param name="width">너비</param>
    /// <param name="height">높이</param>
    /// <param name="speed">변환 소요 시간</param>
    public void Scale(int width, int height, float speed)
    {
        _scaleCoroutineQue.Enqueue(ScaleCoroutine(new Vector2(width, height), speed));
        if (!_isScaling)
        {
            StartCoroutine(ProcessScaleQueue());
        }
    }

    /// <summary>
    /// 스프라이트를 가장 앞에 보이도록.
    /// </summary>
    public void SetFirst()
    {
        _rect.SetAsLastSibling();
    }

    /// <summary>
    /// 스프라이트 이동 실행.
    /// </summary>
    IEnumerator ProcessMoveQueue()
    {
        _isMoving = true;
        
        while (_moveCoroutineQue.Count > 0)
        {
            yield return StartCoroutine(_moveCoroutineQue.Dequeue());
        }
        
        _isMoving = false;
    }
    
    /// <summary>
    /// 스프라이트 크기 변환 실행.
    /// </summary>
    IEnumerator ProcessScaleQueue()
    {
        _isScaling = true;
        
        while (_scaleCoroutineQue.Count > 0)
        {
            yield return StartCoroutine(_scaleCoroutineQue.Dequeue());
        }
        
        _isScaling = false;
    }

    /// <summary>
    /// 스프라이트의 실질적 이동을 처리.
    /// </summary>
    /// <param name="coords">목적지</param>
    /// <param name="duration">이동 소요 시간</param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(Vector2 coords, float duration)
    {
        float currentTime = 0;
        Vector2 startPosition = _rect.localPosition;

        if (duration == 0)
        {
            _rect.localPosition = coords;
        }
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            _rect.localPosition = Vector2.Lerp(startPosition,coords, currentTime / duration);

            yield return null;
        }
    }
    
    /// <summary>
    /// 스프라이트의 실질적 크기 변환을 처리.
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator ScaleCoroutine(Vector2 scale, float duration)
    {
        float currentTime = 0;
        Vector2 startScale = new Vector2(_childRectOne.rect.width, _childRectTwo.rect.height);
        
        if (duration == 0)
        {
            _childRectOne.sizeDelta = scale;
            _childRectTwo.sizeDelta = scale;
        }
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            Vector2 currentScale = Vector2.Lerp(startScale, scale, currentTime / duration);
            _childRectOne.sizeDelta = currentScale;
            _childRectTwo.sizeDelta = currentScale;

            yield return null;
        }
    }

    /// <summary>
    /// 스프라이트 이미지 변경.
    /// </summary>
    /// <param name="sprite"></param>
    public void SwitchSprite(Sprite sprite)
    {
        if (_switcher.GetImage() != sprite)
        {
            _switcher.SwitchImage(sprite);
        }
    }
}
