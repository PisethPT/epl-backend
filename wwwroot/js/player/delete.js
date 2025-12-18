const $backdrop = $("#modalBackdrop");
let isStaticModal = false;
let textColor = "white";

function showModal(modalId, button, sizeClass = "modal-md", isStatic = false) {
  isStaticModal = !!isStatic;
  const $modal = $("#" + modalId);
  const $box = $modal.find(".modalBox");
  const $playerId = $(button).find('input[name="playerId"]').val();
  const $firstName = $(button).find('input[name="firstName"]').val();
  const $lastName = $(button).find('input[name="lastName"]').val();
  const $playerNumber = $(button).find('input[name="playerNumber"]').val();
  const $position = $(button).find('input[name="position"]').val();
  const $clubName = $(button).find('input[name="clubName"]').val();
  const $clubCrest = $(button).find('input[name="clubCrest"]').val();
  const $clubTheme = $(button).find('input[name="clubTheme"]').val();
  const $photo =
    $(button).find('input[name="photo"]').val() == ""
      ? "placeholder.png"
      : $(button).find('input[name="photo"]').val();

  const form = $("#confirmDeleteForm");

  textColor = $clubTheme == "#ffffff" ? "black" : "white";

  form.find("#playerId").val($playerId);
  $modal.find("#firstName").text($firstName).css("color", textColor);
  $modal.find("#lastName").text($lastName).css("color", textColor);
  $modal.find("#playerNumber").text($playerNumber).css("color", textColor);
  $modal.find("#position").text($position).css("color", textColor);
  $modal.find("#clubName").text($clubName).css("color", textColor);
  $modal.find("#period").css("color", textColor);
  $modal.find("#clubTheme").css("background-color", $clubTheme);
  const photoPath = "/upload/players/" + $photo;
  const clubPath = "/upload/clubs/" + $clubCrest;
  $modal.find("#photo").attr("src", photoPath);
  $modal.find("#clubCrest").attr("src", clubPath);

  $box.attr(
    "class",
    `
      modalBox
      !bg-[#1e0021] rounded-2xl shadow-xl p-6 transition-all duration-300
      scale-90 opacity-0 h-auto w-full ${sizeClass}
    `.trim()
  );

  $modal.removeClass("hidden");
  $backdrop.removeClass("hidden");

  setTimeout(() => {
    $box.removeClass("scale-90 opacity-0");
    $box.addClass("scale-100 opacity-100");
  }, 10);
}

function hideModal(modalId) {
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

// close on backdrop click (non-static only)
if ($backdrop && $backdrop.length) {
  $backdrop.on("click", () => {
    if (!isStaticModal) hideModal(); // closes the topmost visible modal
  });
}
