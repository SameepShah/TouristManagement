using BranchAPI.Models;
using FluentValidation;

namespace BranchAPI.Validations
{
    public class BranchValidator : AbstractValidator<Branch>
    {
        public BranchValidator()
        {
            RuleFor(branch => branch.Website).Must(x => x!.Contains("www")).WithMessage("Website must contain www");
            RuleFor(branch => branch.Email).NotNull().WithMessage("Email should not be empty");
            RuleFor(branch => branch.Email).EmailAddress().WithMessage("Email address is not valid");
            RuleFor(branch => branch.Contact).NotNull().WithMessage("Contact should not be empty");
            RuleFor(branch => branch.Contact).Matches("^[0-9]*$").WithMessage("Contact should be number only");
            RuleFor(branch => branch.Contact).MaximumLength(10).WithMessage("Maximum Length of contact must be 10 characters");
        }
    }
}
