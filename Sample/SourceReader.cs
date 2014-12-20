using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Chaow.Sample
{
    public static class SourceReader
    {
        //constants
        const string regContent = @"
            (                                       #any of
                (?://.*)|                           #- single line comment
                (?:/\*([^*]|\*(?!/))*\*/)|          #- multi line comment
                [^{]                                #- any except open brace
            )*
            \{(?'content'(                          #start content any of
                (?://.*\n)|                         #- single line comment
                (?:/\*([^*]|\*(?!/))*\*/)|          #- multi line comment
                (?:'[^']+')|                        #- char
                (?:@""([^""]|"""")*"")|             #- @string
                (?:""([^""]|(?<=\\)"")*"")|         #- string
                (?'brace'\{)|                       #- open brace
                (?(brace)                           #  * on brace open
                    ([^}]|(?'-brace'}))|            #    . match any char or close brace
                    [^}])                           #  * else match any except close brace
            )*(?(brace)(?!)))}                      #failed on brace is openned";

        //static fields
        static readonly Regex matchNS = createRegex(0);
        static readonly Regex matchCls = createRegex(1);
        static readonly Regex matchMthd = createRegex(2);
        static readonly Regex decreaseInd = createRegex(3);

        //static methods
        static Regex createRegex(int id)
        {
            switch (id)
            {
                case 0:
                    return new Regex(@"
                    (?'summary'(\s*//.*\n)+)?\s*
                    namespace\s+(?'namespace'[^\s/{]+)" + regContent,
                        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                case 1:
                    return new Regex(@"
                    (?'summary'(\s*//.*\n)+)?\s*
                    public\s+(static\s+)?class\s+(?'class'[^\s/{]+)" + regContent,
                        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                case 2:
                    return new Regex(@"public\s+static\s+void\s+(?'method'[^\s(]+)\s*\(\s*\)" + regContent,
                        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                case 3:
                    return new Regex(@"(?'indent'\n[\t ]+)", RegexOptions.Compiled | RegexOptions.Multiline);
                default:
                    return null;
            }
        }

        static string adjustContent(string cnt)
        {
            var match = decreaseInd.Match(cnt);
            if (match.Success)
                cnt = cnt.Replace(match.Value, "\n");
            cnt = cnt.Replace("\r\n", "\n");
            var startIndex = 0;
            while (startIndex < cnt.Length && cnt[startIndex] == '\n')
                startIndex++;
            return cnt.Substring(startIndex);
        }

        //public static methods
        public static IEnumerable<string[]> Read(string path)
        {
            //File not found, exit
            if (!File.Exists(path))
                yield break;

            //Read sample file
            string content;
            using (var reader = File.OpenText(path))
                content = reader.ReadToEnd();

            //Read and match content
            foreach (Match m in matchNS.Matches(content))
            {
                var ns = m.Groups["namespace"].Value;
                if (m.Groups["summary"].Success)
                    yield return new[] {ns, null, null, adjustContent(m.Groups["summary"].Value)};
                foreach (Match m2 in matchCls.Matches(m.Groups["content"].Value))
                {
                    var cls = m2.Groups["class"].Value;
                    if (m2.Groups["summary"].Success)
                        yield return new[] {ns, cls, null, adjustContent(m2.Groups["summary"].Value)};
                    foreach (Match m3 in matchMthd.Matches(m2.Groups["content"].Value))
                        yield return new[] {ns, cls, m3.Groups["method"].Value, adjustContent(m3.Groups["content"].Value)};
                }
            }
        }
    }
}