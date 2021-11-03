# Addressables

Addressables 系统提供工具和脚本为应用程序组织和打包的内容，并提供一个 API 来在运行时加载和释放资产。

当您将资产设为“Addressable”时，您可以使用该资产的地址从任何地方加载它。无论该资产位于本地应用程序中还是位于内容交付网络中，Addressable 系统都会定位并返回它。

采用可寻址系统来帮助您在以下领域改进项目：

- **Flexibility**：Addressables 使您可以灵活地调整托管资产的位置。您可以随应用程序安装资产或按需下载。您可以在项目的任何阶段更改访问特定资产的位置，而无需重写任何游戏代码。
- **Dependency management**：系统会自动加载您加载的任何资产的所有依赖项，以便在系统将内容返回给您之前加载所有网格、着色器、动画和其他资产。
- **Memory management**：系统卸载和加载资产，自动计算引用并提供强大的分析器来帮助您发现潜在的内存问题。
- **Content packing**：由于系统映射并理解复杂的依赖链，因此即使您移动或重命名资产，它也能高效地打包 AssetBundles。您可以为本地和远程部署准备资产，以支持可下载的内容并减小应用程序大小。

有关 Addressables 系统的介绍，请参阅[Simplify your content management with Addressables](https://unity.com/how-to/simplify-your-content-management-addressables)。

##### NOTE

Addressables 系统建立在 Unity AssetBundles 之上。如果你想在你的项目中使用 AssetBundles 而不需要编写自己的详细管理代码，你应该使用 Addressables。

## Adding Addressables to an existing Project

您可以通过安装 Addressables 包在现有 Unity 项目中采用 Addressables。为此，您需要为资产分配地址并重构任何运行时加载代码。有关更多信息，请参阅 [Upgrading to the Addressables system](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsMigrationGuide.html)。

尽管您可以在项目开发的任何阶段集成 Addressables，但 Unity 建议您立即在新项目中开始使用 Addressables，以避免在开发后期进行不必要的代码重构和内容规划更改。

