﻿using System.Threading.Tasks;

namespace BioEngine.Site.ViewModels.Errors
{
    public class ErrorsViewModel : BaseViewModel
    {
        public int ErrorCode { get; }

        public ErrorsViewModel(BaseViewModelConfig config, int errorCode) : base(config)
        {
            ErrorCode = errorCode;
        }

        public override string Title()
        {
            return "Ошибка";
        }

        protected override async Task<string> GetDescription()
        {
            return await Task.FromResult("Error");
        }
    }
}
