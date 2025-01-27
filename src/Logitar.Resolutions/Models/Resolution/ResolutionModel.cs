namespace Logitar.Resolutions.Models.Resolution;

public class ResolutionModel
{
  public Guid Id { get; set; }

  public string Title { get; set; } = string.Empty;
  public ushort Year { get; set; }
  public double Completion { get; set; }

  public override bool Equals(object? obj) => obj is ResolutionModel resolution && resolution.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Title} (Id={Id})";
}
