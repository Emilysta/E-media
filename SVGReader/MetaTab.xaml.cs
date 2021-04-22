using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SVGReader
{

    public class XMLPair
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
    public class XMLNode
    {
        public string Name { get; set; }
        public List<XMLPair> Attributes { get; set; }
        public List<XMLNode> Children { get; set; }
        public XMLNode Parent { get; set; }
        public string Content { get; set; }
    }
    /// <summary>
    /// Logika interakcji dla klasy MetaTab.xaml
    /// </summary>
    public partial class MetaTab : Page
    {
        List<string> metadataToRead = new List<string> { "?xml", "!DOCTYPE", "svg", "title", "desc" };
        public int myID;
        public List<XMLNode> Metadata { get; set; }
        public List<XMLNode> Nodes { get; set; }
        NavPage navPage;

        public MetaTab(int id, NavPage parentNavPage)
        {
            this.InitializeComponent();
            myID = id;
            navPage = parentNavPage;
            Metadata = new List<XMLNode>();
        }

        public void StartMethod()
        {
            ParseSVGFile();
            CountStandardShapes();
            ReadMetadata();
        }

        private XMLPair ReadXMLPair(string keyValuePair)
        {
            string[] s = keyValuePair.Split('=');
            XMLPair pair = new XMLPair();
            pair.PropertyName = s[0];
            s[1] = s[1].TrimStart('"');
            s[1] = s[1].TrimEnd('"');
            pair.Value = s[1];
            //Debug.WriteLine(pair.PropertyName + "=" + pair.Value);
            return pair;
        }

        private void ParseSVGFile()
        {
            Nodes = new List<XMLNode>();
            string content;
            using (StreamReader reader = new StreamReader(FileManager.FileSourceList[myID]))
            {
                content = reader.ReadToEnd();
            }
            content = content.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            string pattern = "(?<=[>])";
            string[] lines = Regex.Split(content, pattern);

            XMLNode currentNode = null;
            XMLNode currentParent = null;

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith("</")) //Pomin linijki zamykajace tag
                {
                    currentNode = currentParent;
                    if (currentParent != null)
                        currentParent = currentParent.Parent;
                    else
                        currentParent = null;
                    continue;
                }

                if (lines[i].StartsWith('<')) //Otwarcie tagu
                {
                    XMLNode node = ReadNode(lines[i]);
                    Nodes.Add(node);
                    currentParent = currentNode;
                    currentNode = Nodes.Last();
                    if (currentParent != null)
                        currentParent.Children.Add(currentNode);
                    currentNode.Parent = currentParent;
                }
                else
                {
                    string tmp = lines[i].Split("</")[0];
                    Nodes.Last().Content = tmp;
                    if (lines[i].Contains("</"))
                    {
                        currentNode = currentParent;
                        if (currentParent != null)
                            currentParent = currentParent.Parent;
                        else
                            currentParent = null;
                    }
                }

                if (lines[i].EndsWith("/>") || lines[i].EndsWith("?>"))
                {
                    currentNode = currentParent;
                    if (currentParent != null)
                        currentParent = currentParent.Parent;
                    else
                        currentParent = null;
                }

            }
        }
        private XMLNode ReadNode(string line)
        {
            string[] words = line.Split();
            string temp = line.TrimStart('<').TrimStart('/').TrimEnd('>').TrimEnd('/').Split()[0];
            XMLNode node = new XMLNode();
            node.Name = temp;
            node.Attributes = new List<XMLPair>();
            node.Children = new List<XMLNode>();

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("="))
                {
                    string s = words[i].TrimEnd('>').TrimEnd('/').TrimEnd('?');
                    if (!s.EndsWith('"'))
                    {
                        int j = 1;
                        while (!s.EndsWith('"'))
                        {
                            s += " " + words[i + j].TrimEnd('>').TrimEnd('/').TrimEnd('?');
                            j++;
                        }
                    }
                    node.Attributes.Add(ReadXMLPair(s));
                }
            }
            return node;
        }

        private void CountStandardShapes()
        {
            int rectCount = Nodes.Where(x => x.Name == "rect").Count();
            int ellipseCount = Nodes.Where(x => x.Name == "ellipse").Count();
            int circleCount = Nodes.Where(x => x.Name == "circle").Count();
            int lineCount = Nodes.Where(x => x.Name == "line").Count();
            int pathCount = Nodes.Where(x => x.Name == "path").Count();
            int textCount = Nodes.Where(x => x.Name == "text").Count();
            int polylineCount = Nodes.Where(x => x.Name == "polyline").Count();
            int polygonCount = Nodes.Where(x => x.Name == "polygon").Count();
            int gradientsCount = Nodes.Where(x => x.Name == "linearGradient").Count();
            gradientsCount += Nodes.Where(x => x.Name == "radialGradient").Count();
            CountOfRectanglesText.Text = rectCount.ToString();
            CountOfElipsesText.Text = ellipseCount.ToString();
            CountOfCirclesText.Text = circleCount.ToString();
            CountOfLinesText.Text = lineCount.ToString();
            CountOfPathsText.Text = pathCount.ToString();
            CountOfTextsText.Text = textCount.ToString();
            CountOfPolylinesText.Text = polylineCount.ToString();
            CountOfPolygonsText.Text = polygonCount.ToString();
            CountOfGradientsText.Text = gradientsCount.ToString();
        }

        private void ReadMetadata()
        {
            foreach (string metadata in metadataToRead)
            {
                bool contains = Nodes.Any(x => x.Name == metadata);
                if (contains)
                {
                    Metadata.Add(Nodes.Find(x => x.Name == metadata));
                }
            }
            bool haveMetadata = Nodes.Any(x => x.Name == "metadata");
            if (haveMetadata)
            {
                XMLNode metadataNode = Nodes.Find(x => x.Name == "metadata");
                XMLNode ccWorkNode;
                if (metadataNode.Children.Count != 0)
                {
                    if (metadataNode.Children[0].Children.Count != 0)
                    {
                        ccWorkNode = metadataNode.Children[0].Children[0];
                        List<XMLNode> listOfNodesWithoutCCAgent = ccWorkNode.Children.FindAll(x => x.Children.Count == 0);
                        List<XMLNode> listOfNodesWithCCAgent = ccWorkNode.Children.FindAll(x => x.Children.Count != 0);
                        XMLNode metaNodeToDisplay = new XMLNode();
                        metaNodeToDisplay.Name = "metadata";
                        metaNodeToDisplay.Attributes = new List<XMLPair>();
                        if (metadataNode.Attributes.Count != 0)
                        {
                            foreach (XMLPair attr in metadataNode.Attributes)
                            {
                                metaNodeToDisplay.Attributes.Add(attr);
                            }
                        }
                        metaNodeToDisplay.Children = new List<XMLNode>();

                        foreach (XMLNode node in listOfNodesWithoutCCAgent)
                        {
                            metaNodeToDisplay.Attributes.Add(new XMLPair()
                            {
                                PropertyName = node.Name,
                                Value = node.Content
                            });
                        }
                        foreach (XMLNode node in listOfNodesWithCCAgent)
                        {
                            XMLNode dctitle = node.Children[0].Children[0];
                            if (dctitle.Name == "dc:title")
                            {
                                metaNodeToDisplay.Attributes.Add(new XMLPair()
                                {
                                    PropertyName = node.Name,
                                    Value = dctitle.Content
                                });
                            }
                        }
                        Metadata.Add(metaNodeToDisplay);
                    }
                }
            }
            metaList.ItemsSource = Metadata;
            DataContext = this;
        }
    }
}
