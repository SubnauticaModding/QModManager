using System.Collections.Generic;

namespace BlueFire.Debugger
{
    public class TreeNode<T>
    {
        public List<TreeNode<T>> Children = new List<TreeNode<T>>();

        public T Item { get; set; }

        public TreeNode()
        {
        }

        public TreeNode(T item)
        {
            Item = item;
        }

        public TreeNode<T> AddChild(T item)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(item);
            Children.Add(nodeItem);
            return nodeItem;
        }
    }
}