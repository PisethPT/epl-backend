const $initBackdrop = $("#modalBackdrop");
let isInitStaticModal = false;

function showModal(modalId, sizeClass = "modal-md", isStatic = false) {
  isInitStaticModal = !!isStatic;
  const $modal = $("#" + modalId);
  const $box = $modal.find(".modalBox");

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

function hideModal(modalId) {
  if (isInitStaticModal && typeof modalId === "undefined") return;

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
    $initBackdrop.addClass("hidden");
    isInitStaticModal = false;
  }, 200);
}

if ($initBackdrop && $initBackdrop.length) {
  $initBackdrop.on("click", () => {
    if (!isInitStaticModal) hideModal();
  });
}
