using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rami.WebApi.Core.Framework
{
    /// <summary>
    /// Container manager
    /// </summary>
    public class IocManager : IIocManager
    {
        /// <summary>
        /// IContainer
        /// </summary>
        private static IContainer _container;

        /// <summary>
        /// IContainer
        /// </summary>
        public virtual IContainer Container
        {
            get
            {
                return _container;
            }
        }

        /// <summary>
        /// SingletonInstance
        /// </summary>
        private static readonly IocManager SingletonInstance = new IocManager();

        /// <summary>
        /// Instance
        /// </summary>
        public static IocManager Instance { get { return SingletonInstance; } }

        /// <summary>
        /// 注入IContainer
        /// </summary>
        /// <param name="container"></param>
        public static void Configure(IContainer container)
        {
            _container = container;
        }

        #region 自带

        /// <summary>
        /// Get current scope
        /// </summary>
        /// <returns>Scope</returns>
        public virtual ILifetimeScope Scope()
        {
            try
            {
                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope();
            }
            catch (Exception)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">key</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<T>();
            }

            return scope.ResolveKeyed<T>(key);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="parameters">key</param>
        /// <returns>Resolved service</returns>
        public virtual T Resolve<T>(params Parameter[] parameters) where T : class
        {
            var scope = Scope();
            return scope.Resolve<T>(parameters);
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            return scope.Resolve(type);
        }

        /// <summary>
        /// Resolve all
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">key</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved services</returns>
        public virtual T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<IEnumerable<T>>().ToArray();
            }

            return scope.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return ResolveUnregistered(typeof(T), scope) as T;
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new Exception("Unknown dependency");
                        parameterInstances.Add(service);
                    }

                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Exception)
                {

                }
            }

            throw new Exception("No constructor  was found that had all the dependencies satisfied.");
        }

        /// <summary>
        /// Try to resolve srevice
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <param name="instance">Resolved service</param>
        /// <returns>Value indicating whether service has been successfully resolved</returns>
        public virtual bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            return scope.TryResolve(serviceType, out instance);
        }

        /// <summary>
        /// Check whether some service is registered (can be resolved)
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Result</returns>
        public virtual bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            return scope.IsRegistered(serviceType);
        }

        /// <summary>
        /// Resolve optional
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="scope">Scope; pass null to automatically resolve the current scope</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }

            return scope.ResolveOptional(serviceType);
        }

        #endregion

        /// <summary>
        /// Resolve
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        ///// <summary>
        ///// Resolve
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //public virtual T Resolve<T>(params Parameter[] parameters) where T : class
        //{
        //    return Container.Resolve<T>(parameters);
        //}

        /// <summary>
        /// ResolveKeyed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T ResolveKeyed<T>(Object serviceKey, params Parameter[] parameters)
        {
            return Container.ResolveKeyed<T>(serviceKey, parameters);
        }

        /// <summary>
        /// ResolveNamed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T ResolveNamed<T>(String serviceName, params Parameter[] parameters) where T : class
        {
            return Container.ResolveNamed<T>(serviceName, parameters);
        }

        /// <summary>
        /// ResolveOptional
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T ResolveOptional<T>(params Parameter[] parameters) where T : class
        {
            return Container.ResolveOptional<T>(parameters);
        }

        /// <summary>
        /// ResolveOptionalKeyed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T ResolveOptionalKeyed<T>(Object serviceKey, params Parameter[] parameters) where T : class
        {
            return Container.ResolveOptionalKeyed<T>(serviceKey, parameters);
        }

        /// <summary>
        /// ResolveOptionalNamed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T ResolveOptionalNamed<T>(String serviceName, params Parameter[] parameters) where T : class
        {
            return Container.ResolveOptionalNamed<T>(serviceName, parameters);
        }
    }
}
