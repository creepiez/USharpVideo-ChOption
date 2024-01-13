using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3;
using VRC.SDK3.Components;
using VRC.Udon;

namespace UdonSharp.Video.Plugin
{
    /// <summary>
    /// 2個の映像切り替えオブジェクトに表示したいCH番号を送信するオブジェクト
    /// </summary>
    public class VideoChIndexSender : UdonSharpBehaviour
    {
        [SerializeField]
        VideoChController VideoChController;

        [SerializeField]
        int SwitchedVideoIndex;

        /// <summary>
        /// 各映像切り替えオブジェクトにCH番号を送る
        /// </summary>
        public void SendChIndex()
        {
            //インタラクト条件で同期管理オブジェクトに値を送るだけ
            if (VideoChController != null)
            { VideoChController.ChangeSynced(SwitchedVideoIndex); }
        }
    }
}