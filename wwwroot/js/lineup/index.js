const LINEUP_BASE_CONTROLLER = "/lineups";
const LINEUP_ENDPOINT = {
  CREATE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/create",
  UPDATE_LINEUP_ENDPOINT: LINEUP_BASE_CONTROLLER + "/update",
  FIND_LINEUP_BY_ID_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-lineup",
  FIND_FORMATION_ENDPOINT: LINEUP_BASE_CONTROLLER + "/get-formations",
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
