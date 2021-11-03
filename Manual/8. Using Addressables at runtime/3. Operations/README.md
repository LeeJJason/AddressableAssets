# Operations

Addressables 中的许多任务需要加载或下载信息才能返回结果。为了避免阻塞程序执行，Addressables 实现了异步操作等任务。

与同步操作（在结果可用之前不返回控制权）相反，异步操作几乎立即将控制权返回给调用函数。但是，结果可能要到未来的某个时间才能获得。当您调用函数（例如[LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html))时，它不会直接返回加载的资产。相反，它返回一个[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)对象，您可以使用它在加载的资产可用时访问它们。

您可以使用以下技术来等待异步操作的结果（同时允许其他脚本继续处理）。

- [Coroutines and IEnumerator loops](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#coroutine-operation-handling)
- [Events](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#event-based-operation-handling)
- [Tasks](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#task-based-operation-handling)

**NOTE**

*您可以阻塞当前线程以等待异步操作完成。这样做会带来性能问题和帧速率问题。请参阅 [Using operations synchronously](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#using-operations-synchronously)。*

## Releasing AsyncOperationHandle instances

方法（如 [LoadAssetsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync.html)）返回[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)实例，这些实例既提供操作结果，又提供释放结果和操作对象本身的方法。只要您想使用结果，就必须保留句柄对象。根据情况，这可能是一帧，直到级别结束，甚至应用程序的生命周期。使用[Addressables.Release](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.Release.html)函数释放操作句柄和任何关联的可寻址资产。

释放操作句柄会减少操作加载的任何资产的引用计数，并使操作句柄对象本身无效。有关可寻址系统中引用计数的更多信息，请参阅[Memory management](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/MemoryManagement.html)。

如果您不需要使用超出有限范围的操作结果，您可以立即释放句柄。一些 Addressables 方法，例如[UnloadSceneAsync，](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync.html)允许您在操作完成时自动释放操作句柄。

如果操作不成功，仍应释放操作句柄。通常，Addressables 会释放在失败操作期间加载的任何资产，但释放句柄仍会清除句柄的实例数据。请注意，某些函数（例如[LoadAssetsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync.html)会加载多个资产）让您可以选择保留它可以加载的任何资产，或者在加载操作的任何部分失败时失败并释放所有内容。

## Coroutine- and IEnumerator-based operation handling

该[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)实现[了IEnumerator](https://docs.microsoft.com/dotnet/api/system.collections.ienumerator)接口，并会继续反复，直到操作完成。在协程中，您可以生成操作句柄以等待下一次迭代。完成后，执行流程将继续执行以下语句。回想一下，您可以将[MonoBehaviour Start](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html)函数实现为协程，这是加载 GameObject 并实例化所需资产的好方法。

以下脚本使用 Start 函数协程加载 Prefab 作为其 GameObject 的子项。它产生 AsyncOperationHandle 直到操作完成，然后使用相同的句柄来实例化 Prefab。

有关更多信息，请参阅[Coroutines](https://docs.unity3d.com/2019.4/Documentation/Manual/Coroutines.html)。

### Grouping operations in a coroutine

在进入游戏逻辑的下一步之前，您可能会遇到想要执行多个操作的情况。例如，您想在开始关卡之前加载许多预制件和其他资产。

如果操作都加载资产，您可以将它们与对[Addressables.LoadAssetsAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync.html)函数的单个调用结合起来。此方法的 AsyncOperationhandle 与[LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html) 的工作方式相同；您可以在协程中产生句柄以等待操作中的所有资产加载。此外，您可以将回调函数传递给 LoadAssetsAsync，并且该操作在完成加载特定资产时调用该函数。有关示例，请参阅[Loading multiple assets](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/LoadingAddressableAssets.html#loading-multiple-assets)。

另一种选择是使用[ResourceManager.CreateGenericGroupOperation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.CreateGenericGroupOperation.html)创建一个组操作，该操作在其所有成员完成时完成。

## Event-based operation handling

您可以向[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)的[Completed](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Completed.html)事件添加委托函数。该操作在完成时调用委托函数。

以下脚本执行与 [Coroutine- and IEnumerator-based operation handling](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/AddressableAssetsAsyncOperationHandle.html#coroutine-operation-handling)的示例相同的功能，但使用事件委托而不是协程。

请注意，传递给事件委托的句柄实例与原始函数调用返回的句柄实例相同。您可以使用两者之一来访问操作的结果和状态，并最终释放操作句柄和加载的资产。

## Task-based operation handling

该[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)提供了一个[Task](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Task.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Task) 对象，您可以用C＃中 [async](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/async) 和[await](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/await)关键字序列代码调用异步函数并处理结果。

以下示例使用键列表加载可寻址资产。这种基于任务的方法与协程或基于事件的方法之间的区别在于调用函数的签名，它必须包含[async](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/async)关键字以及使用带有操作句柄的 Task 属性的[await](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/await)关键字。在这种情况下，调用函数 Start() 在任务完成时暂停操作。然后继续执行并且示例实例化所有加载的预制件（以网格模式）。

**IMPORTANT**

*该[AsyncOperationHandle.Task](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Task.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Task)属性不可用Unity WebGL的平台，不支持多任务处理上。*

当您使用基于任务的操作处理时，您可以使用诸如[WhenAll ](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.whenall)之类的 C#[任务](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Task.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Task)类方法来控制您并行运行哪些操作以及您希望顺序运行哪些操作。以下示例说明了如何在继续下一个任务之前等待多个操作完成：

## Using operations synchronously

您可以等待操作完成而不 yielding, waiting事件或通过调用操作的[WaitForCompletion](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.WaitForCompletion.html)方法使用`async await`。此方法在等待操作完成时阻止当前程序执行线程，然后再在当前范围内继续。

避免对可能需要大量时间的操作（例如必须下载数据的操作）调用 WaitForCompletion。调用 WaitForCompletion 可能会导致帧故障并中断 UI 响应。

在 Unity 2020.1 或更早版本中，Unity 还会等待所有其他挂起的异步操作完成，因此执行延迟可能比调用此方法的单个操作所需的延迟要长得多。在 Unity 2020.2 或更高版本中，性能影响可能不那么明显，至少在加载已下载的资产时是这样。

以下示例按地址加载一个 Prefab 资源，等待操作完成，然后实例化 Prefab：

## Custom operations

要创建自定义操作，请扩展[AsyncOperationBase](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase-1.html)类并覆盖其虚拟方法。

您可以将派生操作传递给[ResourceManager.StartOperation](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.StartOperation.html)方法以启动操作并接收[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)结构。该[ResourceManager](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.html)的寄存器操作开始这个并且显示他们在Addressables [Event Viewer](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/EventViewer.html)。

### Executing the operation

一旦可选的依赖操作完成，[ResourceManager ](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.html)就会为您的自定义操作调用[AsyncOperationBase.Execute](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase-1.Execute.html)方法。

### Completion handling

自定义操作完成后，对自定义操作对象调用[AsyncOperationBase.Complete](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase-1.Complete.html)。您可以在[Execute](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase-1.Execute.html)方法中调用它或将其推迟到调用之外。AsyncOperationBase.Complete 通知[ResourceManager](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.html)操作已完成。ResourceManager为自定义操作的相关实例调用关联的[AsyncOperationHandle.Completed](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Completed.html)事件。

### Terminating the operation

当您释放引用它的[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)时，[ResourceManager ](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.ResourceManager.html)会为您的自定义操作调用[AsyncOperationBase.Destroy](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationBase-1.Destroy.html)方法。您应该在此处释放与自定义操作相关的任何内存或资源。

## Using typed versus typeless operation handles

大多数启动操作的[Addressables](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.html)方法返回一个通用的[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle-1.html)结构，允许[AsyncOperationHandle.Completed](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Completed.html)事件和[AsyncOperationHandle.Result](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.Result.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_Result)对象的类型安全。您还可以使用非通用[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)结构并根据需要在两种句柄类型之间进行转换。

请注意，如果您尝试将非泛型句柄强制转换为类型不正确的泛型句柄，则会发生运行时异常。例如：

## Reporting operation progress

[AsyncOperationHandle](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.html)有两个方法可用于监视和报告操作的进度：

- [GetDownloadStatus](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.GetDownloadStatus.html)返回一个[DownloadStatus](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.DownloadStatus.html)结构。此结构包含有关已下载多少字节以及仍需下载多少字节的信息。该[DownloadStatus.Percent](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.DownloadStatus.Percent.html#UnityEngine_ResourceManagement_AsyncOperations_DownloadStatus_Percent)报告下载字节数的百分比。
- [AsyncOperationHandle.PercentComplete](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle.PercentComplete.html#UnityEngine_ResourceManagement_AsyncOperations_AsyncOperationHandle_PercentComplete)返回所有已完成子操作的加权聚合百分比。例如，如果一个工序有五个子工序，则每个子工序占总数的 20%。该值不考虑各个子操作必须下载的数据量。

例如，如果您调用[Addressables.DownloadDependenciesAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync.html)并且需要下载五个 AssetBundle，GetDownloadStatus 会告诉您到目前为止已下载的所有子操作的总字节数的百分比。PercentComplete 会告诉您已完成的操作数量的百分比，而不管它们的大小。

另一方面，如果您调用[LoadAssetAsync](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/api/UnityEngine.AddressableAssets.Addressables.LoadAssetAsync.html)，并且必须先下载一个包才能从中加载资产，则下载百分比可能会产生误导。从 GetDownloadStatus 获取的值会在操作完成之前达到 100%，因为该操作还有额外的子操作要执行。当下载子操作完成时，PercentComplete 的值为 50%，当实际加载到内存中完成时，PercentComplete 的值为 100%。