using MV_Armadillo.TimelineState;
using UnityEngine;

namespace MV_Armadillo.Objects
{
    /// <summary>
    /// Spriteをリズムにあわせて動かす.
    /// </summary>
    public class RhythmSpriteBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Timelineステート
        /// </summary>
        [SerializeField] private TimelineStateRhythm rhythmStateBehaviour;
        [SerializeField] private TimelineStateRhythm rhythmStateLowBehaviour;

        /// <summary>
        /// BPM
        /// </summary>
        [SerializeField] private float bpm = 200f;
        
        /// <summary>
        /// 伸縮させるためのパラメータ
        /// </summary>
        [SerializeField] private float amplitudeVertical = 0.05f; // 縦方向の揺れの大きさ
        [SerializeField] private float amplitudeHorizontal = 0.05f; // 横方向の揺れの大きさ
        [SerializeField] private float pulsesPerBeat = 1f; // 頻度 (1拍あたりの伸縮回数)
        
        /// <summary>
        /// ランダム移動のためのパラメータ
        /// </summary>
        [SerializeField] private Vector2 positionOffset = new Vector2(0.1f, 0.1f); // ランダム位置移動の振れ幅
        [SerializeField] private float positionLerpSpeed = 5f; // 位置オフセットの移動速度
        
        /// <summary>
        /// Sprite切り替えのためのパラメータ
        /// </summary>
        [SerializeField] private Sprite secondSprite; // 設定すると2枚目のSpriteに切り替わる
        [SerializeField] private float pulsesSpritePerBeat = 1f; // 何伸縮ごとにSpriteを切り替えるか (pulsesPerBeatと同じ基準)

        private SpriteRenderer _spriteRenderer;
        private Sprite _firstSprite;

        private Vector3 _initScale;
        private Vector3 _initPosition;
        private Vector3 _currentPositionOffset;
        private Vector3 _targetPositionOffset;
        private float _elapsedTime;
        private int _lastBeatIndex = -1;
        private int _lastPulseIndex = -1;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _firstSprite = _spriteRenderer.sprite;
            _initScale = transform.localScale;
            _initPosition = transform.localPosition;
        }

        private void Update()
        {
            if (!rhythmStateBehaviour.IsEnable)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;

            // 伸縮させる
            var beatDuration = 60f / bpm;
            var pulse = Mathf.Sin(2f * Mathf.PI * pulsesPerBeat * _elapsedTime / beatDuration);
            var scale = _initScale;
            scale.y *= 1f + amplitudeVertical * pulse;
            scale.x *= 1f - amplitudeHorizontal * pulse;
            transform.localScale = scale;

            // ランダム移動させる
            var beatIndex = Mathf.FloorToInt(_elapsedTime / beatDuration);
            if (beatIndex != _lastBeatIndex)
            {
                _lastBeatIndex = beatIndex;
                _targetPositionOffset = new Vector3(
                    Random.Range(-positionOffset.x, positionOffset.x),
                    Random.Range(-positionOffset.y, positionOffset.y),
                    0f);
            }
            _currentPositionOffset = Vector3.Lerp(_currentPositionOffset, _targetPositionOffset, Time.deltaTime * positionLerpSpeed);
            transform.localPosition = _initPosition + _currentPositionOffset;
            
            // Spriteを切り替える
            if (secondSprite != null)
            {
                if (rhythmStateLowBehaviour.IsEnable)
                {
                    _spriteRenderer.sprite = _firstSprite;
                }
                else
                {
                    var switchDuration = beatDuration / Mathf.Max(0.0001f, pulsesSpritePerBeat);
                    var switchIndex = Mathf.FloorToInt(_elapsedTime / switchDuration);
                    if (switchIndex != _lastPulseIndex)
                    {
                        _lastPulseIndex = switchIndex;
                        _spriteRenderer.sprite = switchIndex % 2 != 0 ? secondSprite : _firstSprite;
                    }
                }
            }
        }
    }
}
