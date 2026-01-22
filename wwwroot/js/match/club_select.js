// Init single with options
(async () => {
  const { CustomSelect } = await import("/js/shared/select_custom.js");

  window.CustomSelect = CustomSelect;

  window.homeClubInst = CustomSelect.init(document.getElementById("homeClub"), {
    showImage: true,
    placeholder: "Home Club",
    imgSize: "w-auto h-7",
  });

  window.awayClubInst = CustomSelect.init(document.getElementById("awayClub"), {
    showImage: true,
    placeholder: "Away Club",
    imgSize: "w-auto h-7",
  });

  window.seasonInst = CustomSelect.init(document.getElementById("seasonId"), {
    showImage: true,
    placeholder: "Season",
    imgSize: "w-auto h-7",
  });

  // referee
  window.refereeInst = CustomSelect.init(document.getElementById("refereeId"), {
    showImage: false,
    placeholder: "Referee",
  });

  // assist referee 1
  window.assistReferee1Inst = CustomSelect.init(
    document.getElementById("assistReferee1Id"),
    {
      showImage: false,
      placeholder: "Referee",
    },
  );

  // assist referee 2
  window.assistReferee2Inst = CustomSelect.init(
    document.getElementById("assistReferee2Id"),
    {
      showImage: false,
      placeholder: "Referee",
    },
  );

  // fourth official
  window.refereeFourthOfficialInst = CustomSelect.init(
    document.getElementById("refereeFourthOfficialId"),
    {
      showImage: false,
      placeholder: "Referee",
    },
  );

  // VAR
  window.refereeVARInst = CustomSelect.init(
    document.getElementById("refereeVARId"),
    {
      showImage: false,
      placeholder: "Referee",
    },
  );
})();
