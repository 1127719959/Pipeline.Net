using Pipeline.Net.Middleware;
using System;
using System.Threading.Tasks;

namespace Pipeline.Net.Pipelines
{
    public interface IAsyncPipeline<TParameter>
    {
        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <returns></returns>
        IAsyncPipeline<TParameter> Add<TMiddleware>()
            where TMiddleware : IAsyncMiddleware<TParameter>;

        /// <summary>
        /// 执行配置的管道。
        /// </summary>
        /// <param name="parameter"></param>
        Task Execute(TParameter parameter);

        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <param name="middlewareType">要执行的中间件类型。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">如果抛出 <paramref name="middlewareType"/> is 
        /// 不是实现的 <see cref="IMiddleware{TParameter}"/>.</exception>
        /// <exception cref="ArgumentNullException">抛出如果 <paramref name="middlewareType"/>是空的。</exception>
        IAsyncPipeline<TParameter> Add(Type middlewareType);
    }
}
