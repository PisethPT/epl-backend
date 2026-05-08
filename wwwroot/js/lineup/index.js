const LINEUP_BASE_CONTROLLER = "/en/lineups";
const LINEUP_ENDPOINT = {
  CREATE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/create",
  UPDATE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/update",
  DELETE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/delete",
  FIND_LINEUP_BY_ID_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-lineup",
  FIND_FORMATION_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-formations",
  GET_PLAYERS_BY_MATCH_ID_ENDPOINT:
    LINEUP_BASE_CONTROLLER + "/get-players-by-match",
  GET_LINEUP_CLUB_INFO_BY_MATCH_ID_ENDPOINT:
    LINEUP_BASE_CONTROLLER + "/get-lineup-club-info-by-match",
  GET_LINEUP_FORMATION_DETAIL_BY_MATCH_ID_ENDPOINT:
    LINEUP_BASE_CONTROLLER + "/get-lineup-formation-detail-by-match",
  GET_LINEUP_FORMATION_LAYOUT_BY_FORMATION_ID_ENDPOINT:
    LINEUP_BASE_CONTROLLER + "/get-lineup-formation-layout-by-formation-id",
  GET_LINEUP_BY_MATCH_ID_ENDPOINT:
    LINEUP_BASE_CONTROLLER + "/get-lineup-by-match-id",
  GET_MATCH_ITEMS_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-match-items",
};

const BASE_CLUB_PATH = "/upload/clubs/";
const BASE_PLAYER_PATH = "/upload/players/";
const tabBtns = document.querySelectorAll(".tab-btn");
const tabContents = document.querySelectorAll(".tab-content");

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
  let form = $("#lineupForm");
  form[0].reset();

  if (window.matchSelectInst) {
    window.matchSelectInst.reset();
  }

  $("#homeClubFormation").val("1");
  $("#awayClubFormation").val("1");

  if (window.homeClubFormationInst) window.homeClubFormationInst.setValue("1");
  if (window.awayClubFormationInst) window.awayClubFormationInst.setValue("1");

  playerStore.home = {};
  playerStore.away = {};
  state.home.slotPlayers = [];
  state.away.slotPlayers = [];
  state.home.draggedPlayer = null;
  state.away.draggedPlayer = null;

  const noPlayersHtml = '<p class="text-gray-400 text-sm">No players found</p>';
  $("#homeClubPlayerId").empty().html(noPlayersHtml);
  $("#awayClubPlayerId").empty().html(noPlayersHtml);

  renderFormation("home", 1);
  renderFormation("away", 1);
  renderSubSlots("home");
  renderSubSlots("away");

  activateTab("tab-match");
};

(function () {
  $("#resetBtn").on("click", resetForm);
})();

$("#btnAddLineup").on("click", async function () {
  const form = $("#lineupForm");

  resetForm();

  form.attr("action", LINEUP_ENDPOINT.CREATE_LINEUP_ENDPOINT);

  try {
    await getFormations();

    await getMatchItems(false);

    $("#modalTitle").text("Create Lineup");

    openModal("modal-8xl", true);
  } catch (err) {
    console.error("Init lineup failed:", err);
    alert("Failed to initialize lineup");
  }
});

$("#lineupForm").on("submit", function (e) {
  e.preventDefault();

  const dto = {
    MatchId: Number($("#matchSelectId").val()),
    HomeClubFormationId: Number($("#homeClubFormation").val()),
    AwayClubFormationId: Number($("#awayClubFormation").val()),
    HomeClubLineup: [],
    AwayClubLineup: [],
  };

  buildLineup("home", dto.HomeClubLineup);
  buildLineup("away", dto.AwayClubLineup);

  if (dto.HomeClubLineup.filter((x) => x.IsStarting).length !== 11) {
    alert("Home club must have 11 starting players.");
    activateTab("tab-home-club");
    return;
  } else if (dto.AwayClubLineup.filter((x) => x.IsStarting).length !== 11) {
    alert("Away club must have 11 starting players.");
    activateTab("tab-away-club");
    return;
  }

  submitDto(dto);
});

