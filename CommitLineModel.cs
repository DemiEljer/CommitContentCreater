﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class CommitLineModel
    {
        private string _Line { get; set; } = "";

        public string Line 
        {
            get => _Line;
            set => _Line =  value == null ? "" : value;
        }

        public int Priority { get; set; } = int.MaxValue;

        public bool IsInited => !string.IsNullOrEmpty(_Line);
    }
}
