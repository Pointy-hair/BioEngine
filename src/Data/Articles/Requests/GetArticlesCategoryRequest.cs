﻿using BioEngine.Common.Interfaces;
using BioEngine.Common.Models;
using BioEngine.Data.Core;

namespace BioEngine.Data.Articles.Requests
{
    public class GetArticlesCategoryRequest : RequestBase<ArticleCat>, ICategoryRequest<ArticleCat>
    {
        public GetArticlesCategoryRequest(IParentModel parent = null, ArticleCat parentCat = null,
            bool loadChildren = false,
            int? loadLastItems = null, string url = null)
        {
            Parent = parent;
            LoadChildren = loadChildren;
            ParentCat = parentCat;
            LoadLastItems = loadLastItems;
            Url = url;
        }

        public IParentModel Parent { get; }
        public bool LoadChildren { get; }
        public ArticleCat ParentCat { get; }
        public int? LoadLastItems { get; }
        public string Url { get; }
    }
}