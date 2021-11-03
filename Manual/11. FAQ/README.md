# Addressables FAQ

### Is it better to have many small bundles or a few bigger ones?

有几个关键因素可以决定生成多少包。首先，请务必注意，您可以通过组的大小和组的构建设置来控制您拥有的捆绑包数量。例如，“Pack Together”会为每组创建一个捆绑包，而“Pack Separately”会创建许多捆绑包。有关详细信息，请参阅 [schema build settings for more information](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.BundleMode.html#UnityEditor_AddressableAssets_Settings_GroupSchemas_BundledAssetGroupSchema_BundleMode)。

一旦您知道如何控制捆绑布局，如何设置这些布局的决定将取决于游戏。以下是有助于做出该决定的关键数据：

捆绑过多的危险：

- 每个包都有内存开销。详细信息 [on the memory management page](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#assetbundle-memory-overhead)。这与该页面上概述的许多因素有关，但简而言之，这种开销可能很大。如果您预计一次将 100 个甚至 1000 个包加载到内存中，这可能意味着大量内存被消耗掉。
- 下载包有并发限制。如果您同时需要 1000 个捆绑包，则不可能同时下载所有捆绑包。将下载一些数量，当它们完成时，再触发更多。在实践中，这是一个相当小的问题，如此小以至于您通常会受到下载的总大小的限制，而不是它被分成多少个包。
- 捆绑信息会使目录膨胀。为了能够下载或加载目录，我们会存储有关您的捆绑包的基于字符串的信息。数以千计的数据包可以大大增加目录的大小。
- 重复资产的可能性更大。假设两个材料被标记为可寻址，并且每个都依赖于相同的纹理。如果它们在同一个包中，则纹理被拉入一次，并被两者引用。如果它们在单独的包中，并且纹理本身不是可寻址的，那么它将被复制。然后，您需要将纹理标记为可寻址，接受重复，或将材料放在同一个包中。

捆绑太少的危险：

- UnityWebRequest（我们用来下载）不会恢复失败的下载。因此，如果正在下载大包并且您的用户失去连接，则一旦他们重新获得连接，下载就会重新开始。
- 物品可以从捆绑包中单独加载，但不能单独卸载。例如，如果你在一个包中有 10 个材料，加载所有 10 个，然后告诉 Addressables 释放其中的 9 个，所有 10 个都可能在内存中。这也包含 [on the memory management page](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#when-is-memory-cleared)。

### What compression settings are best?

Addressables 提供三种不同的包压缩选项：未压缩、LZ4 和 LZMA。一般来说，LZ4 应该用于本地内容，而 LZMA 应该用于远程内容，但下面概述了更多详细信息，因为可能有例外。
您可以使用每个组的高级设置来设置压缩选项。压缩不会影响加载内容的内存大小。

- 未压缩 - 此选项在磁盘上最大，通常加载速度很快。如果您的游戏恰好有空闲空间，则至少应考虑本地内容使用此选项。未压缩包的一个关键优势是它们如何处理补丁。如果您正在为平台本身提供补丁（例如 Steam 或 Switch）的平台进行开发，则未压缩的捆绑包可提供最准确（最小）的补丁。其他压缩选项中的任何一个都会导致至少一些补丁膨胀。
- LZ4 - 如果 Uncompressed 不是一个可行的选项，那么 LZ4 应该用于所有其他本地内容。这是一种基于块的压缩，它提供了加载文件的一部分而无需加载整个文件的能力。
- LZMA - LZMA 应用于所有远程内容，但不适用于任何本地内容。它提供了最小的包大小，但加载速度很慢。如果您要在 LZMA 中存储本地包，您可以创建一个更小的播放器，但加载时间会比未压缩或 LZ4 明显更差。对于下载的包，我们通过在将其存储在 AssetBundle 缓存中时重新压缩下载的包来避免缓慢的加载时间。默认情况下，bundle 将存储在未压缩的缓存中。如果你想用 LZ4 压缩缓存，你可以通过创建一个[`CacheInitializationSettings`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.CacheInitializationSettings.html). 有关设置的更多信息，请参阅 [Initialization Objects](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/InitializeAsync.html#initialization-objects)。

**NOTE**

*LZMA AssetBundle 压缩不适用于 WebGL 上的 AssetBundle。可以使用 LZ4 压缩代替。有关更多 WebGL AssetBundle 信息，请参阅 [Building and running a WebGL project](https://docs.unity3d.com/2019.4/Documentation/Manual/webgl-building.html#AssetBundles)。*

请注意，平台的硬件特性可能意味着未压缩的包并不总是加载最快的。加载未压缩包的最大速度由 IO 速度控制，而加载 LZ4 压缩包的速度可以由 IO 速度或 CPU 控制，具体取决于硬件。在大多数平台上，加载 LZ4 压缩包受 CPU 限制，加载未压缩包会更快。在低 IO 速度和高 CPU 速度的平台上，LZ4 加载可以更快。运行性能分析以验证您的游戏是否符合常见模式或需要一些独特的调整始终是一个好习惯。

有关 Unity 压缩选择的更多信息，请参阅 [Asset Bundle documentation](https://docs.unity3d.com/Manual/AssetBundles-Cache.html)。

### Are there ways to miminize the catalog size?

目前有两种优化可用。

1. 压缩本地目录。如果您主要关心的是构建中的目录有多大，则检查器中有一个选项可用于**Compress Local Catalog**的顶级设置。此选项将您的游戏附带的目录构建到 AssetBundle 中。压缩目录会使文件本身变小，但请注意，这会增加目录加载时间。
2. 禁用内置场景和资源。Addressables 提供从资源和内置场景列表加载内容的能力。默认情况下，此功能处于开启状态，如果您不需要此功能，这会使目录膨胀。要禁用它，请在 Groups 窗口中选择“Built In Data”组（**Window** > **Asset Management** > **Addressables** > **Groups**）。从该组的设置中，您可以取消选中“Include Resources Folders”和“Include Build Settings Scenes”。取消选中这些选项只会从 Addressables 目录中删除对这些资产类型的引用。内容本身仍然内置在您创建的播放器中，您仍然可以通过旧 API 加载它。

### What is addressables_content_state?

在每个新的可寻址内容构建之后，我们会生成一个 addressables_content_state.bin 文件，该文件保存在可寻址资产设置值“内容状态构建路径”中定义的文件夹路径中，并附加 /. 此处的新内容构建定义为不属于[content update workflow](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ContentUpdateWorkflow.html)的内容构建。如果此值为空，则默认位置将是`Assets/AddressableAssetsData/<Platform>/`您的 Unity 项目的文件夹。此文件对我们的[content update workflow](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ContentUpdateWorkflow.html)至关重要。如果您没有进行任何内容更新，则可以完全忽略此文件。如果您计划进行内容更新，您将需要为先前版本生成的此文件的版本。我们建议在每次发布播放器构建时将其签入版本控制并创建一个分支。我们的 [content update workflow page](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ContentUpdateWorkflow.html) 提供了更多信息。

### What are possible scale implications?

随着您的项目变得越来越大，请注意资产和捆绑包的以下方面：

- 总包大小 - 过去 Unity 不支持大于 4GB 的文件。这已在最近的一些编辑器版本中得到修复，但仍然存在问题。建议将给定包的内容保持在此限制之下，以便在所有平台上实现最佳兼容性。
- 影响 UI 性能的子资产 - 这里没有硬性限制，但是如果您有很多资产，并且这些资产有很多子资产，最好关闭子资产显示。此选项仅影响数据在“组”窗口中的显示方式，不影响运行时可以加载和不可以加载的内容。该选项在**Tools** > **Groups View** > **Show Sprite and Subobject Addresses**下的组窗口中可用。禁用此功能将使 UI 更具响应性。
- 组层次结构显示 - 另一个有助于缩放的仅限 UI 的选项是**Group Hierarchy with Dashes**。该选项在**Tools** > **Groups View** > **Group Hierarchy with Dashes**下的组窗口中可用。启用此功能后，名称中包含破折号“-”的组将显示为好像破折号代表文件夹层次结构。这不会影响实际的组名，也不会影响事物的构建方式。例如，两个名为“x-y-z”和“x-y-w”的组将显示为好像在名为“x”的文件夹中，有一个名为“y”的文件夹。该文件夹内有两个组，分别称为“x-y-z”和“x-y-w”。这不会真正影响 UI 响应能力，
- 大规模捆绑布局 - 有关如何最好地设置布局的更多信息，请参阅前面的问题： [*Is it better to have many small bundles or a few bigger ones*](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressablesFAQ.html#faq-bundle-size)

### What Asset Load Mode to use?

对于大多数平台和内容集合，建议使用`Requested Asset and Dependencies`. 此模式将仅加载使用`LoadAssetAsync`或请求的资产所需的内容`LoadAssetsAsync`。这可以防止将资产加载到未使用的内存中的情况。

在加载打包在一起的所有资产的情况下的性能，例如加载屏幕。大多数类型的内容在使用`Requested Asset and Dependencies`模式单独加载时将具有相似或改进的性能。加载性能可能因内容类型而异。例如，使用`All Packed Assets and Dependencies`. 对于一些其他资产，如纹理，单独加载每个资产通常会更高效。如果使用[Synchronous Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/SynchronousAddressables.html)，则资产加载模式之间的性能很小。由于更大的灵活性，建议`Requested Asset and Dependencies`在您知道内容将同步加载的地方使用。

**注意**：以上示例适用于桌面和移动设备。性能可能因平台而异。`All Packed Assets and Dependencies`模式通常比在 Nintendo Switch 上单独加载资产表现更好。建议为您的特定内容和平台分析加载性能，以查看哪些适用于您的应用程序。

加载第一个资产时`All Packed Assets and Dependencies`，所有资产都加载到内存中。稍后对该包中的资源的 LoadAssetAsync 调用将返回预加载的资源，而无需加载它。即使在使用 All Packed Assets and Dependencies 选项时将组中的所有资产和任何依赖项加载到内存中，单个资产的引用计数也不会增加，除非您明确加载它（或者它是资产的依赖项）您显式加载）。如果您稍后调用[`Resources.UnloadUnusedAssets`](https://docs.unity3d.com/ScriptReference/Resources.UnloadUnusedAssets.html)，或者使用 加载新场景[`LoadSceneMode.Single`](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.html)，则将卸载任何未使用的资产（引用计数为零的资产）。

### Is it safe to edit loaded Assets?

编辑从捆绑包加载的资产时，在播放器中或使用 “Use Existing Build (requires built groups)”播放模式设置时。资产从捆绑包中加载，仅存在于内存中。更改不能写回到磁盘上的 Bundle，并且对内存中对象的任何修改都不会在会话之间持续存在。

当使用“Use Asset Database (fastest)”或“Simulate Groups (advanced)”播放模式设置时，这是不同的，在这些模式下，资产是从项目文件加载的。对加载的资产所做的任何修改都会修改项目资产，并保存到文件中。

为了防止这种情况发生，在进行运行时更改时，创建一个新的 Object 实例进行修改，并作为 Object 使用 Instantiate 方法创建一个实例。如下面的示例代码所示。

```csharp
var op = Addressables.LoadAssetAsync<GameObject>("myKey");
yield return op;
if (op.Result != null)
{
    GameObject inst = UnityEngine.Object.Instantiate(op.Result);
    // can now use and safely make edits to inst, without the source Project Asset being changed.
}
```

**请注意**，实例化对象时：

- 释放资产时必须使用 AsyncOperationHandle 或原始资产，而不是实例。
- 实例化具有对其他资产的引用的资产不会创建其他引用资产的新实例。引用仍然针对项目资产。
- 在新实例上调用 Unity 方法，例如 Start、OnEnable 和 OnDisable。

### Is it possible to retrieve the address of an asset or reference at runtime?

在最一般的情况下，加载的资产不再关联其地址或`IResourceLocation`. 但是，有一些方法可以正确关联`IResourceLocation`并使用它来读取字段 PrimaryKey。PrimaryKey 字段将设置为资产的地址，除非为此对象来自的组禁用“Include Address In Catalog”。在这种情况下，PrimaryKey 将是键列表中的下一项（可能是 GUID，但也可能是标签或空字符串）。

#### Examples

检索 AssetReference 的地址。这可以通过查找与该引用关联的 Location 并获取 PrimaryKey 来完成：

```
var op = Addressables.LoadResourceLocationsAsync(MyRef1);
yield return op;
if (op.Status == AsyncOperationStatus.Succeeded &&
    op.Result != null &&
    op.Result.Count > 0)
{
    Debug.Log("address is: " + op.Result[0].PrimaryKey);
}
```

按标签加载多个资产，但将每个资产与其地址相关联。这里，再次需要 LoadResourceLocationsAsync：

```
Dictionary<string, GameObject> _preloadedObjects = new Dictionary<string, GameObject>();
private IEnumerator PreloadHazards()
{
    //find all the locations with label "SpaceHazards"
    var loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync("SpaceHazards", typeof(GameObject));
    if( !loadResourceLocationsHandle.IsDone )
        yield return loadResourceLocationsHandle;

    //start each location loading
    List<AsyncOperationHandle> opList = new List<AsyncOperationHandle>();
    foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
    {
        AsyncOperationHandle<GameObject> loadAssetHandle = Addressables.LoadAssetAsync<GameObject>(location);
        loadAssetHandle.Completed += obj => { _preloadedObjects.Add(location.PrimaryKey, obj.Result); };
        opList.Add(loadAssetHandle);
    }

    //create a GroupOperation to wait on all the above loads at once. 
    var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opList);
    if( !groupOp.IsDone )
        yield return groupOp;

    Addressables.Release(loadResourceLocationsHandle);

    //take a gander at our results.
    foreach (var item in _preloadedObjects)
    {
        Debug.Log(item.Key + " - " + item.Value.name);
    }
}
```

### Can I build Addressables when recompiling scripts?

如果您有一个触发域重新加载的预构建步骤，那么您必须特别注意，在域重新加载完成之前，Addressables 构建本身不会启动。

使用设置脚本定义符号 ( [PlayerSettings.SetScriptingDefineSymbolsForGroup](https://docs.unity3d.com/ScriptReference/PlayerSettings.SetScriptingDefineSymbolsForGroup.html) ) 或切换活动构建目标 ( [EditorUserBuildSettings.SwitchActiveBuildTarget](https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTarget.html) ) 等方法，触发脚本重新编译和重新加载。编辑器代码的执行将继续使用当前加载的域，直到域重新加载并停止执行。在域重新加载之前，不会设置任何 [platform dependant compilation](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html)或自定义定义。这可能会导致代码依赖这些定义来正确构建的意外问题，并且很容易被遗漏。

#### Best Practice

通过命令行参数或 CI 构建时，Unity 建议使用 [command line arguments](https://docs.unity3d.com/Manual/CommandLineArguments.html) 为每个所需平台重新启动编辑器。这可确保在调用 -executeMethod 之前为平台编译脚本。

#### Is there a safe way to change scripts before building?

要切换平台或修改代码中的编辑器脚本，然后继续定义集，必须执行域重新加载。请注意，在这种情况下，不应使用 -quit 参数，否则编辑器将在执行调用的方法后立即退出。

当域重新加载时，会调用 InitialiseOnLoad。下面的代码演示了如何设置脚本定义符号并对编辑器代码中的符号做出反应，在域重新加载完成后构建可寻址。可以为切换平台和 [platform dependant compilation](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html) 完成相同的过程。

```csharp
[InitializeOnLoad]
public class BuildWithScriptingDefinesExample
{
    static BuildWithScriptingDefinesExample()
    {
        bool toBuild = SessionState.GetBool("BuildAddressables", false);
        SessionState.EraseBool("BuildAddressables");
        if (toBuild)
        {
            Debug.Log("Domain reload complete, building Addressables as requested");
            BuildAddressablesAndRevertDefines();
        }
    }

    [MenuItem("Build/Addressables with script define")]
    public static void BuildTest()
    {
#if !MYDEFINEHERE
        Debug.Log("Setting up SessionState to inform an Addressables build is requested on next Domain Reload");
        SessionState.SetBool("BuildAddressables", true);
        string originalDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        string newDefines = string.IsNullOrEmpty(originalDefines) ? "MYDEFINEHERE" : originalDefines + ";MYDEFINEHERE";
        Debug.Log("Setting Scripting Defines, this will then start compiling and begin a domain reload of the Editor Scripts.");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
#endif
    }

    static void BuildAddressablesAndRevertDefines()
    {
#if MYDEFINEHERE
        Debug.Log("Correct scripting defines set for desired build");
        AddressableAssetSettings.BuildPlayerContent();
        string originalDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (originalDefines.Contains(";MYDEFINEHERE"))
            originalDefines = originalDefines.Replace(";MYDEFINEHERE", "");
        else
            originalDefines = originalDefines.Replace("MYDEFINEHERE", "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, originalDefines);
        AssetDatabase.SaveAssets();
#endif
        EditorApplication.Exit(0);
    }
}
```