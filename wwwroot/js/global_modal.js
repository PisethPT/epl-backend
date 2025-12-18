const $backdrop = $("#modalBackdrop");
let isStaticModal = false;

function showModal(modalId, button, sizeClass = "modal-md", isStatic = false) {
  isStaticModal = !!isStatic;
  const $modal = $("#" + modalId);
  const $box = $modal.find(".modalBox");
  const $Id = $(button).find('input[name="userId"]').val();

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
