using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.CodeDom.Compiler;
using System.IO;


namespace SVGReader
{
    /// <summary>
    /// Interaction logic for ImageAfterRemove.xaml
    /// </summary>
    public partial class ImageAfterRemove : Page
    {
        public int myID;
        NavPage navPage;
        public ImageAfterRemove(int id, NavPage parentNavPage)
        {
            this.InitializeComponent();
            myID = id;
            navPage = parentNavPage;
        }

        public void StartMethod()
        {

            List<XMLNode> nodes = navPage.metatab.Nodes;

            List<string> metaToRemove = new List<string> { "?xml", "!DOCTYPE", "metadata", "title", "desc", "defs" };
            foreach (string metaName in metaToRemove)
            {
                if (nodes.Any(x => x.Name == metaName))
                {
                    XMLNode nodeToRemove = nodes.Find(x => x.Name == metaName);
                    if (nodeToRemove.Children.Count == 0)
                    {
                        if(nodeToRemove.Parent == null)
                            nodes.Remove(nodeToRemove);
                        else
                        {
                            XMLNode parent = nodeToRemove.Parent;
                            parent.Children.Remove(nodeToRemove);
                            nodes.Remove(nodeToRemove);
                        }
                    }
                    else
                    {
                        RemoveChildren(ref nodes, nodeToRemove.Children);
                        if (nodeToRemove.Parent == null)
                            nodes.Remove(nodeToRemove);
                        else
                        {
                            XMLNode parent = nodeToRemove.Parent;
                            parent.Children.Remove(nodeToRemove);
                            nodes.Remove(nodeToRemove);
                        }
                    }
                }
            }
            RemoveChildren(ref nodes, nodes.Find(x => x.Name == "svg").Children);
            string path = MakeFileNamePath();
            SaveToFile(nodes, path);
            OpenFile(path);
        }

        private void OpenFile(string file)
        {
            SVGImageAfterRemove.Source = new Uri(file);
        }

        private void RemoveChildren(ref List<XMLNode> nodes, List<XMLNode> children)
        {
            foreach (XMLNode kid in children)
            {
                if (kid.Children.Count == 0)
                {
                    nodes.Remove(kid);
                }
                else
                {
                    RemoveChildren(ref nodes, kid.Children);
                    nodes.Remove(kid);
                }
            }
        }

        private string MakeFileNamePath()
        {
            string fileName = "";
            if (FileManager.FileSourceList.TryGetValue(myID, out string file))
            {
                if (file != null)
                {
                    fileName = file;
                    string ending = ".svg";
                    char[] end = ending.ToArray();
                    fileName = fileName.TrimEnd(end);
                    fileName += "_MRemoved.svg";
                    //Trace.WriteLine(fileName);
                }
            }
            return fileName;
        }

        private void SaveToFile(List<XMLNode> nodes, string filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false);
            IndentedTextWriter indentWriter = new IndentedTextWriter(writer, "  ");
            RecursiveSave(nodes, indentWriter);
            indentWriter.Close();
            writer.Close();
        }

        private void RecursiveSave(List<XMLNode> nodes, IndentedTextWriter writer)
        {
            foreach (var item in nodes)
            {

                if (item.Attributes.Count != 0)
                {
                    writer.WriteLine("<" + item.Name);
                    writer.Indent++;
                    foreach (XMLPair attr in item.Attributes)
                    {
                        if (attr == item.Attributes.Last())
                        {
                            if (item.Children.Count != 0 || item.Content != null)
                            {
                                writer.WriteLine(attr.PropertyName + "=\"" + attr.Value + "\"  >");
                            }
                            else
                            {
                                if (item.Name == "?xml")
                                {
                                    writer.WriteLine(attr.PropertyName + "=\"" + attr.Value + "\" ?>");
                                }
                                else if (item.Name == "!DOCTYPE")
                                {
                                    writer.WriteLine(attr.PropertyName + "=\"" + attr.Value + "\"  >");
                                }
                                else
                                {
                                    writer.WriteLine(attr.PropertyName + "=\"" + attr.Value + "\"  />");
                                }
                                writer.Indent--;
                            }
                        }
                        else
                        {
                            writer.WriteLine(attr.PropertyName + "=\"" + attr.Value + "\"");
                        }
                    }
                }
                else
                {
                    writer.WriteLine("<" + item.Name + ">");
                    writer.Indent++;
                }

                if (item.Content != null)
                {
                    writer.WriteLine(item.Content);
                    writer.Indent--;
                    writer.WriteLine("</" + item.Name + ">");
                }
                else
                {
                    if (item.Children.Count == 0 && item.Attributes.Count == 0)
                    {
                        writer.Indent--;
                        writer.WriteLine("</" + item.Name + ">");
                    }
                }
                if (item.Children.Count != 0)
                {
                    RecursiveSave(item.Children, writer);
                    writer.Indent--;
                    writer.WriteLine("</" + item.Name + ">");
                }
            }
        }
    }
}
