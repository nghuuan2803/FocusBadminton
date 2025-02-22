namespace Web.Policies
{
    public static class DI
    {
        public static void AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(o=>
            {
                o.AddPolicy("ConfirmBooking", p => p.Requirements.Add(new HasBookingPermission("booking_confirm")));
            });
        }
    }
}
