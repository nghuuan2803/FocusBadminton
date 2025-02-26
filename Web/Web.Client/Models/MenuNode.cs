namespace Web.Client.Models
{
    public class MenuNode : IMenuComponent
    {
        public string Title { get; }
        public string Url { get; }
        public string IconClass { get; }
        private List<IMenuComponent> Children { get; } = new List<IMenuComponent>();

        public MenuNode(string title, string url = "#", string iconClass = "")
        {
            Title = title;
            Url = url;
            IconClass = iconClass;
        }

        public void Add(IMenuComponent component)
        {
            Children.Add(component);
        }

        public string Render()
        {
            var iconHtml = string.IsNullOrEmpty(IconClass)
                ? ""
                : $"<i class='{IconClass}' aria-hidden='true' style='padding-right: 0.75rem;'></i>";
            var html = Url != "#"
                ? $"<a class='nav-link' href='{Url}'>{iconHtml}{Title}</a>"
                : $"<span class='nav-link'>{iconHtml}{Title}</span>";
            if (Children.Count > 0)
            {
                html += "<nav class='nav flex-column'>";
                foreach (var child in Children)
                {
                    html += $"<div class='nav-item px-3'>{child.Render()}</div>";
                }
                html += "</nav>";
            }
            return html;
        }
    }
}
