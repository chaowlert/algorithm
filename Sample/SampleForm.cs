using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Chaow.Threading;

namespace Chaow.Sample
{
    public partial class SampleForm : Form
    {
        //constants
        //const string Keyword = "var|abstract|event|new|struct|as|explicit|null|switch|base|extern|object|this|bool|false|operator|throw|break|finally|out|true|byte|fixed|override|try|case|float|params|typeof|catch|for|private|uint|char|foreach|protected|ulong|checked|goto|public|unchecked|class|if|readonly|unsafe|const|implicit|ref|ushort|continue|in|return|using|decimal|int|sbyte|virtual|default|interface|sealed|volatile|delegate|internal|short|void|do|is|sizeof|while|double|lock|stackalloc|else|long|static|enum|namespace|string|from|get|group|into|join|let|orderby|partial|select|set|value|where|yield";
        const string Keyword = "var|by|new|as|null|switch|object|bool|false|throw|break|finally|out|true|byte|fixed|try|case|float|typeof|catch|for|uint|char|foreach|ulong|checked|goto|unchecked|if|unsafe|const|ref|ushort|continue|in|return|using|decimal|int|sbyte|default|delegate|short|do|is|sizeof|while|double|lock|stackalloc|else|long|string|from|group|into|join|let|orderby|select|where";

        //static fields
        static readonly Regex keywordSearcher = createRegex(0);

        //fields
        Worker worker = Worker.Empty;

        //constructors
        public SampleForm()
        {
            InitializeComponent();
            Console.SetOut(new CallbackWriter(appendText));
        }

        //events
        void SampleForm_Load(object sender, EventArgs e)
        {
            new Action(loadFiles).Run();
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;
            fillCode((string)e.Node.Tag);
            textBox1.Clear();
            worker.UnsafeAbort();
            if (e.Node.ImageIndex == 2)
                worker = new Action(() => execute(e.Node)).Run();
        }

        void Link_Clicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        //private methods
        void loadFiles()
        {
            var directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "Samples", "*", SearchOption.AllDirectories);
            Array.Sort(directories);
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.cs");
                foreach (var file in files)
                    loadFile(file);
            }
        }

        void loadFile(string path)
        {
            foreach (var found in SourceReader.Read(path))
            {
                TreeNode ns;
                if (!treeView1.Nodes.ContainsKey(found[0]))
                    ns = addNode(treeView1.Nodes, found[0], found[0], 0, 0);
                else
                    ns = treeView1.Nodes[found[0]];

                if (found[1] == null)
                {
                    setTag(ns, found[3]);
                    continue;
                }

                TreeNode cls;
                if (!ns.Nodes.ContainsKey(found[1]))
                    cls = addNode(ns.Nodes, found[1], found[1].Replace('_', ' '), 1, 1);
                else
                    cls = ns.Nodes[found[1]];

                if (found[2] == null)
                {
                    setTag(cls, found[3]);
                    continue;
                }

                var mthd = addNode(cls.Nodes, found[2], found[2].Replace('_', ' '), 2, 2);
                setTag(mthd, found[3]);
            }
        }

        void fillCode(string code)
        {
            richTextBox1.Clear();
            richTextBox1.Text = code;
            foreach (Match m in keywordSearcher.Matches(code))
            {
                if (m.Groups["comment"].Success)
                {
                    richTextBox1.Select(m.Index, m.Length);
                    richTextBox1.SelectionColor = Color.Green;
                }
                else if (m.Groups["string"].Success)
                {
                    richTextBox1.Select(m.Index, m.Length);
                    richTextBox1.SelectionColor = Color.Brown;
                }
                else if (m.Groups["keyword"].Success)
                {
                    richTextBox1.Select(m.Index, m.Length);
                    richTextBox1.SelectionColor = Color.Blue;
                }
                else
                {
                    richTextBox1.Select(m.Index, m.Length);
                    richTextBox1.SelectionColor = Color.FromArgb(43, 145, 175);
                }
            }
        }

        void appendText(string value)
        {
            Invoke(new Action<string>(textBox1.AppendText), new object[] {value});
        }

        TreeNode addNode(TreeNodeCollection nodes, string key, string text, int imageIndex, int selectedImageIndex)
        {
            return (TreeNode)Invoke(new Func<string, string, int, int, TreeNode>(nodes.Add), new object[] {key, text, imageIndex, selectedImageIndex});
        }

        void setTag(TreeNode node, object obj)
        {
            Invoke(new Action<object>(o => node.Tag = o), new[] {obj});
        }

        //static methods
        static Regex createRegex(int id)
        {
            switch (id)
            {
                case 0:
                    return new Regex(@"(                    #any of
                    (?'comment'//.*\n)|                 #- single line comment
                    (?'comment'/\*([^*]|\*(?!/))*\*/)|  #- multi line comment
                    (?'string''[^']+')|                 #- char
                    (?'string'@""([^""]|"""")*"")|      #- @string
                    (?'string'""([^""]|(?<=\\)"")*"")|  #- string
                    (?'keyword'(?<!@)\b(" + Keyword + @")\b)| #- keyword
                    (?'class'(?<!\.)\b[\p{Lu}][\w]*\b)) #- class"
                        , RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                default:
                    return null;
            }
        }

        static void execute(TreeNode node)
        {
            var type = Type.GetType(string.Format("{0}.{1}", node.Parent.Parent.Name, node.Parent.Name));
            var method = type.GetMethod(node.Name, BindingFlags.Public | BindingFlags.Static);

            try
            {
                var sw = new Stopwatch();
                sw.Start();
                method.Invoke(null, null);
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}