(function () {
  const form = document.getElementById("clubForm");
  const removeLogo = document.getElementById("removeLogo");

  // remove button
  removeLogo.addEventListener("click", () => {
    resetFile();
  });

  // reset form
  document.getElementById("resetBtn").addEventListener("click", () => {
    form.reset();
    form.querySelectorAll("input").forEach(
      (input) =>
        function () {
          if (input.name != "__RequestVerificationToken") input.value = "";
        }
    );
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
    const logoInput = document.getElementById("fileInput");
    const logoError = document.getElementById("fileError");

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
  const clubForm = document.getElementById("clubForm");
  $("#modalTitle").text("Create New Club");
  clubForm.reset();
  resetFile();
  clubForm.querySelectorAll("input").forEach(
    (input) =>
      function () {
        if (input.name != "__RequestVerificationToken") input.value = "";
      }
  );
  resetColorPicker();
  $("#clubForm").attr("action", "/clubs/create");
});

function attachUpdateClub(clubId) {
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
      $("#crest").val(data.crest ?? "");

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
        const logoPath = "/upload/clubs/" + data.crest; // adjust your folder path
        $("#filePreview").attr("src", logoPath);
        $("#fileName").text(data.crest);
        $("#previewArea").removeClass("hidden");
        $("#fileInput").attr("required", false);
      } else {
        // reset preview
        $("#previewArea").addClass("hidden");
        $("#filePreview").attr("src", "");
        $("#fileName").text("");
        $("#fileInput").attr("required", true);
      }

      $("#modalTitle").text("Update Club");
      $("#clubForm").attr("action", "/clubs/update/" + data.id);
      // Open modal after fill
      openModal("modal-8xl", true);
    },
    error: function (err) {
      alert(JSON.stringify(err));
      console.error(err);
    },
  });
}
