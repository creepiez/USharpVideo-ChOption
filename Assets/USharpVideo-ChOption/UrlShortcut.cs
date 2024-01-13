
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace UdonSharp.Video.Plugin
{
    /// <summary>
    /// 動画URLを自動入力する
    /// </summary>
    public class UrlShortcut : UdonSharpBehaviour
    {
        [Header("操作先のUSharpVideoに含まれるゲームオブジェクトをそれぞれ指定する")]
        [SerializeField]
        SyncModeController SyncModeToggle;

        [SerializeField]
        VideoControlHandler VideoControlsUI;

        [SerializeField]
        VRCUrlInputField UrlInputField;

        [Header("USharpVideoに送るURLの設定")]
        [SerializeField]
        bool IsStreamMode = false;

        [SerializeField]
        VRCUrl EnteredUrl;

        /// <summary>
        /// 任意のゲームオブジェクトからこのメソッドをコールすると
        /// 設定されているUSharpVideoオブジェクトにURLが自動入力される
        /// </summary>
        public void InputUrl()
        {
            if (VideoControlsUI == null || UrlInputField == null || EnteredUrl == null)
            { return; }
            //ストリームモード切り替え（U#Videoにあるモード切り替えボタンの操作と同じ動作をするだけ）
            if (IsStreamMode)
            { SyncModeToggle.ClickStreamToggle(); }
            else
            { SyncModeToggle.ClickVideoToggle(); }
            //ここでSetUrlをコールしてもイベントが発火しないのでOnURLInputをコールする必要がある
            UrlInputField.SetUrl(EnteredUrl);
            VideoControlsUI.OnURLInput();
        }
    }
}
