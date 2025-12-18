const PLAYER_BASE_CONTROLLER = "/players";
const PLAYER_ENDPOINT = {
  CREATE_PLAYER_ENDPOINT: PLAYER_BASE_CONTROLLER + "/create",
  UPDATE_PLAYER_ENDPOINT: PLAYER_BASE_CONTROLLER + "/update",
  FIND_PLAYER_BY_ID_ENDPOINT: PLAYER_BASE_CONTROLLER + "/get-player",
};

const resetForm = () => {
  let form = $("#playerForm");
  form[0].reset();
  resetFile();
  form.find("input").not("[name='__RequestVerificationToken']").val("");
  form.find("select.js-custom-select").each(function () {
    const placeholder = $(this).data("placeholder") || "Please select";
    const $btn = $(this).parent().find("button");
    $btn.find("img").addClass("hidden");
    $btn.find("span.truncate.block").text(placeholder);
  });
};

(function () {
  $("#removePhoto").on("click", function () {
    resetFile();
  });

  $("#resetBtn").on("click", resetForm);

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
      alert("file required");
      fileError.removeClass("hidden").text("Please upload a player photo");
      e.preventDefault();
      return;
    }
  });
})();

$("#btnAddNewPlayer").on("click", function () {
  const form = $("#playerForm");
  resetForm();
  form.attr("action", PLAYER_ENDPOINT.CREATE_PLAYER_ENDPOINT);
});

const attachPlayerByIdForUpdate = (playerId) => {
  resetForm();
  $.ajax({
    url: PLAYER_ENDPOINT.FIND_PLAYER_BY_ID_ENDPOINT + "/" + playerId,
    method: "POST",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]'
      ).val(),
    },
    success: function (data) {
      $("#playerId").val(data.playerId);
      $("#photo").val(data.photo);
      $("#firstName").val(data.firstName);
      $("#lastName").val(data.lastName);
      $("#dateOfBirth").val(data.dateOfBirth);
      $("#height").val(data.height);
      $("#playerNumber").val(data.playerNumber);

      // selects box
      $("#placeOfBirth").val(data.placeOfBirth);
      $("#nationality").val(data.nationality);
      $("#preferredFoot").val(data.preferredFoot);
      $("#position").val(data.position);
      $("#joinedClub").val(data.joinedClub);
      $("#playerClub").val(data.clubId);

      placeOfBirthsInst.setValue(data.placeOfBirth);
      nationalitiesInst.setValue(data.nationality);
      positionInst.setValue(data.position == 0 ? "0" : data.position);
      preferredFootInst.setValue(
        data.preferredFoot == 0 ? "0" : data.preferredFoot
      );
      joinedClubFootInst.setValue(data.joinedClub == 0 ? "0" : data.joinedClub);
      playerClubInst.setValue(data.clubId);

      if (data.photo) {
        const photoPath = "/upload/players/" + data.photo ?? "placeholder.png";
        $("#filePreview").attr("src", photoPath);
        $("#fileName").text(data.photo);
        $("#previewArea").removeClass("hidden");
        $("#fileInput").attr("required", false);
      } else {
        $("#previewArea").addClass("hidden");
        $("#filePreview").attr("src", "");
        $("#fileName").text("");
        $("#fileInput").attr("required", true);
      }

      $("#modalTitle").text("Update Player");
      $("#playerForm").attr(
        "action",
        PLAYER_ENDPOINT.UPDATE_PLAYER_ENDPOINT + "/" + data.playerId
      );

      // Open modal after fill
      openModal("modal-8xl", true);
    },
    error: function (err) {
      alert(JSON.stringify(err));
      console.error(err);
    },
  });
};
