const LINEUP_BASE_CONTROLLER = "/lineups";
const LINEUP_ENDPOINT = {
  CREATE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/create",
  UPDATE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/update",
  FIND_LINEUP_BY_ID_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-lineup",
  FIND_FORMATION_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-formations",
  GET_PLAYERS_BY_MATCH_ID_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-players-by-match",
};

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

$("#btnAddNewLineup").on("click", function () {});

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

$("#matchSelectId").on("change", function () {
  const matchId = $(this).val();
  if (!matchId) return;

  $.ajax({
    url: LINEUP_ENDPOINT.GET_PLAYERS_BY_MATCH_ID_ENDPOINT + `/${matchId}`,
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    type: "POST",
    success: function (response) {
      if (response.statusCode !== 200) return;

      renderPlayers({
        containerId: "#homeClubPlayerId",
        players: response.data.homePlayers,
        side: "home",
      });

      renderPlayers({
        containerId: "#awayClubPlayerId",
        players: response.data.awayPlayers,
        side: "away",
      });

      renderFormation("home", 1);
      renderFormation("away", 1);

      slotHomePlayers = getPlayerIds("homeClubPlayerId");
      slotAwayPlayers = getPlayerIds("awayClubPlayerId");

      state.home.slotPlayers = [...slotHomePlayers];
      state.away.slotPlayers = [...slotAwayPlayers];

      renderBench("home");
      renderBench("away");
    },
    error: function (err) {
      console.error("Error fetching players:", err);
    },
  });
});

function renderPlayers({ containerId, players, side }) {
  const $container = $(containerId);
  $container.empty();

  playerStore[side] = {};

  if (!players || players.length === 0) {
    $container.html(`<p class="text-gray-400 text-sm">No players found</p>`);
    return;
  }

  let html = "";

  players.forEach((player) => {
    const normalizedPlayer = {
      id: player.playerId,
      firstName: player.firstName,
      lastName: player.lastName,
      position: player.position,
      positionId: Number(player.positionId),
      playerNumber: player.playerNumber,
      img: `/upload/players/${player.photo}`,
      clubTheme: player.clubTheme,
    };

    playerStore[side][normalizedPlayer.id] = normalizedPlayer;

    html += `
      <div
        data-player-id="${normalizedPlayer.id}"
        class="flex items-center gap-3 bg-[#1e0021] p-2 rounded-2xl cursor-grab hover:ring-2 hover:ring-[#8a3fbf]"
      >
        <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-center"
             style="background-color:${normalizedPlayer.clubTheme}">
          <img src="${normalizedPlayer.img}"
               class="h-[4rem] mt-5 object-contain"
               draggable="true"
               ondragstart="onPlayerDrag(event, '${side}')" />
        </div>

        <div>
          <p class="text-sm text-white font-medium">${normalizedPlayer.firstName}</p>
          <p class="text-xs text-white font-medium">${normalizedPlayer.lastName}</p>
          <p class="text-xs text-gray-300 mt-2">
            ${normalizedPlayer.position} • #${normalizedPlayer.playerNumber}
          </p>
        </div>
      </div>
    `;
  });

  $container.html(html);
}
