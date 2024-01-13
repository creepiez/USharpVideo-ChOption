# USharpVideo-ChOption

[USharpVideo](https://github.com/MerlinVR/USharpVideo) のオプションとして使えるスクリプト集です。

クラブイベントのワールド実装にどうぞ。

- インタラクトでビデオURLを自動入力
  - RTSPやTwitchなどのストリームにも対応
- USharpVideoオブジェクトのチャンネル切り替え
  - 映像・音声ともに同時切り替え
  - メッシュ等任意のオブジェクトの同時切り替え
- チャンネル、表示/非表示の同時切り替え（サンプルコードのみ）

# 設計思想

クラブイベントを主催していると、DJごとにRTSP (Topazchat, VRCDN)を使いたい人とTwitchを使いたい人で分かれる事に気づきました。

一般的なワールド実装ではRTSP専用、もしくはビデオプレイヤーを1個設置となっているはずです。

操作方法の観点からすると、RTSPの場合はワールドに対してURLが静的であり使用時に変更出来ないようになっている事がほとんどです。一方、Twitchは都度ユーザがURLを都度入力する必要があります。どちらの場合でも、ストリーム再生開始は出番の直前にしか行えず、操作ミスが発生する可能性も高いため、無駄な時間が発生する事がよくあります。

URLを簡単に入力出来る、もしくは出番前に先にURLを入力し再生しておけば、全体の待ち時間は最小となり、前の演者の時点で準備も可能です。

また、ビデオプレイヤーアセットの共通化も可能となります。

ここではVRChatで基本的なビデオプレイヤーであるUSharpVideoを再生環境としました。

# 注意事項

タイミングによってはビデオを複数同時再生する事となります。ユーザ側の負荷が上がるため、可能な限り複数再生を抑えましょう。

# ファイル

詳しい使い方はコードを読んでください。めんどくさいので取説や英語版のREADMEはありません。

## UrlShortcut.cs

対象のUSharpVideoコントロールにURLを自動入力します。

手操作でビデオモードを変更してURLを入力する事と同じ動作をします。

任意のGameObjectに追加してください。

インタラクトやUGUIから`InputUrl()`をコールしてください。

## VideoChIndexSender.cs

後述する`VideoChangerMaster`オブジェクトと組み合わせてください。

`VideoChangerMaster`オブジェクトに、あらかじめ設定されたチャンネル番号を送るだけのオブジェクトです。UGUIのようにコール時に引数を与えられない場合に使用します。

任意のGameObjectに追加してください。

インタラクトやUGUIから`ChangeCh()`をコールしてください。

## VideoChController.cs

複数設置したUSharpVideoのチャンネル切り替えを行います。

実際は映像を表示するメッシュとオーディオソースグループを切り替えているだけです。

各配列フィールドのインデックスがチャンネル番号となります。

USharpVideoツリーの中にある下記オブジェクトを、本オブジェクトのフィールドに設定してください。

- 映像表示メッシュ（`MeshRenderer`要素）
- オーディオソースグループ（`GameObject`要素）
- USharpVideo制御オブジェクト（`VideoControlHandler`要素）

オーディオソースは一つのUSharpVideoオブジェクトに対して複数存在するため、オーディオソースの親オブジェクトを指定してください。コード内で子オブジェクトであるオーディオソース全てに対して操作を行っています。

また、映像以外にも任意のGameObjectを表示/非表示出来ます。`SwitchedOtherObjects`にチャンネルに対応するオブジェクトを指定してください。

任意のGameObjectに追加してください。

チャンネル切り替え時は`ChangeSynced(int index)`をコールするか、`VideoChIndexSender`を使用してください。

TopazchatのようにローカルResyncを使用したい場合は`Resync()`をコールしてください。対象であるUSharpVideoのリロードボタンを押す事と同じ操作となります。

## VideoChControllerWithVisibleSwitch.cs

継承のサンプルです。`VideoChController`に映像表示メッシュを全て非表示する機能を追加した物です。

表示/非表示はUGUIのToggleオブジェクトを使用しているため、汎用性はありません。

任意のGameObjectに追加してください。

Toggleの"On Value Changed (Boolean)"に"SendCustomEvent (string)"を設定し、`ChangeVisibleSynced()`をコールしてください。

また、チェック表示のグローバル同期を行うために、本オブジェクトのフィールドにToggleオブジェクトを指定してください。
