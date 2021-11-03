# Memory management

Addressables 系统通过保持它加载的每个项目的引用计数来管理用于加载资产和包的内存。

当一个 Addressable 被加载时，系统增加引用计数；当资产被释放时，系统会减少引用计数。当一个 Addressable 的引用计数归零时，它就有资格被卸载。当您显式加载可寻址资产时，您还必须在完成后释放该资产。

避免“内存泄漏”（不再需要后仍保留在内存中的资产）的基本经验法则是将每次对加载函数的调用与对释放函数的调用进行镜像。您可以使用对资产实例本身的引用或使用原始加载操作返回的结果句柄来释放资产。

但是请注意，释放的资产不一定会立即从内存中卸载。资产使用的内存在它所属的 AssetBundle 也被卸载之前不会被释放。（释放的资产也可以通过调用[Resources.UnloadUnusedAssets](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.UnloadUnusedAssets.html)卸载，但这往往是一个缓慢的操作，可能会导致帧速率故障。）

AssetBundle 有自己的引用计数（系统将它们视为可寻址对象，并将它们包含的资产视为依赖项）。当您从包中加载资产时，包的引用计数会增加，而当您释放资产时，包的引用计数会减少。当一个包的引用计数归零时，这意味着它包含的所有资产都没有在使用，并且包和它包含的所有资产都从内存中卸载。

使用[Event Viewer](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/EventViewer.html) 来监控您的运行时内存管理。查看器显示加载和卸载资产及其依赖项的时间。

## Understanding when memory is cleared

不再引用的资产（由 [Event Viewer](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/EventViewer.html)蓝色部分的末尾表示）并不一定意味着该资产已卸载。一个常见的适用场景涉及到一个 AssetBundle 中的多个资产。例如：

- 您在 AssetBundle (`stuff` ) 中有三个资产 ( `tree`、`tank`和`cow`)。
- 当`tree`加载时，剖析器显示一个单一REF-计数`tree`，和一个用于`stuff`。
- 后来，当`tank`负载，探查显示一个参考数为两个`tree`，并`tank`和两个REF-数为`stuff`AssetBundle。
- 如果你释放`tree`，它的引用计数变为零，蓝条消失。

在此示例中，此时`tree`资产并未实际卸载。您可以加载 AssetBundle 或其部分内容，但不能部分卸载 AssetBundle。在 AssetBundle 本身完全卸载之前，不会卸载任何资产。此规则的例外是引擎接口[Resources.UnloadUnusedAssets](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.UnloadUnusedAssets.html)。在上述场景中执行此方法会导致`tree`卸载。因为 Addressables 系统无法意识到这些事件，所以分析器图只反映了 Addressables 引用计数（不完全是内存保存的内容）。请注意，如果您选择使用[Resources.UnloadUnusedAssets](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.UnloadUnusedAssets.html)，这是一个非常慢的操作，并且只能在不会显示任何故障的屏幕上调用（例如加载屏幕）。

## Avoiding asset churn

如果您释放恰好是 AssetBundle 中最后一项的对象，然后立即重新加载该资产或捆绑包中的其他资产，则可能会出现asset churn问题。

例如，假设你有两种材料，`boat`并且`plane`共享纹理`cammo`，已被拉入了自己的AssetBundle。关卡1使用`boat`和 关卡2 使用`plane`。当您退出关卡1时，您释放`boat`并立即加载`plane`. 当您释放时`boat`，Addressables 卸载纹理`cammo`。然后，当您加载 时`plane`，Addressables 会立即重新加载`cammo`。

您可以使用 [Event Viewer](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/EventViewer.html)通过监控资产加载和卸载来帮助检测asset churn。

## AssetBundle memory overhead

当你加载一个 AssetBundle 时，Unity 会分配内存来存储包的内部数据，这是除了用于它包含的资产的内存之外。加载的 AssetBundle 的主要内部数据类型包括：

