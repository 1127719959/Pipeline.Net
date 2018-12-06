using Pipeline.Net.Middleware;
using Pipeline.Net.MiddlewareResolver;
using System;

namespace Pipeline.Net.Pipelines
{
    /// <summary>
    /// 管道存储当执行时执行的中间件。 <see cref="Execute(TParameter)"/>被称为。
    /// 中间件以它们添加的相同顺序执行。
    /// </summary>
    /// <typeparam name="TParameter">将是所有中间件的输入类型。</typeparam>
    public class Pipeline<TParameter> : BaseMiddlewareFlow<IMiddleware<TParameter>>, IPipeline<TParameter>
    {
        /// <summary>
        /// 创建管道的新实例。
        /// </summary>
        /// <param name="middlewareResolver">Resolver responsible for resolving instances out of middleware types.</param>
        public Pipeline(IMiddlewareResolver middlewareResolver) : base(middlewareResolver)
        {}

        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <returns></returns>
        public IPipeline<TParameter> Add<TMiddleware>()
            where TMiddleware : IMiddleware<TParameter>
        {
            MiddlewareTypes.Add(typeof(TMiddleware));
            return this;
        }

        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <param name="middlewareType">要执行的中间件类型。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">如果抛出 <paramref name="middlewareType"/> is 
        /// 不是实现的 <see cref="IMiddleware{TParameter}"/>.</exception>
        /// <exception cref="ArgumentNullException">抛出如果 <paramref name="middlewareType"/>是空的。</exception>
        public IPipeline<TParameter> Add(Type middlewareType)
        {
            base.AddMiddleware(middlewareType);
            return this;
        }

        /// <summary>
        /// 执行配置的管道。
        /// </summary>
        /// <param name="parameter">将提供给所有中间件的输入。</param>
        public void Execute(TParameter parameter)
        {
            if (MiddlewareTypes.Count == 0)
                return;

            int index = 0;
            Action<TParameter> action = null;
            action = (param) =>
            {
                var type = MiddlewareTypes[index];
                var middleware = (IMiddleware<TParameter>)MiddlewareResolver.Resolve(type);

                index++;
                if (index == MiddlewareTypes.Count)
                    action = (p) => { };

                middleware.Run(param, action);
            };

            action(parameter);
        }
    }
}