using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 静态 AutofacIocManger
    /// </summary>
    public class StaticAutofacIocManger
    {
        /// <summary>
        /// The Singleton IContainer.
        /// </summary>
        private static IContainer IocContainer;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="iocContainer"></param>
        public static void Configure(IContainer iocContainer)
        {
            IocContainer = iocContainer;
        }

        /// <summary>
        /// IocManager
        /// </summary>
        public static IocManager Current => IocManager.Instance;
    }
}
