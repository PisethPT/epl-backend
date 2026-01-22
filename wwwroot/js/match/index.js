const MATCH_BASE_CONTROLLER = "/matches";
const MATCH_ENDPOINT = {
  CREATE_MATCH_ENDPOINT: MATCH_BASE_CONTROLLER + "/create",
  UPDATE_MATCH_ENDPOINT: MATCH_BASE_CONTROLLER + "/update",
  FIND_MATCH_BY_ID_ENDPOINT: MATCH_BASE_CONTROLLER + "/get-match",
};

const tabBtns = document.querySelectorAll(".tab-btn");
const tabContents = document.querySelectorAll(".tab-content");

const requiredFields = [
  {
    selector: "#seasonId",
    validation: "#seasonId-validation",
    message: "Season is required.",
  },
  {
    selector: "#homeClub",
    validation: "#homeClubId-validation",
    message: "Home club is required.",
  },
  {
    selector: "#awayClub",
    validation: "#awayClubId-validation",
    message: "Away club is required.",
  },
  {
    selector: "#refereeId",
    validation: "#referee-validation",
    message: "Referee is required.",
  },
  {
    selector: "#assistReferee1Id",
    validation: "#assistReferee1-validation",
    message: "Assist Referee 1 is required.",
  },
  {
    selector: "#assistReferee2Id",
    validation: "#assistReferee2-validation",
    message: "Assist Referee 2 is required.",
  },
  {
    selector: "#refereeFourthOfficialId",
    validation: "#refereeFourthOfficial-validation",
    message: "Referee Fourth Official is required.",
  },
  {
    selector: "#refereeVARId",
    validation: "#refereeVAR-validation",
    message: "Referee VAR is required.",
  },
];

tabBtns.forEach((btn) => {
  btn.addEventListener("click", () => {
    tabBtns.forEach((b) => {
      b.classList.remove("text-[#8a3fbf]", "border-b-2", "border-[#8a3fbf]");
      b.classList.add("text-gray-400");
    });

    tabContents.forEach((c) => c.classList.add("hidden"));

    btn.classList.add("text-[#8a3fbf]", "border-b-2", "border-[#8a3fbf]");
    btn.classList.remove("text-gray-400");

    document.getElementById(btn.dataset.tab).classList.remove("hidden");
  });
});

const resetForm = () => {
  let form = $("#matchForm");
  form[0].reset();
  form
    .find("input")
    .not("[name='__RequestVerificationToken']")
    .not("#matchWeek")
    .not("#seasonIdHidden")
    .not("#isHomeStadium")
    .not("#refereeRoleId")
    .not("#assistReferee1RoleId")
    .not("#assistReferee2RoleId")
    .not("#refereeFourthOfficialRoleId")
    .not("#refereeVARRoleId")
    .val("");
  form.find("input[type='checkbox']").prop("checked", false);
  form.find("input[type='checkbox']").closest("div").find("span").text("No");
};

(function () {
  $("#resetBtn").on("click", resetForm);
  validateForm("#matchForm");
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
  $("#homeStadiumLabel").text(this.checked ? "Yes" : "No");
  $("#isHomeStadiumHidden").val(this.checked ? "true" : "false");
});

function validateForm(formSelector) {
  $(formSelector).on("submit", function (e) {
    e.preventDefault();
    const form = this;
    let isValid = true;

    requiredFields.forEach((f) => $(f.validation).text(""));

    for (const field of requiredFields) {
      if (!$(field.selector).val()) {
        $(field.validation).text(field.message);
        $(field.selector).focus();
        isValid = false;
        break;
      }
    }

    if (isValid && !form.checkValidity()) {
      form.reportValidity();
      return false;
    }
    if (isValid) form.submit();
  });
}

function clearValidationOnChange(selectSelector, validationSelector) {
  $(document).on("change", selectSelector, function () {
    if ($(this).val()) {
      $(validationSelector).text("");
    }
  });
}

clearValidationOnChange("#awayClub", "#awayClubId-validation");
clearValidationOnChange("#homeClub", "#homeClubId-validation");
clearValidationOnChange("#seasonId", "#seasonId-validation");

clearValidationOnChange("#refereeId", "#referee-validation");
clearValidationOnChange("#assistReferee1Id", "#assistReferee1-validation");
clearValidationOnChange("#assistReferee2Id", "#assistReferee2-validation");
clearValidationOnChange(
  "#refereeFourthOfficialId",
  "#refereeFourthOfficial-validation",
);
clearValidationOnChange("#refereeVARId", "#refereeVAR-validation");
