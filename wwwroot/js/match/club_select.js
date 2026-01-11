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

  window.refereeRoleInst = CustomSelect.init(
    document.getElementById("refereeRoleId"),
    {
      showImage: false,
      placeholder: "Referee Role",
    }
  );

  window.refereeBadgeLevelInst = CustomSelect.init(
    document.getElementById("refereeBadgeLevelId"),
    {
      showImage: false,
      placeholder: "Referee Badge Level",
    }
  );

  // assist referee 1
  window.assistReferee1Inst = CustomSelect.init(
    document.getElementById("assistReferee1Id"),
    {
      showImage: false,
      placeholder: "Referee",
    }
  );

  window.assistReferee1RoleInst = CustomSelect.init(
    document.getElementById("assistReferee1RoleId"),
    {
      showImage: false,
      placeholder: "Referee Role",
    }
  );

  window.assistReferee1BadgeLevelInst = CustomSelect.init(
    document.getElementById("assistReferee1BadgeLevelId"),
    {
      showImage: false,
      placeholder: "Referee Badge Level",
    }
  );

  // assist referee 2
  window.assistReferee2Inst = CustomSelect.init(
    document.getElementById("assistReferee2Id"),
    {
      showImage: false,
      placeholder: "Referee",
    }
  );

  window.assistReferee2RoleInst = CustomSelect.init(
    document.getElementById("assistReferee2RoleId"),
    {
      showImage: false,
      placeholder: "Referee Role",
    }
  );

  window.assistReferee2BadgeLevelInst = CustomSelect.init(
    document.getElementById("assistReferee2BadgeLevelId"),
    {
      showImage: false,
      placeholder: "Referee Badge Level",
    }
  );

  // fourth official
  window.refereeFourthOfficialInst = CustomSelect.init(
    document.getElementById("refereeFourthOfficialId"),
    {
      showImage: false,
      placeholder: "Referee",
    }
  );

  window.refereeFourthOfficialRoleInst = CustomSelect.init(
    document.getElementById("refereeFourthOfficialRoleId"),
    {
      showImage: false,
      placeholder: "Referee Role",
    }
  );

  window.refereeFourthOfficialBadgeLevelInst = CustomSelect.init(
    document.getElementById("refereeFourthOfficialBadgeLevelId"),
    {
      showImage: false,
      placeholder: "Referee Badge Level",
    }
  );

  // VAR
  window.refereeVARInst = CustomSelect.init(
    document.getElementById("refereeVARId"),
    {
      showImage: false,
      placeholder: "Referee",
    }
  );

  window.refereeVARRoleInst = CustomSelect.init(
    document.getElementById("refereeVARRoleId"),
    {
      showImage: false,
      placeholder: "Referee Role",
    }
  );

  window.refereeVARBadgeLevelInst = CustomSelect.init(
    document.getElementById("refereeVARBadgeLevelId"),
    {
      showImage: false,
      placeholder: "Referee Badge Level",
    }
  );

  window.formationSelectInst = CustomSelect.init(
    document.getElementById("formationSelect"),
    {
      showImage: false,
      placeholder: "Formation",
    }
  );
})();
