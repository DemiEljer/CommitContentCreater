using CommitContentCreater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater.Handlers
{
    public class CommitLineExtracter
    {
        private FileParsingState _State { get; set; } = FileParsingState.OrdinaryReading;

        public CommitLineModel? ExtractLine(string line)
        {
            var lineType = CommitLineHandler.IsCommitLine(line);

            if (_State == FileParsingState.MulltilineCommentReading
                || lineType == CommitLineType.SingleCommitLine
                || lineType == CommitLineType.StartOfCommitLines
                || (lineType == CommitLineType.EndOfCommitLines && _State == FileParsingState.MulltilineCommentReading))
            {
                switch (_State)
                {
                    case FileParsingState.OrdinaryReading:
                        if (lineType == CommitLineType.StartOfCommitLines)
                        {
                            _State = FileParsingState.MulltilineCommentReading;
                        }
                        break;

                    case FileParsingState.MulltilineCommentReading:
                        if (lineType == CommitLineType.EndOfCommitLines)
                        {
                            _State = FileParsingState.OrdinaryReading;
                        }
                        break;
                }

                return CommitLineHandler.CreateCommitLine(line);
            }
            else
            {
                return null;
            }
        }
    }
}
