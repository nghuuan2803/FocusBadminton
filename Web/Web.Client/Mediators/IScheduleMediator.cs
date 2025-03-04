namespace Web.Client.Mediators
{
    public interface IScheduleMediator
    {
        //thông báo có người chọn slot
        void SlotHeldNotify(object payload);

        //thông báo slot được nhả
        void SlotReleasedNotify(object payload);

        //thông báo có đơn đặt sân được tạo
        void BookingCreatedNotify(object payload);

        //thông báo có đơn đặt sân bị hủy
        void BookingCaneledNotify(object payload);

        //thông báo có đơn đặt sân được duyệt
        void BookingApprovedNotify(object payload);

        //thông báo có đơn đặt sân bị từ chối
        void BookingRejectedNotify(object payload);

        //thông báo có đơn đặt sân đã thanh toán hoàn tất
        void BookingCompletedNotify(object payload);

        //thông báo có đơn đặt sân tạm ngưng
        void BookingPausedNotify(object payload);

        //thông báo có đơn đặt sân hết tạm ngưng
        void BookingResumedNotify(object payload);

        //thông báo quá giờ
        void TimeOutNotify();

        void AddSlot(SlotComponent slot);
        void RemoveSlot(SlotComponent slot);
    }
}
