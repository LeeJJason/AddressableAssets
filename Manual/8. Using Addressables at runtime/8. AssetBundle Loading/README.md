# AssetBundle Loading

Addressables 系统将您的资产打包在 AssetBundles 中，并在您加载单个资产时在“幕后”加载这些包。您可以控制 AssetBundles 如何加载在`BundledAssetGroupSchema`类上公开的内容。您可以通过脚本 API 或在检查器的`AddressablesAssetGroup`检查器中的高级选项下设置这些选项。

## UnityWebRequestForLocalBundles

Addressables 可以通过两个引擎 API 加载 AssetBundles：`UnityWebRequest.GetAssetBundle`, 和`AssetBundle.LoadFromFileAsync`. 默认行为是`AssetBundle.LoadFromFileAsync`当 AssetBundle 在本地存储时使用，当 AssetBundle 路径是 URL 时使用`UnityWebRequest`。

您可以通过设置`BundledAssetGroupSchema.UseUnityWebRequestForLocalBundles`为 true来覆盖此行为以`UnityWebRequest`用于本地资产包。也可以通过 BundledAssetGroupSchema GUI 进行设置。

其中一些情况包括：

1. 您正在运送使用 LZMA 压缩的本地 AssetBundle，因为您希望运送的游戏包尽可能小。在这种情况下，您可能希望使用 UnityWebRequest 将这些 AssetBundles LZ4 重新压缩到本地磁盘缓存中。
2. 您正在发布一款 Android 游戏，并且您的 APK 包含使用默认 APK 压缩方式压缩的 AssetBundle。
3. 您希望将整个本地 AssetBundle 加载到内存中以避免磁盘搜索。如果您使用`UnityWebRequest`并禁用了缓存，则整个 AssetBundle 文件将被加载到内存缓存中。这会增加您的运行时内存使用量，但可能会提高加载性能，因为它消除了初始 AssetBundle 加载后的磁盘搜索。上面的情况 1 和 2 都会导致 AssetBundle 在播放器设备上存在两次（原始和缓存表示）。这意味着初始加载（解压并复制到缓存）比后续加载（从缓存加载）慢