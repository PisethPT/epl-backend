const formations = {};
const homeClubFormation = $("#homeClubFormation");
const awayClubFormation = $("#awayClubFormation");
const state = {
  home: {
    draggedPlayer: null,
    slotPlayers: [],
    formationContainer: "homeClubFormationSlots",
    playerList: "homeClubPlayerId",
  },
  away: {
    draggedPlayer: null,
    slotPlayers: [],
    formationContainer: "awayClubFormationSlots",
    playerList: "awayClubPlayerId",
  },
};
const playerStore = {
  home: {},
  away: {},
};

function renderFormation(side, key) {
  const container = document.getElementById(state[side].formationContainer);
  container.innerHTML = "";

  formations[key].forEach((count) => {
    const row = document.createElement("div");
    row.className = "flex justify-center gap-6 mb-6";

    for (let i = 0; i < count; i++) {
      const slot = document.createElement("div");

      slot.className = `
        w-14 h-14 rounded-2xl
        border-2 border-dashed border-white/60
        bg-[#1e0021]
        flex items-center justify-center
        hover:border-[#8a3fbf]
        transition
      `;

      slot.dataset.filled = "false";
      slot.dataset.side = side;

      slot.ondragover = (e) => e.preventDefault();
      slot.ondrop = (e) => onDropPlayer(e, slot);

      slot.innerHTML = `<span class="text-white/50 text-lg">+</span>`;
      row.appendChild(slot);
    }

    container.appendChild(row);
  });
}

function onPlayerDrag(e, side) {
  const card = e.target.closest("[data-player-id]");
  if (!card) return;

  state[side].draggedPlayer = {
    id: Number(card.dataset.playerId),
    firstName: card.dataset.playerFirstname,
    lastName: card.dataset.playerLastname,
    playerNumber: card.dataset.playerPlayernumber,
    img: card.dataset.playerImg,
    clubTheme: card.dataset.playerClubtheme,
  };

  e.dataTransfer.setData("text/plain", card.dataset.playerId);
}

function onDropPlayer(e, slot) {
  e.preventDefault();

  const side = slot.dataset.side;
  const ctx = state[side];
  const player = ctx.draggedPlayer;

  if (!player) return;
  // prevent duplicate
  if (
    document.querySelector(
      `#${ctx.formationContainer} [data-player-slot="${player.id}"]`,
    )
  ) {
    alert("Player already placed");
    return;
  }

  ctx.slotPlayers = ctx.slotPlayers.filter((id) => id !== Number(player.id));
  renderBench(side);

  slot.dataset.filled = "true";
  slot.classList.remove("border-dashed");
  slot.classList.add("relative");

  const wrapper = document.createElement("div");
  wrapper.className = "grid justify-items-center gap-1";

  slot.innerHTML = `
    <div class="w-full h-full rounded-xl flex items-center justify-center overflow-hidden"
         data-player-slot="${player.id}"
         style="background-color:${player.clubTheme}">
      <img src="${player.img}" class="h-[3.2rem] mt-1 object-contain pointer-events-none"/>
    </div>
    <button class="absolute -top-2 -right-2 z-20
                   w-5 h-5 bg-red-600 text-white
                   rounded-full text-xs
                   flex items-center justify-center">
      ✕
    </button>
  `;

  const label = document.createElement("div");
  label.className = "flex gap-1 items-center leading-none";
  label.innerHTML = `
    <span class="text-xs text-gray-400">${player.playerNumber}</span>
    <span class="text-xs text-white truncate max-w-[3.5rem]">${player.lastName ?? player.firstName}</span>
  `;

  slot.replaceWith(wrapper);
  wrapper.appendChild(slot);
  wrapper.appendChild(label);

  slot.querySelector("button").onclick = () => clearSlot(slot);

  ctx.slotPlayers = ctx.slotPlayers.filter((id) => id !== player.id);
  ctx.draggedPlayer = null;
}

function clearSlot(slot) {
  const wrapper = slot.parentElement;
  const playerEl = slot.querySelector("[data-player-slot]");
  if (!playerEl) return;

  const playerId = Number(playerEl.dataset.playerSlot);
  const side = slot.dataset.side;

  if (!state[side].slotPlayers.includes(playerId)) {
    state[side].slotPlayers.push(playerId);
  }
  renderBench(side);

  slot.innerHTML = `<span class="text-white/50 text-lg">+</span>`;
  slot.dataset.filled = "false";

  slot.className = `
    w-14 h-14 rounded-2xl
    border-2 border-dashed border-white/60
    bg-[#1e0021]
    flex items-center justify-center
  `;

  wrapper.replaceWith(slot);
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
  const sortedIds = [...state[side].slotPlayers].sort((a, b) => {
    const pA = playerStore[side][a];
    const pB = playerStore[side][b];

    if (pA.positionId === pB.positionId) {
      return pA.id - pB.id;
    }
    return pA.positionId - pB.positionId;
  });

  sortedIds.forEach((playerId) => {
    const p = playerStore[side][playerId];
    if (!p) return;

    container.insertAdjacentHTML(
      "beforeend",
      `
      <div data-player-id="${p.id}"
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
               class="h-[4rem] mt-5 object-contain"
               draggable="true"
               ondragstart="onPlayerDrag(event, '${side}')" />
        </div>

        <div>
          <p class="text-xs text-white font-thin truncate max-w-[6rem]">${p.firstName}</p>
          <p class="text-sm text-white font-medium truncate max-w-[6rem]">${p.lastName}</p>
          <p class="text-xs text-gray-300 mt-1">
            ${p.position} • #${p.playerNumber}
          </p>
        </div>
      </div>
      `,
    );
  });
}

(async () => {
  $.ajax({
    url: "/lineups/get-formations",
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

      listItem.forEach((data) => {
        const key = data.formationId;
        const value = data.primaryFormation.split("-").map(Number);

        formations[key] = [1, ...value];

        homeClubFormation.append(
          `<option value="${key}">${data.primaryFormation}</option>`,
        );

        awayClubFormation.append(
          `<option value="${key}">${data.primaryFormation}</option>`,
        );
      });

      const defaultFormationKey = listItem[0].formationId;

      renderFormation("home", defaultFormationKey);
      renderFormation("away", defaultFormationKey);

      slotHomePlayers = getPlayerIds("homeClubPlayerId");
      slotAwayPlayers = getPlayerIds("awayClubPlayerId");

      state.home.slotPlayers = [...slotHomePlayers];
      state.away.slotPlayers = [...slotAwayPlayers];

      renderBench("home");
      renderBench("away");
    },
    error: function (error) {
      alert("Failed to load formations");
      console.error(error);
    },
  });

  $("#homeClubPlayerId [data-player-id]").each(function () {
    const el = this;
    playerStore.home[el.dataset.playerId] = {
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
})();
