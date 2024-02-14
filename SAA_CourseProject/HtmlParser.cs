using System;
namespace SAA_CourseProject
{
	public class HtmlParser
	{
        private readonly string filePath;
        private int position;

        public HtmlParser(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("File path cannot be null!");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("The file does not exist!");

            this.filePath = filePath;
            this.position = 0;
        }

        public TreeNode<string> BuildHtmlTree(List<TreeNode<string>> treeNodes)
        {
            if (treeNodes == null || treeNodes.Count == 0)
            {
                throw new ArgumentException("The list is null or empty.");
            }

            TreeNode<string> root = treeNodes[0];
            Stack<TreeNode<string>> treeNodesStack = new Stack<TreeNode<string>>();

            treeNodesStack.Push(root);

            int currentNode = 1;

            while (currentNode < treeNodes.Count)
            {
                // Opening tag case:
                if (treeNodes[currentNode].Tag[0] == '<' && treeNodes[currentNode].Tag[1] != '/')
                {
                    // Self-closing tag case:
                    if (treeNodes[currentNode].Tag[treeNodes[currentNode].Tag.Length - 2] == '/')
                    {
                        treeNodesStack.Peek().AddChild(treeNodes[currentNode]);
                    }
                    else
                    {
                        treeNodesStack.Peek().AddChild(treeNodes[currentNode]);
                        treeNodesStack.Push(treeNodes[currentNode]);
                    }
                }
                // Closing tag case:
                else
                {
                    treeNodesStack.Peek().ClosedTag = treeNodes[currentNode].Tag;
                    treeNodesStack.Pop();
                }

                currentNode++;
            }

            return root;
        }

        public List<TreeNode<string>> HtmlTokenizer()
        {
            List<TreeNode<string>> treeNodes = new List<TreeNode<string>>();
            TreeNode<string> treeNode;

            string htmlContent = File.ReadAllText(filePath);

            while (position < htmlContent.Length)
            {
                char currentChar = htmlContent[position];

                if (currentChar == '<')
                {
                    string tag = ReadTag(htmlContent);
                    List<string> attributes = ReadAttributes(tag);

                    // Opening bracket of the closing tag.
                    if (currentChar == '<')
                    {
                        string text = ReadText(htmlContent);

                        treeNode = new TreeNode<string>(tag);

                        foreach (var attr in attributes)
                        {
                            treeNode.AddAttribute(attr);
                        }

                        treeNode.Text = text;

                        treeNodes.Add(treeNode);
                    }
                }

            }

            return treeNodes;
        }

        private string ReadTag(string htmlContent)
        {
            int startIdx = position;
            int endIdx = FindClosingTag(htmlContent, position);

            if (endIdx == -1)
                throw new InvalidOperationException("Invalid HTML format: Unclosed Tag...");

            position = endIdx + 1;

            string tag = htmlContent.Substring(startIdx, endIdx - startIdx + 1);

            return tag;
        }

        private List<string> ReadAttributes(string tag)
        {
            List<string> attributes = new List<string>();

            int attrStartIdx = FindWhiteSpaceInTag(tag, FindOpeningTag(tag, 0));
            int attrEndIdx;
            string attribute;

            // Attribute is found.
            if (attrStartIdx != -1)
            {
                attrEndIdx = FindClosingTag(tag, attrStartIdx);

                if (attrEndIdx == -1)
                    throw new InvalidOperationException("Invalid HTML format: Unclosed Tag...");

                // Self-closing tag check.
                if (tag[attrEndIdx - 1] == '/')
                {
                    attribute = tag.Substring(attrStartIdx, attrEndIdx - 1 - attrStartIdx).Trim();
                }
                else
                {
                    attribute = tag.Substring(attrStartIdx, attrEndIdx - attrStartIdx).Trim();
                }

                // First attribute.
                int firstAttrStartIdx = 0;

                for (int i = 0; i < attribute.Length; i++)
                {
                    if (attribute[i] == ' ')
                    {
                        string singleAttribute = attribute.Substring(firstAttrStartIdx, i - firstAttrStartIdx);
                        attributes.Add(singleAttribute);

                        firstAttrStartIdx = i + 1;
                    }
                }

                string lastAttribute = attribute.Substring(firstAttrStartIdx);
                attributes.Add(lastAttribute);

            }

            return attributes;
        }

        private string ReadText(string htmlContent)
        {
            int startIdx = position;
            int endIdx = FindOpeningTag(htmlContent, position);

            if (endIdx == -1)
                endIdx = htmlContent.Length;

            position = endIdx;

            string text = htmlContent.Substring(startIdx, endIdx - startIdx);

            return text;
        }

        private int FindOpeningTag(string fileContent, int startIdx)
        {
            for (int i = startIdx; i < fileContent.Length; i++)
            {
                if (fileContent[i] == '<')
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindClosingTag(string fileContent, int startIdx)
        {
            for (int i = startIdx; i < fileContent.Length; i++)
            {
                if (fileContent[i] == '>')
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindWhiteSpaceInTag(string tag, int startIdx)
        {
            for (int i = startIdx; i < tag.Length; i++)
            {
                if (tag[i] == ' ')
                {
                    return i;
                }
            }

            return -1;
        }
    }
}