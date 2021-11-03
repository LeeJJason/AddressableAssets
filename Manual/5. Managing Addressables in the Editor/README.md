# Managing Addressables in the Editor

虽然不可能对组织项目中资产的所有不同方式进行全面编目，但 [Organizing Addressable assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsDevelopmentCycle.html#organizing-addressable-assets)概述了在规划组织策略时要考虑的几个注意事项。

在考虑如何管理资产时，您还应该了解 [How Addressables interact with your Project assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ManagingAssets.html)。

可寻址[组](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Groups.html)是您管理可寻址资产的主要组织单位。使用 Addressables 时的一个重要考虑因素是 [Packing groups into AssetBundles](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/PackingGroupsAsBundles.html)的选项。

除了组设置之外，您还可以使用以下内容来控制 Addressables 在项目中的工作方式：

- [Addressable Asset Settings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetSettings.html)：项目级设置
- [Profiles](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsProfiles.html)：定义构建路径设置的集合，您可以根据构建的目的在这些设置之间进行切换。（如果您打算远程分发内容，主要是感兴趣。）
- [Labels](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Labels.html)：编辑项目中使用的可寻址资产标签。
- [Play Mode Scripts](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Groups.html#play-mode-scripts)：选择在编辑器中进入播放模式时可寻址系统如何加载资产。

[AssetReferences](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AssetReferences.html)提供了一种 UI 友好的方式来使用可寻址资产。您可以在 MonoBehaviour 和 ScriptableObject 类中包含 AssetReference 字段，然后在编辑器中使用拖放或对象选择器对话框将资产分配给它们。

Addressables 系统提供以下附加工具来帮助开发：

- [Analyze tool](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html)：提供各种分析规则，您可以运行这些规则来验证您是否按照您想要的方式组织了资产，包括关于 Addressables 如何将您的资产打包成包的报告。
- [Event viewer](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/EventViewer.html)：提供一个配置文件视图，显示您的资产何时加载和释放。使用事件查看器来验证您是否正在释放资产并监控内存使用高峰。
- [Hosting Service](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsHostingServices.html)：提供一个简单的资产服务器，您可以使用它来托管远程资产以进行本地开发。
- [Build layout report](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildLayoutReport.html)：提供构建生成的 AssetBundles 的描述。
- [Build profile log](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildProfileLog.html)：提供配置构建过程本身的日志，以便您可以查看哪些部分花费的时间最长。

## Organizing Addressable Assets

没有单一的最佳方式来组织您的资产；这取决于每个项目的具体要求。在规划如何管理项目中的资产时要考虑的方面包括：

- 逻辑组织：将资产保持在逻辑类别中可以更容易地了解您的组织并发现不合适的项目。
- 运行时性能：如果您的包变得非常大，或者如果您有非常多的包，就会出现性能瓶颈。
- 运行时内存管理：将您一起使用的资产放在一起有助于降低峰值内存需求。
- 规模：一些组织资产的方法可能适用于小型游戏，但不适用于大型游戏，反之亦然。
- 平台特性：平台的特性和要求可能是如何组织资产的一个重要考虑因素。一些例子：
  - 提供丰富虚拟内存的平台比虚拟内存有限的平台可以更好地处理大包。
  - 一些平台不支持下载内容，完全排除了资产的远程分发。
  - 某些平台不支持 AssetBundle 缓存，因此在可能的情况下将资产放在本地包中会更有效。
- 分发：无论您是否远程分发您的内容，至少意味着您必须将远程内容与本地内容分开。
- 资产多久更新一次：将您希望经常更新的资产与您计划很少更新的资产分开。
- 版本控制：处理相同资产和资产组的人越多，项目中发生版本控制冲突的机会就越大。

## Common strategies

典型的策略包括：

- 并发使用：将您同时加载的资产组合在一起，例如给定级别的所有资产。从长远来看，这种策略通常是最有效的，可以帮助减少项目中的峰值内存使用。
- 逻辑实体：将属于同一逻辑实体的资产组合在一起。例如，UI 布局资产、纹理、声音效果。或者角色模型和动画。
- 类型：将相同类型的资产组合在一起。例如，音乐文件、纹理。

根据项目的需要，这些策略之一可能比其他策略更有意义。例如，在具有多个级别的游戏中，从项目管理和运行时内存性能的角度来看，根据并发使用情况进行组织可能是最有效的。同时，您可能会对不同类型的资产使用不同的策略。例如，您的菜单屏幕的 UI 资产可能会在基于级别的游戏中全部组合在一起，否则将其级别数据单独分组。您还可以将包含某个级别资产的组打包到包含特定类型资产的捆绑包中。

有关其他信息，请参阅[Preparing Assets for AssetBundles](https://docs.unity3d.com/2019.4/Documentation/Manual/AssetBundles-Preparing.html)。

