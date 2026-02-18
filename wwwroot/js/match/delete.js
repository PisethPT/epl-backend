const $backdrop = $("#modalBackdrop");
let isStaticModal = false;

function showModalDelete(modalId, button, sizeClass = "modal-md", isStatic = false) {
  isStaticModal = !!isStatic;
  const $modal = $("#" + modalId);
  const $box = $modal.find(".modalBox");

  const $matchId = $(button).find('input[name="matchId"]').val();

  const $homeClubName = $(button).find('input[name="homeClubName"]').val();
  const $homeClubCrest = $(button).find('input[name="homeClubCrest"]').val();
  const $homeClubTheme = $(button).find('input[name="homeClubTheme"]').val();

  const $awayClubName = $(button).find('input[name="awayClubName"]').val();
  const $awayClubCrest = $(button).find('input[name="awayClubCrest"]').val();
  const $awayClubTheme = $(button).find('input[name="awayClubTheme"]').val();

  const $matchDate = $(button).find('input[name="matchDate"]').val();
  const $matchTime = $(button).find('input[name="matchTime"]').val();
  const $seasonName = $(button).find('input[name="seasonName"]').val();

  const form = $("#confirmDeleteForm");
  form.find("#fr_matchId").val($matchId);

  const CLUB_PATH = "/upload/clubs/";

  $modal.find("#fr_homeClubName").text($homeClubName);
  $modal.find("#fr_homeClubCrest").attr("src", CLUB_PATH + $homeClubCrest);
  $modal.find("#fr_homeClubTheme").css("background-color", $homeClubTheme);

  $modal.find("#fr_awayClubName").text($awayClubName);
  $modal.find("#fr_awayClubCrest").attr("src", CLUB_PATH + $awayClubCrest);
  $modal.find("#fr_awayClubTheme").css("background-color", $awayClubTheme);

  $modal.find("#fr_homeClubFrame").css("background-color", $homeClubTheme);
  $modal.find("#fr_awayClubFrame").css("background-color", $awayClubTheme);

  $modal.find("#fr_matchTime").text($matchTime.replace(/PM|AM/g, ""));
  $modal.find("#fr_matchDate").text($matchDate);
  $modal
    .find("#fr_stadiumName")
    .text($(button).find('input[name="kickoffStadium"]').val());
  $modal.find("#fr_seasonName").text($seasonName);

  $box.attr(
    "class",
    `
      modalBox
      !bg-[#37003c] rounded-2xl shadow-xl p-6 transition-all duration-300
      scale-90 opacity-0 h-auto w-full ${sizeClass}
    `.trim(),
  );

  $modal.removeClass("hidden");
  $backdrop.removeClass("hidden");

  setTimeout(() => {
    $box.removeClass("scale-90 opacity-0");
    $box.addClass("scale-100 opacity-100");
  }, 10);
}

function hideModalDelete(modalId) {
  if (isStaticModal && typeof modalId === "undefined") return;

  let $modal, $box;

  if (modalId) {
    $modal = $("#" + modalId);
  } else {
    $modal = $(".modal").not(".hidden").last();
  }

  if (!$modal || $modal.length === 0) return;

  $box = $modal.find(".modalBox");
  $box.removeClass("scale-100 opacity-100");
  $box.addClass("scale-90 opacity-0");

  setTimeout(() => {
    $modal.addClass("hidden");
    $backdrop.addClass("hidden");
    isStaticModal = false;
  }, 200);
}

if ($backdrop && $backdrop.length) {
  $backdrop.on("click", () => {
    if (!isStaticModal) hideModal();
  });
}
