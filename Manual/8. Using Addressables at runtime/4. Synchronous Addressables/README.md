## Synchronous Workflow

同步可寻址 API 有助于更密切地反映 Unity 资产加载工作流程。 `AsyncOperationHandles`现在有一个称为`WaitForCompletion()`强制异步操作完成并返回`Result`操作的方法。

## API

```csharp
TObject WaitForCompletion()
```

## Result 

`WaitForCompletion`的结果是被调用的异步操作的`Result`。如果操作失败，则返回`default(TObject)`。

当操作没有失败时，可能会得到一个结果`default(TObject)`。`AsyncOperationHandles`在完成时自动释放的异步操作就是这种情况。 即使操作本身成功`Addressables.InitializeAsync()`，任何`autoReleaseHandle`参数设置为 true 的API也会返回`default(TObject)`。

## Performance

值得注意的是`WaitForCompletion`，与直接调用`Resources.Load`或`Instantiate`直接调用相比，调用可能会对您的运行时产生性能影响。如果您`AssetBundle`是本地的或之前已下载并缓存，则这些性能影响可能可以忽略不计。但是，对于您的个人项目设置，情况可能并非如此。

由于在引擎中处理异步操作的方式，当在任何资产加载操作上调用`WaitForCompletion`时，所有当前活动的资产加载操作都会完成。为避免意外停顿，请在当前操作计数已知时使用`WaitForCompletion`，目的是让所有活动操作同步完成。

使用`WaitForCompletion`时，会有性能影响。使用 2021.2.0 或更新版本时，这些是最小的。使用旧版本可能会导致延迟，延迟会随着调用时加载的引擎资产加载调用的数量而增加`WaitForCompletion`。

不建议您调用`WaitForCompletion`将获取和下载远程`AssetBundle`. 但是，如果这适合您的特定情况，那是可能的。

## Code Sample

```csharp
void Start()
{
    //Basic use case of forcing a synchronous load of a GameObject
    var op = Addressables.LoadAssetAsync<GameObject>("myGameObjectKey");
    GameObject go = op.WaitForCompletion();

    //Do work...

    Addressables.Release(op);
}
```

### Synchronous Addressables with Custom Operations

Addressables 支持自定义`AsyncOperations`，它支持`InvokeWaitForCompletion`. 您将使用此可重写方法来实现自定义同步操作。

自定义操作与`ChainOperations`和 一起使用`GroupsOperations`。如果您需要同步完成链式操作，请确保您的自定义操作实现`InvokeWaitForCompletion`并`ChainOperation`使用您的自定义操作创建。同样，`GroupOperations`非常适合确保`AsyncOperations`包括自定义操作在内的集合一起完成。双方`ChainOperation`并`GroupOperation`有自己的实现`InvokeWaitForCompletion`依赖于`InvokeWaitForCompletion`它们依赖于操作的实现。

### WebGL

WebGL 不支持`WaitForCompletion`. 在 WebGL 上，所有文件都是使用 Web 请求加载的。在其他平台上，Web 请求在后台线程上启动，主线程在等待 Web 请求完成时在紧密循环中旋转。这就是 Addressables`WaitForCompletion`在使用 Web 请求时所做的。

由于 WebGL 是单线程的，紧密循环会阻止 Web 请求并且永远不允许操作完成。如果 Web 请求完成它创建的同一帧，`WaitForCompletion`则不会有任何问题。但是，我们不能保证情况确实如此，并且在大多数情况下可能并非如此。