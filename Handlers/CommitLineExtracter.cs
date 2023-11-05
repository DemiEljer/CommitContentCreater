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

        private static int _MultilineCommentIndex { get; set; } = 0;

        public CommitLineModel? ExtractLine(string line)
        {
            var lineType = CommitLineHandler.IsCommitLine(line);

            if (_State == FileParsingState.MulltilineCommentReading
                || lineType == CommitLineType.SingleCommitLine
                || lineType == CommitLineType.StartOfCommitLines
                || (lineType == CommitLineType.EndOfCommitLines && _State == FileParsingState.MulltilineCommentReading))
            {
                var commitLineModel = CommitLineHandler.CreateCommitLine(line);
                // Фиксация индекса многострочного комментария
                if (lineType == CommitLineType.StartOfCommitLines
                    || _State == FileParsingState.MulltilineCommentReading)
                {
                    commitLineModel.MultilineCommentIndex = _MultilineCommentIndex;
                }
                
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
                            _MultilineCommentIndex++;
                        }
                        break;
                }

                return commitLineModel;
            }
            else
            {
                return null;
            }
        }
    }
}
