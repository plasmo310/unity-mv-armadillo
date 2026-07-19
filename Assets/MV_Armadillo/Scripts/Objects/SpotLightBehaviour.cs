using MV_Armadillo.TimelineState;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MV_Armadillo.Objects
{
    /// <summary>
    /// SpotLight
    /// 一定角度でループ回転させる.
    /// </summary>
    public class SpotLightBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Timelineステート
        /// </summary>
        [SerializeField] private TimelineStateRhythm rhythmState;
        [SerializeField] private TimelineStateRhythm rhythmHighState;
        
        /// <summary>
        /// ハイリズムの時のみ有効にするライトか？
        /// </summary>
        [SerializeField] private bool isHighLight = false;

        /// <summary>
        /// 回転させるためのパラメータ
        /// </summary>
        [SerializeField] private float minZAngle = -30f;
        [SerializeField] private float maxZAngle = 30f;
        [SerializeField] private float rotateSpeed = 30f;
        
        /// <summary>
        /// 回転反転フラグ
        /// </summary>
        [SerializeField] private bool startTowardsMax = true;

        private Light2D _spotLight;
        private float _initZRotate = 0f;
        private float _initIntensity = 0f;
        private bool _isStartMove = false;

        private void Awake()
        {
            _initZRotate = transform.eulerAngles.z;
            _spotLight = GetComponent<Light2D>();

            // ハイリズムのみ有効にする
            _initIntensity = _spotLight.intensity;
            if (isHighLight)
            {
                _spotLight.intensity = rhythmHighState.IsEnable ? _initIntensity : 0f;
            }
        }

        private void Update()
        {
            // ハイリズムが始まるまで動かさない
            if (rhythmHighState.IsEnable)
            {
                _isStartMove = true;
            }
            if (!_isStartMove)
            {
                return;
            }

            // パラメータから回転させる
            var range = maxZAngle - minZAngle;
            var pingPong = Mathf.PingPong(Time.time * rotateSpeed, range);
            var offset = startTowardsMax ? minZAngle + pingPong : maxZAngle - pingPong;

            var euler = transform.eulerAngles;
            euler.z = _initZRotate + offset;
            transform.eulerAngles = euler;
            
            // ハイリズムのみ有効にする
            if (isHighLight)
            {
                _spotLight.intensity = rhythmHighState.IsEnable ? _initIntensity : 0f;
            }
        }
    }
}
