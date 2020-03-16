using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIocManager
    {
        /// <summary>
        /// IContainer
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// ILifetimeScope
        /// </summary>
        /// <returns></returns>
        ILifetimeScope Scope();

        /// <summary>
        /// IsRegistered
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        bool IsRegistered(Type serviceType, ILifetimeScope scope = null);

        /// <summary>
        /// Resolve
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        object Resolve(Type type, ILifetimeScope scope = null);

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class;

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T Resolve<T>(params Parameter[] parameters) where T : class;

        /// <summary>
        /// ResolveAll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null);

        /// <summary>
        /// ResolveOptional
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        object ResolveOptional(Type serviceType, ILifetimeScope scope = null);

        /// <summary>
        /// ResolveUnregistered
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        object ResolveUnregistered(Type type, ILifetimeScope scope = null);

        /// <summary>
        /// ResolveUnregistered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class;

        /// <summary>
        /// TryResolve
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance);

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// ResolveKeyed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T ResolveKeyed<T>(Object serviceKey, params Parameter[] parameters);

        /// <summary>
        /// ResolveNamed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T ResolveNamed<T>(String serviceName, params Parameter[] parameters) where T : class;

        /// <summary>
        /// ResolveOptional
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T ResolveOptional<T>(params Parameter[] parameters) where T : class;

        /// <summary>
        /// ResolveOptionalKeyed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T ResolveOptionalKeyed<T>(Object serviceKey, params Parameter[] parameters) where T : class;

        /// <summary>
        /// ResolveOptionalNamed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T ResolveOptionalNamed<T>(String serviceName, params Parameter[] parameters) where T : class;
    }
}
