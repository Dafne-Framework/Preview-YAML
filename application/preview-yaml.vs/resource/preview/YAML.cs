
using System;
using System.Collections;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace resource.preview
{
    public class YAML : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            var a_Context = new Deserializer();
            try
            {
                var a_Context1 = "";
                {
                    __Execute(a_Context.Deserialize<dynamic>(new StringReader(File.ReadAllText(url))) as IEnumerable, 0, context, "", NAME.PATTERN.ELEMENT, ref a_Context1);
                }
                if (GetState() == STATE.CANCEL)
                {
                    context.
                        SendWarning(1, NAME.WARNING.TERMINATED);
                }
            }
            catch (YamlException ex)
            {
                context.
                    SetUrl(url).
                    SetUrlLine(ex.Start.Line).
                    SetUrlPosition(ex.Start.Column).
                    SendError(1, __GetErrorMessage(ex.Message));
            }
        }

        private static void __Execute(Object node, int level, atom.Trace context, string name, string pattern, ref string trail)
        {
            if (node == null)
            {
                return;
            }
            if (GetState() == STATE.CANCEL)
            {
                return;
            }
            if (string.IsNullOrEmpty(name) == false)
            {
                context.
                    Clear().
                    SetContent(name).
                    SetValue(__GetValue(node)).
                    SetComment(__GetComment(node, pattern)).
                    SetPattern(__GetPattern(node, pattern)).
                    SetCommentHint("[[Data type]]").
                    SetLevel(level).
                    Send();
            }
            {
                var a_Context = node.GetHashCode() + ";";
                if (trail.Contains(a_Context))
                {
                    return;
                }
                {
                    trail += a_Context;
                }
            }
            if (node is IList)
            {
                var a_Context = node as IList;
                var a_Index = 0;
                foreach (var a_Context1 in a_Context)
                {
                    {
                        a_Index++;
                    }
                    {
                        __Execute(a_Context1, level + 1, context, "[" + a_Index.ToString() + "]", NAME.PATTERN.VARIABLE, ref trail);
                    }
                }
            }
            if (node is IDictionary)
            {
                var a_Context = node as IDictionary;
                foreach (string a_Context1 in a_Context.Keys)
                {
                    __Execute(a_Context[a_Context1], level + 1, context, a_Context1, NAME.PATTERN.PARAMETER, ref trail);
                }
            }
        }

        private static string __GetErrorMessage(string value)
        {
            var a_Index = value.IndexOf("): ");
            if (a_Index > 0)
            {
                return GetCleanString(value.Substring(a_Index + 3));
            }
            return value;
        }

        private static string __GetValue(object node)
        {
            if ((node is IList) || (node is IDictionary))
            {
                return "";
            }
            return GetCleanString(node.ToString());
        }

        private static string __GetComment(object node, string pattern)
        {
            if (node is IList)
            {
                return "[[Array]]";
            }
            if (node is IDictionary)
            {
                return "[[Object]]";
            }
            if (pattern == NAME.PATTERN.PARAMETER)
            {
                return "[[Property]]";
            }
            return "[[Item]]";
        }

        private static string __GetPattern(object node, string pattern)
        {
            if ((node is IList) || (node is IDictionary))
            {
                return NAME.PATTERN.ELEMENT;
            }
            return pattern;
        }
    };
}
