namespace Logitar.Resolutions.Models.Components;

public record ProgressBar
{
  public string? AriaLabel { get; set; }
  public bool IsAnimated { get; set; }
  public bool IsStriped { get; set; }
  public string? Label { get; set; }
  public double Maximum { get; set; } = 100;
  public double Minimum { get; set; } = 0;
  public double Value { get; set; } = 0;
  public ProgressVariant? Variant { get; set; }
}
