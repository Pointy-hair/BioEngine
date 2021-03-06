﻿using System.Linq;
using System.Threading.Tasks;
using BioEngine.Common.Models;
using BioEngine.Data.Articles.Queries;
using BioEngine.Data.Core;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Data.Articles.Handlers
{
    [UsedImplicitly]
    internal class GetArticlesCategoryHandler : QueryHandlerBase<GetArticlesCategoryQuery, ArticleCat>
    {
        public GetArticlesCategoryHandler(HandlerContext<GetArticlesCategoryHandler> context) : base(context)
        {
        }

        protected override async Task<ArticleCat> RunQueryAsync(GetArticlesCategoryQuery message)
        {
            var catQuery = DBContext.ArticleCats.AsQueryable();

            if (!string.IsNullOrEmpty(message.Url))
            {
                catQuery = catQuery.Where(x => x.Url == message.Url);
            }

            if (message.Parent != null)
            {
                catQuery = ApplyParentCondition(catQuery, message.Parent);
            }

            if (message.ParentCat != null)
            {
                catQuery = catQuery.Where(x => x.CatId == message.ParentCat.Id);
            }
            else
            {
                catQuery = catQuery.Include(x => x.ParentCat);
            }

            var cat = await catQuery.FirstOrDefaultAsync();
            if (cat != null)
            {
                cat = await Mediator.Send(new ArticleCategoryProcessQuery(cat, message));
            }
            return cat;
        }
    }
}