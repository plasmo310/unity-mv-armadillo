using UnityEngine;

namespace MV_Armadillo.TimelineState
{
    /// <summary>
    /// リズムを刻んでいるか. (Timelineからの指定用)
    /// </summary>
    public class TimelineStateRhythm : MonoBehaviour
    {
        public bool IsEnable => gameObject.activeSelf;
    }
}