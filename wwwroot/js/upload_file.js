function resetFile() {
  logoInput.value = "";
  previewArea.classList.add("hidden");
  logoPreview.src = "";
  logoName.textContent = "";
  logoSize.textContent = "";
  logoError.classList.add("hidden");
  $("#fileInput").attr("required", true);
}

(function () {
  const dropZone = document.getElementById("dropZone");
  const logoInput = document.getElementById("fileInput");
  const previewArea = document.getElementById("previewArea");
  const logoPreview = document.getElementById("filePreview");
  const logoName = document.getElementById("fileName");
  const logoSize = document.getElementById("fileSize");
  const logoError = document.getElementById("fileError");

  const MAX_FILE_BYTES = 2 * 1024 * 1024; // 2 MB
  const VALID_TYPES = [
    "image/png",
    "image/jpeg",
    "image/jpg",
    "image/svg+xml",
    "image/webp",
  ];

  // drag and drop
  if (!dropZone) return;

  dropZone.addEventListener("dragover", (e) => {
    e.preventDefault();
    dropZone.classList.add("border-white/60", "bg-white/5");
  });
  dropZone.addEventListener("dragleave", () => {
    dropZone.classList.remove("border-white/60", "bg-white/5");
  });
  dropZone.addEventListener("drop", (e) => {
    e.preventDefault();
    dropZone.classList.remove("border-white/60", "bg-white/5");
    const f = e.dataTransfer.files?.[0];
    if (!f) return;
    // set file to input programmatically (for form send)
    const dataTransfer = new DataTransfer();
    dataTransfer.items.add(f);
    logoInput.files = dataTransfer.files;
    logoInput.dispatchEvent(new Event("change"));
  });

  // file helpers
  function showFile(file) {
    const url = URL.createObjectURL(file);
    logoPreview.src = url;
    logoName.textContent = file.name;
    logoSize.textContent = Math.round(file.size / 1024) + " KB";
    previewArea.classList.remove("hidden");
  }

  // handle selection
  logoInput.addEventListener("change", (ev) => {
    const f = ev.target.files[0];
    if (!f) return;
    if (!VALID_TYPES.includes(f.type) || f.size > MAX_FILE_BYTES) {
      logoError.classList.remove("hidden");
      logoError.textContent = "Invalid file type or too large (max 2 MB).";
      resetFile();
      return;
    }
    logoError.classList.add("hidden");
    showFile(f);
  });
})();
