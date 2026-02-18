const MATCH_BASE_CONTROLLER = "/matches";
const MATCH_ENDPOINT = {
  CREATE_MATCH_ENDPOINT: MATCH_BASE_CONTROLLER + "/create",
  UPDATE_MATCH_ENDPOINT: MATCH_BASE_CONTROLLER + "/update",
  FIND_MATCH_BY_ID_ENDPOINT: MATCH_BASE_CONTROLLER + "/get-match",
  GET_MATCH_DETAILS_BY_MATCH_ID_ENDPOINT:
    MATCH_BASE_CONTROLLER + "/get-match-details",
};

const tabBtns = document.querySelectorAll(".tab-btn");
const tabContents = document.querySelectorAll(".tab-content");

const requiredFields = [
  {
    selector: "#seasonId",
    validation: "#seasonId-validation",
    message: "Season is required.",
    tab: "tab-match",
  },
  {
    selector: "#homeClub",
    validation: "#homeClubId-validation",
    message: "Home club is required.",
    tab: "tab-match",
  },
  {
    selector: "#awayClub",
    validation: "#awayClubId-validation",
    message: "Away club is required.",
    tab: "tab-match",
  },
  {
    selector: "#refereeId",
    validation: "#referee-validation",
    message: "Referee is required.",
    tab: "tab-referee",
  },
  {
    selector: "#assistReferee1Id",
    validation: "#assistReferee1-validation",
    message: "Assist Referee 1 is required.",
    tab: "tab-referee",
  },
  {
    selector: "#assistReferee2Id",
    validation: "#assistReferee2-validation",
    message: "Assist Referee 2 is required.",
    tab: "tab-referee",
  },
  {
    selector: "#refereeFourthOfficialId",
    validation: "#refereeFourthOfficial-validation",
    message: "Referee Fourth Official is required.",
    tab: "tab-referee",
  },
  {
    selector: "#refereeVARId",
    validation: "#refereeVAR-validation",
    message: "Referee VAR is required.",
    tab: "tab-referee",
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

  form.find("select.js-custom-select").each(function () {
    const placeholder = $(this).data("placeholder") || "Please select";
    const $btn = $(this).parent().find("button");
    $btn.find("img").addClass("hidden");
    $btn.find("span.truncate.block").text(placeholder);
  });

  activateTab("tab-match");
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

  $("#isHomeStadium").prop("checked", true);
  $("#homeStadiumLabel").text("Yes");
  $("#isHomeStadiumHidden").val("true");

  $("#matchDate").attr("min", today);
  form.attr("action", MATCH_ENDPOINT.CREATE_MATCH_ENDPOINT);
});

$("#isHomeStadium").on("change", function () {
  $("#homeStadiumLabel").text(this.checked ? "Yes" : "No");
  $("#isHomeStadiumHidden").val(this.checked ? "true" : "false");
});

const attachMatchByIdForUpdate = (matchId) => {
  resetForm();
  $.ajax({
    url: MATCH_ENDPOINT.FIND_MATCH_BY_ID_ENDPOINT + "/" + matchId,
    method: "POST",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    success: async function (data) {
      // Fill form data
      const referees = JSON.parse(JSON.stringify(await data.matchReferees));

      $("#matchId").val(data.matchId);
      $("#seasonId").val(data.seasonId);
      $("#matchDate").val(data.matchDate);
      $("#matchTime").val(data.matchTime);
      $("#matchWeek").val(data.matchWeek);
      $("#isHomeStadium").prop("checked", data.isHomeStadium);
      $("#homeStadiumLabel").text(data.isHomeStadium ? "Yes" : "No");
      $("#isHomeStadiumHidden").val(data.isHomeStadium);

      seasonInst.setValue(data.seasonId);
      homeClubInst.setValue(data.homeClubId);
      awayClubInst.setValue(data.awayClubId);
      homeClubInst.setValue(data.homeClubId);

      referees.forEach((ref) => {
        switch (ref.roleId) {
          case 1:
            refereeInst.setValue(ref.refereeId);
            $("#refereeMatchId").val(ref.matchId);
            $("#refereeMatchRefereeId").val(ref.matchRefereeId);
            $("#refereeRoleId").val(ref.roleId);
            break;
          case 2:
            assistReferee1Inst.setValue(ref.refereeId);
            $("#assistReferee1MatchId").val(ref.matchId);
            $("#assistReferee1MatchRefereeId").val(ref.matchRefereeId);
            $("#assistReferee1RoleId").val(ref.roleId);
            break;
          case 3:
            assistReferee2Inst.setValue(ref.refereeId);
            $("#assistReferee2MatchId").val(ref.matchId);
            $("#assistReferee2MatchRefereeId").val(ref.matchRefereeId);
            $("#assistReferee2RoleId").val(ref.roleId);
            break;
          case 4:
            refereeFourthOfficialInst.setValue(ref.refereeId);
            $("#refereeFourthOfficialMatchId").val(ref.matchId);
            $("#refereeFourthOfficialMatchRefereeId").val(ref.matchRefereeId);
            $("#refereeFourthOfficialRoleId").val(ref.roleId);
            break;
          case 5:
            refereeVARInst.setValue(ref.refereeId);
            $("#refereeVARMatchId").val(ref.matchId);
            $("#refereeVARMatchRefereeId").val(ref.matchRefereeId);
            $("#refereeVARRoleId").val(ref.roleId);
            break;
        }
      });

      $("#modalTitle").text("Update Match");
      $("#matchForm").attr(
        "action",
        MATCH_ENDPOINT.UPDATE_MATCH_ENDPOINT + "/" + data.matchId,
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
        activateTab(field.tab);
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

function activateTab(tabId) {
  const tabBtn = document.querySelector(`[data-tab="${tabId}"]`);

  if (tabBtn) {
    tabBtn.click();
  }
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
