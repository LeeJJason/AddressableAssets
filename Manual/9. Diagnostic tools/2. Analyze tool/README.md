# Analyze tool

分析是一种收集有关项目的可寻址布局信息的工具。在某些情况下，Analyze 可能会采取适当的措施来清理您的项目状态。在其他情况下，Analyze 纯粹是一种信息工具，可让您对 Addressables 布局做出更明智的决定。

## Using Analyze

在编辑器中，打开**Addressables Analyze**窗口（**Window** > **Asset Management** > **Addressables** > **Analyze**），或者通过单击**Tools** > **Window** > **Analyze**按钮通过**Addressables Groups**窗口打开它。

分析窗口显示分析规则列表，以及以下操作：

- Analyze Selected Rules
- Clear Selected Rules
- Fix Selected Rules

### The analyze operation

分析操作收集规则所需的信息。对一条或一组规则运行此操作以收集有关构建、依赖关系映射等的数据。每个规则都必须收集任何所需的数据并将其作为[AnalyzeResult](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.AnalyzeResult.html)对象列表报告回来。

在分析步骤期间，不应采取任何行动来修改任何数据或项目的状态。根据在此步骤中收集的数据，[fix](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html#the-fix-operation)操作可能是适当的操作过程。但是，某些规则仅包含分析步骤，因为无法根据收集到的信息采取合理适当且通用的操作。[Check Scene to Addressable Duplicate Dependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html#check-scene-to-addressable-duplicate-dependencies)和 [Check Resources to Addressable Duplicate Dependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html#check-resources-to-addressable-duplicate-dependencies)是此类规则的示例。

纯粹提供信息且不包含修复操作的规则被归类为**Unfixable Rules**。那些有修复操作的被归类为**Fixable Rules**。

### The clear step

清除操作会删除分析收集的任何数据并相应地更新`TreeView`数据。

### The fix operation

对于**Fixable Rules**，您可以选择运行修复操作。修复操作使用在分析步骤期间收集的数据来执行任何必要的修改并解决问题。

提供的[Check Duplicate Bundle Dependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AnalyzeTool.html#check-duplicate-bundle-dependencies)规则是可修复规则的一个示例。可以修复此规则分析检测到的问题，因为可以采取合理的适当措施来解决这些问题。

## Provided Analyze rules

### Fixable rules

#### Check Duplicate Bundle Dependencies

此规则通过使用[BundledAssetGroupSchemas](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.html)扫描所有组并投影资产组布局来检查潜在的重复资产。这本质上需要触发一个完整的构建，所以这个检查是耗时且性能密集的。

**Issues**：重复资产是由不同组中共享依赖项的资产导致的，例如两个预制件共享存在于不同可寻址组中的材质。该材质（及其任何依赖项）将被拉入包含预制件的两个组中。为了防止这种情况，必须将材质标记为可寻址，或者使用预制件之一，或者在其自己的空间中，从而将材质及其依赖项放在单独的可寻址组中。

**Resolution**：如果此检查发现任何问题，请对此规则运行修复操作以创建一个新的可寻址组，将所有相关资产移至该组。

**Exceptions**：如果您的资产包含多个对象，则不同的组可能只拉入资产的一部分，而实际上并不复制。具有许多网格的 FBX 就是一个例子。如果一个mesh在“GroupA”，另一个在“GroupB”，这个规则会认为FBX是共享的，如果你运行修复操作，就把它提取到自己的组中。在这种极端情况下，运行修复操作实际上是有害的，因为两个组都不会拥有完整的 FBX 资产。

另请注意，重复资产可能并不总是一个问题。如果资产永远不会被同一组用户请求（例如，特定于区域的资产），那么可能需要重复的依赖关系，或者至少是无关紧要的。每个项目都是独一无二的，因此应根据具体情况评估修复重复的资产依赖项。

### Unfixable rules

#### Check Resources to Addressable Duplicate Dependencies

此规则检测在构建的可寻址数据和驻留在`Resources`文件夹中的资产之间是否存在重复的资产或资产依赖关系。

**Issues**：这些重复意味着数据将包含在应用程序构建和可寻址构建中。

**Resolution**：此规则无法修复，因为不存在适当的操作。它纯粹是提供信息，提醒您注意冗余。您必须决定如何进行以及采取何种行动（如果有）。一个可能的手动修复示例是将违规资产移出`Resources`文件夹，并使它们可寻址。

#### Check Scene to Addressable Duplicate Dependencies

此规则检测在编辑器场景列表中的场景和可寻址对象之间共享的任何资产或资产依赖项。

**Issues**：这些重复意味着数据将包含在应用程序构建和可寻址构建中。

**Resolution**：它纯粹是提供信息，提醒您注意冗余。您必须决定如何进行以及采取何种行动（如果有）。可能的手动修复的一个示例是将具有重复引用的内置场景从构建设置中拉出并使其成为可寻址场景。

#### Build Bundle Layout

此规则将显示明确标记为可寻址的资产将如何在可寻址构建中布局。鉴于这些显式资产，我们还展示了哪些资产被构建隐式引用，因此将被拉入构建中。

此规则收集的数据并不表示任何特定问题。它纯粹是信息性的。

## Extending Analyze

除了预先打包的内容之外，每个独特的项目可能需要额外的分析规则。可寻址资产系统允许您创建自己的自定义规则类。

请参阅 [Custom analyze rule project](https://github.com/Unity-Technologies/Addressables-Sample/tree/master/Advanced/CustomAnalyzeRule)在[Addressables-Sample](https://github.com/Unity-Technologies/Addressables-Sample)为例库。

### AnalyzeRule objects

创建[AnalyzerRule](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.html)类的新子类，覆盖以下属性：

- [CanFix](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.CanFix.html#UnityEditor_AddressableAssets_Build_AnalyzeRules_AnalyzeRule_CanFix)告诉分析规则是否被认为是可修复的。
- [ruleName](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.ruleName.html#UnityEditor_AddressableAssets_Build_AnalyzeRules_AnalyzeRule_ruleName)是您将在**Analyze window**中看到的此规则的显示名称。

您还需要覆盖以下方法，详情如下：

- [List RefreshAnalysis（AddressableAssetSettings 设置）](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.RefreshAnalysis.html)
- [void FixIssues（AddressableAssetSettings 设置）](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.FixIssues.html)
- [void ClearAnalysis()](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.ClearAnalysis.html#UnityEditor_AddressableAssets_Build_AnalyzeRules_AnalyzeRule_ClearAnalysis)

**TIP**

*如果您的规则被指定为不可修复，则您不必覆盖该`FixIssues`方法。*

#### RefreshAnalysis

这是您的分析操作。在此方法中，执行您想要的任何计算并缓存您可能需要的任何数据以进行潜在修复。返回值是一个`List<AnalyzeResult>`列表。收集数据后，为分析中的每个条目创建一个新的[AnalyzeResult](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.AnalyzeResult.html)，其中包含数据作为第一个参数的字符串和第二个参数的[MessageType](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.AnalyzeResult.severity.html#UnityEditor_AddressableAssets_Build_AnalyzeRules_AnalyzeRule_AnalyzeResult_severity)（可选择将消息类型指定为警告或错误）。返回您创建的对象列表。

如果您需要`TreeView`为特定的[AnalyzeResult](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.AnalyzeResult.html)对象创建子元素，您可以使用[kDelimiter](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.kDelimiter.html)来描述父项和任何子项。在父项和子项之间包含分隔符。

#### FixIssues

这是您的修复操作。如果要对分析步骤采取适当的操作，请在此处执行。

#### ClearAnalysis

这是你的明确操作。您在分析步骤中缓存的任何数据都可以在此功能中清除或删除。在`TreeView`将更新以反映缺乏数据。

### Adding custom rules to the GUI

自定义规则必须使用 向 GUI 类注册`AnalyzeSystem.RegisterNewRule<RuleType>()`，才能显示在 **Analyze** 窗口中。例如：

#### AnalyzeRule classes

为了更快地设置自定义规则，Addressables 包括以下类，它们继承自[AnalyzeRule](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.html)：

- [BundleRuleBase](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.BundleRuleBase.html)是用于处理[AnalyzeRule](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.html)任务的基类。它包括一些基本方法来检索有关包和资源依赖项的信息。
- **Check bundle duplicates**基类有助于检查包依赖重复。覆盖[FixIssues](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.CheckBundleDupeDependencies.FixIssues.html)方法实现以执行一些自定义操作。
  - [CheckBundleDupeDependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.CheckBundleDupeDependencies.html)继承自[BundleRuleBase](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.BundleRuleBase.html)并包含更多方法让[AnalyzeRule](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.AnalyzeRule.html)检查包依赖项是否存在重复项以及尝试解决这些重复项的方法。
  - [CheckResourcesDupeDependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.CheckResourcesDupeDependencies.html)是相同的，但特定于资源依赖项。
  - [CheckSceneDupeDependencies](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEditor.AddressableAssets.Build.AnalyzeRules.CheckSceneDupeDependencies.html)是相同的，但特定于场景依赖项。

