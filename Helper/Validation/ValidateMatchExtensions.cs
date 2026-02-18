using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PremierLeague_Backend.Helper.Validation;

public static class ValidateMatchExtensions
{
    public static async Task<bool> ValidateMatchAsync(this IMatchRepository repository, MatchDto matchDto, ModelStateDictionary modelState)
    {
        var matchSeasonDto = await repository.GetMatchWeekAsync();
        if (matchSeasonDto is null)
        {
            modelState.AddModelError(string.Empty,
                "No active season found. Please set an active season before creating matches.");
            return false;
        }

        matchDto.SeasonId = matchSeasonDto.SeasonId;

        if (!await repository.FindMatchDateCannotBeEarlierThanToday(matchDto.MatchDate))
        {
            modelState.AddModelError(nameof(matchDto.MatchDate),
                "Match date cannot be earlier than today.");
            return false;
        }

        if (!await repository.FindMatchDateIsOutsideTheActiveSeason(
                matchDto.MatchDate,
                matchSeasonDto.StartSeasonDate,
                matchSeasonDto.EndSeasonDate))
        {
            modelState.AddModelError(nameof(matchDto.MatchDate),
                "Match date is outside the active season dates.");
            return false;
        }

        if (!await repository.FindMatchAlreadyExistsOnTheSelectedDate(
                matchDto.MatchId,
                matchDto.MatchDate,
                matchDto.HomeClubId,
                matchDto.AwayClubId))
        {
            modelState.AddModelError(nameof(matchDto.MatchDate),
                "Match already exists on the selected date.");
            return false;
        }

        if (matchDto.HomeClubId == matchDto.AwayClubId)
        {
            modelState.AddModelError(nameof(matchDto.AwayClubId),
                "Home and away clubs cannot be the same.");
            return false;
        }

        return true;
    }
}



