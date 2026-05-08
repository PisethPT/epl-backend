const VIDEO_BASE_CONTROLLER = "/en/videos";

const VIDEO_ENDPOINT = {
  CREATE: VIDEO_BASE_CONTROLLER + "/create",
  UPDATE: VIDEO_BASE_CONTROLLER + "/update",
  GET: VIDEO_BASE_CONTROLLER + "/get-video",
  GET_VIDEO_CATEGORY: VIDEO_BASE_CONTROLLER + "/get-video-category",
};

window.clubSelectInstances = [];
window.playerSelectInstances = [];

let clubIndex = 0;
let playerIndex = 0;

(async () => {
  const { CustomSelect } = await import("/js/shared/select_custom.js");
  const { MatchSelect } = await import("/js/shared/match_select.js");

  window.CustomSelect = CustomSelect;
  window.MatchSelect = MatchSelect;

  window.selectCategoryInst = CustomSelect.init(
    document.getElementById("selectVideoCategory"),
    {
      showImage: false,
      placeholder: "Select Category",
    },
  );

  window.selectVideoTagInst = CustomSelect.init(
    document.getElementById("selectVideoTag"),
    {
      showImage: false,
      placeholder: "Select Video Tag",
    },
  );

  window.selectSeasonInst = CustomSelect.init(
    document.getElementById("selectSeason"),
    {
      showImage: false,
      placeholder: "Select Season",
    },
  );

  window.selectMatchInst = MatchSelect.init(
    document.getElementById("selectMatch"),
    {
      showImage: true,
      placeholder: "Select Match",
      imgSize: "w-auto h-7",
    },
  );
})();

function updateCharCounter(input) {
  const display = document.querySelector(`[data-count-for="${input.id}"]`);
  if (!display) return;
  display.textContent = input.value.length;
}

document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll(".char-counter-input").forEach((input) => {
    updateCharCounter(input);
    input.addEventListener("input", () => updateCharCounter(input));
  });
  toggleGetVideoCategory(false);
});

function addClubSelect(selectedValue = "") {
  const container = document.getElementById("clubContainer");

  container.insertAdjacentHTML(
    "beforeend",
    `
    <div class="flex gap-2 items-center">
        <select name="VideoDto.ClubIds" class="js-custom-select w-full">
            <option value="">Select Club</option>
            ${window.clubOptions || ""}
        </select>

        <button type="button" class="remove-btn bg-red-500 text-white px-2 py-1 rounded">
            ✕
        </button>
    </div>
    `,
  );

  const row = container.lastElementChild;
  const select = row.querySelector("select");

  const instance = window.CustomSelect.init(select, {
    showImage: true,
    placeholder: "Select Club",
  });

  if (selectedValue) instance.setValue(selectedValue);

  window.clubSelectInstances.push(instance);

  document.querySelectorAll("#clubContainer label").forEach((el) => {
    el.style.width = "100%";
  });

  row.querySelector(".remove-btn").onclick = () => {
    instance.destroy?.();
    row.remove();

    window.clubSelectInstances = window.clubSelectInstances.filter(
      (i) => i !== instance,
    );
  };
}

function addPlayerSelect(selectedValue = "") {
  const container = document.getElementById("playerContainer");

  container.insertAdjacentHTML(
    "beforeend",
    `
    <div class="flex gap-2 items-center">
        <select name="VideoDto.PlayerIds" class="js-custom-select w-full">
            <option value="">Select Player</option>
            ${window.playerOptions || ""}
        </select>

        <button type="button" class="remove-btn bg-red-500 text-white px-2 py-1 rounded">
            ✕
        </button>
    </div>
    `,
  );

  const row = container.lastElementChild;
  const select = row.querySelector("select");

  const instance = window.CustomSelect.init(select, {
    showImage: true,
    placeholder: "Select Player",
  });

  if (selectedValue) instance.setValue(selectedValue);

  window.playerSelectInstances.push(instance);

  document.querySelectorAll("#playerContainer label").forEach((el) => {
    el.style.width = "100%";
  });

  row.querySelector(".remove-btn").onclick = () => {
    instance.destroy?.();
    row.remove();

    window.playerSelectInstances = window.playerSelectInstances.filter(
      (i) => i !== instance,
    );
  };
}

const resetForm = () => {
  const form = $("#videoForm");
  form[0].reset();

  form.find("input").not("[name='__RequestVerificationToken']").val("");
  form.find("textarea").val("");

  window.selectCategoryInst.setValue("");
  window.selectVideoTagInst.setValue("");

  document.getElementById("clubContainer").innerHTML = "";
  document.getElementById("playerContainer").innerHTML = "";

  window.clubSelectInstances = [];
  window.playerSelectInstances = [];

  clubIndex = 0;
  playerIndex = 0;

  if (window.selectMatchInst) {
    window.selectMatchInst.reset();
  }

  //addClubSelect();
  //addPlayerSelect();

  document.querySelectorAll(".char-counter-input").forEach(updateCharCounter);
};

