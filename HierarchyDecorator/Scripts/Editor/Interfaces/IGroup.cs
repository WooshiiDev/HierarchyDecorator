using System;

namespace HierarchyDecorator
{
    /// <summary>
    /// Interface for a named group of elements of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type this group represents.</typeparam>
    public interface IGroup<T> where T : IComparable<T>
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The number of elements apart of this group.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get an instance in this group.
        /// </summary>
        /// <param name="index">The element index.</param>
        /// <returns>The found element.</returns>
        T Get(int index);

        /// <summary>
        /// Add an instance to this group.
        /// </summary>
        /// <param name="instance">The instance to add.</param>
        /// <returns>Returns a bool based on if the instance was added to the group successfully.</returns>
        bool Add(T instance);

        /// <summary>
        /// Remove an instance that belongs to this group.
        /// </summary>
        /// <param name="instance">The instance to remove.</param>
        /// <returns>Returns a bool based on if the instance was removed from the group suggessfully.</returns>
        bool Remove(T instance);
    }
}