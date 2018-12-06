using System;

namespace Pipeline.Net.MiddlewareResolver
{
    /// <summary>
    ///用于创建中间件实例。
    /// 您可以为首选的依赖注入容器实现此接口。
    /// </summary>
    public interface IMiddlewareResolver
    {
        /// <summary>
        /// 创建给定中间件类型的实例。
        /// </summary>
        /// <param name="type">将创建的中间件类型。</param>
        /// <returns>中间件的一个实例。</returns>
        object Resolve(Type type);
    }
}
