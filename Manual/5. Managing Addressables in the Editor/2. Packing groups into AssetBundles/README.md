# Packing groups into AssetBundles

在选择如何将组中的资产打包到 AssetBundles 中时，您有几个选项：

- 您可以将一个组中的所有可寻址对象打包在一个包中。
- 您可以将每个 Addressable 单独打包在自己的包中。
- 您可以将共享同一组标签的所有可寻址对象打包到它们自己的包中。

场景资产始终与组中的其他可寻址资产分开打包。因此，一个包含场景和非场景资产混合的组在构建时总是至少产生两个包，一个用于场景，一个用于其他所有内容。

当您选择单独打包每个可寻址的文件夹时，标记为可寻址的文件夹中的资产和复合资产（如 Sprite 表）会被特殊对待：

- 文件夹中标记为可寻址的所有资产都打包在同一个文件夹中（文件夹中的资产本身单独标记为可寻址的除外）。
- 可寻址精灵图集中的精灵包含在同一个包中。

有关详细信息，请参阅 [Content Packing & Loading settings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/GroupSettings.html#content-packing-loading-settings)。

```
NOTE
当许多人在同一个项目上工作时，将许多资产保存在同一个组中会增加版本控制冲突的机会。
```





选择将您的内容打包成几个大包还是许多小包，可能会产生两种极端的后果：

包过多的危险：

- 每个包都有 [memory overhead](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html#assetbundle-memory-overhead)。这与该页面上概述的许多因素有关，但简而言之，这种开销可能很大。如果您预计一次将 100 个甚至 1000 个包加载到内存中，这可能意味着大量内存被消耗掉。
- 下载包有并发限制。如果您同时需要 1000 个捆绑包，则不可能同时下载所有捆绑包。将下载一定数量，当它们完成时，将触发更多。在实践中，这是一个相当小的问题，如此小以至于您通常会受到下载总大小的限制，而不是它被分成多少个包。
- 捆绑信息会使目录膨胀。为了能够下载或加载目录，我们会存储有关您的捆绑包的基于字符串的信息。数以千计的数据包可以大大增加目录的大小。
- 重复资产的可能性更大。假设两个材料被标记为可寻址，并且每个都依赖于相同的纹理。如果它们在同一个包中，则纹理被拉入一次，并被两者引用。如果它们在单独的包中，并且纹理本身不是可寻址的，那么它将被复制。然后，您要么将纹理标记为可寻址，要么接受重复，或将材料放在同一个包中。

包太少的危险：

- UnityWebRequest（我们用来下载）不会恢复失败的下载。因此，如果正在下载大包并且您的用户失去连接，则一旦他们重新获得连接，下载就会重新开始。
- 物品可以从捆绑包中单独加载，但不能单独卸载。例如，如果你在一个包中有 10 个材料，加载所有 10 个，然后告诉 Addressables 释放其中的 9 个，所有 10 个都可能在内存中。有关更多信息，请参阅 [Memory management](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html)。

## Scale implications as your project grows larger

随着您的项目变得越来越大，请注意资产和包的以下方面：

- **Total bundle size**：过去 Unity 不支持大于 4GB 的文件。这已在最近的一些编辑器版本中得到修复，但仍然存在问题。建议将给定包的内容保持在此限制之下，以便在所有平台上实现最佳兼容性。
- **Bundle layout at scale**：内容构建生成的 AssetBundle 数量与这些捆绑包的大小之间的内存和性能权衡可能会随着项目变大而发生变化。
- **Sub assets affecting UI performance**：这里没有硬性限制，但是如果您有很多资产，并且这些资产有很多子资产，最好关闭子资产显示。此选项仅影响数据在“组”窗口中的显示方式，不影响运行时可以加载和不可以加载的内容。该选项在**Tools** > **Show Sprite and Subobject Addresses**下的组窗口中可用。禁用此功能将使 UI 相应更快。
- **Group Hierarchy display**：另一个有助于缩放的仅 UI 选项是**Group Hierarchy with Dashes**。这在顶级设置的检查器中可用。启用此功能后，名称中包含破折号“-”的组将显示为好像破折号代表文件夹层次结构。这不会影响实际的组名，也不会影响事物的构建方式。例如，两个名为“x-y-z”和“x-y-w”的组将显示为好像在名为“x”的文件夹中，有一个名为“y”的文件夹。该文件夹内有两个组，分别称为“x-y-z”和“x-y-w”。这不会真正影响 UI 响应能力，但只会使浏览大量组更容易。

