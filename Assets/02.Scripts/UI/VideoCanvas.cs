using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Utils;
using TMPro;
using System;
using UnityEngine.InputSystem;

namespace UI{
    public class VideoCanvas : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;               // 비디오 플레이어
        private TextMeshProUGUI _skipTextGUI;           // 스킵 텍스트
        private Coroutine _skipTextCoroutine;           // 스킵 텍스트 코루틴
        private AnyKeyInputAction _anyKeyInputAction;   // AnyKeyInputAction


        private bool _isSkipTextActive => _skipTextGUI.gameObject.activeSelf;  // 스킵 텍스트 활성화 여부
        private bool _isVideoPlaying => _videoPlayer.isPlaying;               // 비디오 플레이어 재생 여부

        private bool _canReceiveInput = true; // 입력을 받을 수 있는지 여부
        private float _inputCooldownTime = 0.5f; // 쿨타임 (예: 0.5초)
        private float _lastInputTime = 0f; // 마지막 입력 시간

        void Start()
        {
            Debug.Log("비디오 캔버스 시작");
            _anyKeyInputAction = GameManager.inputManager.GetInputActionStrategy("AnyKey") as AnyKeyInputAction;
        }

        void Update()
        {
            if(_isVideoPlaying){
                if(_anyKeyInputAction.IsAnyKeyPressed && _canReceiveInput){
                    if(_isSkipTextActive){
                        SetSkipTextActive(false);
                        StopVideo();
                    }else{
                        SetSkipTextActive(true);
                    }
                    
                    // 입력 처리 후 일정 시간 동안 입력을 받지 않도록 설정
                    _canReceiveInput = false;
                    _lastInputTime = Time.time;
                }

                // 쿨타임이 지나면 다시 입력을 받을 수 있도록 설정
                if (!_canReceiveInput && Time.time - _lastInputTime > _inputCooldownTime)
                {
                    _canReceiveInput = true;
                }
            }
            
        }

        /// <summary>
        /// 비디오 컴포넌트 설정
        /// </summary>
        /// <param name="settings"></param>
        public void SetVideoSetting(VideoSetting settings)
        {
            if(_videoPlayer == null){
                _videoPlayer = GetComponent<VideoPlayer>();
                _skipTextGUI = GetComponentInChildren<TextMeshProUGUI>();

                _skipTextGUI.gameObject.SetActive(false);   // 초기 SkipText 비활성화
            }

            // 비디오 플레이어 설정
            _skipTextGUI.text = settings.skipText;
            _videoPlayer.clip = settings.videoClip;
            _videoPlayer.isLooping = settings.isLoop;
            _videoPlayer.playbackSpeed = settings.playbackSpeed;
            _videoPlayer.SetDirectAudioVolume(0, settings.volume);  // 비디오 플레이어의 볼륨 설정

            // 비디오가 끝났을 때 호출될 이벤트 핸들러 등록
            _videoPlayer.loopPointReached += OnVideoFinished;
        }


        #region video control
        /// <summary>
        /// 비디오 재생
        /// </summary>
        public void PlayVideo()
        {
            if (_videoPlayer == null)
            {
                _videoPlayer = GetComponent<VideoPlayer>();
            }

            // 비디오 재생 전 첫 프레임 대기
            _videoPlayer.waitForFirstFrame = true;
            _videoPlayer.Play();
        }

        /// <summary>
        /// 비디오 정지
        /// </summary>
        public void StopVideo()
        {
            if (_videoPlayer == null)
            {
                _videoPlayer = GetComponent<VideoPlayer>();
            }

            _videoPlayer.Stop();
            OnVideoFinished(_videoPlayer);
        }

        /// <summary>
        /// 비디오 일시 정지
        /// </summary>
        public void PauseVideo()
        {
            if (_videoPlayer == null)
            {
                _videoPlayer = GetComponent<VideoPlayer>();
            }

            _videoPlayer.Pause();
        }
        


        #endregion

        #region Skip Text Control
        /// <summary>
        /// 스킵 텍스트를 활성화/비활성화 하는 함수
        /// </summary>
        /// <param name="isActive">스킵 텍스트 활성화 여부</param>
        public void SetSkipTextActive(bool isActive)
        {
            if(isActive){
                _skipTextCoroutine = StartCoroutine(ShowSkipText(1.0f, 2.0f));
            }else{
                // 스킵  텍스트 코루틴이 실행 중이면 중지
                if(_skipTextCoroutine != null){
                    StopCoroutine(_skipTextCoroutine);
                    _skipTextCoroutine = null;
                }
                _skipTextGUI.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 스킵 텍스트를 보여주는 코루틴
        /// </summary>
        /// <param name="fadeDuration">텍스트를 온전히 출력하는데 걸리는 시간</param>
        /// <param name="displayDuration">텍스트 유지 시간</param>
        /// <returns></returns>
        private IEnumerator ShowSkipText(float fadeDuration, float displayDuration)
        {
            // 텍스트를 천천히 나타나게 함
            _skipTextGUI.gameObject.SetActive(true);
            _skipTextGUI.alpha = 0;
            float elapsed = 0;
            
            while (elapsed < fadeDuration)
            {
                _skipTextGUI.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _skipTextGUI.alpha = 1;

            // 특정 시간 동안 텍스트를 유지함
            yield return new WaitForSeconds(displayDuration);

            // 텍스트를 천천히 사라지게 함
            elapsed = 0;
            while (elapsed < fadeDuration)
            {
                _skipTextGUI.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _skipTextGUI.alpha = 0;
            _skipTextGUI.gameObject.SetActive(false);
        }
        #endregion
    
        private void OnVideoFinished(VideoPlayer vp)
        {
            GameManager.uiManager.UIFinished(gameObject);
        }
    }
}

