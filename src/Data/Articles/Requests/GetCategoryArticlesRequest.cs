﻿using System.Collections.Generic;
using BioEngine.Common.Models;
using BioEngine.Data.Core;

namespace BioEngine.Data.Articles.Requests
{
    public class GetCategoryArticlesRequest : RequestBase<(IEnumerable<Article> articles, int count)>
    {
        public GetCategoryArticlesRequest(ArticleCat cat, int page = 1)
        {
            Cat = cat;
            Page = page;
        }

        public int PageSize { get; set; } = 20;

        public ArticleCat Cat { get; }
        public int Page { get; }
    }
}