function submitDto(dto) {
  $.ajax({
    url: $("#lineupForm").attr("action"),
    type: "POST",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    data: JSON.stringify(dto),

    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },

    success: function (response) {
      console.log("Success:", response);

      if (response.redirectUrl) {
        window.location.href = response.redirectUrl;
      }
    },

    error: function (xhr) {
      console.error("Submit error:", xhr);

      if (xhr.responseJSON?.redirectUrl) {
        window.location.href = xhr.responseJSON.redirectUrl;
        return;
      }

      console.error("Response text:", xhr.responseText);

      alert(xhr.responseText || "A critical error occurred.");
    },
  });
}

function buildLineup(side, dtoArray) {
  const ctx = state[side];

  const pitchSlots = document.querySelectorAll(
    `#${ctx.formationContainer} [data-player-slot]`,
  );

  pitchSlots.forEach((el) => {
    const playerId = Number(el.dataset.playerSlot);
    if (!playerId) return;

    const player = playerStore[side][playerId];
    if (!player) {
      console.warn("Player not found in store:", playerId);
      return;
    }

    const wrapper = el.closest("[data-formation-slot]");
    const positionId = Number(wrapper?.dataset.formationSlot);

    if (!positionId) {
      console.warn("Missing formation positionId for player:", playerId);
      return;
    }

    dtoArray.push({
      ClubId: player.clubId,
      PlayerId: playerId,
      FormationPositionId: positionId,
      IsStarting: true,
      FormationSlot: positionId,
    });
  });

  const subSlots = document.querySelectorAll(
    `#${ctx.substitutionContainer} [data-is-sub-slot="true"]`,
  );

  subSlots.forEach((slot) => {
    const card = slot.querySelector("[data-player-id]");
    if (!card) return;

    const playerId = Number(card.dataset.playerId);
    const clubId = Number(card.dataset.clubId);
    const slotNumber = Number(slot.dataset.subSlot);

    const player = playerStore[side][playerId];
    if (!player) {
      console.warn("Sub player not found in store:", playerId);
      return;
    }

    dtoArray.push({
      ClubId: clubId,
      PlayerId: playerId,
      FormationPositionId: player.positionId,
      IsStarting: false,
      FormationSlot: slotNumber,
    });
  });
}

(async () => {
  const { MatchSelect } = await import("/js/shared/match_select.js");
  const { CustomSelect } = await import("/js/shared/select_custom.js");

  window.MatchSelect = MatchSelect;
  window.CustomSelect = CustomSelect;

  window.matchSelectInst = MatchSelect.init(
    document.getElementById("matchSelectId"),
    {
      showImage: true,
      placeholder: "Select Match",
      imgSize: "w-auto h-7",
    },
  );

  window.homeClubFormationInst = CustomSelect.init(
    document.getElementById("homeClubFormation"),
    {
      showImage: false,
      placeholder: "",
    },
  );

  window.awayClubFormationInst = CustomSelect.init(
    document.getElementById("awayClubFormation"),
    {
      showImage: false,
      placeholder: "",
    },
  );
})();

