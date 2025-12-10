function resetFile() {
  fileInput.value = "";
  previewArea.classList.add("hidden");
  filePreview.src = "";
  fileName.textContent = "";
  fileSize.textContent = "";
  fileError.classList.add("hidden");
  $("#fileInput").attr("required", true);
}

(function () {
  const dropZone = document.getElementById("dropZone");
  const fileInput = document.getElementById("fileInput");
  const previewArea = document.getElementById("previewArea");
  const filePreview = document.getElementById("filePreview");
  const fileName = document.getElementById("fileName");
  const fileSize = document.getElementById("fileSize");
  const vError = document.getElementById("fileError");

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
    fileInput.files = dataTransfer.files;
    fileInput.dispatchEvent(new Event("change"));
  });

  // file helpers
  function showFile(file) {
    const url = URL.createObjectURL(file);
    filePreview.src = url;
    fileName.textContent = file.name;
    fileSize.textContent = Math.round(file.size / 1024) + " KB";
    previewArea.classList.remove("hidden");
  }

  // handle selection
  fileInput.addEventListener("change", (ev) => {
    const f = ev.target.files[0];
    if (!f) return;
    if (!VALID_TYPES.includes(f.type) || f.size > MAX_FILE_BYTES) {
      fileError.classList.remove("hidden");
      fileError.textContent = "Invalid file type or too large (max 2 MB).";
      resetFile();
      return;
    }
    fileError.classList.add("hidden");
    showFile(f);
  });
})();
