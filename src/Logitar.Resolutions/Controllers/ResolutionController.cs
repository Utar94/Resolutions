using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Resolutions.Controllers;

[Route("")]
public class ResolutionController : Controller
{
  private readonly IPublishedContentQuerier _publishedContentQuerier;

  public ResolutionController(IPublishedContentQuerier publishedContentQuerier)
  {
    _publishedContentQuerier = publishedContentQuerier;
  }

  [HttpGet]
  public async Task<ActionResult> ResolutionList(CancellationToken cancellationToken)
  {
    SearchPublishedContentsPayload payload = new();
    payload.ContentType.Names.Add("Resolution");
    payload.Sort.Add(new PublishedContentSortOption(PublishedContentSort.DisplayName, isDescending: false));
    SearchResults<PublishedContentLocale> publishedLocales = await _publishedContentQuerier.SearchAsync(payload, cancellationToken);

    Dictionary<Guid, PublishedContent> publishedContents = new(capacity: publishedLocales.Items.Count);
    foreach (PublishedContentLocale publishedLocale in publishedLocales.Items)
    {
      publishedContents[publishedLocale.Content.Id] = publishedLocale.Content;
    }

    return View(publishedContents.Values.ToArray());
  }
}
