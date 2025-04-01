using Application.Interfaces;
using Application.Models.PaymentModels;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentAdapterFactory _adapterFactory;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<Booking> _bookingRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(
            IPaymentAdapterFactory adapterFactory,
            IRepository<Payment> paymentRepo,
            IRepository<Booking> bookingRepo,
            IUnitOfWork unitOfWork)
        {
            _adapterFactory = adapterFactory;
            _paymentRepo = paymentRepo;
            _bookingRepo = bookingRepo;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("momo-notify")]
        public async Task<IActionResult> HandleMomoIpn([FromBody] IDictionary<string, string> ipnData)
        {
            var adapter = _adapterFactory.CreateAdapter(PaymentMethod.Momo);
            var result = await adapter.VerifyPaymentAsync(new PaymentVerificationRequest { IpnData = ipnData });

            if (!result.IsSuccess) return Ok(result);

            await _unitOfWork.BeginAsync();
            try
            {
                var payment = await _paymentRepo.FindAsync(p => p.BookingId == int.Parse(result.OrderId));
                if (payment == null) throw new Exception($"Payment not found for BookingId: {result.OrderId}");

                payment.Status = result.IsSuccess ? PaymentStatus.Succeeded : PaymentStatus.Failed;
                //payment.TransactionId = result.TransactionId;
                payment.PaidAt = DateTime.UtcNow;
                _paymentRepo.Update(payment);

                var booking = await _bookingRepo.FindAsync(b => b.Id == payment.BookingId);
                if (booking != null && result.IsSuccess)
                {
                    var totalPaid = (await _paymentRepo.GetAllAsync(p => p.BookingId == booking.Id && p.Status == PaymentStatus.Succeeded))
                        .Sum(p => p.Amount);
                    if (booking.Status == BookingStatus.Pending && totalPaid >= booking.Deposit)
                    {
                        booking.Status = BookingStatus.Approved;
                        booking.ApprovedAt = DateTimeOffset.UtcNow;
                        _bookingRepo.Update(booking);
                    }
                }

                await _unitOfWork.CommitAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("momo-callback")]
        public async Task<IActionResult> HandleMomoCallback()
        {
            var adapter = _adapterFactory.CreateAdapter(PaymentMethod.Momo);
            var result = await adapter.VerifyPaymentAsync(new PaymentVerificationRequest { QueryData = Request.Query });
            string[] info = result.OrderInfo.Split();
            int bookingId = int.Parse(info[info.Length - 1]);
            if (!result.IsSuccess) return Redirect($"focusbadminton://payment-callback?bookingId={result.OrderId}&resultCode=1000&amount={result.Amount}");

            await _unitOfWork.BeginAsync();
            try
            {
                var payment = await _paymentRepo.FindAsync(p => p.BookingId == bookingId);
                if (payment != null)
                {
                    payment.Status = result.IsSuccess ? PaymentStatus.Succeeded : PaymentStatus.Failed;
                    payment.PaidAt = DateTime.UtcNow;
                    _paymentRepo.Update(payment);

                    var booking = await _bookingRepo.FindAsync(b => b.Id == payment.BookingId);
                    if (booking != null && result.IsSuccess)
                    {
                        var totalPaid = (await _paymentRepo.GetAllAsync(p => p.BookingId == booking.Id && p.Status == PaymentStatus.Succeeded))
                            .Sum(p => p.Amount);
                        if (booking.Status == BookingStatus.Pending)
                        {
                            booking.Status = BookingStatus.Approved;
                            booking.ApprovedAt = DateTimeOffset.UtcNow;
                            _bookingRepo.Update(booking);
                        }
                    }
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
            }

            var deepLink = $"focusbadminton://payment-callback?bookingId={bookingId}&resultCode={(result.IsSuccess ? "0" : "1000")}&amount={result.Amount}";
            return Redirect(deepLink);
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> HandleVnPayCallback()
        {
            var adapter = _adapterFactory.CreateAdapter(PaymentMethod.VnPay);
            var result = await adapter.VerifyPaymentAsync(new PaymentVerificationRequest { QueryData = Request.Query });
            int bookingId = result.BookingId ?? 0;
            await _unitOfWork.BeginAsync();
            try
            {
                var payment = await _paymentRepo.FindAsync(p => p.BookingId == bookingId);
                if (payment != null)
                {
                    payment.Status = result.IsSuccess ? PaymentStatus.Succeeded : PaymentStatus.Failed;
                    //payment.Status = result.IsSuccess ? PaymentStatus.Succeeded : PaymentStatus.Failed;
                    payment.PaidAt = DateTime.UtcNow;
                    _paymentRepo.Update(payment);

                    var booking = await _bookingRepo.FindAsync(b => b.Id == bookingId);
                    if (booking != null && result.IsSuccess)
                    {
                        var totalPaid = (await _paymentRepo.GetAllAsync(p => p.BookingId == bookingId && p.Status == PaymentStatus.Succeeded))
                            .Sum(p => p.Amount);
                        if (booking.Status == BookingStatus.Pending)
                        {
                            booking.Status = BookingStatus.Approved;
                            booking.ApprovedAt = DateTimeOffset.UtcNow;
                            _bookingRepo.Update(booking);
                        }
                    }
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
            }

            var deepLink = $"focusbadminton://payment-callback?bookingId={bookingId}&resultCode={(result.IsSuccess ? "0" : "1000")}&amount={result.Amount}";
            return Redirect(deepLink);
        }
    }
    public class ConfirmPaymentRequest
    {
        public bool IsApproved { get; set; }
    }

    public class MomoResultRequest
    {
        public string OrderId { get; set; }
        public string ResultCode { get; set; }
        public double Amount { get; set; }
    }
}
