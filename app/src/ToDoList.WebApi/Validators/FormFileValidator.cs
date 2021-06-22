using System;
using System.IO;
using System.Linq;

using FluentValidation;

using Microsoft.AspNetCore.Http;

namespace ToDoList.WebApi.Validators
{
    public class FormFileValidator : AbstractValidator<IFormFile>
    {
        public FormFileValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .Must(HavePermittedExtension).WithMessage("Invalid extension. Allowed: '.jpg', '.jpeg', '.png'")
                .When(x => x is not null);
        }

        private bool HavePermittedExtension(string fileName)
        {
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
            string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            return permittedExtensions.Contains(fileExtension);
        }
    }
}
