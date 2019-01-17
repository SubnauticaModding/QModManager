// CODE FROM https://github.com/SubnauticaNitrox/Nitrox/

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace QModManager.SceneDebugger
{
    internal static class Validate
    {
        // Prevent non-nullable valuetypes from getting boxed to object.
        // In other words: Error when trying to assert non-null on something that can't be null in the first place.
        internal static void NotNull<T>(T o) where T : class
        {
            if (o == null)
            {
                Optional<string> paramName = GetParameterName<T>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get());
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
        }

        internal static void NotNull<T>(T o, string message)
            where T : class
        {
            if (o == null)
            {
                Optional<string> paramName = GetParameterName<T>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get(), message);
                }
                else
                {
                    throw new ArgumentNullException(message);
                }
            }
        }

        internal static void IsTrue(bool b)
        {
            if (!b)
            {
                throw new ArgumentException();
            }
        }

        internal static void IsTrue(bool b, string message)
        {
            if (!b)
            {
                throw new ArgumentException(message);
            }
        }

        internal static void IsFalse(bool b)
        {
            if (b)
            {
                throw new ArgumentException();
            }
        }

        internal static void IsFalse(bool b, string message)
        {
            if (b)
            {
                throw new ArgumentException(message);
            }
        }

        internal static void IsPresent<T>(Optional<T> opt)
        {
            if (opt.IsEmpty())
            {
                throw new OptionalEmptyException<T>();
            }
        }

        internal static void IsPresent<T>(Optional<T> opt, string message)
        {
            if (opt.IsEmpty())
            {
                throw new OptionalEmptyException<T>(message);
            }
        }

        internal static Optional<string> GetParameterName<TParam>()
        {
            ParameterInfo[] parametersOfMethodBeforeValidate = new StackFrame(2).GetMethod().GetParameters();
            return Optional<string>.OfNullable(parametersOfMethodBeforeValidate.SingleOrDefault(pi => pi.ParameterType == typeof(TParam))?.Name);
        }
    }
}