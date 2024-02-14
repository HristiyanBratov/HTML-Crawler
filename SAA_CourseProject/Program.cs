using System;
using System.IO;
using SAA_CourseProject;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    private static void Main(string[] args)
    {
        string htmlPath = "index.html";
        HtmlParser htmlParser = new HtmlParser(htmlPath);
        Tree<string> tree = new Tree<string>();
        tree.Root = htmlParser.BuildHtmlTree(htmlParser.HtmlTokenizer());

        // The program starts here...
        Console.WriteLine("Welcome to my HTML Crawler...");

        while (true)
        {
            Console.Write("\nEnter a command. -> (PRINT, SET, COPY, SAVE): ");
            string command = Console.ReadLine();

            if(command != "PRINT" && command != "SET" && command != "COPY" && command != "SAVE")
            {
                Console.WriteLine("\nInvalid command! Try again.");
            }
            else
            {
                string XPath = null;
                string newValue = null;
                string secondXPath = null;
                List<string> inputElements = new List<string>();
                List<string> results = new List<string>();

                if (command == "PRINT")
                {
                    Console.Write("\nWhat would you like to be printed?: ");
                    XPath = Console.ReadLine();
                }
                else if (command == "SET")
                {
                    Console.Write("\nEnter XPath and requested new value: ");
                    string input = Console.ReadLine();

                    if (input == "")
                    {
                        Console.Write("\nThe input was empty!\n");
                        continue;
                    }
                    else
                    {
                        inputElements = SplitInput(input);

                        if (inputElements.Count != 2)
                        {
                            Console.Write("\nNo new value was given.\n");
                            continue;
                        }
                        else
                        {
                            XPath = inputElements[0];
                            newValue = inputElements[1];
                        }
                    }         
                }
                else if (command == "COPY")
                {
                    Console.Write("\nEnter XPath and second XPath holding the wanted value: ");
                    string input = Console.ReadLine();

                    if(input == "")
                    {
                        Console.Write("\nThe input was empty!\n");
                        continue;
                    }
                    else
                    {
                        inputElements = SplitInput(input);

                        if(inputElements.Count != 2)
                        {
                            Console.Write("\nNo second XPath was given.\n");
                            continue;
                        }
                        else
                        {
                            XPath = inputElements[0];

                            if(inputElements[1] == "" || inputElements[1][0] != '/' || inputElements[1][1] != '/')
                            {
                                Console.Write("\nInvalid second XPath. '//' is missing or nothing has been entered.\n");
                                continue;
                            }
                            else
                            {
                                secondXPath = inputElements[1];
                            }

                        } 
                    }
                    
                }
                else if(command == "SAVE")
                {
                    Console.Write("\nEnter file name to save the changes: ");
                    string fileName = Console.ReadLine();

                    if(fileName == "")
                    {
                        Console.Write("\nNo file name was entered.\n");
                    }
                    else
                    {
                        tree.SaveToHtmlFile(fileName);
                    }     

                    continue;
                }

                if (XPath == "" || XPath[0] != '/' || XPath[1] != '/')
                {
                    Console.Write("\nInvalid XPath. '//' is missing or nothing has been entered.\n");
                }
                else if (XPath == "//" || secondXPath == "//")
                {
                    if(command == "PRINT")
                        tree.TraverseDFS();
                    else
                        Console.Write("\n '//' is used only for printing the tree!\n");
                }
                else if (XPath[XPath.Length - 1] != ']' && XPath[XPath.Length - 1] != '*' && !IsContained(XPath, '@'))
                {
                    if(command == "PRINT")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, null, -1, command, null);

                        foreach (var item in results)
                        {
                            Console.Write(item + " ");
                        }
                    }
                    else if (command == "SET")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, null, -1, command, newValue);

                        Console.Write("\nSuccessfully changed.\n");
                    }
                    else if(command == "COPY")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, null, -1, "PRINT", newValue);

                        string result = null;

                        foreach(var item in results)
                        {
                            result += item + " ";
                        }

                        results = SearchAndAddToCollection(tree.Root, secondXPath, null, -1, "SET", result);

                        Console.Write("\nCopied.\n");
                    }
                }
                else if (XPath[XPath.Length - 1] == ']' && XPath[XPath.Length - 2] != '\'')
                {
                    char secondToLastChar = XPath[XPath.Length - 2];

                    if (char.IsDigit(secondToLastChar))
                    {
                        int index = int.Parse(secondToLastChar.ToString());

                        if(command == "PRINT" || command == "COPY")
                        {
                            results = SearchAndAddToCollection(tree.Root, XPath, null, -1, "PRINT", " ");

                            if (index > 0 && index <= results.Count)
                            {
                                if (command == "PRINT")
                                    Console.WriteLine($"\nResult: {results[index - 1]}");
                                else if (command == "COPY")
                                {
                                    string result = results[index - 1];

                                    results = SearchAndAddToCollection(tree.Root, secondXPath, null, -1, "SET", result);

                                    Console.Write("\nCopied.\n");
                                }
                            }
                            else
                            {   
                                Console.WriteLine("\nIndex out of range!");
                            }
                        }
                        else if (command == "SET")
                        {
                            results = SearchAndAddToCollection(tree.Root, XPath, null, index, command, newValue);
                            Console.Write("\nSuccessfully changed.\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nThe value between the brackets is not a digit!");
                    }
                }
                else if (XPath[XPath.Length - 1] == '*')
                {
                    if(command == "PRINT" || command == "COPY")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, null, -1, "PRINT", " ");
                        string trimmedItem = null;

                        if (command == "PRINT")
                        {
                            foreach (var item in results)
                            {
                                if (item != null && !IsContained(item, '<'))
                                    Console.Write(item + " ");
                            }
                        }
                        else if (command == "COPY")
                        {
                            foreach (var item in results)
                            {
                                if (!IsContained(item, '<'))
                                    trimmedItem += item + " ";
                            }

                            results = SearchAndAddToCollection(tree.Root, secondXPath, null, -1, "SET", trimmedItem);
                            Console.Write("\nCopied.\n");
                        }
                    }
                    else if(command == "SET")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, null, -1, command, newValue);
                        Console.Write("\nSuccessfully changed.\n");
                    }
                }
                else if (XPath[XPath.Length - 1] == ']' && XPath[XPath.Length - 2] == '\'')
                {
                    string attribute = XPath.Substring(FindOpeningBracket(XPath, 0) + 2, FindClosingBracket(XPath, FindOpeningBracket(XPath, 0)) - FindOpeningBracket(XPath, 0) - 2);

                    if(command == "PRINT")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, attribute, -1, command, " ");

                        for (int i = 0; i < results.Count; i++)
                        {
                            Console.Write(results[i]);
                        }
                    }
                    else if(command == "SET")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, attribute, -1, command, newValue);
                        Console.Write("\nSuccessfully changed.\n");
                    }
                    else if(command == "COPY")
                    {
                        results = SearchAndAddToCollection(tree.Root, XPath, attribute, -1, "PRINT", " ");
                        string result = null;

                        foreach (var item in results)
                            result += item + " ";

                        results = SearchAndAddToCollection(tree.Root, secondXPath, null, -1, "SET", result);
                        Console.Write("\nCopied.\n");
                    }
                }
                else if (IsContained(XPath, '@'))
                {
                    char charIndex = FindChildIndex(XPath);

                    if (char.IsDigit(charIndex))
                    {
                        int index = int.Parse(charIndex.ToString());

                        if(command == "PRINT")
                        {
                            results = SearchAndAddToCollection(tree.Root, XPath, null, index, command, " ");

                            foreach (var item in results)
                            {
                                Console.Write(item + " ");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nThe value between the brackets is not a digit!");
                    }
                }
            }
        }

        Console.ReadLine();
    }

    // Split the XPath when finds '/'
    static List<TreeNode<string>> SplitXPath(string XPath)
    {
        List<TreeNode<string>> result = new List<TreeNode<string>>();

        int startIdx = 2;

        for (int i = 2; i < XPath.Length; i++)
        {
            if (XPath[i] == '/')
            {
                if (XPath[i - 1] == ']')
                {
                    string entireTag = XPath.Substring(startIdx, i - startIdx);
                    TreeNode<string> currentNode = BuildTreeNode(entireTag.Substring(0, FindOpeningBracket(entireTag, 0)));
                    result.Add(currentNode);

                    string attribute = XPath.Substring(FindOpeningBracket(XPath, startIdx), i - 1 - FindOpeningBracket(XPath, startIdx));
                    currentNode.AddAttribute(attribute);
                }
                else
                {
                    result.Add(BuildTreeNode(XPath.Substring(startIdx, i - startIdx)));
                }

                startIdx = i + 1;
            }

        }

        if (startIdx < XPath.Length)
        {
            if (XPath[XPath.Length - 1] == ']' && XPath[XPath.Length - 3] == '[')
            {
                result.Add(BuildTreeNode(XPath.Substring(startIdx, XPath.Length - 3 - startIdx)));
            }
            else if (XPath[XPath.Length - 1] == '*' && XPath[XPath.Length - 2] == '/')
            {

            }
            else if (XPath[XPath.Length - 1] == ']' && XPath[XPath.Length - 2] == '\'')
            {
                result.Add(BuildTreeNode(XPath.Substring(startIdx, FindOpeningBracket(XPath, startIdx) - startIdx)));
            }
            else
            {
                result.Add(BuildTreeNode(XPath.Substring(startIdx)));
            }
        }

        return result;
    }

    // Helper method for building the TreeNode
    private static TreeNode<string> BuildTreeNode(string tagValue)
    {
        return new TreeNode<string>('<' + tagValue + '>');
    }

    // Split the input when finds " "
    static List<string> SplitInput(string input)
    {
        List<string> inputElements = new List<string>();

        int startIndex = 0;

        for(int i = 0; i < input.Length; i++)
        {
            if (input[i] == ' ')
            {
                inputElements.Add(input.Substring(startIndex, i - startIndex));

                startIndex = i + 1;
            }

        }

        inputElements.Add(input.Substring(startIndex));

        return inputElements;
    }

    static List<string> SearchAndAddToCollection(TreeNode<string> root, string XPath, string attribute, int nodeIndex, string command, string newValue)
    {
        List<string> result = new List<string>();
        List<TreeNode<string>> splitedTags = SplitXPath(XPath);

        TraverseTree(root, splitedTags, result, attribute, nodeIndex, command, newValue);

        return result;
    }

    private static void TraverseTree(TreeNode<string> node, List<TreeNode<string>> splitedTags,
                                     List<string> result, string attribute, int nodeIndex, string command,
                                     string newValue, bool continueRecursion = true)
    {
        for (int i = 0; i < splitedTags.Count; i++)
        {
            if (splitedTags[i].Tag == ExtractOnlyTag(node.Tag))
            {
                splitedTags[i] = node;

                if (i == splitedTags.Count - 1)
                {
                    if (splitedTags[i].Children.Count == 0 && attribute == null)
                    {
                        if (command == "PRINT")
                        {
                            result.Add(splitedTags[i].Text);
                        }
                        else if(command == "SET")
                        {
                            if(nodeIndex != -1)
                            {
                                foreach (var child in splitedTags[i - 1].Children)
                                {
                                    splitedTags[i - 1].Children[nodeIndex - 1].Text = newValue;
                                }
                            }
                            else
                            {
                                splitedTags[i].Text = newValue;
                            }
                        }
                    }

                    if (splitedTags[i].Children.Count > 0)
                    {
                        if (splitedTags[i].Tag != splitedTags[i - 1].Tag)
                        {
                            foreach (var child in splitedTags[i].Children)
                            {
                                if(command == "PRINT")
                                {
                                    result.AddRange(new[] { child.Tag, child.Text, child.ClosedTag });
                                }
                                else if(command == "SET")
                                {
                                    child.Text = newValue;
                                }

                                continueRecursion = false;
                            }
                        }    
                    }

                    if (splitedTags[i].Attributes.Count > 0)
                    {
                        foreach (var attr in splitedTags[i].Attributes)
                        {
                            if (attr == attribute)
                            {
                                if(command == "PRINT")
                                    result.Add(splitedTags[i].Text);
                                else if(command == "SET")
                                {
                                    splitedTags[i].Text = newValue;
                                }
                            }
                        }
                    }

                }
                else
                {
                    TreeNode<string> currentNode;

                    if (splitedTags[i].Attributes.Count > 0 && splitedTags[i].Children.Count > 0)
                    {
                        currentNode = splitedTags[i];

                        // Prevent IndexOutOfRange Exception.
                        if (nodeIndex - 1 >= 0 && nodeIndex - 1 < currentNode.Children.Count)
                        {
                            for (int j = 0; j < currentNode.Children.Count; j++)
                            {
                                currentNode = currentNode.Children[nodeIndex - 1];

                                foreach (var child in currentNode.Children)
                                {
                                    result.Clear();
                                    result.Add(child.Text);
                                    continueRecursion = false;
                                }
                            }
                        }
                                              
                    }
                }
            }
        }

        if (continueRecursion)
        {
            foreach (var child in node.Children)
            {
                TraverseTree(child, splitedTags, result, attribute, nodeIndex, command, newValue);
            }
        }
    }

    private static string ExtractOnlyTag(string tagWithAttr)
    {
        int endIdx = tagWithAttr.IndexOf(' ');

        if (endIdx != -1)
        {
            return tagWithAttr.Substring(0, endIdx) + '>';
        }
        else
        {
            return tagWithAttr;
        }
    }

    private static bool IsContained(string item, char symbol)
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] == symbol)
            {
                return true;
            }
        }

        return false;
    }

    private static int FindOpeningBracket(string elementPath, int startIndex)
    {
        for (int i = 0; i < elementPath.Length; i++)
        {
            if (elementPath[i] == '[')
            {
                return i;
            }
        }

        return -1;
    }

    private static int FindClosingBracket(string elementPath, int startIndex)
    {
        for (int i = 0; i < elementPath.Length; i++)
        {
            if (elementPath[i] == ']')
            {
                return i;
            }
        }

        return -1;
    }

    private static char FindChildIndex(string XPath)
    {
        for (int i = 0; i < XPath.Length; i++)
        {
            if (XPath[i] == '[' && XPath[i + 2] == ']')
            {
                return XPath[i + 1];
            }
        }

        return '\0';
    }

}