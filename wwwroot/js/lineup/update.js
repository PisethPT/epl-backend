window.toggleUpdate = async function (matchId) {
  console.log("toggleUpdate called", matchId);

  try {
    resetForm();

    const form = $("#lineupForm");
    form.attr("action", LINEUP_ENDPOINT.UPDATE_LINEUP_ENDPOINT + "/" + matchId);
    form.attr("method", "POST");
    form.attr("enctype", "multipart/form-data");

    $("#modalTitle").text("Update Lineup");
    openModal("modal-8xl", true);

    await loadLineupForUpdate(matchId);

    activateTab("tab-home-club-lineup");
  } catch (err) {
    console.error("Update error:", err);
  }
};

async function loadLineupForUpdate(matchId) {
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

    const clubInfo = response.data.lineupClubInfo;

    const detailResponse = await getLineupFormationDetail(matchId);
    const { lineupFormation, substitutionFormation } = detailResponse.data;

    await getFormations();

    await getMatchItems(true, String(matchId));

    await waitForPlayersLoaded();

    $("#homeClubFormation").val(clubInfo.homeClubFormationId).trigger("change");
    $("#awayClubFormation").val(clubInfo.awayClubFormationId).trigger("change");

    if (window.homeClubFormationInst) {
      window.homeClubFormationInst.setValue(
        String(clubInfo.homeClubFormationId),
      );
    }

    if (window.awayClubFormationInst) {
      window.awayClubFormationInst.setValue(
        String(clubInfo.awayClubFormationId),
      );
    }

    await wait(500);

    populateSlotsFromAPI(lineupFormation, substitutionFormation);
  } catch (err) {
    console.error("loadLineupForUpdate error:", err);
    alert("Failed to load lineup");
  }
}

function populateSlotsFromAPI(lineupFormation, substitutionFormation) {
  ["home", "away"].forEach((side) => {
    const ctx = state[side];

    const players = lineupFormation.filter(
      (p) => p.isHomeClub === (side === "home"),
    );

    const subs = substitutionFormation.filter(
      (p) => p.isHomeClub === (side === "home"),
    );

    players.forEach((player) => {
      const storePlayer =
        playerStore[side][player.playerId] || buildPlayerFromAPI(player);

      const wrapper = findBestSlot(ctx, player);

      if (!wrapper) {
        console.error("No slot found for:", player);
        return;
      }

      const slot = wrapper.querySelector("[data-side]");
      if (!slot) return;

      slot.dataset.filled = "true";
      slot.classList.remove("border-dashed", "border-white/20");

      slot.innerHTML = `
        <div class="w-full h-full rounded-[0.88rem] flex items-center justify-center overflow-hidden"
             data-player-slot="${storePlayer.id}"
             data-player-id="${storePlayer.id}"
             data-club-id="${storePlayer.clubId}"
             style="background-color:${storePlayer.clubTheme}">
          <img src="${storePlayer.img}"
               class="h-[4rem] mt-5 object-contain pointer-events-none"/>
        </div>

        <button class="absolute -top-2 -right-2 z-20 w-5 h-5 bg-red-600 text-white rounded-full text-xs flex items-center justify-center">
          ✕
        </button>
      `;

      slot.querySelector("button").onclick = (ev) => {
        ev.stopPropagation();
        clearSlot(slot);
      };

      const labelName = wrapper.querySelector("span.text-white");
      const labelNum = wrapper.querySelector("span.text-gray-400");

      if (labelName)
        labelName.innerText = storePlayer.lastName || storePlayer.firstName;

      if (labelNum) labelNum.innerText = storePlayer.playerNumber;

      ctx.slotPlayers = ctx.slotPlayers.filter((id) => id !== storePlayer.id);
    });

    subs.forEach((player, index) => {
      const storePlayer =
        playerStore[side][player.playerId] || buildPlayerFromAPI(player);

      const subSlot = document.querySelector(
        `#${ctx.substitutionContainer} [data-sub-slot="${index + 1}"]`,
      );

      if (!subSlot) return;

      subSlot.dataset.filled = "true";
      subSlot.classList.remove("border-dashed", "border-white/10");
      subSlot.classList.add("relative");

      subSlot.innerHTML = `
        <div class="w-14 h-14 rounded-2xl overflow-hidden flex justify-center items-center"
             style="background-color:${storePlayer.clubTheme}"
             data-player-slot="${storePlayer.id}"
             data-player-id="${storePlayer.id}"
             data-club-id="${storePlayer.clubId}">
          <img src="${storePlayer.img}"
               class="h-[4rem] mt-[16px] object-contain pointer-events-none"/>
        </div>

        <div class="overflow-hidden">
            <p class="text-sm text-white font-medium">${storePlayer.firstName}</p>
            <p class="text-xs text-white font-medium">${storePlayer.lastName}</p>
            <p class="text-[10px] text-gray-300 mt-2">
              ${storePlayer.position} • #${storePlayer.playerNumber}
            </p>
        </div>

        <button class="absolute -top-1 -right-1 z-20 w-5 h-5 bg-red-600 text-white rounded-full text-[10px] flex items-center justify-center">
            ✕
        </button>
      `;

      subSlot.querySelector("button").onclick = (ev) => {
        ev.stopPropagation();
        clearSubSlot(subSlot);
      };

      ctx.slotPlayers = ctx.slotPlayers.filter((id) => id !== storePlayer.id);
    });
  });
}

function wait(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function waitForPlayersLoaded() {
  return new Promise((resolve) => {
    let tries = 0;

    const interval = setInterval(() => {
      const homeReady =
        playerStore.home && Object.keys(playerStore.home).length > 0;

      const awayReady =
        playerStore.away && Object.keys(playerStore.away).length > 0;

      if (homeReady && awayReady) {
        clearInterval(interval);
        resolve();
      }

      if (++tries > 50) {
        console.warn("Player loading timeout");
        clearInterval(interval);
        resolve();
      }
    }, 300);
  });
}

function findBestSlot(ctx, player) {
  const allSlots = document.querySelectorAll(
    `#${ctx.formationContainer} [data-pos-code]`,
  );

  const playerPos = (player.formation || "").toLowerCase();

  for (const wrapper of allSlots) {
    const posCode = wrapper.dataset.posCode?.toLowerCase();
    const slot = wrapper.querySelector("[data-side]");

    if (!slot || slot.dataset.filled === "true") continue;

    if (posCode === playerPos) {
      return wrapper;
    }
  }

  for (const wrapper of allSlots) {
    const posCode = wrapper.dataset.posCode?.toLowerCase();
    const slot = wrapper.querySelector("[data-side]");

    if (!slot || slot.dataset.filled === "true") continue;

    if (posCode && playerPos && posCode.includes(playerPos)) {
      return wrapper;
    }
  }

  for (const wrapper of allSlots) {
    const slot = wrapper.querySelector("[data-side]");
    if (slot && slot.dataset.filled === "false") {
      return wrapper;
    }
  }

  return null;
}

function buildPlayerFromAPI(player) {
  return {
    id: player.playerId,
    clubId: player.clubId,
    firstName: player.firstName || "",
    lastName: player.lastName || "",
    position: player.position || "",
    positionId: player.positionId || 0,
    playerNumber: player.playerNumber || "",
    img: player.playerPhoto
      ? BASE_PLAYER_PATH + player.playerPhoto
      : "/upload/players/placeholder.png",
    clubTheme: player.clubTheme || "#37003c",
  };
}
