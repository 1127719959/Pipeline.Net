using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipeline.Net.Tests.Infrastructure
{
    public static class PipelineNetAssert
    {
        /// <summary>
        /// 断言给定操作中的代码将引发异常。
        /// </summary>
        /// <typeparam name="TException">预期的异常类型。</typeparam>
        /// <param name="statements">预期抛出异常的动作。</param>
        public static void ThrowsException<TException>(Action statements)
            where TException : Exception
        {
            TException exception = null;
            try
            {
                statements();
            }
            catch(TException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception,
                string.Format("The code did not thrown the expected exception \"{0}\".", typeof(TException)));
        }
    }
}
