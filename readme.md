# Pipeline.Net
***
## Pipeline.Net是一个微框架，可帮助您实现管道和责任链模式。您可以轻松分离业务逻辑并扩展您的应用程序。管道可用于顺序执行一系列中间件而返回，而责任链则执行相同的操作但有返回。你也可以异步执行。

## 引用方式
```Install-Package Pipeline.Net -Version 1.0.0```
## 案例
使用Pipeline.Net很容易，这里有一个使用责任链的异常处理示例：

首先我们定义一些中间件：
```
public class OutOfMemoryExceptionHandler : IMiddleware<Exception, bool>
{
    public bool Run(Exception parameter, Func<Exception, bool> next)
    {
        if(parameter is OutOfMemoryException)
        {
            //以某种方式处理
            return true;
        }
        return next(parameter);
    }
}
public class ArgumentExceptionHandler : IMiddleware<Exception, bool>
{
    public bool Run(Exception parameter, Func<Exception, bool> next)
    {
        if(parameter is ArgumentException)
        {
            //以某种方式处理
            return true;
        }
        return next(parameter);
    }
}
```
现在我们用中间件创建一个责任链：
```
var exceptionHandlersChain = new ResponsibilityChain<Exception, bool>(new ActivatorMiddlewareResolver())
    .Chain<OutOfMemoryExceptionHandler>() //中间件按照添加顺序执行
    .Chain<ArgumentExceptionHandler>();
```
现在您的实例ResponsibilityChain可以根据需要执行多次：

```
// 下面的行只执行OutOfMeMyExeExtActudiAtter，这是第一个中间件。
var result = exceptionHandlersChain.Execute(new OutOfMemoryException()); //结果将是真

// 这一个将首先执行OutOfMemoryExceptionHandler，然后执行ArgumentExceptionHandler。
result = exceptionHandlersChain.Execute(new ArgumentExceptionHandler()); // 结果将是真

// 如果没有匹配的中间件返回一个值，则返回返回类型的默认值，在“bool”的情况下，返回类型为false。
result = exceptionHandlersChain.Execute(new InvalidOperationException()); //结果将是假
```
您甚至可以在所有中间件执行结束后添加函数处理返回结果

```
var exceptionHandlersChain = new ResponsibilityChain<Exception>(new ActivatorMiddlewareResolver())
    .Chain<OutOfMemoryExceptionHandler>() // 中间件按照添加顺序执行
    .Chain<ArgumentExceptionHandler>()
    .Finally((parameter) =>
    {
        //做某事
        return true;
    })
```
现在，如果执行相同的行：

```
var result = exceptionHandlersChain.Execute(new InvalidOperationException()); // 结果将是真
```
由于Finally方法中定义的函数，结果将为真。

## 管道与责任链
以下是PipelineNet中这两者之间的区别：

* 责任链：
   * 有返回值;
   * 有一个后备函数在链的末尾执行;
   * 当您希望只根据输入执行一个中间件时使用，例如异常处理示例;
* 管道：
 * 没有返回值;
 * 当您想要在输入上执行各种中间件时使用，例如在图像上进行过滤;
 
## 中间件
在Pipeline.Net中，中间件是一段代码的定义，它将在管道或责任链中执行。

我们有四个定义中间件的接口：

* ```IMiddleware<TParameter>```专门用于没有返回值的管道。
* ```IAsyncMiddleware<TParameter>```与上述相同但用于异步管线。
* ```IMiddleware<TParameter, TReturn>```专门用于责任链，它具有返回值。
* ```IAsyncMiddleware<TParameter, TReturn>```与上述相同但用于责任的异步链。


除了那些4种中间件之间的差别，它们都具有类似的结构中，他们都的定义Run函数 ，其中，第一参数是传递进来的参数，第二个是一个Action的方法用于执行流程中的下一个中间件。重要的是要记住通过执行传入的函数，函数作为第二个参数来调用下一个中间件。

