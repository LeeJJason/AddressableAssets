# Loading Addressable assets

该[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html)类提供了加载寻址资产的几种方法。您可以一次一个或批量加载资产。要识别要加载的资产，您可以将单个键或键列表传递给加载函数。键可以是以下对象之一：

- Address：包含您分配给资产的地址的字符串
- Label：包含分配给一个或多个资产的标签的字符串
- AssetReference对象：[AssetReference](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.AssetReference.html)实例
- [IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)实例：一个中间对象，包含加载资产及其依赖项的信息。

当您调用资产加载功能之一时，Addressables 系统开始执行以下任务的异步操作：

1. 查找指定键的资源位置（IResourceLocation 键除外）
2. 收集依赖项列表
3. 下载所需的任何远程 AssetBundle
4. 将 AssetBundles 加载到内存中
5. 将操作的[Result](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Result.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Result)对象设置为加载的对象
6. 更新操作的 [Status](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Status.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Status) 并调用任何 [Completed](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Completed.html)事件侦听器

如果加载操作成功，Status 将设置为 Succeeded，并且可以从[Result](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Result.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Result)对象访问加载的对象。

如果发生错误，则将异常复制到操作对象的[OperationException](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.OperationException.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_OperationException)成员中，并将 Status 设置为 Failed。默认情况下，异常不会作为操作的一部分抛出。但是，您可以为[ResourceManager.ExceptionHandler](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler.html#UnityEngine_ResourceManagement_ResourceManager_ExceptionHandler)属性分配一个处理程序函数来处理任何异常。此外，您可以在 Addressable 系统设置中启用[Log Runtime Exceptions](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetSettings.html#diagnostics)选项以将错误记录到 Unity[Console](https://docs.unity3d.com/2019.4/Documentation/Manual/Console.html)。

当您调用可以加载多个可寻址资产的加载函数时，您可以指定是否应该在任何单个加载操作失败时中止整个操作，或者该操作是否应该加载任何可以加载的资产。在这两种情况下，操作状态都设置为失败。（`releaseDependenciesOnFailure`在调用加载函数时将参数设置为 true 以在任何失败时中止整个操作。）

有关异步操作和在 Unity 脚本中编写异步代码的更多信息，请参阅 [Operations](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html) 。

## Loading a single asset

使用[LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html)方法加载单个可寻址资产，通常以地址为键：

**NOTE**

*调用[LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html) 时，您可以使用标签或其他类型的键，而不仅仅是地址。但是，如果键解析为多个资产，则仅加载找到的第一个资产。例如，如果您使用应用于多个资产的标签调用此方法，Addressables 将返回碰巧最先定位的资产中的任何一个。*

## Loading multiple assets

使用[LoadAssetsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync.html)方法在单个操作中加载多个可寻址资产。使用此函数时，您可以指定单个键，例如标签或键列表。

当您指定多个键时，您可以指定一种 [merge mode](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.MergeMode.html)来确定如何组合与每个键匹配的资产集：

- **Union**：包括匹配任何键的资产
- **Intersection**：包括匹配每个键的资产
- **UseFirst**：仅包含解析为有效位置的第一个键中的资产

您可以使用`releaseDependenciesOnFailure`参数指定如何处理加载错误。如果为 true，则操作在加载任何单个资产时遇到错误时失败。释放成功加载的操作和任何资产。

如果为 false，则该操作加载它可以但不释放该操作的任何对象。在失败的情况下，操作仍以失败状态完成。此外，返回的资产列表具有空值，否则将出现失败的资产。

加载必须作为集合加载才能使用的一组资产时设置 `releaseDependenciesOnFailure` 为 true。例如，如果您正在加载游戏关卡的资产，那么将整个操作失败而不是仅加载一些必需的资产可能是有意义的。

### Correlating loaded assets to their keys

当您在一次操作中加载多个资产时，加载单个资产的顺序不一定与传递给加载函数的列表中键的顺序相同。

如果您需要将组合操作中的资产与用于加载它的键相关联，您可以分两步执行该操作：

1. 使用带资产键列表的[IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)实例加载。
2. 使用它们的 IResourceLocation 实例作为键加载各个资产。

IResourceLocation 对象包含密钥信息，例如，您可以保存字典以将密钥与资产相关联。请注意，当您调用加载函数，例如[LoadAssetsAsync ](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync.html)时，该操作首先查找与键对应的[IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)实例，然后使用它来加载资产。当您使用 IResourceLocation 加载资产时，该操作会跳过第一步。因此，分两步执行操作不会增加大量额外工作。

以下示例加载键列表的资产，并按其地址 ( [PrimaryKey](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.PrimaryKey.html#UnityEngine_ResourceManagement_ResourceLocations_IResourceLocation_PrimaryKey) )将它们插入到字典中。该示例首先加载指定键的资源位置。当该操作完成时，它会加载每个位置的资产，使用 Completed 事件将各个操作句柄插入到字典中。操作句柄可用于实例化资产，并在不再需要资产时释放它们。

请注意，加载函数使用[ResourceManager.CreateGenericGroupOperation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.CreateGenericGroupOperation.html)创建了一个组操作。这允许函数在所有加载操作完成后继续。在这种情况下，该函数会调度“Ready”事件以通知其他脚本可以使用加载的数据。

## Loading an AssetReference

该[AssetReference](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.AssetReference.html)类都有自己的加载方法，[LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html)。

您还可以使用 AssetReference 对象作为[Addressables.LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html)方法的键。如果您需要生成资产的多个实例分配给 AssetReference，请使用[Addressables.LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html)，它为您提供可用于释放每个实例的操作句柄。

有关使用[AssetReferences](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.AssetReference.html)的更多信息，请参阅AssetReference。

## Loading Scenes

使用[Addressables.LoadSceneAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadSceneAsync.html)方法按地址或其他可寻址键对象加载可寻址场景资产。

该方法的其余参数对应于与 Unity Engine [SceneManager.LoadSceneAsync](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html)方法一起使用的参数：

- **loadMode** : 是否将加载的 Scene 添加到当前 Scene 或卸载并替换当前 Scene。
- **activateOnLoad**：是在场景加载完成后立即激活场景，还是等到您调用 SceneInstance 对象的[ActivateAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceProviders.SceneInstance.ActivateAsync.html)方法。对应于[AsyncOperation.allowSceneActivation](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AsyncOperation-allowSceneActivation.html)选项。默认为真。
- **priority** : 用于加载场景的 AsyncOperation 的优先级。对应于[AsyncOperation.priority](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AsyncOperation-priority.html)选项。默认为 100。

**WARNING**

*将该`activateOnLoad`参数设置为 false 会阻止 AsyncOperation 队列，包括加载任何其他可寻址资产，直到您激活场景。要激活场景，请调用[LoadSceneAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceProviders.SceneInstance.html)返回的[SceneInstance](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html)的[ActivateAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceProviders.SceneInstance.ActivateAsync.html)方法。有关其他信息，请参阅[AsyncOperation.allowSceneActivation](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AsyncOperation-allowSceneActivation.html)。*

以下示例以附加方式加载场景。加载场景的组件，存储操作句柄并在父游戏对象被销毁时使用它来卸载和释放场景。

在 [Addressables-Sample](https://github.com/Unity-Technologies/Addressables-Sample) 额外的实例库见[Scene loading project](https://github.com/Unity-Technologies/Addressables-Sample/tree/master/Basic/Scene Loading)。

如果您使用[LoadSceneMode.Single](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/SceneManagement.LoadSceneMode.Single.html)加载场景，Unity 运行时会卸载当前场景并调用[Resources.UnloadUnusedAssets](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.UnloadUnusedAssets.html)。卸载的场景被释放，这允许卸载它的 AssetBundle。您单独加载的单个 Addressables 及其操作句柄不会被释放；你必须自己释放它们。（唯一的例外是，任何可寻址的资产，你使用实例[Addressables.InstantiateAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.InstantiateAsync.html)与`trackHandle`设置为true，则默认情况下，自动释放。）

**NOTE**

在编辑器中，您始终可以加载当前项目中的场景，即使它们打包在不可用的远程包中并且您将播放模式脚本设置为**Use Existing Build**。编辑器使用资产数据库加载场景。

## Loading assets by location

当您通过地址、标签或 AssetReference 加载可寻址资产时，可寻址系统首先查找资产的资源位置，并使用这些[IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)实例下载所需的 AssetBundles 和任何依赖项。您可以分两步执行资产加载操作，首先使用[LoadResourceLocationsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync.html)获取 IResourceLocation 对象，然后使用这些对象作为键来加载或实例化资产。

[IResourceLocation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation.html)对象包含加载一项或多项资产所需的信息。

该[LoadResourceLocationsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync.html)方法永远不会失败。如果它无法将指定的键解析为任何资产的位置，则返回一个空列表。您可以通过在`type`参数中指定特定类型来限制函数返回的资产位置类型。

以下示例加载label是“knight”或“village”的所有资产的位置：

## Loading locations of sub-objects

子对象的位置在运行时生成，以减少内容目录的大小并提高运行时性能。当您使用带有子对象的资产的键调用[LoadResourceLocationsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync.html)并且不指定类型时，该函数将为所有子对象以及主对象（如果适用）生成 IResourceLocation 实例。同样，如果您没有为指向具有子对象的资产的 AssetReference 指定使用哪个子对象，则系统会为每个子对象生成 IResourceLocations。

例如，如果您使用地址“myFBXObject”加载 FBX 资产的位置，您可能会获得三个资产的位置：游戏对象、网格和材质。相反，如果您在地址中指定类型“myFBXObject[Mesh]”，您将只能获得 Mesh 对象。您还可以使用LoadResourceLocationsAsync 函数的`type`参数指定类型。

## Instantiating objects from Addressables

您可以加载资源，例如预制件，然后使用[Instantiate](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Object.Instantiate.html)创建它的[实例](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Object.Instantiate.html)。您还可以使用[Addressables.InstantiateAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.InstantiateAsync.html)加载和创建资产的实例。这两种实例化对象的主要区别在于资产引用计数如何受到影响。

使用 InstantiateAsync 时，每次调用该方法时，加载的资产的引用计数都会增加。因此，如果您将 Prefab 实例化五次，则 Prefab 资产及其任何依赖项的引用计数会增加 5。然后，您可以在每个实例在游戏中被销毁时分别释放它们。

当您使用 LoadAssetAsync 和 Object.Instantiate 时，对于初始加载，资产引用计数只会增加一次。如果您释放加载的资产（或其操作句柄）并且引用计数下降到零，那么资产将被卸载并且所有额外的实例化副本也会丢失它们的子资产——它们仍然作为游戏对象存在于场景中，但是没有它们所依赖的网格、材料或其他资产。

哪种方案更好，取决于您如何组织目标代码。例如，如果您有一个管理器对象，它提供了一个预制敌人池以生成游戏关卡，那么在关卡完成时使用存储在管理器类中的单个操作句柄将它们全部释放可能是最方便的。在其他情况下，您可能希望单独实例化和发布资产。

下面的示例调用[InstantiateAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.InstantiateAsync.html)来实例化一个 Prefab。该示例向实例化的 GameObject 添加了一个组件，该组件在销毁 GameObject 时释放资产。

当您调用[InstantiateAsync 时，](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.InstantiateAsync.html)您拥有与[Object.Instantiate](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Object.Instantiate.html)方法相同的选项，以及以下附加参数：

- **instantiationParameters**：这个参数需要[InstantiationParameters](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceProviders.InstantiationParameters.html)结构，您可以使用，而不是指定每次调用InstantiateAsync呼叫指定它们的化选项。如果您对多个实例使用相同的值，这会很方便。
- **trackHandle**：如果为 true，这是默认值，则 Addressables 系统会跟踪实例化实例的操作句柄。这允许您使用[Addressables.ReleaseInstance](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.ReleaseInstance.html)方法释放资产。如果为 false，则不会为您跟踪操作句柄，您必须存储对 InstantiateAsync 返回的句柄的引用，以便在销毁实例时释放实例。

## Releasing Addressable assets

因为 Addressables 系统使用引用计数来确定资产是否在使用中，所以您必须在完成后释放您加载或实例化的每个资产。有关更多信息，请参阅 [Memory Management](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html)。

卸载场景时，场景中的隐式资产不会自动卸载。您必须调用[Resources.UnloadUnusedAssets](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.UnloadUnusedAssets.html)或[UnloadAsset](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.UnloadAsset.html)来释放这些资产。请注意，`UnloadUnusedAssets`当您使用Single加载场景时自动调用。

## Using Addressables in a Scene

如果场景本身是可寻址的，您可以像使用任何资产一样在场景中使用可寻址资产。您可以在场景中放置预制件和其他资产，将资产分配给组件属性，等等。如果您使用不可寻址的资产，则该资产将成为场景的隐式依赖项，并且在您进行内容构建时，构建系统会将其打包在与场景相同的 AssetBundle 中。（可寻址资产根据它们所在的组被打包到它们自己的 AssetBundle 中。）

**NOTE**

*`在多个地方使用的隐式依赖项可以在多个 AssetBundle 和内置场景数据中复制。使用分析工具中的 [Check Duplicate Bundle Dependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html#check-duplicate-bundle-dependencies) 规则来查找不需要的资产重复。`*

如果场景不可寻址，则您直接添加到场景层次结构的任何可寻址资产都将成为隐式依赖项，并且 Unity 将这些资产的副本包含在内置场景数据中，即使它们也存在于可寻址组中。对于分配给场景中游戏对象上的组件的任何资产（例如材质）也是如此。

在自定义组件类中，您可以使用[AssetReference](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.AssetReference.html)字段来允许在非可寻址场景中分配可寻址资产。否则，您可以使用 [addresses](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsOverview.html#asset-addresses) 和[labels](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Labels.html)在运行时从脚本加载资产。请注意，无论场景是否可寻址，您都必须在代码中加载 AssetReference。

