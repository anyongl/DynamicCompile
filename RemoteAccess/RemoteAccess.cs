using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RemoteAccess
{
    /// <summary>
    /// Interface that can be run over the remote AppDomain boundary.
    /// </summary>
    public interface IRemoteInterface
    {
        object Invoke(string lcMethod, object[] Parameters);
    }

    /// <summary>
    /// Factory class to create objects exposing IRemoteInterface
    /// </summary>
    public class RemoteLoaderFactory : MarshalByRefObject
    {
        private const BindingFlags bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
        public RemoteLoaderFactory() { }
        public IRemoteInterface Create(string assemblyFile, string typeName, object[] constructArgs)
        {
            return (IRemoteInterface)Activator.CreateInstanceFrom(
                     assemblyFile, typeName, false, bfi, null, constructArgs,
                     null, null, null).Unwrap();
        }
    }
}
