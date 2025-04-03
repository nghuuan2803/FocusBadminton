using Application.Features.Bookings.Commands;
using Application.Interfaces;
using Application.Models.PaymentModels;
using Domain.Repositories;
using Shared.Enums;

namespace Application.Features.Bookings
{
    public class PaymentHandler
    {
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IPaymentAdapterFactory _paymentAdapterFactory;
        private readonly Logger _logger;

        public PaymentHandler(
            IRepository<Payment> paymentRepo,
            IPaymentAdapterFactory paymentAdapterFactory,
            Logger logger)
        {
            _paymentRepo = paymentRepo;
            _paymentAdapterFactory = paymentAdapterFactory;
            _logger = logger;
        }

        public async Task<(Payment payment, string paymentUrl)> ProcessPaymentAsync(
            Booking booking,
            CreateBookingCommand request)
        {
            string paymentUrl = string.Empty;
            var payment = new Payment
            {
                BookingId = booking.Id,
                Method = request.PaymentMethod,
                Amount = booking.Amount > 0 ? booking.Amount : booking.EstimateCost,
                Type = request.Deposit > 0 ? PaymentType.Deposit : PaymentType.CompleteBooking,
                Status = PaymentStatus.Pending
            };

            switch (request.PaymentMethod)
            {
                case PaymentMethod.Cash:
                    payment.PaidAt = DateTime.Now;
                    booking.Status = BookingStatus.Pending;
                    break;

                case PaymentMethod.BankTransfer:
                    break;

                case PaymentMethod.Momo:
                    var adapter = _paymentAdapterFactory.CreateAdapter(request.PaymentMethod);
                    var paymentRequest = new PaymentRequest
                    {
                        FullName = "User",
                        OrderId = booking.Id.ToString(),
                        OrderInfo = $"Thanh toan booking {booking.Id}",
                        Amount = payment.Amount,
                        Action = "mobile",
                        Context = request.HttpContext
                    };
                    var paymentLink = await adapter.GeneratePaymentLinkAsync(paymentRequest);
                    if (!paymentLink.IsSuccess)
                    {
                        throw new Exception($"Failed to create payment link: {paymentLink.ErrorMessage}");
                    }
                    paymentUrl = paymentLink.Deeplink;
                    break;
                case PaymentMethod.VnPay:
                    adapter = _paymentAdapterFactory.CreateAdapter(request.PaymentMethod);
                    paymentRequest = new PaymentRequest
                    {
                        FullName = "User",
                        OrderId = booking.Id.ToString(),
                        OrderInfo = $"Thanh toan booking {booking.Id}",
                        Amount = payment.Amount,
                        Action = "mobile",
                        Context = request.HttpContext
                    };
                    paymentLink = await adapter.GeneratePaymentLinkAsync(paymentRequest);
                    if (!paymentLink.IsSuccess)
                    {
                        throw new Exception($"Failed to create payment link: {paymentLink.ErrorMessage}");
                    }
                    paymentUrl = paymentLink.PaymentUrl;
                    break;
            }

            await _paymentRepo.AddAsync(payment);
            return (payment, paymentUrl);
        }
    }

}
