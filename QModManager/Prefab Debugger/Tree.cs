using System.Collections.Generic;

namespace QModManager.Debugger
{
    internal class TreeNode<T>
    {
        internal List<TreeNode<T>> Children = new List<TreeNode<T>>();

        internal T Item { get; set; }

        internal TreeNode()
        {
        }
        
        internal TreeNode(T item)
        {
            Item = item;
        }

        internal TreeNode<T> AddChild(T item)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(item);
            Children.Add(nodeItem);
            return nodeItem;
        }
    }
}