const modal = document.getElementById("modal");
const backdrop = document.getElementById("modalBackdrop");
const box = document.getElementById("modalBox");

let staticModal = false;

function openModal(sizeClass = "modal-md", isStatic = false) {
  staticModal = isStatic;

  // completely reset and re-apply classes
  box.className = `
    !bg-[#37003c] rounded-2xl shadow-xl p-6 transition-all duration-300
    scale-90 opacity-0 h-auto w-full ${sizeClass}
  `.trim();

  modal.classList.remove("hidden");
  backdrop.classList.remove("hidden");

  setTimeout(() => {
    box.classList.remove("scale-90", "opacity-0");
    box.classList.add("scale-100", "opacity-100");
  }, 10);
}

function closeModal() {
  box.classList.add("scale-90", "opacity-0");

  setTimeout(() => {
    modal.classList.add("hidden");
    backdrop.classList.add("hidden");
  }, 200);
}

// close on background click (non-static only)
backdrop.addEventListener("click", () => {
  if (!staticModal) closeModal();
});