async function viewLineupFormation(matchId) {
  try {
    const response = await $.ajax({
      url:
        LINEUP_ENDPOINT.GET_LINEUP_CLUB_INFO_BY_MATCH_ID_ENDPOINT +
        `/${matchId}`,
      method: "GET",
      headers: {
        RequestVerificationToken: $(
          'input[name="__RequestVerificationToken"]',
        ).val(),
      },
    });

    const lineupClubInfoData = response.data.lineupClubInfo;
    $("#homeClubCrest").attr(
      "src",
      BASE_CLUB_PATH + lineupClubInfoData.homeClubCrest,
    );

    $("#awayClubCrest").attr(
      "src",
      BASE_CLUB_PATH + lineupClubInfoData.awayClubCrest,
    );

    $("#homeClubTheme").css(
      "background-color",
      lineupClubInfoData.homeClubTheme,
    );

    $("#awayClubTheme").css(
      "background-color",
      lineupClubInfoData.awayClubTheme,
    );

    $("#homeClubName").text(lineupClubInfoData.homeClubName);
    $("#awayClubName").text(lineupClubInfoData.awayClubName);

    $("#homeClubFormationTitle").text(lineupClubInfoData.homeClubFormation);
    $("#awayClubFormationTitle").text(lineupClubInfoData.awayClubFormation);
    $("#homeClubManager").text(lineupClubInfoData.homeClubManager);
    $("#awayClubManager").text(lineupClubInfoData.awayClubManager);

    const detailResponse = await getLineupFormationDetail(matchId);
    const { lineupFormation, substitutionFormation } = detailResponse.data;

    renderTeam(
      "homePitch",
      lineupClubInfoData.homeClubFormation,
      lineupFormation,
    );
    renderTeam(
      "awayPitch",
      lineupClubInfoData.awayClubFormation,
      lineupFormation,
    );

    renderSubstitutes(
      "homeSubstitutes",
      substitutionFormation.filter((p) => p.isHomeClub),
    );
    renderSubstitutes(
      "awaySubstitutes",
      substitutionFormation.filter((p) => !p.isHomeClub),
    );
  } catch (err) {
    console.error(err);
    alert(JSON.stringify(err));
  }
}

async function getLineupFormationDetail(matchId) {
  return await $.ajax({
    url:
      LINEUP_ENDPOINT.GET_LINEUP_FORMATION_DETAIL_BY_MATCH_ID_ENDPOINT +
      `/${matchId}`,
    method: "GET",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
  });
}

