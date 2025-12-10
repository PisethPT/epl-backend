// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function closeAlertPanel(alertPanel) {
  const alert = document.getElementById(alertPanel);
  if (!alert.classList.contains("hidden")) alert.classList.add("hidden");
}
