# Loading from Multiple Projects

如果您的情况需要多项目工作流，例如一个大型项目被拆分为安装了 Addressables 包的多个 Unity 项目，我们必须[`Addressables.LoadContentCatalogAsync`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadContentCatalogAsync.html)将各个项目的代码和内容链接在一起。拥有同时处理应用程序多个方面的团队的工作室可能会从此工作流程中受益。

## Setting up multiple projects

多项目设置需要注意的主要事项是确保：

1. 每个项目使用相同版本的Unity Editor
2. 每个项目使用相同版本的 Addressables 包

从那里项目可以包含您认为适合您给定情况的任何内容。您的项目之一必须是您的“main project”或“source project”。这是您实际构建和部署游戏二进制文件的项目。通常，此源项目主要由代码组成，几乎没有内容。您希望在主要项目中的主要内容至少是引导场景。在任何 AssetBundle 有机会被下载和缓存之前，包含出于性能目的需要本地化的任何场景也可能是可取的。

在大多数情况下，二级项目正好相反。主要是内容，几乎没有代码。**These projects need to have all remote Addressable Groups and Build Remote Catalog turned on**。这些项目中内置的任何本地数据都无法加载到源项目的应用程序中。非关键场景可以存在于这些项目中，并在需要时由主项目下载。

#### The Typical Workflow

项目设置完成后，工作流程通常如下：

1. 为所有二级项目构建远程内容
2. 为源项目构建可寻址内容
3. 启动源项目播放模式或构建源项目二进制文件
4. 在源项目中，用于[`Addressables.LoadContentCatalogAsync`](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadContentCatalogAsync.html)加载其他各种项目的远程目录
5. 像往常一样继续游戏运行时。现在加载了目录，Addressables 能够从这些位置中的任何一个加载资产。

关于源项目，应该注意的是，使用该项目在本地构建一些最少量的内容可能是值得的。每个项目都是独一无二的，因此有独特的需求，但在互联网连接问题或其他各种问题的情况下，拥有一小部分内容来运行您的游戏可能是可取的。

#### Handling Shaders

Addressables 为构建的每组 Addressables 播放器数据构建一个 Unity 内置着色器包。这意味着当加载多个内置于辅助项目中的 AssetBundle 时，可能会同时加载多个内置着色器包。

根据您的具体情况，您可能需要在`AddressableAssetSettings`对象上使用 Shader Bundle Naming Prefix 。每个内置着色器包的名称都需要与其他项目中内置的其他包不同。如果它们的名称不同，则会`The AssetBundle [bundle] can't be loaded because another AssetBundle with the same files is already loaded.`出现错误。