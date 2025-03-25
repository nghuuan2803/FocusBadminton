namespace Web.Client.Models
{
    public class MenuLeaf : MenuComponent
    {
        public MenuLeaf(string title, string url, string iconClass = ""): base(title, url, iconClass)
        {
        }
        public override string Render()
        {
            var iconHtml = string.IsNullOrEmpty(IconClass)
                ? ""
                : $"<i class='{IconClass}' aria-hidden='true' style='padding-right: 0.75rem;'></i>";
            return $"<a class='nav-link' href='{Url}'>{iconHtml}{Title}</a>";
        }
    }
}
