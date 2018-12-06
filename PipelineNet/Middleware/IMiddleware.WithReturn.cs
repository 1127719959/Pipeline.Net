using System;

namespace Pipeline.Net.Middleware
{
    public interface IMiddleware<TParameter, TReturn>
    {
        TReturn Run(TParameter parameter, Func<TParameter, TReturn> next);
    }
}
