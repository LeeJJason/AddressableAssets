# Build scripting

您可以通过多种方式使用 Addressables API 来自定义您的项目构建：

- 从脚本开始构建
- 覆盖现有脚本
- 扩展[BuildScriptBase](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.DataBuilders.BuildScriptBase.html)或实现[IDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)

当您自定义构建脚本以处理不同的资产类型或以不同的方式处理资产时，您可能还需要自定义[Play Mode Scripts](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/Groups.html#play-mode-scripts)以便编辑器可以在播放模式下以相同的方式处理这些资产。

### Starting an Addressables build from a script

要从另一个脚本启动 Addressables 构建，请调用[AddressableAssetSettings.BuildPlayerContent](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent.html)方法。

在开始构建之前，您应该设置激活[Profile](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsProfiles.html) 和激活构建脚本。如果需要，您还可以设置与默认值不同的[AddressableAssetSettings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)对象。

BuildPlayerContent 在执行构建时会考虑一些信息：[AddressableAssetSettingsDefaultObject](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.html)、[ActivePlayerDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.ActivePlayerDataBuilder.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_ActivePlayerDataBuilder)和`addressables_content_state.bin`文件。

#### Set the AddressableAssetSettings

[AddressableAssetSettings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)定义的设置包括组列表和要使用的配置文件。

要访问您在编辑器中看到的设置（菜单：**Window > Asset Management > Addressables > Settings**），请使用静态[AddressableAssetSettingsDefaultObject.Settings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.html#UnityEditor_AddressableAssets_AddressableAssetSettingsDefaultObject_Settings)属性。但是，如果需要，您可以为构建使用不同的设置对象。

要在构建中加载自定义设置对象：

#### Set the active Profile

以 BuildContent 开始的构建使用活动配置文件的变量设置。要将活动配置文件设置为自定义构建脚本的一部分，请将所需配置文件的 ID 分配给[AddressableAssetSettingsDefaultObject.Settings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.html#UnityEditor_AddressableAssets_AddressableAssetSettingsDefaultObject_Settings)对象的[activeProfileId](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.activeProfileId.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_activeProfileId)字段。

该[AddressableAssetSettings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)对象包含配置文件的列表。使用所需配置文件的名称查找其 ID 值，然后将 ID 分配给[activeProfileId](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.activeProfileId.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_activeProfileId)变量：

#### Set the active build script

BuildContent 方法基于当前的[ActivePlayerDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.ActivePlayerDataBuilder.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_ActivePlayerDataBuilder)设置启动构建。要使用特定的构建脚本，请将[AddressableAssetSetting.DataBuilders](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.DataBuilders.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_DataBuilders)列表中IDataBuilder 对象的索引分配给[ActivePlayerDataBuilderIndex](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.ActivePlayerDataBuilderIndex.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_ActivePlayerDataBuilderIndex)属性。

构建脚本必须是实现[IDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)的 ScriptableObject，您必须将其添加到[AddressableAssetSettings](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.html)实例中的[DataBuilders](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.DataBuilders.html#UnityEditor_AddressableAssets_Settings_AddressableAssetSettings_DataBuilders)列表中。添加到列表后，使用标准[List.IndexOf](https://docs.microsoft.com/dotnet/api/system.collections.generic.list-1.indexof)方法获取对象的索引。

#### Launch a build

设置要使用的配置文件和构建器后（如果需要），您可以启动构建：

要检查是否成功，请使用 BuildPlayerContent(out AddressablesPlayerBuildResult 结果)。result.Error 包含在 Addressables 构建失败时返回的任何错误消息。如果 string.IsNullOrEmpty(result.Error) 为 true，则构建成功。

#### Example script to launch build

以下示例向编辑器中的 Asset Management > Addressables 菜单添加了几个菜单命令。第一个命令使用预设配置文件和构建脚本构建可寻址内容。第二个命令构建可寻址内容，如果成功，也构建播放器。

请注意，如果您的构建脚本进行了需要重新加载域的设置更改，您应该使用 Unity 命令行选项运行构建脚本，而不是在编辑器中以交互方式运行它。有关更多信息，请参阅 [Domain reloads and Addressable builds](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/BuildPlayerContent.html#domain-reloads-and-addressables-builds)。

#### Domain reloads and Addressables builds

如果您的脚本化构建过程涉及在进行可寻址构建之前更改触发域重新加载的设置，那么您应该编写此类构建以使用 Unity 编辑器 [command line interface](https://docs.unity3d.com/2019.4/Documentation/Manual/CommandLineArguments.html)而不是在编辑器中以交互方式运行脚本。这些类型的设置包括：

- 更改定义的编译器符号
- 改变平台目标或目标群体

当您在编辑器中以交互方式运行触发域重新加载的脚本时（例如，使用菜单命令），您的编辑器脚本会在域重新加载发生之前完成执行。因此，如果您立即开始 Addressables 构建，您的代码和导入的资产仍处于原始状态。在开始内容构建之前，您必须等待域重新加载完成。

当您从命令行运行构建时，等待域重新加载完成相对简单，但在交互式脚本中很难或不可能可靠地完成（出于多种原因）。

以下示例脚本定义了在命令行上运行 Unity 时可以调用的两个函数。该`ChangeSettings`示例设置指定的定义符号。该`BuildContentAndPlayer`函数运行可寻址构建和播放器构建。

要调用这些函数，请在终端或命令提示符或 shell 脚本中使用[Unity's command line arguments](https://docs.unity3d.com/2019.4/Documentation/Manual/CommandLineArguments.html)：

```
D:\Unity\2020.3.0f1\Editor\Unity.exe -quit -batchMode -projectPath . -executeMethod BatchBuild.ChangeSettings -defines=FOO;BAR -buildTarget Android
D:\Unity\2020.3.0f1\Editor\Unity.exe -quit -batchMode -projectPath . -executeMethod BatchBuild.BuildContentAndPlayer -buildTarget Android
```

**NOTE**

*如果将平台目标指定为命令行参数，则可以在同一命令中执行可寻址构建。但是，如果您想在脚本中更改平台，则应在单独的命令中进行，例如`ChangeSettings`本示例中的函数。*

### Overriding an existing script

如果您想使用与默认相同的基本构建，但希望以不同方式处理特定的资产组或类型，您可以扩展默认构建脚本并覆盖其中的功能。如果构建脚本正在处理的组或资产是您想要区别对待的组或资产，您可以运行自己的代码，否则您可以调用函数的基类版本以使用默认算法。

请参阅 [Addressable variants project](https://github.com/Unity-Technologies/Addressables-Sample/tree/master/Advanced/Addressable Variants)在 [Addressables-Sample](https://github.com/Unity-Technologies/Addressables-Sample)为例库。

### Extending BuildScriptBase or implementing IDataBuilder

您可以扩展[BuildScriptBase](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.DataBuilders.BuildScriptBase.html)或实现[IDataBuilder](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.IDataBuilder.html)以大幅更改可寻址构建系统。要了解 Addressables 系统如何构建内容，首先检查默认构建脚本 ，`BuildScriptPackedMode.cs`您可以在 Addressables 包文件夹 Addressables/EditorBuild/DataBuilders 中找到该脚本。

#### Save the content state

如果您支持 [remote content distribution](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/RemoteContentDistribution.html)并在播放器版本之间更新您的内容，则必须在构建时记录 Addressables 组的状态。记录状态允许您使用[Update a Previous Build](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/ContentUpdateWorkflow.html#building-content-updates)脚本执行差异构建。

有关`BuildScriptPackedMode.cs`详细信息，请参阅默认构建脚本的实现。

