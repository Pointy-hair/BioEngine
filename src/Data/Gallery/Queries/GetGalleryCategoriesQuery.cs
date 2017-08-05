﻿using System.Collections.Generic;
using BioEngine.Common.Interfaces;
using BioEngine.Common.Models;
using BioEngine.Data.Core;

namespace BioEngine.Data.Gallery.Queries
{
    public class GetGalleryCategoriesQuery : QueryBase<IEnumerable<GalleryCat>>, ICategoryQuery<GalleryCat>
    {
        public GetGalleryCategoriesQuery(IParentModel parent = null, GalleryCat parentCat = null,
            string url = null, bool loadChildren = false, int? loadLastItems = null, bool onlyRoot = false)
        {
            Url = url;
            Parent = parent;
            ParentCat = parentCat;
            LoadChildren = loadChildren;
            LoadLastItems = loadLastItems;
            OnlyRoot = onlyRoot;
        }

        public bool OnlyRoot { get; }
        public IParentModel Parent { get; }
        public bool LoadChildren { get; }
        public GalleryCat ParentCat { get; }
        public int? LoadLastItems { get; }
        public string Url { get; }
    }
}