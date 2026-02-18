const matchDetailTabBtns = document.querySelectorAll(".match-detail-tab-btn");
const matchDetailTabContents = document.querySelectorAll(
  ".match-detail-tab-content",
);

matchDetailTabBtns.forEach((btn) => {
  btn.addEventListener("click", () => {
    matchDetailTabBtns.forEach((b) => {
      b.classList.remove("text-[#8a3fbf]", "border-b-2", "border-[#8a3fbf]");
      b.classList.add("text-gray-400");
    });

    matchDetailTabContents.forEach((c) => c.classList.add("hidden"));

    btn.classList.add("text-[#8a3fbf]", "border-b-2", "border-[#8a3fbf]");
    btn.classList.remove("text-gray-400");

    document.getElementById(btn.dataset.tab).classList.remove("hidden");
  });
});

function openMatchDetailTab(matchId) {
  $.ajax({
    url: MATCH_ENDPOINT.GET_MATCH_DETAILS_BY_MATCH_ID_ENDPOINT + "/" + matchId,
    method: "POST",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    success: async function (response) {
      const { matchDetail, matchOfficials } = await response.data;

      if (!matchDetail) {
        $("#kickoff").text("N/A");
        $("#stadium").text("N/A");
        $("#attendance").text("N/A");
      } else {
        $("#kickoff").text(matchDetail.kickoff ?? "N/A");
        $("#stadium").text(matchDetail.stadium ?? "N/A");
        $("#attendance").text(matchDetail.attendance ?? "N/A");
      }

      $("#matchOfficials").empty();
      if (matchOfficials.length === 0) {
        let noOfficialDiv = `<div class="flex justify-center">
                                <span class="text-gray-400 text-md text-center">No officials assigned for this match.</span>
                            </div>`;
        $("#matchOfficials").append(noOfficialDiv);
      } else {
        matchOfficials.forEach((o) => {
          let officialDiv = `<div class="flex justify-between">
                                <span class="text-gray-400 text-md text-start">${o.refereeRole}</span>
                                <span class="text-white text-md text-end font-bold">${o.refereeName}</span>
                            </div>`;
          $("#matchOfficials").append(officialDiv);
        });
      }

      showModal("matchDetailModal", "modal-8xl", true);
    },
    error: function (err) {
      alert(JSON.stringify(err));
      console.error(err);
    },
  });
}
