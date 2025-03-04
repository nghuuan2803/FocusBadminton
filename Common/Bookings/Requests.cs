namespace Shared.Bookings
{
    public class ApproveBookingRequest
    {
        public ApproveBookingRequest()
        {
        }

        public ApproveBookingRequest(int bookingId, string? approveBy)
        {
            BookingId = bookingId;
            ApproveBy = approveBy;
        }


        public int BookingId { get; set; }
        public string? ApproveBy { get; set; }
    }
    public class RejectBookingRequest
    {
        public RejectBookingRequest()
        {
        }

        public RejectBookingRequest(int bookingId, string? rejectBy)
        {
            BookingId = bookingId;
            RejectBy = rejectBy;
        }

        public int BookingId { get; set; }
        public string? RejectBy { get; set; }
    }
}
