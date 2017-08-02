﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioEngine.Common.DB;
using BioEngine.Common.Models;
using BioEngine.Data.Articles.Requests;
using BioEngine.Data.Core;
using BioEngine.Data.Files.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Data.Files.Handlers
{
    public class GetCategoryFilesHandler : RequestHandlerBase<GetCategoryFilesRequest, (IEnumerable<File>
        files, int count)>
    {
        public GetCategoryFilesHandler(IMediator mediator, BWContext dbContext) : base(mediator, dbContext)
        {
        }

        public override async Task<(IEnumerable<File> files, int count)> Handle(
            GetCategoryFilesRequest message)
        {
            var filesQuery = DBContext.Files.Where(x => x.CatId == message.Cat.Id)
                .OrderByDescending(x => x.Id).AsQueryable();

            var count = await filesQuery.CountAsync();

            if (message.Page > 0)
            {
                filesQuery = filesQuery.Skip(message.Page - 1 * message.PageSize).Take(message.PageSize);
            }

            var files = await filesQuery.ToListAsync();

            return (files, count);
        }
    }
}