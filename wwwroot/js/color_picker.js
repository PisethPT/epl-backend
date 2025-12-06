// elements
const themeColor = document.getElementById("themeColor");
const themeHex = document.getElementById("themeHex");
// const colorSwatch = document.getElementById("colorSwatch");

// reset color picker
function resetColorPicker() {
  const defaultColor = "#55005a";
  themeColor.value = defaultColor;
  themeHex.value = defaultColor;
  // colorSwatch.style.background = defaultColor;
}

(function () {
  // color sync
  if (!themeColor || !themeHex) return;

  themeColor.addEventListener("input", (e) => {
    themeHex.value = e.target.value;
    // colorSwatch.style.background = e.target.value;
  });
  themeHex.addEventListener("input", (e) => {
    const v = e.target.value.trim();
    if (/^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(v)) {
      themeColor.value = v;
      // colorSwatch.style.background = v;
    }
  });
})();
