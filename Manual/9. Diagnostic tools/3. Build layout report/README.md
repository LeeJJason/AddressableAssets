# Build layout report

构建布局报告提供有关您的可寻址构建的详细信息和统计信息，包括：

- AssetBundles 的描述

- 每个 Asset 和 AssetBundle 的大小

- 对作为依赖项隐式包含在 AssetBundles 中的不可寻址资产的解释

- AssetBundle 依赖项

  启用后，Addressables 构建脚本会在您构建 Addressables 内容时创建报告。您可以在[Preferences 窗口](https://docs.unity3d.com/Manual/Preferences.html)的 Addressables 部分启用报告。您可以在项目文件夹中的 中找到该报告`Library/com.unity.addressables/buildlayout.txt`。生成报告确实会增加构建时间。

有关构建内容的更多信息，请参阅[Building your Addressable content](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildingContent.html)。

## Creating a build report

要创建构建报告：

1. 启用构建报告。
   1. 打开 Unity 首选项窗口（菜单：Edit > Preferences）。
   2. 从首选项类型列表中选择**Addressables** 。
   3. 检查 **Debug Build Layout** 选项。 ![img](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/images/addr_diagnostics_0.png)
2. 执行可寻址内容的完整构建。（有关更多信息，请参阅[Builds](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Builds.html)。）
3. 在文件系统窗口中，导航到`Library/com.unity.addressables/`Unity 项目的文件夹。
4. `buildlayout.txt`在合适的文本编辑器中打开文件。

## Report data

构建布局报告包含以下信息：

- [Summary](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#summary-section): provides an overview of the build
- [Group](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#group-section): provides information for each group
- [Asset bundle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#assetbundle-information): provides information about each bundle built for a group
- [Asset](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#asset-information): provides information about each explicit asset in a bundle
- [File](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#file-information): provides information about each serialized file in an AssetBundle archive
- [Built-in bundles](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#built-in-bundles): provides information about bundles created for assets, such as the default shader, that are built into Unity

### Summary section

提供构建的摘要。

| Name                          | Purpose                                                      |
| :---------------------------- | :----------------------------------------------------------- |
| Addressable Groups            | 构建中包含的组数。                                           |
| Explicit Assets Addressed     | 构建中的可寻址资产的数量（此数字不包括构建中被可寻址资产引用但未标记为可寻址的资产）。 |
| Total Bundle                  | 构建创建的 AssetBundle 数量，包括包含场景数据的数量。        |
| Total Build Size              | 所有 AssetBundles 的总大小。                                 |
| Total MonoScript Size         | 序列化 MonoBehaviour 和 SerializedObject 实例的大小。        |
| Total AssetBundle Object Size |                                                              |

### Group section

报告 Addressables 如何将组中的资产打包到 AssetBundles 中。

| 姓名          | 目的                                                         |
| :------------ | :----------------------------------------------------------- |
| Group summary | 名称、为群组创建的捆绑包数量、总大小以及为群组构建的显式资产数量。 |
| Schemas       | 组的架构和设置。                                             |
| Asset bundles | 看 [AssetBundle information](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#assetbundle-information). |

### AssetBundle information

报告为组构建的每个 AssetBundle 的详细信息。

| Name                         | Purpose                                                      |
| :--------------------------- | :----------------------------------------------------------- |
| File name                    | AssetBundle 的文件名。                                       |
| Size                         |                                                              |
| Compression                  | 用于捆绑的压缩设置。                                         |
| Object size                  |                                                              |
| Bundle Dependencies          | 当前包依赖的其他 AssetBundles 列表。这些包总是与当前包一起加载。 |
| Expanded Bundle Dependencies |                                                              |
| Explicit Assets              | [Asset information](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#asset-information) 关于捆绑中包含的可寻址对象。 |
| Files                        | [File information](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html#file-information)关于 AssetBundle 存档中的文件。场景包每个场景最多包含两个文件，非场景包仅包含一个文件。 |

### Asset information

在显式资产部分提供每个资产的信息。

| Name                    | Purpose                                 |
| :---------------------- | :-------------------------------------- |
| Asset path              | 项目中资产的路径                        |
| Total Size              |                                         |
| Size from Objects       |                                         |
| Size from Streamed Data |                                         |
| File Index              | 此资产所在的 AssetBundle 中文件的索引。 |
| Addressable Name        | 资产的地址。                            |
| External References     |                                         |
| Internal References     |                                         |

### File information

提供有关 AssetBundle 存档中每个序列化文件的详细信息

| Name          | Purpose                                                |
| :------------ | :----------------------------------------------------- |
| File summary  | 文件列表中的索引，文件中序列化 MonoScript 的数量和大小 |
| File sections | 序列化文件可以包含以下一个或多个部分：                 |

无扩展名 -- .resS -- .resource -- .sharedAssets -- | | 来自其他资产的数据| 文件中资产引用的相关资产。

### Built-in Bundles

列出 Addressables 从资产创建的任何包，例如作为 Unity 引擎的一部分提供的默认着色器。Addressables 构建在需要时将这些资产放置在此处列出的单独包中，以避免将资产作为隐式依赖项复制到多个包中。