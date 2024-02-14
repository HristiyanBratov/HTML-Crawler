using System;
namespace SAA_CourseProject
{
	public class TreeNode<T>
	{
        private string tag;
        private List<string> attributes;
        private string text;
        private List<TreeNode<T>> children;
        private string closedTag;

        public TreeNode(string tag)
        {
            this.Tag = tag;
            this.Attributes = new List<string>();
            this.Text = null;
            this.Children = new List<TreeNode<T>>();
            this.ClosedTag = null;
        }

        public string Tag
        {
            get { return this.tag; }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Tag cannot have null as a value.");
                }

                this.tag = value;
            }
        }

        public List<string> Attributes
        {
            get { return this.attributes; }
            private set { this.attributes = value; }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
            }
        }

        public List<TreeNode<T>> Children
        {
            get { return this.children; }
            private set { this.children = value; }
        }

        public string ClosedTag
        {
            get { return this.closedTag; }
            set { this.closedTag = value; }
        }

        public void AddChild(TreeNode<T> child)
        {
            if (child == null)
                throw new ArgumentNullException("Cannot insert null value!");

            this.Children.Add(child);
        }

        public void AddAttribute(string attribute)
        {
            this.Attributes.Add(attribute);
        }

        public TreeNode<T> GetChildByIndex(int index)
        {
            return this.Children[index];
        }

    }
}