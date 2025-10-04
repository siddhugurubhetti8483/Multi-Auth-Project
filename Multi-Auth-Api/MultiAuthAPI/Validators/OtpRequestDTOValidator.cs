using FluentValidation;
using MultiAuthAPI.DTOs;

namespace MultiAuthAPI.Validators
{
    public class OtpRequestDTOValidator : AbstractValidator<SendOtpDTO>
    {
        public OtpRequestDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
