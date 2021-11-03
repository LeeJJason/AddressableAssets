# Continuous integration

您可以使用持续集成 (CI) 系统来执行可寻址内容构建和应用程序播放器构建。本节提供了使用 CI 系统构建 Addressables 的一般指南，但请注意，每个项目都有自己的要求和约束，因此某些指南可能并不适用于所有情况。

## Selecting a content builder

构建可寻址内容时的主要选择之一是选择内容构建器。默认情况下，如果您调用[AddressableAssetSettings.BuildPlayerContent()](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_BuildPlayerContent)，它会将`BuildScriptPackedMode`脚本用作[IDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)实例。该`BuildPlayerContent()`函数检查[ActivePlayerDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.ActivePlayerDataBuilder.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_ActivePlayerDataBuilder)设置并调用该脚本的`BuildDataImplementation(..)`

如果您已经实现了自己的自定义[IDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)并希望将其用于 CI 构建，请设置[AddressableAssetSettings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)的[ActivePlayerDataBuilderIndex](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.ActivePlayerDataBuilderIndex.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_ActivePlayerDataBuilderIndex)属性。默认情况下，您可以通过[AddressableAssetSettingsDefaultObject.Settings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.html#UnityEditor_AddressableAssets_AddressableAssetSettingsDefaultObject_Settings)访问正确的设置实例。该索引指的是`IDataBuilder` 在[AddressableAssetSettings.DataBuilders](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.DataBuilders.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_DataBuilders)列表中的位置。以下代码示例演示了如何设置自定义：`IDataBuilder`

## Cleaning the Addressables content builder cache

[IDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)实现定义了一个[ClearCachedData()](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.ClearCachedData.html#UnityEditor_AddressableAssets_Build_IDataBuilder_ClearCachedData)方法，该方法清除该数据构建器创建的任何文件。例如，默认`BuildScriptPackedMode`脚本删除以下内容：

- 内容目录
- 序列化设置文件
- 构建的 AssetBundles
- 创建的任何 link.xml 文件

您可以将调用`IDataBuilder.ClearCachedData()`作为 CI 过程的一部分，以确保构建不使用先前构建生成的文件。

## Cleaning the Scriptable Build Pipeline cache

清除可编写脚本的构建管道 (SBP) 缓存会清除目录中的`BuildCache`文件夹`Library`以及构建和类型数据库生成的所有哈希映射。该`Library/BuildCache`文件夹包含`.info`SBP 在构建期间创建的文件，通过从这些`.info`文件读取数据而不是重新生成未更改的数据来加速后续构建。

要在不打开提示对话框的情况下清除脚本中的 SBP 缓存，请调用[BuildCache.PurgeCache(false)](https://docs.unity3d.com/Packages/com.unity.scriptablebuildpipeline@1.19/api/UnityEditor.Build.Pipeline.Utilities.BuildCache.html#UnityEditor_Build_Pipeline_Utilities_BuildCache_PurgeCache_)。