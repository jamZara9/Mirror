using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 심전도 그래프를 그리는 스크립트
/// </summary>
public class EKGGraph : MonoBehaviour
{
    public RawImage rawImage;                   // RawImage 컴포넌트, EKG 그래프를 표시할 UI 이미지
    public int width = 256;                     // 그래프의 너비
    public int height = 128;                     // 그래프의 높이
    public float updateInterval = 0.02f;        // 그래프를 업데이트할 간격 (초 단위)
    public Color lineColor = Color.green;       // EKG 선의 색상
    public Color circleColor = Color.green;     // EKG의 현재 위치를 강조할 원의 색상

    private Color normalColor = Color.green;        // HP 50% 이상일 때 색상
    private Color warningColor = new(1f, 0.5f, 0f); // HP 50% 미만 20% 이상일 때 색상
    private Color criticalColor = Color.red;        // HP 20% 미만일 때 색상

    private Texture2D texture;                  // EKG 그래프를 그릴 텍스처
    private Color[] clearColors;                // 텍스처의 배경색을 저장할 배열
    private float timeSinceLastUpdate = 0f;     // 마지막 업데이트 이후의 시간
    private int currentX = 0;                   // 현재 X 좌표
    private int previousY = 0;                  // 이전 Y 좌표
    private float ekgTime = 0f;                 // EKG 파형의 시간

    void Start()
    {
        // 그래프를 그릴 텍스처를 생성하고 RawImage에 설정
        texture = new Texture2D(width, height);
        rawImage.texture = texture;

        clearColors = new Color[width * height];            // 텍스처를 검은색으로 초기화

        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.black;                   // 배경을 검은색으로 초기화
        }

        ClearTexture();                                     // 텍스처를 초기화
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;              // 매 프레임마다 시간을 누적
        
