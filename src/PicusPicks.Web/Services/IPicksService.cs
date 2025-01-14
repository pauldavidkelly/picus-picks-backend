using Picus.Api.Models.DTOs;

namespace PicusPicks.Web.Services;

public interface IPicksService
{
    Task<(IEnumerable<VisiblePickDto> picks, PicksStatusDto status)> GetMyPicksForWeekAsync(int week, int season);
    Task<PicksStatusDto> GetPickStatusAsync(int week, int season);
    Task<VisiblePickDto> SubmitPickAsync(SubmitPickDto pickDto);
} 