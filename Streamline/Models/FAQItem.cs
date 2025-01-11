using MudBlazor;

namespace Streamline.Models;

public class FAQItem
{
    public int Id { get; set; }
    public string Question { get; set; } = "No question provided.";
    public string Answer { get; set; } = "No answer provided.";
    public bool IsVisible { get; set; }
    public string Icon { get; set; } = Icons.Material.Filled.KeyboardArrowDown;
}