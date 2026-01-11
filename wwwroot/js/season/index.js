const SEASON_BASE_CONTROLLER = "/seasons";
const SEASON_ENDPOINT = {
  CREATE_SEASON_ENDPOINT: SEASON_BASE_CONTROLLER + "/create",
  UPDATE_SEASON_ENDPOINT: SEASON_BASE_CONTROLLER + "/update",
  FIND_SEASON_BY_ID_ENDPOINT: SEASON_BASE_CONTROLLER + "/get-season",
};

const resetForm = () => {
  let form = $("#seasonForm");
  form[0].reset();
  form.find("input").not("[name='__RequestVerificationToken']").val("");
};

(function () {
  $("#resetBtn").on("click", resetForm);

  $("#seasonForm").on("submit", function (e) {
    const domForm = e.target;

    if (!domForm.checkValidity()) {
      domForm.reportValidity();
      e.preventDefault();
      return;
    }
  });
})();

$("#btnAddNewSeason").on("click", function () {
  const form = $("#seasonForm");
  resetForm();
  form.attr("action", SEASON_ENDPOINT.CREATE_SEASON_ENDPOINT);
});

const attachSeasonByIdForUpdate = (seasonId) => {
  $.ajax({
    url: SEASON_ENDPOINT.FIND_SEASON_BY_ID_ENDPOINT + "/" + seasonId,
    method: "POST",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]'
      ).val(),
    },
    success: function (data) {
      // Fill form data
      $("#seasonId").val(data.seasonId);
      $("#seasonName").val(data.seasonName);
      $("#startDate").val(data.startDate);
      $("#endDate").val(data.endDate);

      $("#modalTitle").text("Update Season");
      $("#seasonForm").attr(
        "action",
        SEASON_ENDPOINT.UPDATE_SEASON_ENDPOINT + "/" + data.seasonId
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
