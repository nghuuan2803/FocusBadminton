namespace Web.Client.Models
{
    public class MenuNode : MenuComponent
    {
        private List<MenuComponent> Children { get; } = new List<MenuComponent>();

        public MenuNode(string title, string url = "#", string iconClass = ""): base(title, url, iconClass)
        {
        }
        public void Add(MenuComponent component)
        {
            Children.Add(component);
        }

        public override string Render()
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
