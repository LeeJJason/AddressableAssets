# Upgrading to the Addressables system

本文介绍如何修改现有项目以利用可寻址资产。

在 Addressables 系统之外，Unity 提供了一些“传统”的方式来引用和加载资产：

- **Scene data**：您直接添加到场景或场景中的组件的资产，应用程序会自动加载这些资产。Unity 将序列化的场景数据和场景直接引用的资产打包到一个单独的存档中，该存档包含在您构建的播放器应用程序中。请参阅 [Converting Scenes](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html#converting-scenes)和 [Using Addressable assets in non-Addressable Scenes](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html#addressables-in-regular-scenes)。
- Prefabs：您使用游戏对象和组件创建的资产，并保存在场景之外。请参阅[Converting Prefabs](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html#converting-prefabs)。
- **Resources folders**：您放置在项目的资源文件夹中并使用资源 API 加载的资产。Unity 将资源文件中的资产打包到一个单独的档案中，并包含在您构建的播放器应用程序中。资源存档与场景数据存档分开。请参阅 [Converting Resources folders](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html#converting-resources-folders)。
- **AssetBundles**：您在 AssetBundles 中打包并使用 AssetBundle API 加载的资产。请参阅[Converting AssetBundles](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html#converting-assetbundles)。
- **StreamingAssets**：您放置在 StreamingAssets 文件夹中的文件。Unity 在您构建的播放器应用程序中包含 StreamingAssets 文件夹中的所有文件。查看[Files in StreamingAssets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html#files-in-streamingassets)

## Converting Scenes

将 Addressables 集成到项目中的最简单方法是将您的场景移出**[Build Settings](https://docs.unity3d.com/2019.4/Documentation/Manual/BuildSettings.html)**列表并使这些场景可寻址。您确实需要在列表中有一个场景，即应用程序启动时加载的场景 Unity。您可以为此创建一个新场景，除了加载您的第一个可寻址场景外，什么都不做。

要转换您的场景：

1. 制作一个新的“初始化”场景。
2. 打开 **Build Settings** 窗口（菜单：**File > Build Settings**）。
3. 将初始化场景添加到场景列表中。
4. 从列表中删除其他场景。
5. 单击项目列表中的每个场景并检查其检查器窗口中的可寻址选项。或者，您可以将场景资产拖到可寻址组窗口中的组中。（不要让你的新初始化场景可寻址。）
6. 更新用于加载场景的代码，以使用[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html)类场景加载方法而不是 SceneManager 方法。

此时，您已将场景中的所有资产包含在一个 Addressable 组中，并且在您构建 Addressables 内容时，Addressables 系统会将它们打包到 AssetBundle 中。如果您只为所有场景使用一组，则运行时加载和内存性能应该大致相当于您项目的 pre-Addressables 状态。

您现在可以将一个大型可寻址场景组拆分为多个组。做到这一点的最佳方式取决于项目目标。要继续，您可以将场景移动到它们自己的组中，以便您可以独立加载和卸载每个场景。执行此操作时，您可以使用[[Analyze tool](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html)检查在多个场景之间共享的重复资产。通过使资产本身可寻址，您可以避免复制从两个不同包中引用的资产。通常最好将共享资产移动到他们自己的组中，以减少 AssetBundles 之间的相互依赖。

## Using Addressable assets in non-Addressable Scenes

对于不想设置为可寻址的场景，您仍然可以通过[AssetReferences](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AssetReferences.html)将可寻址资产用作场景数据的[一部分](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AssetReferences.html)。

当您将 AssetReference 字段添加到自定义 MonoBehaviour 或 ScriptableObject 类时，您可以在 Unity 编辑器中为该字段分配一个可寻址资产，其方式与分配资产作为直接引用的方式大致相同。主要区别在于您需要向类添加代码以加载和释放分配给 AssetReference 字段的资产（而 Unity 在场景中实例化您的对象时会自动加载直接引用）。

**NOTE**

*您不能对不可寻址场景中的任何 UnityEngine 组件的字段使用可寻址资产。例如，如果您将可寻址网格资产分配给不可寻址场景中的 MeshFilter 组件，Unity 不会将该网格数据的可寻址版本用于场景。相反，Unity 会复制网格资产并在您的应用程序中包含两个版本的网格，一个在为包含网格的可寻址组构建的 AssetBundle 中，另一个在非可寻址场景的内置场景数据中。（在可寻址场景中使用时，Unity 不会复制网格数据并始终从 AssetBundle 加载它。）*

要在自定义类中用 AssetReferences 替换直接引用，请执行以下步骤：

1. 将您对对象的直接引用替换为资产引用（例如，`public GameObject directRefMember;`变成`public AssetReference assetRefMember;`）。
2. 将资产拖到相应组件的检查器上，就像直接引用一样。
3. 添加运行时代码以使用[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html) API加载分配的资产。
4. 添加代码以在不再需要时释放加载的资产。

有关使用 AssetReference 字段的更多信息，请参阅[Asset References](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AssetReferences.html)。

有关加载可寻址资产的更多信息，请参阅[Loading Addressable assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html)。

## Converting Prefabs

要将预制件转换为可寻址资产，请选中其检查器窗口中的**Addressables** 选项或将其拖到Addressables [Groups](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Groups.html)窗口中的组中。

在可寻址场景中使用时，您并不总是需要使预制件可寻址；Addressables 自动包含您添加到场景层次结构中的预制件，作为场景的 AssetBundle 中包含的数据的一部分。但是，如果您在多个场景中使用预制件，则应将预制件设为可寻址资产，以便预制件数据不会在使用它的每个场景中重复。如果要在运行时动态加载和实例化它，还必须使 Prefab 可寻址。

**NOTE**

*如果在不可寻址的场景中使用预制件，无论预制件是否可寻址，Unity 都会将预制件数据复制到内置场景数据中。您可以使用 [Analyze tool](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html)中的**Check Scene to Addressable Duplicate Dependencies**规则来识别可寻址资产组和不可寻址场景数据之间重复的资产。*

## Converting Resources folders

如果您的项目在 Resources 文件夹中加载资产，您可以将这些资产迁移到 Addressables 系统：

1. 使资产可寻址。为此，请在每个资产的检查器窗口中启用 选项，或者**Addressable** 将资产拖到Addressables [Groups](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Groups.html)窗口中的组中。
2. 更改使用[Resources](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.html) API加载资产的任何运行时代码，以使用[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html) API加载它们。
3. 添加代码以在不再需要时释放加载的资产。

与 Scenes 一样，如果您将所有以前的 Resources 资产保留在一个组中，则加载和内存性能应该是等效的。根据您的项目，您可以通过将资产分成不同的组来提高性能和灵活性。使用[ [Analyze tool](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html)检查 AssetBundles 中不需要的重复。

当您将 Resources 文件夹中的资产标记为可寻址时，系统会自动将该资产移动到项目中名为 Resources_moved 的新文件夹中。移动资产的默认地址是旧路径，省略文件夹名称。例如，您的加载代码可能会从：

```csharp
Resources.LoadAsync<GameObject>("desert/tank.prefab"); 
```

到：

```csharp
Addressables.LoadAssetAsync<GameObject>("desert/tank.prefab");.
```

在修改项目以使用可寻址系统后，您可能必须以不同方式实现[Resources](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.html)类的某些功能。

例如，考虑[Resources.LoadAll](https://docs.unity3d.com/ScriptReference/Resources.LoadAll.html)函数。以前，如果您在文件夹 Resources/MyPrefabs/ 中拥有资产，并运行 Resources.LoadAll<SampleType>("MyPrefabs");，它将加载 Resources/MyPrefabs/ 匹配类型中的所有资产`SampleType`。Addressables 系统不支持这种确切的功能，但您可以使用 Addressable [labels](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Labels.html)获得类似的结果。

## Converting AssetBundles

当您第一次打开**Addressables Groups**窗口时，Unity 提供将所有 AssetBundle 转换为 Addressables 组。这是将 AssetBundle 设置迁移到 Addressables 系统的最简单方法。您仍然必须更新您的运行时代码以使用[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html) API加载和发布资产。

如果您想手动转换您的 AssetBundle 设置，请单击 **Ignore** 按钮。手动将 AssetBundles 迁移到 Addressables 的过程类似于 Scenes 和 Resources 文件夹中描述的过程：

1. 通过在每个资产的检查器窗口上启用 **Addressable** 选项或通过将资产拖到Addressables [Groups](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Groups.html)窗口中的组来使资产可寻址。Addressables 系统会忽略资产的现有 AssetBundle 和 Label 设置。
2. 更改使用[AssetBundle](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/AssetBundle.html)或[UnityWebRequestAssetBundle](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Networking.UnityWebRequestAssetBundle.html) API加载资产的任何运行时代码，以使用[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html) API加载它们。您不需要显式加载 AssetBundle 对象本身或资产的依赖项；Addressables 系统自动处理这些方面。
3. 添加代码以在不再需要时释放加载的资产。

**NOTE**

*资产地址的默认路径是其文件路径。如果使用路径作为资产的地址，则加载资产的方式与从包中加载的方式相同。可寻址资产系统处理包及其所有依赖项的加载。*

如果您选择了自动转换选项或手动将您的资产添加到等效的 Addressables 组，那么根据您的组设置，您最终会得到包含相同资产的相同捆绑包。（捆绑文件本身不会完全相同。）您可以使用 [Analyze tool](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html) 检查不需要的重复和其他潜在问题。您可以使用 [事件查看器] 窗口确保资产加载和卸载的行为符合您的预期。

## Files in StreamingAssets

当您使用 Addressables 系统时，您可以继续从 StreamingAssets 文件夹加载文件。但是，此文件夹中的文件不能是可寻址的，也不能引用项目中的其他资产。

在构建期间，Addressables 系统确实将其运行时配置文件和本地 AssetBundles 放在 StreamingAssets 文件夹中。（Addressables 在构建过程结束时删除这些文件；您不会在编辑器中看到它们。）