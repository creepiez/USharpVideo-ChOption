
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp.Video;

namespace UdonSharp.Video.Plugin
{
    /// <summary>
    /// CH切り替えオブジェクト
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VideoChController : UdonSharpBehaviour
    {
        [SerializeField]
        [Header("Ch別に映像が表示されるメッシュを指定する")]
        public MeshRenderer[] Screens;

        [SerializeField]
        [Header("Ch別にオーディオソースをまとめている親オブジェクトを指定する")]
        GameObject[] Audios;

        [SerializeField]
        [Header("Syncを行うためにVideoControlHandlerをそれぞれ指定する")]
        VideoControlHandler[] VideoControlsUIs;

        [SerializeField]
        [Header("Ch表示など任意の切り替えたいオブジェクトを指定する")]
        GameObject[] SwitchedOtherObjects;

        /// <summary>
        /// 現在有効になっているインデックス（同期用）
        /// </summary>
        [UdonSynced(UdonSyncMode.None)]
        private int _activatedIndex = 0;

        void Start()
        {
            OnScriptStart();
        }

        /// <summary>
        /// スクリプト開始時の処理（Start()をオーバーライド出来ないため）
        /// </summary>
        protected virtual void OnScriptStart()
        {
            Change();
        }

        /// <summary>
        /// 切り替え処理本体
        /// OnDeserialization()からコールする事
        /// </summary>
        protected virtual void Change()
        {
            SwitchScreens(Screens, _activatedIndex);
            SwitchChildAudios(Audios, _activatedIndex);
            SwitchOtherObjects(SwitchedOtherObjects, _activatedIndex);
        }

        /// <summary>
        /// 切り替えて同期する
        /// 外部オブジェクトからコールする事
        /// </summary>
        /// <param name="index"></param>
        public void ChangeSynced(int index)
        {
            if (_activatedIndex == index)
            { return; }

            //インスタンスオーナーを自分にする
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            //値を更新
            _activatedIndex = index;
            //値を更新したので他の人に同期をリクエスト
            RequestSerialization();
            //ここで自分だけ本処理
            Change();
        }

        /// <inheritdoc/>
        public override void OnDeserialization()
        {
            //インスタンスオーナーが同期受信処理しても意味がない（そもそも自分に同期受信処理が来るのか？）
            if (Networking.IsOwner(gameObject))
            { return; }
            //それ以外は同期完了後処理を行う
            Change();
        }

        /// <summary>
        /// ビデオ表示スクリーンの切り替え
        /// </summary>
        /// <param name="objs">スクリーンがあるオブジェクトの配列</param>
        /// <param name="index">切り替え先のインデックス</param>
        private void SwitchScreens(MeshRenderer[] objs, int index)
        {
            //配列インスタンスが無い
            if (objs == null)
            { return; }
            //インデックスがオーバーラン/アンダーラン
            if (objs.Length - 1 < index || index < 0)
            { return; }
            //要素にインスタンスが無い
            if (objs[index] == null)
            { return; }

            //ゲームオブジェクトごと非表示にすると再生機能でエラーになるので表示先メッシュを非表示にする
            foreach (var t in objs)
            { t.enabled = false; }
            objs[index].enabled = true;
        }

        /// <summary>
        /// 子要素にあるオーディオソースの切り替え
        /// </summary>
        /// <param name="objs">オーディオソースをまとめている親オブジェクトの一覧</param>
        /// <param name="index">切り替え先のインデックス</param>
        private void SwitchChildAudios(GameObject[] objs, int index)
        {
            if (objs == null)
            { return; }
            if (objs.Length - 1 < index || index < 0)
            { return; }
            if (objs[index] == null)
            { return; }

            //ゲームオブジェクトごと非表示にするとU#Video側でエラーになるので子要素のオーディオをミュートにする
            foreach (var t in objs)
            { MuteChildAudios(t, true); }
            MuteChildAudios(objs[index], false);
        }

        /// <summary>
        /// 子オブジェクトにあるオーディオソースを取り出して音量をオンオフする
        /// </summary>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="mute">true指定でミュート</param>
        private void MuteChildAudios(GameObject parent, bool mute)
        {
            var audios = parent.GetComponentsInChildren<AudioSource>();
            foreach (var audio in audios)
            { audio.mute = mute; }
        }

        /// <summary>
        /// 他オブジェクトの表示を切り替える
        /// </summary>
        /// <param name="objs">対象のオブジェクト</param>
        /// <param name="index">切り替え先のインデックス</param>
        private void SwitchOtherObjects(GameObject[] objs, int index)
        {
            if (objs == null)
            { return; }
            if (objs.Length - 1 < index || index < 0)
            { return; }
            if (objs[index] == null)
            { return; }

            foreach (var t in objs)
            { t.SetActive(false); }
            objs[index].SetActive(true);
        }

        /// <summary>
        /// ローカルResync
        /// 現在表示中のビデオのみResyncされる
        /// </summary>
        public void Resync()
        {
            if (VideoControlsUIs == null)
            { return; }
            if (VideoControlsUIs.Length - 1 < _activatedIndex || _activatedIndex < 0)
            { return; }
            if (VideoControlsUIs[_activatedIndex] == null)
            { return; }

            VideoControlsUIs[_activatedIndex].OnReloadButtonPressed();
        }
    }
}