# TimeOnlyProgress - 7DTD 時間経過のみモッド

プレイヤーが誰もログインしていない間も、ゲーム内時間だけを進行させる軽量サーバーサイドモッドです。  
作業台・炉などのタイルエンティティのシミュレーションは行わないため、サーバー負荷を最小限に抑えます。

## 機能

- プレイヤーが0人になると自動的に時間経過モードに移行
- `serverconfig.xml` の `DayNightLength` 設定に合わせた速度で時間を進行
- プレイヤーが再接続すると通常動作に戻る
- 時間経過の速度倍率を `config.xml` で設定可能
- サーバーコンソールにログを出力

## 前提条件

- .NET SDK（.NET Framework 4.8 対応）
- 7 Days to Die Dedicated Server
- **EAC（Easy Anti-Cheat）を無効にする必要があります**

## ビルド方法

### 1. サーバーパスの設定

`src\TimeOnlyProgress.csproj` の `<GameDir>` を、お使いの7DTDサーバーのパスに変更してください：

```xml
<GameDir>C:\Program Files (x86)\Steam\steamapps\common\7 Days to Die Dedicated Server</GameDir>
```

### 2. ビルド

`build.bat` を実行するか、以下のコマンドを実行してください：

```
dotnet build src\TimeOnlyProgress.csproj -c Release
```

### 3. インストール

`TimeOnlyProgress` フォルダ（`ModInfo.xml`・`TimeOnlyProgress.dll`・`config.xml` 含む）を  
サーバーの `Mods` フォルダにコピーしてください。

```
7 Days to Die Dedicated Server/
  └── Mods/
      └── TimeOnlyProgress/
          ├── ModInfo.xml
          ├── TimeOnlyProgress.dll
          └── config.xml
```

## 設定

`TimeOnlyProgress\config.xml` で動作を調整できます：

```xml
<TimeOnlyProgress>
  <TimeAdvancement speedMultiplier="1.0" updateIntervalSeconds="1.0" />
</TimeOnlyProgress>
```

| 属性 | デフォルト値 | 説明 |
|------|-------------|------|
| `speedMultiplier` | `1.0` | 時間経過の倍率（2.0で2倍速） |
| `updateIntervalSeconds` | `1.0` | 更新間隔（秒） |

## ログ確認

サーバーコンソールで `[TimeOnlyProgress]` タグ付きのログが出力されます：

```
[TimeOnlyProgress] 初期化完了 (速度倍率: 1x, 更新間隔: 1秒)
[TimeOnlyProgress] ゲーム開始完了。現在のプレイヤー数: 0
[TimeOnlyProgress] プレイヤーログイン: PlayerName (プレイヤー数: 1)
[TimeOnlyProgress] プレイヤー切断: PlayerName (残り: 0)
```

## ForceTimeProgress との違い

| 項目 | TimeOnlyProgress | ForceTimeProgress |
|------|-----------------|-------------------|
| 時間の進行 | ✅ | ✅ |
| タイルエンティティ処理 | ❌（軽量） | ✅ |
| サーバー負荷 | 低 | やや高 |
| 用途 | 夜を飛ばしたいだけ | 完全なオフライン進行 |
