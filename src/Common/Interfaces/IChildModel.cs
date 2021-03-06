﻿using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Common.Base;
using BioEngine.Common.Models;

namespace BioEngine.Common.Interfaces
{
    public interface IChildModel : IBaseModel
    {
        int? GameId { get; set; }
        int? DeveloperId { get; set; }
        int? TopicId { get; set; }

        [ForeignKey(nameof(GameId))]
        Game Game { get; set; }

        [ForeignKey(nameof(DeveloperId))]
        Developer Developer { get; set; }

        [ForeignKey(nameof(TopicId))]
        Topic Topic { get; set; }

        IParentModel Parent { get; set; }

    }
}