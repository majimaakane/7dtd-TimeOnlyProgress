using System;
using System.IO;
using System.Xml;

/// <summary>
/// 7 Days to Die サーバーモッド: 時間経過のみ v1.0.1
///
/// プレイヤーが誰もログインしていない間もゲーム内時間を進め続ける軽量版。
/// タイルエンティティ（作業台・炉等）のシミュレーションは行わない。
///
/// 設定: Mods/TimeOnlyProgress/config.xml
/// </summary>
public class TimeOnlyProgressMod : IModApi
{
    private const string LOG_TAG = "[TimeOnlyProgress]";

    private float speedMultiplier = 1.0f;
    private float updateIntervalSeconds = 1.0f;

    private int playerCount = 0;
    private bool isInitialized = false;
    private float timeSinceLastUpdate = 0f;

    public void InitMod(Mod _modInstance)
    {
        LoadConfig(_modInstance.Path);

        ModEvents.GameUpdate.RegisterHandler(OnGameUpdate);
        ModEvents.PlayerLogin.RegisterHandler(OnPlayerLogin);
        ModEvents.PlayerDisconnected.RegisterHandler(OnPlayerDisconnected);
        ModEvents.GameStartDone.RegisterHandler(OnGameStartDone);

        Log.Out($"{LOG_TAG} 初期化完了 (速度倍率: {speedMultiplier}x, 更新間隔: {updateIntervalSeconds}秒)");
    }

    private void LoadConfig(string modPath)
    {
        string configPath = Path.Combine(modPath, "config.xml");
        if (!File.Exists(configPath)) return;

        try
        {
            var doc = new XmlDocument();
            doc.Load(configPath);
            var root = doc.DocumentElement;

            var node = root?.SelectSingleNode("TimeAdvancement");
            if (node != null)
            {
                if (node.Attributes["speedMultiplier"] != null)
                    float.TryParse(node.Attributes["speedMultiplier"].Value,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out speedMultiplier);

                if (node.Attributes["updateIntervalSeconds"] != null)
                    float.TryParse(node.Attributes["updateIntervalSeconds"].Value,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out updateIntervalSeconds);
            }

            if (speedMultiplier < 0.01f) speedMultiplier = 0.01f;
            if (updateIntervalSeconds < 0.1f) updateIntervalSeconds = 0.1f;

            Log.Out($"{LOG_TAG} config.xml 読み込み完了");
        }
        catch (Exception ex)
        {
            Log.Error($"{LOG_TAG} config.xml 読み込みエラー: {ex.Message}");
        }
    }

    private void OnGameStartDone(ref ModEvents.SGameStartDoneData data)
    {
        isInitialized = true;
        playerCount = GameManager.Instance?.World?.Players?.dict?.Count ?? 0;
        Log.Out($"{LOG_TAG} ゲーム開始完了。現在のプレイヤー数: {playerCount}");
    }

    private ModEvents.EModEventResult OnPlayerLogin(ref ModEvents.SPlayerLoginData data)
    {
        playerCount++;
        Log.Out($"{LOG_TAG} プレイヤーログイン: {data.ClientInfo?.playerName ?? "unknown"} (プレイヤー数: {playerCount})");
        return ModEvents.EModEventResult.Continue;
    }

    private void OnPlayerDisconnected(ref ModEvents.SPlayerDisconnectedData data)
    {
        playerCount = Math.Max(0, playerCount - 1);
        Log.Out($"{LOG_TAG} プレイヤー切断: {data.ClientInfo?.playerName ?? "unknown"} (残り: {playerCount})");
    }

    private void OnGameUpdate(ref ModEvents.SGameUpdateData data)
    {
        if (!isInitialized || playerCount > 0) return;

        timeSinceLastUpdate += UnityEngine.Time.deltaTime;
        if (timeSinceLastUpdate < updateIntervalSeconds) return;

        float elapsed = timeSinceLastUpdate;
        timeSinceLastUpdate = 0f;

        World world = GameManager.Instance?.World;
        if (world == null) return;

        int dayNightLength = GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
        if (dayNightLength <= 0) dayNightLength = 60;

        float ticksPerSecond = 24000f / (dayNightLength * 60f);
        ulong ticksToAdvance = (ulong)(elapsed * ticksPerSecond * speedMultiplier);

        if (ticksToAdvance > 0)
        {
            world.SetTime(world.GetWorldTime() + ticksToAdvance);
        }
    }
}
