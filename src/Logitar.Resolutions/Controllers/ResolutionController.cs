using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Search;
using Logitar.Resolutions.Constants;
using Logitar.Resolutions.Models.Resolution;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Resolutions.Controllers;

[Route("")]
public class ResolutionController : Controller
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IPublishedContentQuerier _publishedContentQuerier;

  public ResolutionController(IContentTypeQuerier contentTypeQuerier, IPublishedContentQuerier publishedContentQuerier)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _publishedContentQuerier = publishedContentQuerier;
  }

  [HttpGet]
  public async Task<ActionResult> ResolutionList(CancellationToken cancellationToken)
  {
    ContentTypeModel contentType = await _contentTypeQuerier.ReadAsync(ContentTypes.Resolution.UniqueName, cancellationToken)
      ?? throw new InvalidOperationException($"The content type 'UniqueName={ContentTypes.Resolution.UniqueName}' could not be found.");
    Dictionary<Guid, string> fieldNameByIds = contentType.Fields.ToDictionary(x => x.Id, x => x.UniqueName);

    SearchPublishedContentsPayload payload = new();
    payload.ContentType.Uids.Add(contentType.Id);
    SearchResults<PublishedContentLocale> publishedLocales = await _publishedContentQuerier.SearchAsync(payload, cancellationToken);

    Dictionary<Guid, ResolutionModel> resolutions = [];
    foreach (PublishedContentLocale publishedLocale in publishedLocales.Items)
    {
      if (!resolutions.TryGetValue(publishedLocale.Content.Id, out ResolutionModel? resolution))
      {
        resolution = new()
        {
          Id = publishedLocale.Content.Id
        };
        resolutions[resolution.Id] = resolution;
      }

      if (publishedLocale.Language != null && publishedLocale.Language.Locale.Code.Equals(Languages.French, StringComparison.InvariantCultureIgnoreCase))
      {
        resolution.Title = publishedLocale.DisplayName ?? publishedLocale.UniqueName;
      }

      foreach (FieldValue fieldValue in publishedLocale.FieldValues)
      {
        string fieldName = fieldNameByIds[fieldValue.Id];
        switch (fieldName)
        {
          case ContentTypes.Resolution.Completion:
            resolution.Completion = double.Parse(fieldValue.Value);
            break;
          case ContentTypes.Resolution.Year:
            resolution.Year = ushort.Parse(fieldValue.Value);
            break;
        }
      }
    }

    ResolutionModel[] filteredResolutions = [.. resolutions.Values
      .Where(resolution => !string.IsNullOrWhiteSpace(resolution.Title) && resolution.Year == App.CurrentYear)
      .OrderBy(resolution => resolution.Title)];
    return View(filteredResolutions);
  }
}
