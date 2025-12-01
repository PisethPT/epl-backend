(function () {
  const removeLogo = document.getElementById("removeLogo");
  const form = document.getElementById("clubForm");

  // remove button
  removeLogo.addEventListener("click", () => {
    resetFile();
  });

  // reset form
  document.getElementById("resetBtn").addEventListener("click", () => {
    form.reset();
    form.querySelectorAll("input").forEach((input) => (input.value = ""));
    resetFile();
    // reset color swatch to default
    resetColorPicker();
  });

  // submit: simple client validation and demo output
  document.getElementById("clubForm").addEventListener("submit", (e) => {
    const form = e.target;

    if (!form.checkValidity()) {
      form.reportValidity();
      return;
    }

    const clubId = document.getElementById("clubId").value;
    const logoInput = document.getElementById("logoInput");
    const logoError = document.getElementById("logoError");

    logoError.classList.add("hidden");
    logoError.textContent = "";

    const isCreate = clubId === "" || clubId === "0";
    const hasNewFile = logoInput.files.length > 0;

    if (isCreate && !hasNewFile) {
      logoError.classList.remove("hidden");
      logoError.textContent = "Please upload a club logo.";
      e.preventDefault();
      return;
    }
  });
})();

document.getElementById("btnAddNewClub").addEventListener("click", (e) => {
  form.reset();
  resetFile();
  form.querySelectorAll("input").forEach((input) => (input.value = ""));
  resetColorPicker();
});

function attachEditClub(clubId) {
  $.ajax({
    url: "/clubs/get-club/" + clubId,
    method: "POST",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]'
      ).val(),
    },
    success: function (data) {
      // Fill fields safely
      $("#clubId").val(data.id);
      $("#clubName").val(data.name ?? "");
      $("#founded").val(data.founded ?? "");
      $("#city").val(data.city ?? "");
      $("#stadium").val(data.stadium ?? "");
      $("#coach").val(data.headCoach ?? "");
      $("#website").val(data.clubOfficialWebsite ?? "");

      // theme color picker + hex box
      if (data.theme) {
        $("#themeColor").val(data.theme);
        $("#themeHex").val(data.theme);
      } else {
        $("#themeColor").val("#55005a");
        $("#themeHex").val("#55005a");
      }

      // logo preview
      if (data.crest) {
        const logoPath = "/folders/clubs/" + data.crest; // adjust your folder path
        $("#logoPreview").attr("src", logoPath);
        $("#logoName").text(data.crest);
        $("#previewArea").removeClass("hidden");
        $("#logoInput").attr("required", false);
      } else {
        // reset preview
        $("#previewArea").addClass("hidden");
        $("#logoPreview").attr("src", "");
        $("#logoName").text("");
        $("#logoInput").attr("required", true);
      }
      
      $("#clubForm").attr("action", "/clubs/edit/" + data.id);
      // Open modal after fill
      openModal("modal-8xl", true);
    },
    error: function (err) {
      console.error(err);
    },
  });
}
