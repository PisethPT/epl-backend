const PLAYER_BASE_CONTROLLER = "/player";
const PLAYER_ENDPOINT = {
  CREATE_PLAYER_ENDPOINT: PLAYER_BASE_CONTROLLER + "/create",
  UPDATE_PLAYER_ENDPOINT: PLAYER_BASE_CONTROLLER + "/update",
  GET_PLAYER_BY_PLAYER_ID_ENDPOINT: PLAYER_BASE_CONTROLLER + "/get-player",
};

(function () {
  $("#removePhoto").on("click", function () {
    resetFile();
  });

  $("#resetBtn").on("click", function () {
    const form = $("#playerForm");
    form[0].reset();
    resetFile();

    form.find("input").each(function () {
      if (this.name !== "__RequestVerificationToken") {
        this.value = "";
      }
    });
  });

  $("#playerForm").on("submit", function (e) {
    const domForm = e.target;

    if (!domForm.checkValidity()) {
      domForm.reportValidity();
      e.preventDefault();
      return;
    }

    const playerId = $("#playerId").val();
    const fileInput = $("#fileInput")[0];
    const fileError = $("#fileError");

    fileError.addClass("hidden").text("");

    const isCreate = !playerId;
    const hasNewFile = fileInput && fileInput.files.length > 0;
    if (isCreate && !hasNewFile) {
      fileError.removeClass("hidden").text("Please upload a user photo");
      e.preventDefault();
      return;
    }
  });
})();

$("#btnAddNewPlayer").on("click", function () {
  const form = $("#playerForm");
  $("#modalTitle").text("Create New Player");
  form[0].reset();
  resetFile();

  form.find("input").each(function () {
    if (this.name !== "__RequestVerificationToken") {
      this.value = "";
    }
  });

  form.attr("action", PLAYER_ENDPOINT.CREATE_PLAYER_ENDPOINT);
});
