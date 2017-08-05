﻿using System.Linq;
using System.Threading.Tasks;
using BioEngine.Common.DB;
using BioEngine.Common.Models;
using BioEngine.Data.Core;
using BioEngine.Data.Files.Queries;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioEngine.Data.Files.Handlers
{
    [UsedImplicitly]
    internal class GetFileByUrlHandler : QueryHandlerBase<GetFileByUrlQuery, File>
    {
        public GetFileByUrlHandler(IMediator mediator, BWContext dbContext, ILogger<GetFileByIdHandler> logger) : base(
            mediator, dbContext, logger)
        {
        }

        protected override async Task<File> RunQuery(GetFileByUrlQuery message)
        {
            var query = DBContext.Files.Include(x => x.Cat).Include(x => x.Author).Include(x => x.Game)
                .Include(x => x.Developer).AsQueryable().Where(x => x.Url == message.Url);
            query = ApplyParentCondition(query, message.Parent);
            var files = await query.ToListAsync();
            if (files.Any())
            {
                File file = null;
                if (files.Count > 1)
                    foreach (var candidate in files)
                    {
                        if (candidate.Cat.Url != message.CatUrl) continue;
                        file = candidate;
                        break;
                    }
                else
                    file = files[0];
                if (file != null)
                {
                    file.Cat =
                        await Mediator.Send(
                            new FileCategoryProcessQuery(file.Cat, new GetFilesCategoryQuery(message.Parent)));
                    return file;
                }
            }

            return null;
        }
    }
}