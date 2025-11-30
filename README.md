# Status Bar Clock for Visual Studio

Visual Studio のステータスバーに現在時刻を表示する VSIX 拡張機能です。

![Status Bar Clock](https://via.placeholder.com/800x100/1e1e1e/ffffff?text=Status+Bar+Clock+Screenshot)

## 機能

- **カスタマイズ可能な日時フォーマット**: .NET の標準的な DateTime フォーマット文字列をサポート
- **調整可能な更新間隔**: 10ms ～ 60000ms の範囲で更新頻度を設定可能
- **ミリ秒表示**: オプションでミリ秒単位の表示に対応
- **プレフィックステキスト**: 時刻の前に任意のテキストを追加可能
- **有効/無効の切り替え**: 時計表示のオン/オフを簡単に切り替え
- **リアルタイム設定反映**: 設定変更が即座に反映され、Visual Studio の再起動不要
- **Visual Studio 2025 対応**: 新しい統合設定 UI に対応

## システム要件

- Visual Studio 2025 以降
- .NET Framework 4.7.2 以降

## インストール方法

### 方法 1: VSIX ファイルからインストール

1. [リリースページ](../../releases)から最新の`StatusBarClock.vsix`をダウンロード
2. ダウンロードした`.vsix`ファイルをダブルクリック
3. VSIX Installer の指示に従ってインストール
4. Visual Studio を再起動

### 方法 2: ソースからビルド

```powershell
# リポジトリをクローン
git clone https://github.com/yourusername/VS_StatusBarClock.git
cd VS_StatusBarClock

# MSBuildでビルド
& "C:\Program Files\Microsoft Visual Studio\2025\Community\MSBuild\Current\Bin\MSBuild.exe" /t:Build /p:Configuration=Release

# 生成されたVSIXファイルをインストール
# bin\Release\StatusBarClock.vsix をダブルクリック
```

## 使用方法

### 基本的な使い方

拡張機能をインストールすると、Visual Studio のステータスバー(画面下部)に自動的に現在時刻が表示されます。

### 設定のカスタマイズ

1. Visual Studio のメニューから**ツール** → **オプション**を選択
2. オプションツリーから**Status Bar Clock** → **General**を選択
3. 右側のプロパティグリッドで設定を変更

#### 設定項目

| 設定項目                 | 説明                       | デフォルト値                 | 有効範囲                               |
| ------------------------ | -------------------------- | ---------------------------- | -------------------------------------- |
| **Date/Time Format**     | 日時の表示フォーマット     | `yyyy-MM-dd (dddd) HH:mm:ss` | 有効な.NET DateTime フォーマット文字列 |
| **Update Interval (ms)** | 更新間隔(ミリ秒)           | `1000`                       | 10 ～ 60000                            |
| **Show Milliseconds**    | ミリ秒を表示               | `false`                      | true/false                             |
| **Prefix Text**          | 時刻の前に表示するテキスト | `""` (空)                    | 任意の文字列                           |
| **Enabled**              | 時計表示の有効/無効        | `true`                       | true/false                             |

### フォーマット文字列の例

以下は、よく使用される日時フォーマットの例です:

| フォーマット文字列            | 表示例                                |
| ----------------------------- | ------------------------------------- |
| `yyyy-MM-dd HH:mm:ss`         | 2025-11-30 14:30:45                   |
| `yyyy-MM-dd (dddd) HH:mm:ss`  | 2025-11-30 (Saturday) 14:30:45        |
| `MM/dd/yyyy hh:mm:ss tt`      | 11/30/2025 02:30:45 PM                |
| `HH:mm:ss`                    | 14:30:45                              |
| `yyyy年MM月dd日 HH時mm分ss秒` | 2025 年 11 月 30 日 14 時 30 分 45 秒 |
| `HH:mm`                       | 14:30                                 |

詳細なフォーマット指定子については、[Microsoft のドキュメント](https://learn.microsoft.com/ja-jp/dotnet/standard/base-types/custom-date-and-time-format-strings)を参照してください。

### ミリ秒表示の設定

滑らかなミリ秒表示を実現するには:

1. **Show Milliseconds**を`true`に設定
2. **Update Interval (ms)**を`100`に設定(推奨)

> **注意**: Update Interval が 1000ms の場合、ミリ秒は常に`.000`付近の値になります。

## トラブルシューティング

### 時計が表示されない

1. **ツール** → **オプション** → **Status Bar Clock** → **General**を開く
2. **Enabled**が`true`に設定されているか確認
3. Visual Studio を再起動してみる

### 設定が保存されない

1. Visual Studio を管理者として実行してみる
2. `%LOCALAPPDATA%\Microsoft\VisualStudio\`フォルダのアクセス権を確認

### 無効なフォーマット文字列エラー

設定の適用時にエラーが表示される場合:

1. **Date/Time Format**に有効な.NET DateTime フォーマット文字列を入力
2. [フォーマット文字列の例](#フォーマット文字列の例)を参考にする
3. フォーマット文字列をリセットしたい場合は、設定画面で**既定値にリセット**ボタンをクリック

## 開発情報

### アーキテクチャ

このプロジェクトは以下の主要コンポーネントで構成されています:

#### StatusBarClockPackage.cs

- Visual Studio パッケージのメインクラス
- `AsyncPackage`を継承
- バックグラウンド読み込みをサポート
- オプションページとプロファイル同期を登録

#### ClockOptions.cs

- `DialogPage`を継承した設定管理クラス
- プロパティグリッドによる自動 UI 生成
- Visual Studio 2025 の統合設定 UI に対応
- 設定の検証とイベント通知

#### ClockStatusBarControl.cs

- ステータスバーへの時刻表示を制御
- `DispatcherTimer`による定期更新
- `IVsStatusbar`サービスを使用した表示
- リアルタイムでの設定反映

### 開発環境のセットアップ

#### 必要なツール

- Visual Studio 2025 (Community 以上)
- Visual Studio Extension Development ワークロード
- .NET Framework 4.7.2 SDK

#### ビルド手順

```powershell
# デバッグビルド
& "C:\Program Files\Microsoft Visual Studio\2025\Community\MSBuild\Current\Bin\MSBuild.exe" /t:Build /p:Configuration=Debug

# リリースビルド
& "C:\Program Files\Microsoft Visual Studio\2025\Community\MSBuild\Current\Bin\MSBuild.exe" /t:Build /p:Configuration=Release

# クリーンビルド
& "C:\Program Files\Microsoft Visual Studio\2025\Community\MSBuild\Current\Bin\MSBuild.exe" /t:Clean,Build /p:Configuration=Debug
```

#### デバッグ実行

1. Visual Studio でソリューションを開く
2. F5 キーを押すか、**デバッグ** → **デバッグの開始**を選択
3. 実験インスタンスが起動し、拡張機能が自動的に読み込まれる
4. デバッグ出力ウィンドウで詳細なログを確認可能

#### 実験インスタンスへのデプロイ

```powershell
# ビルドと実験インスタンスへのデプロイを同時実行
& "C:\Program Files\Microsoft Visual Studio\2025\Community\MSBuild\Current\Bin\MSBuild.exe" /t:Build /p:Configuration=Debug /p:DeployExtension=true
```

### プロジェクト構造

```
VS_StatusBarClock/
├── StatusBarClockPackage.cs      # メインパッケージクラス
├── ClockOptions.cs                # 設定管理クラス
├── ClockStatusBarControl.cs       # ステータスバーコントロール
├── StatusBarClock.csproj          # プロジェクトファイル
├── source.extension.vsixmanifest  # VSIXマニフェスト
├── Properties/
│   └── AssemblyInfo.cs            # アセンブリ情報
└── bin/
    └── Debug/Release/
        └── StatusBarClock.vsix    # ビルド成果物
```

### コーディング規約

- すべての public メンバーには日本語の XML ドキュメントコメントを記述
- UI スレッドを必要とするメソッドには`ThreadHelper.ThrowIfNotOnUIThread()`を追加
- 非同期メソッドには適切な`async/await`パターンを使用
- IDisposable を実装する場合は適切なクリーンアップを保証

## 技術的な詳細

### Visual Studio 2025 対応

この拡張機能は Visual Studio 2025 の新しい統合設定 UI に対応しています。

- `ProvideOptionPage`の 6 番目のパラメータを`false`に設定することで、自動プロパティグリッドを使用
- `ProvideProfile`属性により、設定の同期とインポート/エクスポートをサポート
- 設定はインラインで表示され、従来のポップアップダイアログは表示されません

### パフォーマンス

- `DispatcherTimer`により、UI スレッドで安全に更新
- バックグラウンド読み込み(`AllowsBackgroundLoading=true`)により、起動パフォーマンスへの影響を最小化
- 無効時はタイマーを完全に停止し、リソースを消費しない

### スレッドセーフティ

- すべての UI スレッド操作は`ThreadHelper`で保護
- `AsyncPackage`と`JoinableTaskFactory`による適切な非同期初期化

## ライセンス

このプロジェクトのライセンスについては、リポジトリの LICENSE ファイルを参照してください。

## 貢献

バグレポート、機能リクエスト、プルリクエストを歓迎します！

1. このリポジトリをフォーク
2. 機能ブランチを作成 (`git checkout -b feature/AmazingFeature`)
3. 変更をコミット (`git commit -m 'Add some AmazingFeature'`)
4. ブランチにプッシュ (`git push origin feature/AmazingFeature`)
5. プルリクエストを作成

## 変更履歴

### Version 1.0.0 (2025-11-30)

- 初回リリース
- Visual Studio 2025 対応
- カスタマイズ可能な日時フォーマット
- 調整可能な更新間隔
- ミリ秒表示サポート
- プレフィックステキスト機能
- 統合設定 UI への対応

## サポート

問題が発生した場合や質問がある場合は、[GitHub の Issues](../../issues)で報告してください。

## 謝辞

このプロジェクトは Visual Studio SDK と Managed Package Framework を使用しています。

---

**Status Bar Clock** - Visual Studio での開発をより快適に
