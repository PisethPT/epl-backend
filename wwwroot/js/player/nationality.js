let placeOfBirthsInst = null;
let nationalitiesInst = null;

async function loadNationalities() {
  const res = await fetch("/json/nationality.json");
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  const nationality = await res.json();

  const placeOfBirths = nationality.nationalities.map((c) => ({
    value: c.name,
    label: c.name,
    img: c.icon,
  }));

  const nationalities = nationality.nationalities.map((n) => ({
    value: n.nationality,
    label: n.nationality,
    img: n.icon,
  }));

  placeOfBirthsInst = CustomSelect.init(
    document.getElementById("placeOfBirth"),
    {
      showImage: true,
      placeholder: "Select player's place of birth",
      imgSize: "w-[26px] h-auto",
    }
  );

  nationalitiesInst = CustomSelect.init(
    document.getElementById("nationality"),
    {
      showImage: true,
      placeholder: "Select player's nationality",
      imgSize: "w-[26px] h-auto",
    }
  );

  placeOfBirthsInst.updateOptions(placeOfBirths);
  nationalitiesInst.updateOptions(nationalities);
}

loadNationalities().catch(console.error);
