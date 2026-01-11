// default: reads data attributes from <select>
// const instances = CustomSelect.initAll();
(async () => {
  const { CustomSelect } = await import("/js/shared/select_custom.js");

  // expose CustomSelect globally
  window.CustomSelect = CustomSelect;

  // expose instances globally
  window.playerClubInst = CustomSelect.init(
    document.getElementById("playerClub"),
    {
      showImage: true,
      placeholder: "Choose nationality",
      imgSize: "w-auto h-7",
    }
  );

  window.positionInst = CustomSelect.init(document.getElementById("position"), {
    showImage: false,
    placeholder: "Select Player's Position",
  });

  window.preferredFootInst = CustomSelect.init(
    document.getElementById("preferredFoot"),
    {
      showImage: false,
      placeholder: "Select Player's Preferred Foot",
    }
  );

  window.joinedClubFootInst = CustomSelect.init(
    document.getElementById("joinedClub"),
    {
      showImage: false,
      placeholder: "Select Player's Joined Club",
    }
  );
})();

// // set value programmatically
//inst.setValue("2");

// // replace options from JS
// inst.updateOptions([
//   { value: '1', label: 'Arsenal', img: '/upload/clubs/ars.png', subtitle: 'Premier League' },
//   { value: '2', label: 'Man City', img: '/upload/clubs/mci.png', subtitle: 'Premier League' }
// ]);

//Destroy (restore original select)
//inst.destroy();
