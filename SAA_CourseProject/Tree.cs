using System;
namespace SAA_CourseProject
{
	public class Tree<T>
	{
        private TreeNode<T> root;

        public TreeNode<T> Root
        {
            get { return this.root; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Cannot insert null value!");

                this.root = value;
            }
        }

        public void TraverseDFS()
        {
            this.TreeTraversal(this.Root, string.Empty);
            Console.WriteLine(root.ClosedTag);
        }

        private void TreeTraversal(TreeNode<T> root, string spaces)
        {
            Console.WriteLine(spaces + root.Tag);

            for (int i = 0; i < root.Children.Count; i++)
            {
                TreeNode<T> child = root.GetChildByIndex(i);
                TreeTraversal(child, spaces + " ");

                // Printing the closing tag after each opening one (except self-closing)
                if (child.Tag[child.Tag.Length - 2] != '/')
                    Console.WriteLine(spaces + " " + child.ClosedTag);
            }

        }

        public void SaveToHtmlFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        this.SaveTree(this.Root, writer);
                    }

                    Console.Write($"\nChanges saved to: {fileName}\n");
                }
                else
                {
                    using (StreamWriter writer = File.CreateText(fileName))
                    {
                        this.SaveTree(this.Root, writer);
                    }

                    Console.Write($"\nFile created: {fileName}\n");
                }
            }
            catch (IOException ex)
            {
                Console.Write($"\nError saving changes: {ex.Message}\n");
            }
        }

        private void SaveTree(TreeNode<T> root, StreamWriter writer)
        {
            writer.WriteLine(root.Tag + root.Text);

            for (int i = 0; i < root.Children.Count; i++)
            {
                TreeNode<T> child = root.GetChildByIndex(i);
                SaveTree(child, writer);

                // Saving the closing tag after each opening one (except self-closing)
                if (child.Tag[child.Tag.Length - 2] != '/')
                    writer.WriteLine(child.ClosedTag);
            }
        }

    }
}