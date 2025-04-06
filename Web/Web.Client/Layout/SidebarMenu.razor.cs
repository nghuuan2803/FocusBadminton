namespace Web.Client.Layout
{
    public partial class SidebarMenu
    {
        public List<MenuComponent> MenuItems { get; set; } = new List<MenuComponent>();

        protected override void OnInitialized()
        {
            MenuItems.Add(new MenuLeaf("Home", "/", "fa-solid fa-house"));

            var scheduleNode = new MenuNode("Lịch đặt sân", "/court-schedule/1", "fa-solid fa-calendar-days");
            scheduleNode.Add(new MenuLeaf("Duyệt yêu cầu", "/booking-approve", "fa-solid fa-list-check"));

            var managementNode = new MenuNode("Quản lý thông tin", "#", "fa-solid fa-bars-progress");
            var courtNode = new MenuNode("Sân", "courts", "fa-solid fa-cog");
            var timeSlotNode = new MenuLeaf("Khung giờ", "/timeslots", "fa-solid fa-timeline");
            var teamNode = new MenuLeaf("Team", "/teams", "fa-solid fa-people-line");
            var userNode = new MenuLeaf("Người dùng", "/users", "fa-solid fa-users");
            var voucherNode = new MenuLeaf("Phiếu giảm giá", "/voucher-management", "fa-solid fa-ticket-simple");

            managementNode.Add(courtNode);
            managementNode.Add(timeSlotNode);
            managementNode.Add(teamNode);
            managementNode.Add(userNode);
            managementNode.Add(voucherNode);

            MenuItems.Add(scheduleNode);
            MenuItems.Add(managementNode);

        }
    }
}