$("#btnAddVideo").on("click", function () {
  const form = $("#videoForm");
  form.attr("action", VIDEO_ENDPOINT.CREATE);

  resetForm();

  $("#modalTitle").text("Create Video");

  const today = new Date().toISOString().split("T")[0];
  $("#publishedDate").val(today);

  const expiry = new Date();
  expiry.setFullYear(expiry.getFullYear() + 1);
  $("#expiryDate").val(expiry.toISOString().split("T")[0]);

  $("#isStory").prop("checked", false);
  $("#isStoryHidden").val("false");

  $("#isReference").prop("checked", false);
  $("#isReferenceHidden").val("false");

  $("#isFeatured").prop("checked", false);
  $("#isFeaturedHidden").val("false");

  $("#isActive").prop("checked", true);
  $("#isActiveHidden").val("true");

  $("#isTheArchive").prop("checked", false);
  $("#isTheArchiveHidden").val("false");

  openModal("modal-8xl", true);
});

function toggleEditVideo(videoId) {
  resetForm();

  $.ajax({
    url: VIDEO_ENDPOINT.GET + "/" + videoId,
    method: "GET",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    success: function (response) {
      if (response.statusCode !== 200) return;

      const data = response.data.videoItem;
      const form = $("#videoForm");

      $("#modalTitle").text("Edit Video");
      form.attr("action", VIDEO_ENDPOINT.UPDATE + "/" + videoId);

      form.find("#videoId").val(data.videoId);
      form.find("#title").val(data.title);
      form.find("#thumbnailUrl").val(data.thumbnailUrl);
      form.find("#referenceUrl").val(data.referenceUrl);
      form.find("#videoUrl").val(data.videoUrl);
      form.find("#channel").val(data.channel);
      form.find("#description").val(data.description);
      form.find("#durationSeconds").val(data.durationSeconds);

      $("#isStory").prop("checked", data.isStory);
      $("#isStoryHidden").val(data.isStory ? "true" : "false");
      $("#isReference").prop("checked", data.isReference);
      $("#isReferenceHidden").val(data.isReference ? "true" : "false");
      $("#isFeatured").prop("checked", data.isFeatured);
      $("#isFeaturedHidden").val(data.isFeatured ? "true" : "false");
      $("#isActive").prop("checked", data.isActive);
      $("#isActiveHidden").val(data.isActive ? "true" : "false");

      $("#isTheArchive").prop("checked", data.isTheArchive);
      $("#isTheArchiveHidden").val(data.isTheArchive ? "true" : "false");

      if (data.publishedDate)
        $("#publishedDate").val(data.publishedDate.split("T")[0]);

      if (data.expiryDate) $("#expiryDate").val(data.expiryDate.split("T")[0]);

      window.selectVideoTagInst.setValue(data.videoTagId);
      toggleGetVideoCategory(data.isTheArchive, data.videoCategoryId);

      window.selectMatchInst.setValue(data.matchId);
      window.selectSeasonInst.setValue(data.seasonId);

      if (data.clubIds?.length) data.clubIds.forEach((id) => addClubSelect(id));
      else addClubSelect();

      if (data.playerIds?.length)
        data.playerIds.forEach((id) => addPlayerSelect(id));
      else addPlayerSelect();

      openModal("modal-8xl", true);
    },
  });
}

function toggleGetVideoCategory(isTheArchive = false, selectedValue = "") {
  return $.ajax({
    url: VIDEO_ENDPOINT.GET_VIDEO_CATEGORY + `/${isTheArchive}`,
    method: "GET",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]',
      ).val(),
    },
    success: function (response) {
      let data = response.data;

      if (!Array.isArray(data)) {
        data = [data];
      }
      const itemOptions = data.map((it) => ({
        value: it.value,
        label: it.label || "",
        subtitle: it.subtitle || "",
      }));

      if (window.selectCategoryInst) {
        window.selectCategoryInst.updateOptions(itemOptions);
        window.selectCategoryInst.setValue(selectedValue);
      }
    },
  });
}

$("#isReference").on("change", function () {
  $("#isReferenceHidden").val(this.checked ? "true" : "false");
});

$("#isStory").on("change", function () {
  $("#isStoryHidden").val(this.checked ? "true" : "false");
});

$("#isFeatured").on("change", function () {
  $("#isFeaturedHidden").val(this.checked ? "true" : "false");
});

$("#isActive").on("change", function () {
  $("#isActiveHidden").val(this.checked ? "true" : "false");
});

$("#isTheArchive").on("change", function () {
  toggleGetVideoCategory(this.checked);
});
