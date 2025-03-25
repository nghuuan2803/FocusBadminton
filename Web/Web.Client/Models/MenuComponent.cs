using Microsoft.AspNetCore.Components;

namespace Web.Client.Models
{
    public abstract class MenuComponent
    {
        protected MenuComponent(string title, string url, string iconClass)
        {
            Title = title;
            Url = url;
            IconClass = iconClass;
        }

        public string Title { get; }
        public string Url { get; }
        public string IconClass { get; }
        public abstract string Render();
    }
}
