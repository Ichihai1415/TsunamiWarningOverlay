# TsunamiWarningOverlay (TWO)

OBS等向けに簡易津波地図を描画します。

![動作例](https://github.com/Ichihai1415/TsunamiWarningOverlay/blob/main/TsunamiWarningOverlay/sample/TWO.gif?raw=true)

## 使い方

[.NET デスクトップ ランタイム 9.0.x](https://dotnet.microsoft.com/ja-jp/download/dotnet/9.0) が必要です。

TsunamiWarningOverlay.exeを起動するだけで良いです。**情報が発表されていない時は何も表示されません。** OBS等で黒(RGB:`(0, 0, 0)`)をクロマキーに設定してください。

## 仕様

地図データ: 気象庁(加工して内部リソースに格納)

[P2P地震情報 JSON API v2](https://www.p2pquake.net/develop/json_api_v2/)の/history(codes=552&limit=1,offset=1)を1分毎に取得します。津波注意報以上のみです。

発表・受信時刻表示は`発表時刻 内部処理時刻`で`yyMMddHHmm`形式です。

## 設定

設定は初回起動時に生成され、ソフト起動時に読み込まれます。反映させたい場合再起動してください。

- `Enable_AntiAlias`: アンチエイリアスを有効にするか
- `Enable_DisplayTime`: 発表・受信時刻表示を有効にするか
- `Enable_ViewChange`: 表示点滅を有効にするか
- `ViewChangeSpan`: 表示点滅の間隔

# 更新履歴

## v1.0.0
2025/03/13

- P2P地震情報 JSON API v2からの取得を追加
- 表示点滅機能追加
	

