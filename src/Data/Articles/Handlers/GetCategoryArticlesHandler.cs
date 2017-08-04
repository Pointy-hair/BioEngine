﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Common.DB;
using BioEngine.Common.Models;
using BioEngine.Data.Articles.Requests;
using BioEngine.Data.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Data.Articles.Handlers
{
    public class GetCategoryArticlesHandler : RequestHandlerBase<GetCategoryArticlesRequest, (IEnumerable<Article>
        articles, int count)>
    {
        public GetCategoryArticlesHandler(IMediator mediator, BWContext dbContext) : base(mediator, dbContext)
        {
        }

        public override async Task<(IEnumerable<Article> articles, int count)> Handle(
            GetCategoryArticlesRequest message)
        {
            var articlesQuery = DBContext.Articles.Where(x => x.CatId == message.Cat.Id && x.Pub == 1)
                .OrderByDescending(x => x.Id).AsQueryable();

            var count = await articlesQuery.CountAsync();

            if (message.Page > 0)
            {
                articlesQuery = articlesQuery.Skip((message.Page - 1) * message.PageSize).Take(message.PageSize);
            }

            var articles = await articlesQuery.ToListAsync();

            return (articles, count);
        }
    }
}