        // updateInterval 간격으로 EKG 그래프를 그림
        if (timeSinceLastUpdate >= updateInterval)      
        {
            timeSinceLastUpdate = 0f;
            DrawEKG();
        }
    }

    /// <summary>
    /// EKG 그래프를 그리는 함수
    /// </summary>
    void DrawEKG()
    {
        int yPos = 0;

        // HP가 0일 때 EKG를 수평으로 유지
        if (GameManager.Instance.playerStatus.CurrentHealth <= 0)
        {
            yPos = height / 2; // 수평선의 Y 좌표 (화면 중앙)
        }
        else
        {
            yPos = (int)(GenerateEKGWaveform(ekgTime) * (height / 2f) + (height / 2f)); // 현재 시간에 따라 EKG 파형의 Y 좌표를 계산
        }

        // 플레이어의 HP 상태에 따라 선과 원의 색상을 설정
        lineColor = GetColorBasedOnHP();
        circleColor = GetColorBasedOnHP();

        // EKG 선 Draw
        DrawLine(currentX - 1, previousY, currentX, yPos, lineColor);

        // 현재 위치를 원형 강조 (동그란 형태)
        DrawCircle(currentX, yPos, 1, circleColor);

        previousY = yPos;                                           // 이전 Y 좌표를 업데이트
        currentX++;                                                 // X 좌표를 증가
        ekgTime += Time.deltaTime * GetCurrentBPMFactor();      // 파형 속도 조절

        // 그래프가 화면을 넘어가면 초기화
        if (currentX >= width)
        {
            currentX = 0;
            ClearTexture();     // 한 화면을 다 지나가면 이전 내용을 지움
        }

        texture.Apply();        // 변경된 텍스처를 적용
    }

    /// <summary>
    /// 텍스처를 초기화하는 함수
    /// </summary>
    void ClearTexture()
    {
        texture.SetPixels(clearColors);
        texture.Apply();
    }

    /// <summary>
    /// Bresenham 알고리즘을 사용하여 선을 그리는 함수
    /// </summary>
    void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            texture.SetPixel(x0, y0, color);

            if (x0 == x1 && y0 == y1) break;

            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    /// <summary>
    /// Bresenham 알고리즘을 사용하여 원을 그리는 함수
    /// </summary>
    void DrawCircle(int cx, int cy, int radius, Color color)
    {
        int d = (5 - radius * 4) / 4;
        int x = 0;
        int y = radius;

        do
        {
            SetPixelCircle(cx, cy, x, y, color);
            SetPixelCircle(cx, cy, y, x, color);
            SetPixelCircle(cx, cy, -x, y, color);
            SetPixelCircle(cx, cy, -y, x, color);
            SetPixelCircle(cx, cy, x, -y, color);
            SetPixelCircle(cx, cy, y, -x, color);
            SetPixelCircle(cx, cy, -x, -y, color);
            SetPixelCircle(cx, cy, -y, -x, color);

            if (d < 0)
            {
                d += 2 * x + 1;
            }
            else
            {
                d += 2 * (x - y) + 1;
                y--;
            }
            x++;
        } while (x <= y);
    }

    void SetPixelCircle(int cx, int cy, int x, int y, Color color)
    {
        texture.SetPixel(cx + x, cy + y, color);
    }

    /// <summary>
    /// 심전도 파형을 생성하는 함수
    /// </summary>
    /// <param name="time"></param>
    float GenerateEKGWaveform(float time)
    {   
        // 심박수(bpm)에 따른 심전도 파형을 생성
        float bpm = GetCurrentBPM();            // 현재 BPM
        float heartCycle = 1f / (bpm / 60f);    // 심박 주기 (초 단위)

        float amplitude = GetWaveformAmplitude(); // 현재 진폭

        // 심전도 각 파형의 위치 및 크기 조정
        float pWave = Mathf.Sin((time % heartCycle) * 2f * Mathf.PI / heartCycle) * amplitude; // P 파
        float qWave = -0.15f * Mathf.Exp(-Mathf.Pow((time % heartCycle - 0.2f * heartCycle) * 10f, 2f)); // Q 파
        float rWave = 0.6f * Mathf.Exp(-Mathf.Pow((time % heartCycle - 0.25f * heartCycle) * 30f, 2f)); // R 파
        float sWave = -0.15f * Mathf.Exp(-Mathf.Pow((time % heartCycle - 0.3f * heartCycle) * 10f, 2f)); // S 파
        float tWave = 0.25f * Mathf.Sin((time % heartCycle - 0.5f * heartCycle) * Mathf.PI / heartCycle) * Mathf.Exp(-Mathf.Pow(time % heartCycle - 0.5f * heartCycle, 2f) * 30f); // T 파

        // 각 파형을 합산하여 반환
        return pWave + qWave + rWave + sWave + tWave;
    }

    /// <summary>
    /// 플레이어의 HP 상태에 따라 색상을 반환하는 함수
    /// </summary>
    private Color GetColorBasedOnHP()
    {
        float hpRatio =  GameManager.Instance.playerStatus.CurrentHealth / GameManager.Instance.playerStatus.settings.maxHealth;

        if (hpRatio < 0.2f)
        {
            return criticalColor; // 20% 미만일 때 빨간색
        }
        else if (hpRatio < 0.5f)
        {
            return warningColor; // 50% 미만일 때 주황색
        }
        else
        {
            return normalColor; // 50% 이상일 때 초록색
        }
    }

    // 현재 HP 비율에 따라 심박수(BPM)를 조정
    private float GetCurrentBPM()
    {
        float hpRatio = GameManager.Instance.playerStatus.CurrentHealth / GameManager.Instance.playerStatus.settings.maxHealth;

        // HP가 100%일 때 기본 BPM을 사용하고, HP가 감소할수록 BPM을 증가
        float baseBPM = 60f;                                    // 기본 BPM
        float maxBPMIncrease = 60f;                             // HP가 0%일 때의 최대 BPM 증가량
        float bpm = baseBPM + (1 - hpRatio) * (maxBPMIncrease * 1.2f);

        // Debug.Log($"HP Ratio: {hpRatio}, BPM: {bpm}");

        return bpm;
    }

    // 현재 BPM에 따라 EKG 파형의 속도를 조정
    private float GetCurrentBPMFactor()
    {
        float bpm = GetCurrentBPM();
        float baseBPM = 60f;
        return bpm / baseBPM;
    }
    
    // 현재 HP 비율에 따라 심전도 진폭을 조정
    private float GetWaveformAmplitude()
    {
        float hpRatio = GameManager.Instance.playerStatus.CurrentHealth / GameManager.Instance.playerStatus.settings.maxHealth;

        // HP가 100%일 때 기본 진폭을 사용하고, HP가 감소할수록 진폭을 증가
        float baseAmplitude = 0.05f;                            // 기본 진폭
        float maxAmplitudeIncrease = 0.2f;                      // HP가 0%일 때의 최대 진폭 증가량
        float amplitude = baseAmplitude + (1 - hpRatio) * maxAmplitudeIncrease;

        return amplitude;
    }
    
}