using Microsoft.AspNetCore.Components;

namespace Web.Client.Models
{
    public interface IMenuComponent
    {
        string Title { get; }
        string Url { get; }
        string IconClass { get; }
        string Render();
    }
}
