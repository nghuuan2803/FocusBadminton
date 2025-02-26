using AutoMapper;
using Domain.Repositories;
using Shared.Enums;

namespace Application.Features.Bookings.Commands.Payments
{
    public class DepositCommand : IRequest<Result<Payment>>
    {
        public int BookingId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public double Amount { get; set; }
        public string? Info { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Image { get; set; }
        public string CreatedBy { get; set; } = "System";
        public bool Success { get; set; } = true;
    }
    public class DepositCommandHandler : IRequestHandler<DepositCommand, Result<Payment>>
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Booking> _bookingRepo;
        private readonly IMapper _mapper;

        public DepositCommandHandler(IRepository<Payment> paymentRepo, IRepository<Booking> bookingRepo, IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _bookingRepo = bookingRepo;
            _mapper = mapper;
        }

        public async Task<Result<Payment>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            // Find booking by id
            var booking = await _bookingRepo.FindAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                return Result<Payment>.Failure(Error.NotFound("booking", request.BookingId.ToString()));
            }

            // Map request to payment
            var payment = _mapper.Map<Payment>(request);

            // Update payment and booking status based on payment method and success flag
            switch (request.PaymentMethod)
            {
                case PaymentMethod.Momo:
                case PaymentMethod.VnPay:
                    if (request.Success)
                    {
                        payment.Status = PaymentStatus.Succeeded;
                        booking.Status = BookingStatus.Approved;
                    }
                    else
                    {
                        payment.Status = PaymentStatus.Failed;
                        booking.Status = BookingStatus.Canceled;
                    }
                    break;
                case PaymentMethod.Cash:
                    payment.Status = PaymentStatus.Succeeded;
                    booking.Status = BookingStatus.Pending;
                    break;
                case PaymentMethod.BankTransfer:
                    payment.Status = PaymentStatus.Pending;
                    booking.Status = BookingStatus.Pending;
                    break;
                default:
                    payment.Status = PaymentStatus.Pending;
                    break;
            }

            // Update booking and add payment to the database
            _bookingRepo.Update(booking);
            await _paymentRepo.AddAsync(payment);
            await _bookingRepo.SaveAsync(cancellationToken);

            // Return payment result
            return Result<Payment>.Success(payment);
        }
    }
}
