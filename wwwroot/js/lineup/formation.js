const formations = {};
const currentFormation = {
  home: null,
  away: null,
};

let homeClubId = null;
let awayClubId = null;

const homeClubFormation = $("#homeClubFormation");
const awayClubFormation = $("#awayClubFormation");
const state = {
  home: {
    draggedPlayer: null,
    slotPlayers: [],
    formationContainer: "homeClubFormationSlots",
    substitutionContainer: "homeSubstitutionSlots",
    playerList: "homeClubPlayerId",
  },
  away: {
    draggedPlayer: null,
    slotPlayers: [],
    formationContainer: "awayClubFormationSlots",
    substitutionContainer: "awaySubstitutionSlots",
    playerList: "awayClubPlayerId",
  },
};

const playerStore = {
  home: {},
  away: {},
};

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
    method: "GET",
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
      state.home.slotPlayers = Object.keys(playerStore.home).map(Number);
      state.away.slotPlayers = Object.keys(playerStore.away).map(Number);

      renderSubSlots("home");
      renderSubSlots("away");

      initializePlayerDrag();
      initializeCaptainDropZones();
    },
    error: function (err) {
      console.error("Error fetching players:", err);
    },
  });
});

async function onFormationChange(side, formationId) {
  if (!formationId) return;

  if (currentFormation[side] === formationId) {
    return;
  }

  currentFormation[side] = formationId;

  resetPitchPlayers(side);

  $.ajax({
    url: `${LINEUP_ENDPOINT.GET_LINEUP_FORMATION_LAYOUT_BY_FORMATION_ID_ENDPOINT}/${formationId}`,
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    method: "GET",
    success: function (response) {
      if (response.statusCode !== 200) return;

      let items = response.data.listItem || response.data.ListItem;

      if (typeof items === "string") {
        items = JSON.parse(items);
      }

      if (Array.isArray(items) && items.length > 0) {
        renderFormation(side, items);
      }
    },
  });
}

