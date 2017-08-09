﻿using BioEngine.Data.Core;
using JetBrains.Annotations;

namespace BioEngine.Data.News.Commands
{
    [UsedImplicitly]
    public class CreateNewsCommand : CreateCommand<int>, IChildModelCommand
    {
        public string Source { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string ShortText { get; set; }
        public string AddText { get; set; }
        public int AuthorId { get; set; }
        public int? GameId { get; set; }
        public int? DeveloperId { get; set; }
        public int? TopicId { get; set; }
        public long Date { get; set; }
        public long LastChangeDate { get; set; }
    }
}