// CODE FROM https://github.com/SubnauticaNitrox/Nitrox/

using System;

namespace QModManager.SceneDebugger
{
    [Serializable]
    internal class HasValueOptional<T> : Optional<T>
    {
        internal readonly T value;

        internal HasValueOptional(T value)
        {
            this.value = value;
        }

        internal override T Get()
        {
            return value;
        }

        internal override bool IsPresent()
        {
            return true;
        }

        internal override bool IsEmpty()
        {
            return false;
        }

        internal override T OrElse(T elseValue)
        {
            return value;
        }
    }

    [Serializable]
    internal class NoValueOptional<T> : Optional<T>
    {
        internal override T Get()
        {
            throw new InvalidOperationException("Optional did not have a value");
        }

        internal override bool IsPresent()
        {
            return false;
        }

        internal override bool IsEmpty()
        {
            return true;
        }

        internal override T OrElse(T elseValue)
        {
            return elseValue;
        }
    }

    [Serializable]
    internal abstract class Optional<T>
    {
        internal static Optional<T> Empty()
        {
            return new NoValueOptional<T>();
        }

        internal static Optional<T> Of(T value)
        {
            if (value == null || value.Equals(default(T)))
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null");
            }

            return new HasValueOptional<T>(value);
        }

        internal static Optional<T> OfNullable(T value)
        {
            if (value == null || value.Equals(default(T)))
            {
                return new NoValueOptional<T>();
            }

            return new HasValueOptional<T>(value);
        }

        internal abstract T Get();
        internal abstract bool IsPresent();
        internal abstract bool IsEmpty();
        internal abstract T OrElse(T elseValue);
    }

    [Serializable]
    internal sealed class OptionalEmptyException<T> : Exception
    {
        internal OptionalEmptyException() : base($"Optional <{nameof(T)}> is empty.")
        {
        }

        internal OptionalEmptyException(string message) : base($"Optional <{nameof(T)}> is empty:\n\t{message}")
        {
        }
    }
}
