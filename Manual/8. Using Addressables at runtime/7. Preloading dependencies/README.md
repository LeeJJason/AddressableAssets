# Preloading dependencies

当您远程分发内容时，有时可以通过在应用程序需要它们之前下载依赖项来提高感知性能。例如，您可以在玩家第一次启动您的游戏时在启动时下载基本内容，以确保他们不必在游戏过程中等待内容。

## Downloading dependencies

使用[Addressables.DownloadDependenciesAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync.html)方法确保加载可寻址键所需的所有依赖项在随应用程序安装的本地内容或下载缓存中可用。

**TIP**

如果您有一组要预下载的资产，您可以为资产分配相同的标签，例如“preload”，并在调用[Addressables.DownloadDependenciesAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync.html)时使用该标签作为键 。Addressables 下载包含带有该标签的资产的所有 AssetBundles（如果尚不可用）（以及包含资产依赖项的任何包）。

## Progress

一个[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)实例提供了两种方式来获得进展：

- [AsyncOperationHandle.PercentComplete](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.PercentComplete.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_PercentComplete)：报告已完成的子操作的百分比。例如，如果一个操作使用六个子操作来执行其任务，则`PercentComplete`当其中三个操作完成时，表示整个操作已完成 50%（每个操作加载多少数据无关紧要）。
- [AsyncOperationHandle.GetDownloadStatus](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.GetDownloadStatus.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_GetDownloadStatus)：返回一个[DownloadStatus](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.DownloadStatus.html)结构，报告总下载大小的百分比。例如，如果一个操作有六个子操作，但第一个操作占总下载大小的 50%，则`GetDownloadStatus`在第一个操作完成时表示该操作已完成 50%。

以下示例说明了如何在下载过程中使用[GetDownloadStatus](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.GetDownloadStatus.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_GetDownloadStatus)检查状态和调度进度事件：

要了解加载一项或多项资产需要下载多少数据，您可以调用[Addressables.GetDownloadSizeAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.GetDownloadSizeAsync.html)：

完成操作的结果是必须下载的字节数。如果 Addressables 已经缓存了所有必需的 AssetBundles，则 Result 为零。

读取 Result 对象后，请务必释放下载操作句柄。如果不需要访问下载操作的结果，可以通过将`autoReleaseHandle`参数设置为true来自动释放句柄，如下例所示：

### Clearing the dependency cache

如果要清除 Addressables 缓存的任何 AssetBundles，请调用[Addressables.ClearDependencyCacheAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.ClearDependencyCacheAsync.html)。此函数清除缓存的 AssetBundles，其中包含由键标识的资产以及包含这些资产依赖项的任何包。

请注意， ClearDependencyCacheAsync 仅清除与指定键相关的资产包。如果您更新了内容目录，使得该键不再存在或不再依赖于相同的 AssetBundles，那么这些不再被引用的包将保留在缓存中，直到它们过期（基于[cache settings](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Cache.html)）。

要清除所有 AssetBundles，您可以使用[UnityEngine.Caching](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Caching.html)类中的函数。