- [Serialized file buffers](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#serialized-file-buffers): used to load data from a bundle
- [TypeTrees](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#typetrees): defines the serialized layout of your objects
- [Table of contents](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#table-of-contents): lists the assets in a bundle
- [Preload table](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#preload-table): lists the dependencies of each asset

当你组织你的 Addressable 组和 AssetBundle 时，你通常必须在你创建和加载的 AssetBundle 的大小和数量之间进行权衡。一方面，更少、更大的包可以最大限度地减少 AssetBundles 的总内存使用量。另一方面，使用大量小包可以最大限度地减少峰值内存使用量，因为您可以更轻松地卸载资产和 AssetBundles。

虽然磁盘上 AssetBundle 的大小与其运行时的大小不同，但您可以使用磁盘大小作为构建中 AssetBundle 内存开销的大致指南。您可以从[Build Layout Report ](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html)中获取包大小和其他可用于帮助分析 AssetBundles 的信息。

以下部分讨论 AssetBundles 使用的内部数据以及如何尽可能减少它们所需的内存量。

### Serialized file buffers

当 Unity 加载 AssetBundle 时，它会为包中的每个序列化文件分配一个内部缓冲区，并在 AssetBundle 的生命周期内保留此缓冲区。非场景 AssetBundle 包含一个序列化文件，但场景 AssetBundle 最多可以包含包中每个场景的两个序列化文件。

因为文件缓冲区是按加载的 AssetBundle 分配的，所以您可以通过将给定时间加载的包数量保持在最低限度来减少用于它们的内存量。此外，如果您的 AssetBundle 大小足够小，以至于文件缓冲区的大小占已加载包所用总内存的很大一部分，那么请考虑使用更大的包是否更有意义。

缓冲区大小针对每个平台进行了优化。Switch、Playstation 和 Windows RT 使用 128k 缓冲区。所有其他平台都使用 14k 缓冲区。您可以使用[Build Layout Report](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html)来确定 AssetBundle 中有多少序列化文件。

### TypeTrees

TypeTree 描述了您的一种数据类型的字段布局。

AssetBundle 中的每个序列化文件都包含文件中每个对象类型的 TypeTree。TypeTree 信息允许您加载与序列化方式略有不同的反序列化对象。AssetBundles 之间不共享 TypeTree 信息；每个包都有一组完整的类型树，用于它包含的对象。

当 AssetBundle 被加载并在 AssetBundle 的生命周期内保存在内存中时，所有类型树都会被加载。与 TypeTrees 相关的内存开销与序列化文件中唯一类型的数量和这些类型的复杂性成正比。

您可以通过以下方式减少 AssetBundle TypeTrees 的内存需求：

- 将相同类型的资产放在同一个包中。
- 关闭 TypeTrees -- 关闭 TypeTrees 通过从包中排除此信息来使您的 AssetBundles 更小。但是，如果没有 TypeTree 信息，您可能会在使用较新版本的 Unity 加载旧包时或在项目中进行很小的脚本更改后遇到序列化错误或未定义的行为。
- 首选更简单的数据类型以降低 TypeTree 的复杂性。

您可以通过在禁用和不启用 TypeTrees 的情况下构建它们并比较大小来测试 TypeTrees 对 AssetBundles 大小的影响。使用[BuildAssetBundleOptions.DisableWriteTypeTree](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/BuildAssetBundleOptions.DisableWriteTypeTree.html)禁用 AssetBundle 中的 TypeTree。请注意，并非所有平台都支持 TypeTrees，某些平台需要 TypeTrees（并忽略此设置）。

如果在项目中禁用 TypeTrees，请始终在构建新播放器之前重建本地可寻址组。如果您要远程分发内容，请仅使用与您用于制作当前播放器的 Unity 版本（包括补丁号）相同的 Unity 版本来更新内容，甚至不要对代码进行细微的更改。（当您处理多个播放器版本、更新和 Unity 版本时，您可能会发现禁用 TypeTrees 所节省的内存不值得麻烦。）

### Table of contents

目录是包中的一个地图，允许您按名称查找每个明确包含的资产。它与资产的数量和映射资产的字符串名称的长度成线性关系。

目录数据的大小基于资产总数。您可以通过最小化在给定时间加载的 AssetBundle 数量来最小化专用于保存内容数据表的内存量。

### Preload table

预加载表是资产引用的所有其他对象的列表。当您从 AssetBundle 加载资产时，Unity 使用预加载表加载这些引用的对象。

例如，Prefab 的每个组件以及它可能引用的任何其他资产（材料、纹理等）都有一个预加载条目。每个预加载条目都是 64 位，可以引用其他 AssetBundle 中的对象。

当一个资产引用另一个又引用其他资产的资产时，预加载表可能会变大，因为它包含加载这两个资产所需的条目。如果两个资产都引用了第三个资产，则两者的预加载表都包含加载第三个资产的条目（无论引用的资产是可寻址的还是在同一个 AssetBundle 中）。

例如，考虑这样一种情况，您在 AssetBundle 中有两个资产（PrefabA 和 PrefabB），并且这两个预制件都引用了第三个预制件 (PrefabC)，该预制件很大并且包含多个组件和对其他资产的引用。这个 AssetBundle 包含两个预加载表，一个用于 PrefabA，一个用于 PrefabB。这些表包含其各自预制件的所有对象的条目，但也包含 PrefabC 中所有对象的条目以及由 PrefabC 引用的任何对象的条目。因此，加载 PrefabC 所需的信息最终会在 PrefabA 和 PrefabB 中重复。无论 PrefabC 是否显式添加到 AssetBundle 中，都会发生这种情况。

根据您组织资产的方式，AssetBundles 中的预加载表可能会变得非常大并且包含许多重复条目。如果您有多个可加载资源都引用了复杂资源，例如上述情况中的 PrefabC，则尤其如此。如果您确定预加载表的内存开销有问题，您可以构建可加载资产，以便它们具有较少的复杂加载依赖项。

## AssetBundle dependencies

加载可寻址资产也会加载包含其依赖项的所有 AssetBundle。当一个包中的资产引用另一个包中的资产时，就会发生 AssetBundle 依赖项。一个例子是引用纹理的材质。

Addressables 在包级别计算包之间的依赖关系。如果一个资产引用另一个包中的对象，则整个包都依赖于该包。这意味着即使您在第一个包中加载了一个没有其自身依赖项的资产，第二个 AssetBundle 仍会加载到内存中。

为了避免加载比所需更多的包，您应该努力使 AssetBundle 之间的依赖关系尽可能简单。

**NOTE**

在 Addressables 1.13.0 之前，依赖图不像现在那么彻底。在上面的示例中，RootAsset1 不会依赖 BundleB。当卸载并重新加载另一个 AssetBundle 引用的 AssetBundle 时，之前的这种行为会导致引用中断。如果依赖关系图足够复杂，此修复可能会导致内存中保留额外的数据。