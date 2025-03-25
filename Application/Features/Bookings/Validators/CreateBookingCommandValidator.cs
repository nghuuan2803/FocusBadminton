using Application.Features.Bookings.Commands;
using FluentValidation;
using Shared.Enums;

namespace Application.Features.Bookings.Validators
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.MemberId)
                .GreaterThan(0)
                .WithMessage("MemberId phải lớn hơn 0");

            RuleFor(x => x.Details)
                .NotEmpty()
                .WithMessage("Phải chọn ít nhất một sân (BookingItem)")
                .Must(details => details.All(d => d.CourtId > 0 && d.TimeSlotId > 0 && d.BeginAt.HasValue))
                .WithMessage("Tất cả BookingItem phải có CourtId, TimeSlotId và BeginAt hợp lệ");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Amount không được âm");

            RuleFor(x => x.Deposit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Deposit không được âm")
                .LessThanOrEqualTo(x => x.Amount)
                .When(x => x.Deposit > 0)
                .WithMessage("Deposit không được lớn hơn Amount");

            RuleFor(x => x.Note)
                .MaximumLength(250)
                .WithMessage("Note không được dài quá 250 ký tự")
                .When(x => x.Note != null);

            RuleFor(x => x.AdminNote)
                .MaximumLength(250)
                .WithMessage("AdminNote không được dài quá 250 ký tự")
                .When(x => x.AdminNote != null);

            RuleFor(x => x.TransactionImage)
                .NotEmpty()
                .When(x => x.PaymentMethod == PaymentMethod.BankTransfer)
                .WithMessage("Phải cung cấp TransactionImage khi chọn phương thức BankTransfer");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("BookingType không hợp lệ");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("PaymentMethod không hợp lệ");
        }
    }
}

