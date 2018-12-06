using Pipeline.Net.MiddlewareResolver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pipeline.Net
{
    public abstract class BaseMiddlewareFlow<TMiddleware>
    {
        protected IList<Type> MiddlewareTypes { get; private set; }
        protected IMiddlewareResolver MiddlewareResolver { get; private set; }

        internal BaseMiddlewareFlow(IMiddlewareResolver middlewareResolver)
        {
            if (middlewareResolver == null) throw new ArgumentNullException("middlewareResolver",
                "An instance of IMiddlewareResolver must be provided. You can use ActivatorMiddlewareResolver.");

            MiddlewareResolver = middlewareResolver;
            MiddlewareTypes = new List<Type>();
        }

        /// <summary>
        /// 存储 <see cref="TypeInfo"/> 中间件的类型。
        /// </summary>
        private static readonly TypeInfo MiddlewareTypeInfo = typeof(TMiddleware).GetTypeInfo();

        /// <summary>
        /// 将新的中间件类型添加到类型的内部列表。
        /// 中间件将按照它们添加的顺序执行。
        /// </summary>
        /// <param name="middlewareType">要执行的中间件类型。</param>
        /// <exception cref="ArgumentException">如果抛出 <paramref name="middlewareType"/> is 
        /// 不是实现的 <typeparamref name="TMiddleware"/>.</exception>
        /// <exception cref="ArgumentNullException">抛出如果 <paramref name="middlewareType"/>是空的。</exception>
        protected void AddMiddleware(Type middlewareType)
        {
            if (middlewareType == null) throw new ArgumentNullException("middlewareType");

            bool isAssignableFromMiddleware = MiddlewareTypeInfo.IsAssignableFrom(middlewareType.GetTypeInfo());
            if (!isAssignableFromMiddleware)
                throw new ArgumentException(
                    string.Format("The middleware type must implement \"{0}\".", typeof(TMiddleware)));

            this.MiddlewareTypes.Add(middlewareType);
        }
    }
}
