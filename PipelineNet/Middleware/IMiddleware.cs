using System;

namespace Pipeline.Net.Middleware
{
    public interface IMiddleware<TParameter>
    {
        void Run(TParameter parameter, Action<TParameter> next);
    }
}
