const MATCH_INFO_BASE_CONTROLLER = "/en/match-info";
const MATCH_INFO_ENDPOINT = {
  CREATE_MATCH_INFO_ENDPOINT: MATCH_INFO_BASE_CONTROLLER + "/create",
  UPDATE_MATCH_INFO_ENDPOINT: MATCH_INFO_BASE_CONTROLLER + "/update",
  DELETE_MATCH_INFO_ENDPOINT: MATCH_INFO_BASE_CONTROLLER + "/delete",
  GET_MATCH_INFO_ENDPOINT: MATCH_INFO_BASE_CONTROLLER + "/get-match-info",
  GET_MATCH_ITEMS_ENDPOINT: MATCH_INFO_BASE_CONTROLLER + "/get-match",
  GET_PLAYER_ITEMS_ENDPOINT: MATCH_INFO_BASE_CONTROLLER + "/get-match-player",
};

(async () => {
  const { MatchSelect } = await import("/js/shared/match_select.js");
  const { CustomSelect } = await import("/js/shared/select_custom.js");

  window.MatchSelect = MatchSelect;
  window.CustomSelect = CustomSelect;

  window.selectMatchInst = MatchSelect.init(
    document.getElementById("selectMatch"),
    {
      showImage: true,
      placeholder: "Select Match",
      imgSize: "w-auto h-7",
    },
  );

  window.selectPlayerInst = CustomSelect.init(
    document.getElementById("selectPlayer"),
    {
      showImage: false,
      placeholder: "Select player",
    },
  );
})();

const resetForm = () => {
  let form = $("#matchInfoForm");
  form[0].reset();

  form.find("input").not("[name='__RequestVerificationToken']").val("");

  window.selectPlayerInst.updateOptions([]);
  window.selectPlayerInst.setValue("");

  if (window.selectMatchInst) {
    window.selectMatchInst.reset();
  }

  $("#isHomeClub").prop("checked", false);
  $("#isHomeClubHidden").val("false");
};

(function () {
  $("#resetBtn").on("click", resetForm);
})();

$("#btnAddNewMatchInfo").on("click", function () {
  resetForm();
  getMatchItems(false);
  const form = $("#matchInfoForm");
  form.attr("action", MATCH_INFO_ENDPOINT.CREATE_MATCH_INFO_ENDPOINT);
  openModal("modal-8xl", true);
});

function toggleEdit(matchInfoId) {
  resetForm();
  try {
    $.ajax({
      url: MATCH_INFO_ENDPOINT.GET_MATCH_INFO_ENDPOINT + "/" + matchInfoId,
      method: "GET",
      headers: {
        RequestVerificationToken: $(
          'input[name="__RequestVerificationToken"]',
        ).val(),
      },
      success: function (response) {
        if (response.statusCode !== 200) {
          alert(response.message);
          return;
        }
        const data = response.data.item;

        const form = $("#matchInfoForm");
        form.find("#matchInfoId").val(data.matchInfoId);
        form.find("#selectMatch").val(data.matchId);
        form.find("#attendance").val(data.attendance);
        form.find("#weather").val(data.weather);
        form.find("#pitchCondition").val(data.pitchCondition);
        form.find("#addedTimeFirstHalf").val(data.addedTimeFirstHalf);
        form.find("#addedTimeSecondHalf").val(data.addedTimeSecondHalf);

        form.find("#title").val(data.matchReportTitle);
        form.find("#content").val(data.matchReportContent);
        form.find("#homeClubReportUrl").val(data.homeClubReportUrl);
        form.find("#awayClubReportUrl").val(data.awayClubReportUrl);

        form.find("#awardBy").val(data.awardBy);
        form.find("#notes").val(data.notes);

        getMatchItems(true, data.matchId);

        $("#isHomeClub").prop("checked", data.isHomeClub);
        $("#isHomeClubHidden").val(data.isHomeClub ? "true" : "false");

        toggleGetPlayers(data.isHomeClub, data.matchId, data.playerId);

        form.attr(
          "action",
          MATCH_INFO_ENDPOINT.UPDATE_MATCH_INFO_ENDPOINT + "/" + matchInfoId,
        );
        $("#modalTitle").text("Update Match Info");
        openModal("modal-8xl", true);
      },
      error: function (xhr, status, error) {
        console.error(error);
        alert(JSON.stringify(error));
      },
    });
  } catch (error) {
    console.error(error);
    alert(JSON.stringify(error));
  }
}

function getMatchItems(isEdit = false, existingMatchId = "") {
  try {
    $.ajax({
      url: MATCH_INFO_ENDPOINT.GET_MATCH_ITEMS_ENDPOINT + "/" + isEdit,
      method: "GET",
      headers: {
        RequestVerificationToken: $(
          'input[name="__RequestVerificationToken"]',
        ).val(),
      },
      success: function (response) {
        if (response.statusCode !== 200) {
          alert(response.message);
          return;
        }
        const data = response.data.item;

        if (window.selectMatchInst && data) {
          window.selectMatchInst.setData(data);

          if (existingMatchId) {
            window.selectMatchInst.setValue(existingMatchId);
            form.find("#selectMatch").val(existingMatchId);
          }
        } else {
          console.error("Match selection failed: Instance or data missing", {
            instance: window.selectMatchInst,
            data: data,
          });
        }
      },
      error: function (xhr, status, error) {
        console.error(error);
        alert(JSON.stringify(error));
      },
    });
  } catch (error) {
    console.error(error);
    alert(JSON.stringify(error));
  }
}

function toggleGetPlayers(isHomeClub = false, matchId, selectedValue = "") {
  return $.ajax({
    url: MATCH_INFO_ENDPOINT.GET_PLAYER_ITEMS_ENDPOINT,
    method: "POST",
    data: { isHomeClub: isHomeClub, matchId: matchId },
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    success: function (response) {
      let data = response.data.item;

      if (!Array.isArray(data)) {
        data = [data];
      }
      const itemOptions = data.map((it) => ({
        value: it.value,
        label: it.label || "",
        img: it.img || "",
      }));

      if (window.selectPlayer) {
        window.selectPlayerInst.updateOptions(itemOptions);
        window.selectPlayerInst.setValue(selectedValue);
      }
    },
  });
}

$("#isHomeClub").on("change", function () {
  const matchId = $("#selectMatch").val();
  if (matchId) toggleGetPlayers(this.checked, matchId);

  $("#isHomeClubHidden").val(this.checked ? "true" : "false");
});
