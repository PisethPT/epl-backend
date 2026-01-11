namespace epl_backend.Models.SelectListItems;

public record SelectListItemReferee(int RefereeId, string RefereeName, string DefaultRole, string Nationality);
public record SelectListItemRefereeRole(int RefereeRoleId, string RoleName);
public record SelectListItemRefereeBadgeLevel(int RefereeBadgeId, string BadgeName);
