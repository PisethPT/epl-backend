const MATCH_BASE_CONTROLLER = "/matches";
const MATCH_ENDPOINT = {
  CREATE_MATCH_ENDPOINT: MATCH_BASE_CONTROLLER + "/create",
  UPDATE_MATCH_ENDPOINT: MATCH_BASE_CONTROLLER + "/update",
  FIND_MATCH_BY_ID_ENDPOINT: MATCH_BASE_CONTROLLER + "/get-match",
};

const resetForm = () => {
  let form = $("#matchForm");
  form[0].reset();
  form
    .find("input")
    .not("[name='__RequestVerificationToken']")
    .not("#matchWeek")
    .not("#seasonIdHidden")
    .val("");
  form.find("input[type='checkbox']").prop("checked", false);
  form.find("input[type='checkbox']").closest("div").find("span").text("No");
};

(function () {
  $("#resetBtn").on("click", resetForm);

  $("#matchForm").on("submit", function (e) {
    const domForm = e.target;

    if (!domForm.checkValidity()) {
      domForm.reportValidity();
      e.preventDefault();
      return;
    }
  });
})();

$("#btnAddNewMatch").on("click", function () {
  const form = $("#matchForm");
  resetForm();
  const now = new Date();
  const today = now.toISOString().split("T")[0];
  const timeNow = now.toTimeString().slice(0, 5);

  $("#matchDate").val(today);
  $("#matchTime").val(timeNow);
  const seasonId = $("#seasonIdHidden").val();
  seasonInst.setValue(seasonId);

  $("#matchDate").attr("min", today);
  form.attr("action", MATCH_ENDPOINT.CREATE_MATCH_ENDPOINT);
});

$("#isHomeStadium").on("change", function () {
  const isChecked = $(this).is(":checked");
  const labelSpan = $(this).closest("div").find("span");

  if (isChecked) {
    labelSpan.text("Yes");
  } else {
    labelSpan.text("No");
  }
});
