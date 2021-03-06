﻿using System;
using BioEngine.Common.Models;
using System.Threading.Tasks;

namespace BioEngine.Site.ViewModels.Files
{
    public class FileViewModel : BaseViewModel
    {
        public FileViewModel(BaseViewModelConfig config, File file) : base(config)
        {
            File = file;
        }

        public override string Title()
        {
            var title = File.Title;
            if (File.Cat != null)
                title += " - " + File.Cat.Title;
            if (File.Parent != null)
                title += " - " + File.Parent.DisplayTitle;
            return title;
        }

        protected override async Task<string> GetDescription()
        {
            return await Task.FromResult(GetDescriptionFromHtml(!string.IsNullOrEmpty(File.Announce)
                ? File.Announce
                : File.Desc));
        }

        public File File { get; }

        public DateTimeOffset Date => DateTimeOffset.FromUnixTimeSeconds(File.Date);
    }
}