async function getMatchItems(isEdit = false, existingMatchId = "") {
  try {
    $.ajax({
      url: LINEUP_ENDPOINT.GET_MATCH_ITEMS_ENDPOINT + "/" + isEdit,
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

        if (window.matchSelectInst && data) {
          window.matchSelectInst.setData(data);

          if (existingMatchId) {
            window.matchSelectInst.setValue(existingMatchId);
            $("#lineupForm").find("#matchSelectId").val(existingMatchId);
          }
        } else {
          console.error("Match selection failed: Instance or data missing", {
            instance: window.matchSelectInst,
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

function renderTeam(teamId, formationString, allPlayers) {
  const container = document.getElementById(teamId);
  const isHome = teamId === "homePitch";

  const teamPlayers = allPlayers
    .filter((p) => p.isHomeClub === isHome)
    .sort((a, b) => a.formationSlot - b.formationSlot);

  const layout = [1, ...formationString.split("-").map(Number)];

  container.innerHTML = "";
  if (isHome) {
    container.classList.replace("flex-col-reverse", "flex-col");
  } else {
    container.classList.replace("flex-col", "flex-col-reverse");
  }

  let playerPointer = 0;
  layout.forEach((playerCount, rowIndex) => {
    const rowPlayers = teamPlayers.slice(
      playerPointer,
      playerPointer + playerCount,
    );
    createRow(container, rowPlayers, layout, rowIndex);
    playerPointer += playerCount;
  });
}

function createRow(container, rowPlayers, layout, rowIndex) {
  const row = document.createElement("div");
  row.className = "flex justify-around items-center w-full min-h-[60px]";

  const isHome = container.id === "homePitch";
  const isGKRow = rowIndex === 0;

  rowPlayers.forEach((player) => {
    const isGK = rowIndex === 0;
    const bgColor = isGK ? "#fde047" : player.clubTheme;
    row.innerHTML += `
        <div class="player-node flex flex-col items-center group cursor-pointer hover:scale-110 transition-transform duration-300">
            <div class="relative">
                
                <div class="absolute -top-1 -right-2 flex flex-col gap-1 z-20">
                    ${player.hasYellowCard ? '<div class="w-2 h-3 bg-yellow-400 rounded-sm border border-black/20 shadow-sm"></div>' : ""}
                    ${player.hasGoal ? '<div class="bg-white rounded-full p-0.5 shadow-sm border border-gray-200"><svg class="w-2.5 h-2.5" viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/></svg></div>' : ""}
                </div>

                ${player.subOutMinute ? `<span class="absolute -top-4 -left-2 text-[10px] font-black text-white italic drop-shadow-md">${player.subOutMinute}' <span class="text-red-500">↓</span></span>` : ""}

                <div class="w-14 h-14 rounded-2xl shadow-2xl flex justify-center overflow-hidden" 
                     style="background: #37003c">
                    <img src="${BASE_PLAYER_PATH + player.playerPhoto}" 
                         class="h-[4rem] mt-1 object-contain drop-shadow-lg" 
                         onerror="this.src='/upload/players/placeholder.png'">
                </div>

                ${player.isCaptain ? '<div class="absolute bottom-4 -left-1 bg-white text-black text-[7px] font-black px-1 rounded-sm border border-black/20 shadow-sm">C</div>' : ""}
            </div>

            <div class="mt-1 flex flex-col items-center">
                <div class="flex items-center gap-1">
                    <span class="text-[10px] text-white/80 font-bold">${player.playerNumber}</span>
                    <p class="text-[10px] text-white">
                        ${player.playerShortName}
                    </p>
                </div>
            </div>
        </div>`;
  });
  container.appendChild(row);
}

function renderSubstitutes(containerId, players) {
  const container = document.getElementById(containerId);
  const isAway = containerId === "awaySubstitutes";
  container.innerHTML = "";

  if (!players || players.length === 0) {
    container.innerHTML =
      '<p class="text-[10px] text-white/20 italic p-4">No substitutes listed</p>';
    return;
  }

  players.forEach((player) => {
    const photoPath = player.playerPhoto
      ? BASE_PLAYER_PATH + player.playerPhoto
      : "/upload/players/placeholder.png";
    container.innerHTML += `
            <div class="flex items-center justify-between w-full hover:bg-white/5 rounded-lg transition-colors group">
                <div class="flex items-center gap-3">
                    <div class="relative w-14 h-14 rounded-2xl overflow-hidden"
                         style="background: ${player.clubTheme}">
                        <img src="${photoPath}" 
                             class="h-auto mt-1 object-contain"
                             onerror="this.src='/upload/players/placeholder.png'">
                    </div>

                    <div class="flex flex-col">
                        <h5 class="text-[11px] text-white font-bold text-start">
                            ${player.playerShortName}
                        </h5>
                        <p class="text-[11px] text-start text-white/40 font-medium">
                            <span class="mr-1">${player.playerNumber}</span>${player.position}
                        </p>
                    </div>
                </div>

                <div class="flex items-center gap-2 pr-2">
                    ${player.hasGoal ? '<img src="/upload/icons/ball.png" class="w-3 h-3 opacity-80">' : ""}
                    
                    ${
                      player.subInMinute
                        ? `
                        <div class="flex items-center gap-1 text-green-400 font-bold text-[10px] italic">
                            <svg class="w-3 h-3 fill-current" viewBox="0 0 24 24"><path d="M16 13h-3V3h-2v10H8l4 4 4-4zM4 19v2h16v-2H4z"/></svg>
                            ${player.subInMinute}'
                        </div>
                    `
                        : ""
                    }

                    ${player.hasYellowCard ? '<div class="w-2 h-3 bg-yellow-400 rounded-sm border border-black/20 shadow-sm"></div>' : ""}
                </div>
            </div>
        `;
  });
}

function activateTab(tabId) {
  const tabBtn = document.querySelector(`[data-tab="${tabId}"]`);

  if (tabBtn) {
    tabBtn.click();
  }
}

function handleRowClick(row, matchId) {
  $("#matchTableId")
    .closest("table")
    .find("tbody tr")
    .removeClass("active-row");
  viewLineupFormation(matchId);
  $(row).addClass("active-row");
}
