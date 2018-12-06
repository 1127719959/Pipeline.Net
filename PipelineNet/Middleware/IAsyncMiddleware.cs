using System;
using System.Threading.Tasks;

namespace Pipeline.Net.Middleware
{
    public interface IAsyncMiddleware<TParameter>
    {
        Task Run(TParameter parameter, Func<TParameter, Task> next);
    }
}
