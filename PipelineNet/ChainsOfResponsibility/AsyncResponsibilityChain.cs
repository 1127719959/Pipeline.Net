using System;
using System.Threading.Tasks;
using Pipeline.Net.Middleware;
using Pipeline.Net.MiddlewareResolver;

namespace Pipeline.Net.ChainsOfResponsibility
{
    /// <summary>
    /// 定义异步的责任链。
    /// </summary>
    /// <typeparam name="TParameter">链的输入类型。</typeparam>
    /// <typeparam name="TReturn">链的返回类型。</typeparam>
    public class AsyncResponsibilityChain<TParameter, TReturn> : BaseMiddlewareFlow<IAsyncMiddleware<TParameter, TReturn>>,
        IAsyncResponsibilityChain<TParameter, TReturn>
    {
        private Func<TParameter, Task<TReturn>> _finallyFunc;

        /// <summary>
        /// 创建一个新的异步责任链。
        /// </summary>
        /// <param name="middlewareResolver">用于创建中间件类型的解析器。</param>
        public AsyncResponsibilityChain(IMiddlewareResolver middlewareResolver) : base(middlewareResolver)
        {
        }

        /// <summary>
        /// 为责任链链添加一个新的中间件。
        /// 中间件将按照它们添加的顺序执行。
        /// </summary>
        /// <typeparam name="TMiddleware">正在添加新的中间件。</typeparam>
        /// <returns>当前实例 <see cref="IAsyncResponsibilityChain{TParameter, TReturn}"/>.</returns>
        public IAsyncResponsibilityChain<TParameter, TReturn> Chain<TMiddleware>() where TMiddleware : IAsyncMiddleware<TParameter, TReturn>
        {
            MiddlewareTypes.Add(typeof(TMiddleware));
            return this;
        }

        /// <summary>
        /// 将新的中间件类型链接到责任链。
        /// 中间件将按照它们添加的顺序执行。
        /// </summary>
        /// <param name="middlewareType">要执行的中间件类型。</param>
        /// <exception cref="ArgumentException">如果抛出<paramref name="middlewareType"/> 
        /// 是因为没有实现 <see cref="IAsyncMiddleware{TParameter, TReturn}"/>.</exception>
        /// <exception cref="ArgumentNullException">如果抛出 <paramref name="middlewareType"/> 是空的.</exception>
        /// <returns>当前实例 <see cref="IAsyncResponsibilityChain{TParameter, TReturn}"/>.</returns>
        public IAsyncResponsibilityChain<TParameter, TReturn> Chain(Type middlewareType)
        {
            base.AddMiddleware(middlewareType);
            return this;
        }

        /// <summary>
        /// 执行配置的责任链。
        /// </summary>
        /// <param name="parameter"></param>
        public async Task<TReturn> Execute(TParameter parameter)
        {
            if (MiddlewareTypes.Count == 0)
                return default(TReturn);

            int index = 0;
            Func<TParameter, Task<TReturn>> func = null;
            func = (param) =>
            {
                var type = MiddlewareTypes[index];
                var middleware = (IAsyncMiddleware<TParameter, TReturn>)MiddlewareResolver.Resolve(type);

                index++;
               
                //如果中间件的当前实例是列表中的最后一个实例，
                //下一个函数被分配给最终函数或默认空函数。

                if (index == MiddlewareTypes.Count)
                    func = this._finallyFunc ?? ((p) => Task.FromResult(default(TReturn)));

                return middleware.Run(param, func);
            };

            return await func(parameter);
        }

        /// <summary>
        /// 设置这个函数在责任链后执行回调
        /// 一个链只能有一个最终函数。调用此方法更多
        /// 一个链只能有一个最终函数。调用此方法更多<see cref="Func{TParameter, TResult}"/>.
        /// </summary>
        /// <param name="finallyFunc">将在链的末尾执行的函数。</param>
        /// <returns>当前实例 <see cref="IAsyncResponsibilityChain{TParameter, TReturn}"/>.</returns>
        public IAsyncResponsibilityChain<TParameter, TReturn> Finally(Func<TParameter, Task<TReturn>> finallyFunc)
        {
            this._finallyFunc = finallyFunc;
            return this;
        }
    }
}
