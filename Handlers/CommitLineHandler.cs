using CommitContentCreater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class CommitLineHandler
    {
        public static CommitLineType IsCommitLine(string originLine)
        {
            if (originLine.Contains("//~"))
            {
                return CommitLineType.SingleCommitLine;
            }
            else if (originLine.Contains("/*~"))
            {
                return CommitLineType.StartOfCommitLines;
            }
            else if (originLine.Contains("*/") || originLine.Contains("~*/"))
            {
                return CommitLineType.EndOfCommitLines;
            }
            else
            {
                return CommitLineType.NotACommitLine;
            }
        }

        public static CommitLineModel CreateCommitLine(string commitLine)
        {
            commitLine = commitLine.Replace("//~", "").Replace("/*~", "").Replace("~*/", "").Replace("*/", "").Replace("*", "").Trim();

            Func<string, string, string, string> modifierDelegate = (linePart, key, value) =>
            {
                if (linePart == $"#{key.ToLower()}"
                    || linePart == $"#{key.ToUpper()}")
                {
                    return $"[{value.ToUpper()}]";
                }
                else
                {
                    return linePart;
                }
            };

            Func<string, int> checkPrioriryDelegate = (linePart) =>
            {
                if (linePart.StartsWith("#"))
                {
                    try
                    {
                        return int.Parse(linePart.Substring(1));
                    }
                    catch
                    {
                        
                    }
                }

                return -1;
            };

            Func<string, bool> checkIsCapitalDelegate = (linePart) =>
            {
                return linePart == "#!";
            };

            Func<string, bool> checkIsVersionDelegate = (line) =>
            {
                return commitLine.StartsWith("v") || commitLine.StartsWith("V");
            };

            Action<CommitLineModel> normilizeDelegate = (commitLine) =>
            {
                if (!commitLine.IsCapital 
                    && !commitLine.IsVersion
                    && commitLine.IsInited)
                {
                    if (!commitLine.Line.StartsWith("-"))
                    {
                        commitLine.Line = "- " + commitLine.Line;
                    }
                    else
                    {
                        string tabs = "";

                        while (commitLine.Line.StartsWith("--"))
                        {
                            commitLine.Line = commitLine.Line.Substring(1);
                            tabs += "\t";
                        }

                        commitLine.Line = tabs + commitLine.Line;
                    }
                }
            };

            CommitLineModel resultCommitLine = new CommitLineModel();

            if (checkIsVersionDelegate(commitLine))
            {
                resultCommitLine.Line = commitLine;

                resultCommitLine.IsVersion = true;
            }
            else
            {
                var commitLineElements = commitLine.Split(" ").Where(e => !string.IsNullOrEmpty(e)).ToArray();

                for (int i = 0; i < commitLineElements.Length; i++)
                {
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "f", "FIX");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "m", "MODIFY");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "e", "EVENT");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "u", "UPDATE");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "a", "ADD");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "i", "INFORMATION");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "n", "NOTE");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "c", "CORRECTION");
                    commitLineElements[i] = modifierDelegate(commitLineElements[i], "im", "IMPROVEMENT");

                    var isCapital = checkIsCapitalDelegate(commitLineElements[i]);
                    resultCommitLine.IsCapital |= isCapital;
                    if (isCapital)
                    {
                        commitLineElements[i] = "";
                    }

                    var priority = checkPrioriryDelegate(commitLineElements[i]);
                    resultCommitLine.Priority = priority >= 0 ? priority : resultCommitLine.Priority;
                    if (priority >= 0)
                    {
                        commitLineElements[i] = "";
                    }
                }

                resultCommitLine.Line = string.Join(" ", commitLineElements.Where(e => !string.IsNullOrEmpty(e)));
                normilizeDelegate(resultCommitLine);
            }



            return resultCommitLine;
        }
    }
}
