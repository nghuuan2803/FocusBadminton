﻿namespace Web.Client.Layout
{
    public partial class SidebarMenu
    {
        public List<MenuComponent> MenuItems { get; set; } = new List<MenuComponent>();

        protected override void OnInitialized()
        {
            MenuItems.Add(new MenuLeaf("Home", "/", "fa-solid fa-house"));

            var scheduleNode = new MenuNode("Lịch đặt sân", "/schedules", "fa-solid fa-calendar-days");
            scheduleNode.Add(new MenuLeaf("Sân 1", "/court-schedule/1", "fa-solid fa-square"));
            scheduleNode.Add(new MenuLeaf("Sân 2", "/court-schedule/2", "fa-solid fa-square"));
            scheduleNode.Add(new MenuLeaf("Sân 3", "/court-schedule/3", "fa-solid fa-square"));
            scheduleNode.Add(new MenuLeaf("Sân 4", "/court-schedule/4", "fa-solid fa-square"));
            scheduleNode.Add(new MenuLeaf("Tìm sân trống", "/available-court", "fa-solid fa-calendar-day"));
            scheduleNode.Add(new MenuLeaf("Duyệt yêu cầu", "/booking-approve", "fa-solid fa-list-check"));

            var managementNode = new MenuNode("Quản lý thông tin", "#", "fa-solid fa-bars-progress");
            var courtNode = new MenuNode("Sân", "courts", "fa-solid fa-cog");
            courtNode.Add(new MenuLeaf("Sân 1", "/court/1", "fa-solid fa-square"));
            courtNode.Add(new MenuLeaf("Sân 2", "/court/2", "fa-solid fa-square"));

            var timeSlotNode = new MenuLeaf("Khung giờ", "/timeslots", "fa-solid fa-timeline");
            var teamNode = new MenuLeaf("Team", "/teams", "fa-solid fa-people-line");
            var userNode = new MenuLeaf("Người dùng", "/users", "fa-solid fa-users");
            var voucherNode = new MenuLeaf("Phiếu giảm giá", "/voucher-management", "fa-solid fa-ticket-simple");

            managementNode.Add(courtNode);
            managementNode.Add(timeSlotNode);
            managementNode.Add(teamNode);
            managementNode.Add(userNode);
            managementNode.Add(voucherNode);

            var revenueNode = new MenuNode("Doanh thu", "/revenue", "fa-solid fa-file-invoice-dollar");
            revenueNode.Add(new MenuLeaf("Trong Tháng", "/revenue-monthly", "fa-calendar-week"));
            revenueNode.Add(new MenuLeaf("Khoảng thời gian", "/revenue-filter", "fa-calendar-week"));

            MenuItems.Add(scheduleNode);
            MenuItems.Add(managementNode);
            MenuItems.Add(revenueNode);

        }
    }
}
