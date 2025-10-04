using FluentValidation;
using MultiAuthAPI.DTOs;

namespace MultiAuthAPI.Validators
{
    public class OtpVerifyDTOValidator : AbstractValidator<VerifyOtpDTO>
    {
        public OtpVerifyDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Otp).NotEmpty().Length(6);
        }
    }
}
