namespace Web.Client.Models
{
    public class MenuLeaf : IMenuComponent
    {
        public string Title { get; }
        public string Url { get; }
        public string IconClass { get; }

        public MenuLeaf(string title, string url, string iconClass = "")
        {
            Title = title;
            Url = url;
            IconClass = iconClass;
        }

        public string Render()
        {
            var iconHtml = string.IsNullOrEmpty(IconClass)
                ? ""
                : $"<i class='{IconClass}' aria-hidden='true' style='padding-right: 0.75rem;'></i>";
            return $"<a class='nav-link' href='{Url}'>{iconHtml}{Title}</a>";
        }
    }
}