## 管道
管道有两种：Pipeline<TParameter>和AsyncPipeline<TParameter>。两者具有相同的功能，聚合并执行一系列中间件。

以下是使用三种中间件类型配置的管道示例：

```
var pipeline = new Pipeline<Bitmap>(new ActivatorMiddlewareResolver())
    .Add<RoudCornersMiddleware>()
    .Add<AddTransparencyMiddleware>()
    .Add<AddWatermarkMiddleware>();
```
从现在开始，管道实例可用于在任意数量的Bitmap实例上执行相同的操作：

```
Bitmap image1 = (Bitmap) Image.FromFile("party-photo.png");
Bitmap image2 = (Bitmap) Image.FromFile("marriage-photo.png");
Bitmap image3 = (Bitmap) Image.FromFile("matrix-wallpaper.png");

pipeline.Execute(image1);
pipeline.Execute(image2);
pipeline.Execute(image3);
```
如果您愿意，可以使用异步版本，使用异步中间件。将实例化更改为：
```
var pipeline = new AsyncPipeline<Bitmap>(new ActivatorMiddlewareResolver())
    .Add<RoudCornersAsyncMiddleware>()
    .Add<AddTransparencyAsyncMiddleware>()
    .Add<AddWatermarkAsyncMiddleware>();
```
并且可以优化使用情况：
```
Bitmap image1 = (Bitmap) Image.FromFile("party-photo.png");
Task task1 = pipeline.Execute(image1); //你也可以简单地使用 "await pipeline.Execute(image1);"

Bitmap image2 = (Bitmap) Image.FromFile("marriage-photo.png");
Task task2 = pipeline.Execute(image2);

Bitmap image3 = (Bitmap) Image.FromFile("matrix-wallpaper.png");
Task task3 = pipeline.Execute(image3);

Task.WaitAll(new Task[]{ task1, task2, task3 });
```
## 责任链
责任链还有两个实现：ResponsibilityChain<TParameter, TReturn>和AsyncResponsibilityChain<TParameter, TReturn>。两者具有相同的功能，聚合并执行一系列检索返回类型的中间件。

与管道相比，链责任的一个区别是可以使用该Finally方法定义的回退功能。您可以为责任链设置一个函数，多次调用该方法将替换之前定义的函数。

由于我们已经有一个责任链的例子，这里有一个使用异步实现的例子：如果你愿意，你可以使用异步版本，使用异步中间件。将实例化更改为：

```
var exceptionHandlersChain = new AsyncResponsibilityChain<Exception>(new ActivatorMiddlewareResolver())
    .Chain<OutOfMemoryExceptionHandler>() // 中间件的链接顺序
    .Chain<ArgumentExceptionHandler>()
    .Finally((ex) =>
        {
            ex.Source = ExceptionSource;
            return Task.FromResult(true);
        });
```
这是执行：
```
// 下面的行只执行OutOfMeMyExeExtActudiAtter，这是第一个中间件。
bool result = await exceptionHandlersChain.Execute(new OutOfMemoryException()); // 结果将是真

// 这一个将首先执行OutOfMemoryExceptionHandler，然后执行ArgumentExceptionHandler。
result = await exceptionHandlersChain.Execute(new ArgumentExceptionHandler()); // 结果将是真

// 如果没有匹配的中间件返回一个值，则返回返回类型的默认值，在“bool”的情况下，返回类型为false。
result = await exceptionHandlersChain.Execute(new InvalidOperationException()); //结果将是假
```
## 中间件解析器
您可能想知道，这个ActivatorMiddlewareResolver是什么，他是类被传递给管道和责任链的每个实例。这是它的默认实现，IMiddlewareResolver用于创建中间件类型的实例。
在配置管道/责任链时，您需要定义中间件的类型，当流程执行时，需要实例化这些中间件，因此IMiddlewareResolver负责。您甚至可以创建自己的实现，因为 ActivatorMiddlewareResolver它只适用于无参数构造函数。

