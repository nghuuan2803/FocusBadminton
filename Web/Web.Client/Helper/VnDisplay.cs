namespace Web.Client.Helper
{
    public static class VnDisplay
    {
        public static string ToVnDayOfWeek(this string dayOfWeek)
        {
            return dayOfWeek switch
            {
                "Monday" => "Thứ Hai",
                "Tuesday" => "Thứ Ba",
                "Wednesday" => "Thứ Tư",
                "Thursday" => "Thứ Năm",
                "Friday" => "Thứ Sáu",
                "Saturday" => "Thứ Bảy",
                "Sunday" => "Chủ Nhật",
                _ => dayOfWeek
            };
        }

        public static string DisplayStatus(this BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Creating => "Đang tạo",
                BookingStatus.Pending => "Đang chờ",
                BookingStatus.Rejected => "Từ chối",
                BookingStatus.Canceled => "Hủy",
                BookingStatus.Approved => "Đã duyệt",
                BookingStatus.Completed => "Kết thúc",
                BookingStatus.Paused => "Tạm ngưng",
                _ => "Không xác định"
            };
        }
        public static string ToVnPaymentMethod(this PaymentMethod payMethod)
        {
            return payMethod switch
            {
                PaymentMethod.Cash => "Trả sau",
                PaymentMethod.BankTransfer => "Chuyển khoản",
                PaymentMethod.Momo => "Ví MoMo",
                PaymentMethod.VnPay => "Ví VnPay",
                _ => "Không xác định"
            };
        }
    }
}
