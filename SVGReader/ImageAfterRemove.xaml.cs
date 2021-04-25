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
    public class StringCounter
    {
        public string s_text { get; set; }
        public int s_count { get; set; }
        public XMLNode s_node { get; set; }
        public XMLPair s_pair { get; set; }
    }

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

            List<string> metaToRemove = new List<string> { "?xml", "!DOCTYPE", "metadata", "title", "desc" };
            foreach (string metaName in metaToRemove)
            {
                if (nodes.Any(x => x.Name == metaName))
                {
                    XMLNode nodeToRemove = nodes.Find(x => x.Name == metaName);
                    if (nodeToRemove.Children.Count == 0)
                    {
                        if (nodeToRemove.Parent == null)
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
            XMLNode defsNode = nodes.Find(x => x.Name == "defs");
            XMLNode svgNode = nodes.Find(x => x.Name == "svg");
            if (defsNode != null)
            {
                if (defsNode.Children.Count == 0)
                {
                    svgNode.Children.Remove(defsNode);
                    nodes.Remove(defsNode);
                }
            }

            XMLNode sodipodi = nodes.Find(x => x.Name == "sodipodi:namedview");
            if (sodipodi != null)
            {
                svgNode.Children.Remove(sodipodi);
                nodes.Remove(sodipodi);
            }
            List<XMLPair> attr = svgNode.Attributes.ToList();
            XMLPair xmlns = attr.Find(x => x.PropertyName == "xmlns");
            attr.Remove(xmlns);
            foreach (XMLPair pair in svgNode.Attributes)
            {
                if (!pair.PropertyName.StartsWith("xmlns:"))
                {
                    attr.Remove(pair);
                }
            }
            svgNode.Attributes = attr;

            bool[] toStay = new bool[svgNode.Attributes.Count];
            for (int i = 0; i < svgNode.Attributes.Count; i++)
            {
                toStay[i] = false;
            }

            string toTrim = "xmlns:";
            for (int counter = 0; counter < svgNode.Attributes.Count; counter++)
            {
                XMLPair pair = svgNode.Attributes[counter];
                foreach (XMLNode node in nodes)
                {
                    if (node.Name != "svg")
                    {
                        if (node.Attributes.Count != 0)
                        {
                            foreach (XMLPair pairs in node.Attributes)
                            {
                                string name = pair.PropertyName.Replace(toTrim, "");
                                if (pairs.PropertyName.StartsWith(name))
                                {
                                    toStay[counter] = true;
                                }
                            }
                        }
                    }
                }
            }
            int n = svgNode.Attributes.Count;
            for (int j = n - 1; j >= 0; j--)
            {
                if (toStay[j] == false)
                {
                    svgNode.Attributes.RemoveAt(j);
                }
            }
            svgNode.Attributes.Add(xmlns);
            RemoveNotNeededIDs(ref nodes);
            RemoveChildren(ref nodes, nodes.Find(x => x.Name == "svg").Children);
            string path = MakeFileNamePath();
            SaveToFile(nodes, path);
            OpenFile(path);
        }

        private void RemoveNotNeededIDs(ref List<XMLNode> nodes)
        {
            List<StringCounter> stringCounters = new List<StringCounter>();
            List<XMLPair> toCheck = new List<XMLPair>();
            foreach (XMLNode node in nodes)
            {
                foreach (XMLPair attr in node.Attributes)
                {
                    if (attr.PropertyName == "id")
                    {
                        toCheck.Add(attr);
                        stringCounters.Add(new StringCounter
                        {
                            s_count = 0,
                            s_node = node,
                            s_pair = attr,
                            s_text = attr.Value
                        });
                    }
                }
            }

            for (int i = 0; i < stringCounters.Count; i++)
            {
                StringCounter counter = stringCounters[i];
                foreach (XMLNode node in nodes)
                {
                    foreach (XMLPair attr in node.Attributes)
                    {
                        if (attr.Value.Contains("#" + counter.s_text))
                        {
                            counter.s_count = counter.s_count + 1;
                        }
                    }
                }
            }
            foreach (StringCounter counter in stringCounters)
            {
                if (counter.s_count == 0)
                {
                    List<XMLPair> temp = counter.s_node.Attributes.ToList();
                    temp.Remove(counter.s_pair);
                    counter.s_node.Attributes = temp;
                }
            }
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
                    fileName = fileName.Replace(ending, "");
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
