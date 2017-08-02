﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BioEngine.Common.Base;
using BioEngine.Common.DB;
using BioEngine.Common.Interfaces;
using BioEngine.Common.Models;
using BioEngine.Site.Base;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BioEngine.Site.Controllers
{
    public class PollsController : BaseController
    {
        public PollsController(IMediator mediator, IOptions<AppSettings> appSettingsOptions,
            IContentHelperInterface contentHelper) : base(mediator,
            appSettingsOptions, contentHelper)
        {
        }

        /*[HttpPost("polls/vote.html")]
        public async Task<IActionResult> Vote([FromForm] Dictionary<int, int> votes)
        {
            return null;
        }*/

        [HttpPost("polls/{pollId}/vote.html")]
        public async Task<IActionResult> Vote(int pollId, [FromForm] int vote)
        {
            var poll = await Context.Polls.FirstOrDefaultAsync(x => x.Id == pollId);
            if (poll == null)
            {
                return new NotFoundResult();
            }
            var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var returnUrl = Request.Headers["REFERER"].ToString();
            var userId = 0;
            var userLogin = "guest";
            var sessionId = HttpContext.Session.Id;
            if (User.Identity.IsAuthenticated)
            {
                userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                userLogin = User.Identity.Name;
            }
            if (await poll.GetIsVoted(Context, userId, ip, sessionId))
            {
                return new RedirectResult(returnUrl);
            }

            var pollVote = new PollWho
            {
                PollId = pollId,
                VoteDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                VoteOption = vote,
                Ip = ip,
                UserId = userId,
                Login = userLogin,
                SessionId = sessionId
            };

            Context.Add(pollVote);
            await Context.SaveChangesAsync();

            await poll.Recount(Context);
            HttpContext.Session.SetInt32("voted", poll.Id);
            return new RedirectResult(returnUrl);
        }
    }
}