
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonSharp.Video.Plugin
{
    /// <summary>
    /// 表示画面のオンオフも出来る
    /// </summary>
    public class VideoChControllerWithVisibleSwitch : VideoChController
    {
        /// <summary>
        /// 表示するか？（同期用）
        /// </summary>
        [UdonSynced(UdonSyncMode.None)]
        private bool _isVisible = false;

        /// <summary>
        /// UGUIのToggle（チェックボックス）を指定する
        /// 汎用性なんか知らねえし
        /// </summary>
        [SerializeField]
        public Toggle ToggleSource;

        protected override void OnScriptStart()
        {
            SwitchMeshes(_isVisible);
        }

        /// <summary>
        /// このオブジェクトのインタラクト条件
        /// 表示/非表示を切り替えて同期する
        /// </summary>
        public void ChangeVisibleSynced()
        {
            //インスタンスオーナーを自分にする
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            //値を更新
            _isVisible = ToggleSource.isOn;
            //値を更新したので他の人に同期をリクエスト
            RequestSerialization();
            //ここで自分だけ本処理
            Change();
        }

        /// <inheritdoc/>
        public override void OnDeserialization()
        {
            //インスタンスオーナーが同期受信処理しても意味がない
            if (Networking.IsOwner(gameObject))
            { return; }
            //表示切替とCH切り替えを行う
            Change();
            //操作者以外のチェックボックス表示を同期する
            ToggleSource.isOn = _isVisible;
        }

        /// <summary>
        /// 表示になったらCH切り替え処理
        /// 非表示になったら全て非表示
        /// </summary>
        protected override void Change()
        {
            //非表示であれば全て非表示にして終了
            if (!_isVisible)
            {
                SwitchMeshes(false);
                return;
            }
            //CH切り替えを行う
            base.Change();
        }

        /// <summary>
        /// メッシュの表示状態を変更する
        /// </summary>
        /// <param name="enabled">表示するか？</param>
        private void SwitchMeshes(bool enabled)
        {
            if (Screens == null)
            { return; }

            foreach (var t in Screens)
            {
                if (t != null)
                { t.enabled = enabled; }
            }
        }
    }
}