function renderPlayers({ containerId, players, side }) {
  const $container = $(containerId);
  $container.empty();

  playerStore[side] = {};

  if (!players || players.length === 0) {
    $container.html(`<p class="text-gray-400 text-sm">No players found</p>`);
    return;
  }

  let html = "";

  players.forEach((player, index) => {
    const normalizedPlayer = {
      clubId: player.clubId,
      id: player.playerId,
      firstName: player.firstName,
      lastName: player.lastName,
      position: player.position,
      positionId: Number(player.positionId),
      playerNumber: player.playerNumber,
      img: `/upload/players/${player.photo}`,
      clubTheme: player.clubTheme,
      orderIndex: index,
    };

    playerStore[side][normalizedPlayer.id] = normalizedPlayer;

    html += `
      <div
          data-club-id="${normalizedPlayer.clubId}"
          data-player-id="${normalizedPlayer.id}"
          data-player-firstname="${normalizedPlayer.firstName}"
          data-player-lastname="${normalizedPlayer.lastName}"
          data-player-position="${normalizedPlayer.position}"
          data-player-positionid="${normalizedPlayer.positionId}"
          data-player-playernumber="${normalizedPlayer.playerNumber}"
          data-player-img="${normalizedPlayer.img}"
          data-player-clubtheme="${normalizedPlayer.clubTheme}"
          draggable="true"
          ondragstart="onPlayerDrag(event,'${side}')"
          class="flex items-center gap-3 bg-[#1e0021] p-2 rounded-2xl cursor-grab hover:ring-2 hover:ring-[#8a3fbf]"
        >
        <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-center"
             style="background-color:${normalizedPlayer.clubTheme}">
          <img src="${normalizedPlayer.img}" class="h-[4rem] mt-5 object-contain" />
        </div>

        <div class="overflow-hidden">
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

function renderFormation(side, layoutData) {
  const containerId =
    side === "home" ? "homeClubFormationSlots" : "awayClubFormationSlots";

  const container = document.getElementById(containerId);
  if (!container) {
    console.warn("Formation container not found:", containerId);
    return;
  }

  if (!layoutData || !Array.isArray(layoutData)) {
    console.warn("Invalid formation layout data:", layoutData);
    return;
  }

  const occupiedPlayers = {};

  container.querySelectorAll("[data-player-slot]").forEach((el) => {
    const wrapper = el.closest(".grid");
    const posId = wrapper?.dataset?.formationSlot;

    if (posId) {
      occupiedPlayers[posId] = el.cloneNode(true);
    }
  });

  container.innerHTML = "";

  const rows = layoutData.reduce((acc, slot) => {
    const r = slot?.row ?? slot?.Row;
    if (r == null) return acc;

    if (!acc[r]) acc[r] = [];
    acc[r].push(slot);
    return acc;
  }, {});

  Object.keys(rows)
    .sort((a, b) => Number(a) - Number(b))
    .forEach((rowNum) => {
      const rowDiv = document.createElement("div");
      rowDiv.className = "flex justify-around items-center w-full px-4 mb-4";

      rows[rowNum].forEach((slotData) => {
        const posId = slotData?.positionId ?? slotData?.PositionId;
        const posCode = slotData?.code ?? slotData?.Code ?? "";

        if (!posId) {
          console.warn("Missing positionId in slotData:", slotData);
          return;
        }

        const wrapper = document.createElement("div");
        wrapper.className = "grid justify-items-center gap-1";
        wrapper.dataset.formationSlot = posId;
        wrapper.dataset.posCode = posCode;

        const slot = document.createElement("div");
        slot.dataset.side = side;
        slot.dataset.filled = "false";

        slot.className =
          "w-14 h-14 rounded-2xl border-2 border-dashed border-white/20 flex items-center justify-center cursor-pointer hover:border-[#8a3fbf] transition-all relative";

        if (occupiedPlayers[posId]) {
          const playerNode = occupiedPlayers[posId];

          slot.appendChild(playerNode);

          slot.classList.remove("border-dashed", "border-white/20");
          slot.dataset.filled = "true";

          const btn = slot.querySelector("button");
          if (btn) {
            btn.onclick = (ev) => {
              ev.stopPropagation();
              clearSlot(slot);
            };
          }
        } else {
          slot.innerHTML = `
            <span class="text-white/20 text-[10px] font-bold uppercase">
              ${posCode}
            </span>
          `;
        }

        slot.ondragover = (e) => e.preventDefault();
        slot.ondrop = (e) => onDropPlayer(e, slot);

        const label = document.createElement("div");
        label.className = "flex gap-1 items-center leading-none";
        label.innerHTML = `
          <span class="text-[10px] text-gray-400"></span>
          <span class="text-[10px] text-white uppercase font-medium"></span>
        `;

        wrapper.appendChild(slot);
        wrapper.appendChild(label);
        rowDiv.appendChild(wrapper);
      });

      container.appendChild(rowDiv);
    });
}

function onPlayerDrag(e, side) {
  const img = e.currentTarget;

  const card = img.closest("[data-player-id]");
  if (!card) return;

  const playerId = Number(card.dataset.playerId);

  const player = playerStore[side][playerId];
  if (!player) {
    console.error("Player not found in store", playerId);
    return;
  }

  state[side].draggedPlayer = { ...player };
}

function onDropPlayer(e, slot) {
  e.preventDefault();
  const side = slot.dataset.side;
  const ctx = state[side];
  const player = ctx.draggedPlayer;

  if (!player || !player.id) {
    console.error("Player data is missing or corrupted:", player);
    return;
  }

  const existingOnField = document.querySelector(
    `#${ctx.formationContainer} [data-player-slot="${player.id}"], #${ctx.substitutionContainer} [data-player-slot="${player.id}"]`,
  );

  if (existingOnField) {
    alert("Player already placed");
    return;
  }

  const isSubSlot = slot.dataset.isSubSlot === "true";
  const oldPlayerEl = slot.querySelector("[data-player-slot]");

  if (oldPlayerEl) {
    const oldPlayerId = Number(oldPlayerEl.dataset.playerSlot);
    if (!ctx.slotPlayers.includes(oldPlayerId)) {
      ctx.slotPlayers.push(oldPlayerId);
    }
  }

  ctx.slotPlayers = ctx.slotPlayers.filter((id) => id !== player.id);
  renderBench(side);

  slot.dataset.filled = "true";
  slot.classList.remove("border-dashed", "border-white/10");
  slot.classList.add("relative");

  if (isSubSlot) {
    slot.innerHTML = `
      <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-centerr" 
           style="background-color: ${player.clubTheme}" 
           data-player-slot="${player.id}"
            data-player-id="${player.id}"
            data-club-id="${player.clubId}">
          <img src="${player.img}" class="h-[4rem] mt-[0.3rem] object-contain pointer-events-none" />
      </div>
      <div class="overflow-hidden">
          <p class="text-sm text-white font-medium">${player.firstName}</p>
          <p class="text-xs text-white font-medium">${player.lastName}</p>
          <p class="text-[10px] text-gray-300 mt-2">
            ${player.position} • #${player.playerNumber}
          </p>
      </div>
      <button class="absolute -top-1 -right-1 z-20 w-5 h-5 bg-red-600 text-white rounded-full text-[10px] flex items-center justify-center hover:bg-red-500 shadow-md">
          ✕
      </button>`;
  } else {
    slot.innerHTML = `
      <div class="w-full h-full rounded-[0.88rem] flex items-center justify-center overflow-hidden"
           data-player-slot="${player.id}"
           data-player-id="${player.id}"
           data-club-id="${player.clubId}"
           style="background-color:${player.clubTheme}">

        <img src="${player.img}"
             class="h-[4rem] mt-5 object-contain pointer-events-none"/>

      </div>

      <button class="absolute -top-2 -right-2 z-20 w-5 h-5 bg-red-600 text-white rounded-full text-xs flex items-center justify-center">
        ✕
      </button>
    `;

    const playerWrapper = slot.closest(".grid");
    if (!playerWrapper) {
      const wrapper = document.createElement("div");
      wrapper.className = "grid justify-items-center gap-1";
      const label = document.createElement("div");
      label.className = "flex gap-1 items-center leading-none";
      label.innerHTML = `
        <span class="text-xs text-gray-400">${player.playerNumber}</span>
        <span class="text-xs text-white truncate max-w-[3.5rem]">${player.lastName ?? player.firstName}</span>
      `;
      slot.replaceWith(wrapper);
      wrapper.appendChild(slot);
      wrapper.appendChild(label);
    } else {
      const labelName = playerWrapper.querySelector("span.text-white");
      const labelNum = playerWrapper.querySelector("span.text-gray-400");
      if (labelName) labelName.innerText = player.lastName ?? player.firstName;
      if (labelNum) labelNum.innerText = player.playerNumber;
    }
  }

  slot.querySelector("button").onclick = (ev) => {
    ev.stopPropagation();
    isSubSlot ? clearSubSlot(slot) : clearSlot(slot);
  };

  ctx.draggedPlayer = null;
}

function clearSlot(slot) {
  const wrapper = slot.closest(".grid");
  const playerEl = slot.querySelector("[data-player-slot]");
  if (!playerEl) return;

  const playerId = Number(playerEl.dataset.playerSlot);
  const side = slot.dataset.side;

  if (!state[side].slotPlayers.includes(playerId)) {
    state[side].slotPlayers.push(playerId);
  }

  renderBench(side);

  const posCode = wrapper.dataset.posCode;
  slot.innerHTML = `
    <span class="text-white/20 text-[10px] font-bold uppercase">
      ${posCode}
    </span>
  `;

  slot.dataset.filled = "false";

  slot.className =
    "w-14 h-14 rounded-2xl border-2 border-dashed border-white/20 flex items-center justify-center cursor-pointer hover:border-[#8a3fbf] transition-all relative";

  const label = wrapper.querySelector("span.text-white");
  const number = wrapper.querySelector("span.text-gray-400");

  if (label) label.innerText = "";
  if (number) number.innerText = "";
}

function getPlayerIds(containerId) {
  return $("#" + containerId + " [data-player-id]")
    .map(function () {
      return Number($(this).data("player-id"));
    })
    .get();
}

function renderBench(side) {
  const container = document.getElementById(state[side].playerList);
  container.innerHTML = "";

  [...state[side].slotPlayers]
    .sort((a, b) => {
      const pA = playerStore[side][a];
      const pB = playerStore[side][b];
      return (pA.orderIndex || 0) - (pB.orderIndex || 0);
    })
    .forEach((playerId) => {
      const p = playerStore[side][playerId];
      if (!p) return;

      const html = `
      <div data-club-id="${p.clubId}"
        data-player-id="${p.id}"
        data-player-firstname="${p.firstName}"
        data-player-lastname="${p.lastName}"
        data-player-position="${p.position}"
        data-player-positionid="${p.positionId}"
        data-player-playernumber="${p.playerNumber}"
        data-player-img="${p.img}"
        data-player-clubtheme="${p.clubTheme}"
        class="flex items-center gap-3 bg-[#1e0021] p-2 rounded-2xl cursor-grab hover:ring-2 hover:ring-[#8a3fbf]">
          
          <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-center"
               style="background-color:${p.clubTheme}">
              <img src="${p.img}"
                   draggable="true"
                   ondragstart="onPlayerDrag(event,'${side}')"
                   class="h-[4rem] mt-5 object-contain"/>
          </div>
          
          <div class="overflow-hidden">
              <p class="text-sm text-white font-medium">${p.firstName}</p>
              <p class="text-xs text-white font-medium">${p.lastName}</p>
              <p class="text-xs text-gray-300 mt-2">
                  ${p.position} • #${p.playerNumber}
              </p>
          </div>
          
      </div>
      `;
      container.insertAdjacentHTML("beforeend", html);
    });
}

function renderSubSlots(side) {
  const container = document.getElementById(state[side].substitutionContainer);
  if (!container) return;

  container.innerHTML = "";
  const totalSubSlots = 9;

  for (let i = 0; i < totalSubSlots; i++) {
    const slot = document.createElement("div");

    slot.className = `flex items-center gap-3 bg-[#1e0021] p-2 rounded-2xl border-2 border-dashed border-white/10 cursor-pointer hover:border-[#8a3fbf] transition-all min-h-[70px]`;

    slot.dataset.filled = "false";
    slot.dataset.side = side;
    slot.dataset.isSubSlot = "true";

    slot.dataset.subSlot = i + 1;

    slot.ondragover = (e) => e.preventDefault();
    slot.ondrop = (e) => onDropPlayer(e, slot);

    slot.innerHTML = `
      <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-center bg-[#55005a]/50">
          <span class="text-white/30 text-xl">+</span>
      </div>
      <div class="flex flex-col">
          <p class="text-[10px] text-white/30 font-bold uppercase">Sub ${i + 1}</p>
          <p class="text-[10px] text-white/20">Empty</p>
      </div>
    `;

    container.appendChild(slot);
  }
}

function clearSubSlot(slot) {
  const side = slot.dataset.side;
  const playerEl = slot.querySelector("[data-player-slot]");
  if (!playerEl) return;

  const pId = Number(playerEl.dataset.playerSlot);
  if (!state[side].slotPlayers.includes(pId)) {
    state[side].slotPlayers.push(pId);
  }
  renderBench(side);

  slot.dataset.filled = "false";
  slot.classList.add("border-dashed", "border-white/10");
  slot.innerHTML = `
    <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-center bg-[#55005a]/50">
        <span class="text-white/30 text-xl">+</span>
    </div>
    <div class="flex flex-col">
        <p class="text-[10px] text-white/30 font-bold uppercase">Unknown</p>
        <p class="text-[10px] text-white/20">Unknown</p>
    </div>`;
}

function resetPitchPlayers(side) {
  const ctx = state[side];

  const container = document.getElementById(ctx.formationContainer);
  if (!container) return;

  container.querySelectorAll("[data-player-slot]").forEach((el) => {
    const playerId = Number(el.dataset.playerSlot);

    if (!ctx.slotPlayers.includes(playerId)) {
      ctx.slotPlayers.push(playerId);
    }
  });

  renderBench(side);
}

let isInitializingFormation = false;

async function getFormations() {
  await $.ajax({
    url: "/en/lineups/get-formations",
    method: "GET",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    success: function (response) {
      homeClubFormation.empty();
      awayClubFormation.empty();

      const listItem = response.data.listItem;
      let clubFormations = [];

      listItem.forEach((data) => {
        const key = data.formationId;
        const value = data.primaryFormation.split("-").map(Number);

        formations[key] = [1, ...value];

        clubFormations.push({
          value: key,
          label: data.primaryFormation,
        });
      });

      window.homeClubFormationInst.updateOptions(clubFormations);
      window.awayClubFormationInst.updateOptions(clubFormations);

      if (window.homeClubFormationInst) {
        window.homeClubFormationInst.setValue("1");
      }

      if (window.awayClubFormationInst) {
        window.awayClubFormationInst.setValue("1");
      }

      homeClubFormation.val(1).trigger("change");
      awayClubFormation.val(1).trigger("change");

      renderFormation("home", listItem);
      renderFormation("away", listItem);
    },
    error: function (error) {
      alert("Failed to load formations");
      console.error(error);
    },
  });

  $("#homeClubPlayerId [data-player-id]").each(function () {
    const el = this;
    playerStore.home[el.dataset.playerId] = {
      clubId: Number(el.dataset.clubId),
      id: Number(el.dataset.playerId),
      firstName: el.dataset.playerFirstname,
      lastName: el.dataset.playerLastname,
      position: el.dataset.playerPosition,
      positionId: el.dataset.playerPositionid,
      playerNumber: el.dataset.playerPlayernumber,
      img: el.dataset.playerImg,
      clubTheme: el.dataset.playerClubtheme,
    };
  });

  $("#awayClubPlayerId [data-player-id]").each(function () {
    const el = this;
    playerStore.away[el.dataset.playerId] = {
      clubId: Number(el.datasetclubId),
      id: Number(el.dataset.playerId),
      firstName: el.dataset.playerFirstname,
      lastName: el.dataset.playerLastname,
      position: el.dataset.playerPosition,
      positionId: el.dataset.playerPositionid,
      playerNumber: el.dataset.playerPlayernumber,
      img: el.dataset.playerImg,
      clubTheme: el.dataset.playerClubtheme,
    };
  });
}

function initializePlayerDrag() {
  this.addEventListener("dragstart", function (e) {
    const player = {
      id: Number($(this).data("player-id")),
      firstName: $(this).data("player-firstname"),
      lastName: $(this).data("player-lastname"),
      img: $(this).data("player-img"),
      clubTheme: $(this).data("player-clubtheme"),
      clubId: $(this).data("club-id"),
      playerNumber: $(this).data("player-playernumber"),
      position: $(this).data("player-position"),
      positionId: Number($(this).data("player-positionid")),
    };
    const payload = JSON.stringify(player);
    e.dataTransfer.setData("player", payload);
    console.log("drag payload:", payload);
  });
}

function initializeCaptainDropZones() {
  $(".captain-container").each(function () {
    const container = this;
    const side = $(container).data("side"); // home / away

    container.addEventListener("dragover", (e) => {
      e.preventDefault();
      container.classList.add("border-[#8a3fbf]");
    });

    container.addEventListener("dragleave", () => {
      container.classList.remove("border-[#8a3fbf]");
    });

    container.addEventListener("drop", (e) => {
      e.preventDefault();

      const raw = e.dataTransfer.getData("player");

      if (!raw) {
        console.warn("No player data found in drag event");
        return;
      }

      let player;

      try {
        player = JSON.parse(raw);
      } catch (err) {
        console.error("Invalid player JSON:", raw);
        return;
      }

      setCaptain(side, player);
    });

    $(container)
      .find(".remove-captain-btn")
      .on("click", function (e) {
        e.stopPropagation();
        removeCaptain(side);
      });
  });
}

function setCaptain(side, player) {
  if (!player || !player.id) return;

  // save state
  state[side].captain = player;

  // hidden input
  $(`#${side}ClubCaptainPlayerId`).val(player.id);

  // name
  $(`#${side}ClubCaptainName`).text(player.firstName + " " + player.lastName);

  // image
  $(`#${side}ClubCaptainImage`).attr("src", player.img).removeClass("hidden");

  $(`#${side}ClubCaptainPlaceholder`).addClass("hidden");

  // show remove button
  $(`#remove${capitalize(side)}ClubCaptainBtn`).removeClass("hidden");
}

function removeCaptain(side) {
  state[side].captain = null;

  $(`#${side}ClubCaptainPlayerId`).val("");

  $(`#${side}ClubCaptainName`).text("Empty");

  $(`#${side}ClubCaptainImage`).attr("src", "").addClass("hidden");

  $(`#${side}ClubCaptainPlaceholder`).removeClass("hidden");

  $(`#remove${capitalize(side)}ClubCaptainBtn`).addClass("hidden");
}
