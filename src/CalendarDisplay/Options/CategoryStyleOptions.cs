namespace CalendarDisplay.Options;

public record CategoryStyleOptions
{
    public const string Section = "CategoryStyleOptions";

    public List<CategoryStyle> Categories { get; set; }

    public class CategoryStyle
    {
        public string CategoryName { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }
}
