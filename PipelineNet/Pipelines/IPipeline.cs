using Pipeline.Net.Middleware;
using System;

namespace Pipeline.Net.Pipelines
{
    /// <summary>
    /// 管道存储当执行时执行的中间件。 <see cref="Execute(TParameter)"/> 被称为。
    /// 中间件以它们添加的相同顺序执行。
    /// </summary>
    /// <typeparam name="TParameter">将是所有中间件的输入类型。</typeparam>
    public interface IPipeline<TParameter>
    {
        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <returns></returns>
        IPipeline<TParameter> Add<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter>;

        /// <summary>
        /// 执行配置的管道。
        /// </summary>
        /// <param name="parameter">将提供给所有中间件的输入。</param>
        void Execute(TParameter parameter);

        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <param name="middlewareType">要执行的中间件类型。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">如果抛出 <paramref name="middlewareType"/> is 
        /// 不是实现的 <see cref="IMiddleware{TParameter}"/>.</exception>
        /// <exception cref="ArgumentNullException">抛出如果 <paramref name="middlewareType"/>是空的。</exception>
        IPipeline<TParameter> Add(Type middlewareType);
    }
}
