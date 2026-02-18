namespace PremierLeague_Backend.Models.SelectListItems;

public record SelectListItemReferee(int RefereeId, string RefereeName, string DefaultRole, string Nationality);
public record SelectListItemRefereeRole(int RefereeRoleId, string RoleName);
public record SelectListItemRefereeBadgeLevel(int RefereeBadgeId, string BadgeName);
