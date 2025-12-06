/// <reference types="jquery" />

(function () {
  $("#removePhoto").on("click", function () {
    resetFile();
  });

  $("#resetBtn").on("click", function () {
    const form = $("#userForm");
    form[0].reset();
    resetFile();

    form.find("input").each(function () {
      if (this.name !== "__RequestVerificationToken") {
        this.value = "";
      }
    });
  });

  $("#userForm").on("submit", function (e) {
    const domForm = e.target;

    if (!domForm.checkValidity()) {
      domForm.reportValidity();
      e.preventDefault();
      return;
    }

    const userId = $("#userId").val();
    const fileInput = $("#fileInput")[0];
    const fileError = $("#fileError");

    fileError.addClass("hidden").text("");

    const isCreate = !userId;
    const hasNewFile = fileInput && fileInput.files.length > 0;
    if (isCreate && !hasNewFile) {
      fileError.removeClass("hidden").text("Please upload a user photo");
      e.preventDefault();
      return;
    }
  });
})();

$("#btnAddNewUser").on("click", function () {
  const form = $("#userForm");
  $("#modalTitle").text("Create New User");
  form[0].reset();
  resetFile();

  form.find("input").each(function () {
    if (this.name !== "__RequestVerificationToken") {
      this.value = "";
    }
  });

  form.attr("action", "/auth/create");
});
