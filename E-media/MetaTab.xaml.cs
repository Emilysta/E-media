using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;

namespace E_media
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

    public sealed partial class MetaTab : Page
    {

        List<string> markerWithParameters = new List<string>() { "<svg", "<?xml", "<!DOCTYPE" };
        List<string> markerWithContent = new List<string>() { "title", "metadata", "desc", "<!--" };
        public int myID;
        public List<XMLPair> Metadata { get; set; }

        List<XMLNode> Nodes { get; set; }
        public MetaTab()
        {
            this.InitializeComponent();
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

        private async void ReadMetadata()
        {
            IRandomAccessStream stream = await ImageListClass.FileSourceList[myID].OpenAsync(FileAccessMode.Read);
            Metadata = new List<XMLPair>();
            using (StreamReader reader = new StreamReader(stream.AsStreamForRead()))
            {
                string tempLine;
                string line = "";
                while ((tempLine = reader.ReadLine()) != null)
                {
                    tempLine = tempLine.Trim();
                    if (tempLine.StartsWith("<"))
                    {
                        line = tempLine;
                    }
                    else
                    {
                        line += " ";
                        line += tempLine;
                    }
                    if (line.EndsWith(">"))
                    {
                        int j = 0;
                        for (int i = 0; i < markerWithParameters.Count; i++)
                        {
                            if (line.StartsWith(markerWithParameters[i]))
                            {
                                ReadMarkerLikeSVG(line);
                                j = markerWithContent.Count;
                                break;
                            }
                        }
                        for (int i = j; i < markerWithContent.Count; i++)
                        {
                            if (line.StartsWith("<" + markerWithContent[i] + ">") && line.EndsWith("</" + markerWithContent[i] + ">"))
                            {
                                ReadMarkerLikeTitle(line, markerWithContent[i]);
                                break;
                            }
                        }

                    }
                }
            }
            metaList.ItemsSource = Metadata;
        }

        private void ReadMarkerLikeTitle(string line, string marker)
        {
            line = line.Replace("<" + marker + ">", "");
            line = line.Replace("</" + marker + ">", "");
            Metadata.Add(new XMLPair()
            {
                PropertyName = marker,
                Value = line
            });
        }

        private void ReadMarkerLikeSVG(string line)
        {
            string[] pairs = line.Split();
            int lastIndex = pairs.Count() - 1;
            pairs[lastIndex] = pairs[lastIndex].Trim('>');
            if (line.StartsWith("<?xml"))
            {
                pairs[lastIndex] = pairs[lastIndex].Trim('?');
            }

            for (int i = 0; i < lastIndex + 1; i++)
            {
                string temp = pairs[i];
                if (temp.Contains('='))
                {
                    if (temp.EndsWith('"'))
                    {
                        Metadata.Add(ReadXMLPair(temp));
                    }
                    else
                    {
                        string temp2 = temp;
                        while (!temp2.EndsWith('"'))
                        {
                            ++i;
                            temp2 += " ";
                            temp2 += pairs[i];
                        }
                        Metadata.Add(ReadXMLPair(temp2));
                    }
                }
            }
        }

        private async void ReadMetadata2()
        {
            Nodes = new List<XMLNode>();
            string content;
            IRandomAccessStream stream = await ImageListClass.FileSourceList[myID].OpenAsync(FileAccessMode.Read);
            using (StreamReader reader = new StreamReader(stream.AsStreamForRead()))
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myID = (int)e.Parameter;
            ReadMetadata2();
        }
    }
}
