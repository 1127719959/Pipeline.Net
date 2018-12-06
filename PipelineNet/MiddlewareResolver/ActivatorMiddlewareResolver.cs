using System;

namespace Pipeline.Net.MiddlewareResolver
{
    /// <summary>
    /// 默认实现 <see cref="IMiddlewareResolver"/> that creates
    /// 实例使用 <see cref="System.Activator"/>.
    /// </summary>
    public class ActivatorMiddlewareResolver : IMiddlewareResolver
    {
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
