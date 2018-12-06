using Pipeline.Net.Middleware;
using Pipeline.Net.MiddlewareResolver;
using System;
using System.Threading.Tasks;

namespace Pipeline.Net.Pipelines
{
    public class AsyncPipeline<TParameter> : BaseMiddlewareFlow<IAsyncMiddleware<TParameter>>, IAsyncPipeline<TParameter>
        where TParameter : class
    {
        public AsyncPipeline(IMiddlewareResolver middlewareResolver) : base(middlewareResolver)
        {}

        /// <summary>
        /// 添加要执行的中间件类型。
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <returns></returns>
        public IAsyncPipeline<TParameter> Add<TMiddleware>()
            where TMiddleware : IAsyncMiddleware<TParameter>
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
        public IAsyncPipeline<TParameter> Add(Type middlewareType)
        {
            base.AddMiddleware(middlewareType);
            return this;
        }

        /// <summary>
        /// 执行配置的管道。
        /// </summary>
        /// <param name="parameter"></param>
        public async Task Execute(TParameter parameter)
        {
            if (MiddlewareTypes.Count == 0)
                return;

            int index = 0;
            Func<TParameter, Task> action = null;
            action = async (param) =>
            {
                var type = MiddlewareTypes[index];
                var firstMiddleware = (IAsyncMiddleware<TParameter>)MiddlewareResolver.Resolve(type);

                index++;
                if (index == MiddlewareTypes.Count)
                    action = async (p) => { };

                await firstMiddleware.Run(param, action);
            };

            await action(parameter);
        }
    }
}
