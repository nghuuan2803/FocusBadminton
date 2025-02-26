using System.Xml.Xsl;

namespace Web.Client.StatePattern
{
    public interface ISlotState
    {
        string DisplayName { get; }
        string CssClass { get; }
        bool CanSelect { get; }
        void HandleSelection(SlotContext context);
    }
}
