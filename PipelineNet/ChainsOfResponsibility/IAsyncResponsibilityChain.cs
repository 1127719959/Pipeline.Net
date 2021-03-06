﻿using Pipeline.Net.Middleware;
using System;
using System.Threading.Tasks;

namespace Pipeline.Net.ChainsOfResponsibility
{
    /// <summary>
    /// 定义异步的责任链。
    /// </summary>
    /// <typeparam name="TParameter">链的输入类型。</typeparam>
    /// <typeparam name="TReturn">链的返回类型。</typeparam>
    public interface IAsyncResponsibilityChain<TParameter, TReturn>
    {
        /// <summary>
        /// 设置要在链的末尾执行的函数作为后备。
        /// 一个链只能有一个最终函数。调用此方法更多
        /// 第二次只会取代现有的<see cref="Func{TParameter, TResult}"/>.
        /// </summary>
        /// <param name="finallyFunc">将在链的末尾执行的函数。</param>
        /// <returns>当前实例<see cref="IAsyncResponsibilityChain{TParameter, TReturn}"/>.</returns>
        IAsyncResponsibilityChain<TParameter, TReturn> Finally(Func<TParameter, Task<TReturn>> finallyFunc);

        /// <summary>
        ///为责任链链上一个新的中间件。
        /// 中间件将按照它们添加的顺序执行。
        /// </summary>
        /// <typeparam name="TMiddleware">正在添加新的中间件。</typeparam>
        /// <returns>当前实例<see cref="IAsyncResponsibilityChain{TParameter, TReturn}"/>.</returns>
        IAsyncResponsibilityChain<TParameter, TReturn> Chain<TMiddleware>()
            where TMiddleware : IAsyncMiddleware<TParameter, TReturn>;

        /// <summary>
        /// 将新的中间件类型链接到责任链。
        /// 中间件将按照它们添加的顺序执行。
        /// </summary>
        /// <param name="middlewareType">要执行的中间件类型。</param>
        /// <exception cref="ArgumentException">如果抛出 <paramref name="middlewareType"/> is 
        /// 不是实现的 <see cref="IAsyncMiddleware{TParameter, TReturn}"/>.</exception>
        /// <exception cref="ArgumentNullException">抛出如果 <paramref name="middlewareType"/>是空的。</exception>
        /// <returns>当前实例<see cref="IAsyncResponsibilityChain{TParameter, TReturn}"/>.</returns>
        IAsyncResponsibilityChain<TParameter, TReturn> Chain(Type middlewareType);

        /// <summary>
        /// 执行配置的责任链。
        /// </summary>
        /// <param name="parameter"></param>
        Task<TReturn> Execute(TParameter parameter);
    }
}
