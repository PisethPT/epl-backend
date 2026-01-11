const $backdrop = $("#modalBackdrop");
let isStaticModal = false;

function showModal(modalId, button, sizeClass = "modal-md", isStatic = false) {
  isStaticModal = !!isStatic;
  const $modal = $("#" + modalId);
  const $box = $modal.find(".modalBox");
  const $seasonId = $(button).find('input[name="seasonId"]').val();
  const $seasonName = $(button).find('input[name="seasonName"]').val();
  const $startDate = $(button).find('input[name="startDate"]').val();
  const $endDate = $(button).find('input[name="endDate"]').val();

  const form = $("#confirmDeleteForm");
  form.find("#seasonId").val($seasonId);

  $modal.find("#seasonName").text($seasonName);
  $modal.find("#seasonDate").text($startDate + " - " + $endDate);

  $box.attr(
    "class",
    `
      modalBox
      !bg-[#37003c] rounded-2xl shadow-xl p-6 transition-all duration-300
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

if ($backdrop && $backdrop.length) {
  $backdrop.on("click", () => {
    if (!isStaticModal) hideModal(); 
  });
}
