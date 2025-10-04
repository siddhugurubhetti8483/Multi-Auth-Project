using FluentValidation;
using MultiAuthAPI.DTOs;

namespace MultiAuthAPI.Validators
{
    public class RoleChangeDTOValidator : AbstractValidator<RoleChangeDTO>
    {
        public RoleChangeDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewRole).NotEmpty();
        }
    }
}
