const NEWS_BASE_CONTROLLER = "/en/news";
const NEWS_ENDPOINT = {
  CREATE_NEWS_ENDPOINT: NEWS_BASE_CONTROLLER + "/create",
  UPDATE_NEWS_ENDPOINT: NEWS_BASE_CONTROLLER + "/update",
  DELETE_NEWS_ENDPOINT: NEWS_BASE_CONTROLLER + "/delete",
  GET_NEWS_ENDPOINT: NEWS_BASE_CONTROLLER + "/get-news",
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

  window.selectNewsTagInst = CustomSelect.init(
    document.getElementById("selectNewsTag"),
    {
      showImage: false,
      placeholder: "Select news tag",
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

  window.selectNewsCategoryInst = CustomSelect.init(
    document.getElementById("selectNewsCategory"),
    {
      showImage: true,
      placeholder: "Select News Category",
    },
  );
})();

function updateCharCounter(input) {
  const display = document.querySelector(`[data-count-for="${input.id}"]`);
  if (!display) return;

  const length = input.value.length;
  const max = input.getAttribute("maxlength");

  display.textContent = length;

  if (length >= max * 0.95) {
    display.parentElement.classList.replace("text-gray-400", "text-orange-400");
  } else {
    display.parentElement.classList.replace("text-orange-400", "text-gray-400");
  }
}

function addClubSelect(selectedValue = "") {
  const container = document.getElementById("clubContainer");

  container.insertAdjacentHTML(
    "beforeend",
    `
    <div class="flex gap-2 items-center">
        <select name="NewsDto.ClubIds" class="js-custom-select w-full">
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
        <select name="NewsDto.PlayerIds" class="js-custom-select w-full">
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

document.addEventListener("DOMContentLoaded", function () {
  const trackedInputs = document.querySelectorAll(".char-counter-input");

  trackedInputs.forEach((input) => {
    updateCharCounter(input);
    input.addEventListener("input", () => updateCharCounter(input));
  });
});

$("#removePhoto").on("click", function () {
  resetFile();
});

const resetForm = () => {
  let form = $("#newsForm");
  form[0].reset();

  if (window.selectMatchInst) {
    window.selectMatchInst.reset();
  }

  resetFile();

  form.find("input").not("[name='__RequestVerificationToken']").val("");
  form.find("textarea").val("");

  if (window.matchSelectInst) {
    window.matchSelectInst.reset();
  }

  form.find("select.js-custom-select").each(function () {
    const placeholder = $(this).data("placeholder") || "Please select";
    const $btn = $(this).parent().find("button");
    $btn.find("img").addClass("hidden");
    $btn.find("span.truncate.block").text(placeholder);
  });

  $("#newsForm")
    .find("input[type='file']")
    .each(function () {
      $(this).attr("required", true);
    });

  $("#newsForm").find("span[asp-validation-for='NewsDto.ImageUrl']").text("");
  document
    .querySelectorAll(".char-counter-input")
    .forEach((el) => updateCharCounter(el));

  document.getElementById("clubContainer").innerHTML = "";
  document.getElementById("playerContainer").innerHTML = "";

  window.clubSelectInstances = [];
  window.playerSelectInstances = [];

  clubIndex = 0;
  playerIndex = 0;
};

$("#btnAddNews").on("click", function () {
  const form = $("#newsForm");
  form.attr("action", NEWS_ENDPOINT.CREATE_NEWS_ENDPOINT);
  resetForm();
  const today = new Date();
  $("#publishedDate").val(today.toISOString().split("T")[0]);

  const expiry = new Date();
  expiry.setDate(expiry.getDate() + 7);
  $("#expiryDate").val(expiry.toISOString().split("T")[0]);

  $("#isFeatured").prop("checked", false);
  $("#isFeaturedHidden").val("false");

  $("#isActive").prop("checked", true);
  $("#isActiveHidden").val("true");

  $("#isVideo").prop("checked", false);
  $("#isVideoHidden").val("false");

  $("#isQuizzes").prop("checked", false);
  $("#isQuizzesHidden").val("false");

  $("#isRelatedContent").prop("checked", false);
  $("#isRelatedContentHidden").val("false");

  $("#isPremierLeagueGame").prop("checked", false);
  $("#isPremierLeagueGameHidden").val("false");

  openModal("modal-8xl", true);
});

function toggleEditNews(newsId) {
  resetForm();
  try {
    $.ajax({
      url: NEWS_ENDPOINT.GET_NEWS_ENDPOINT + "/" + newsId,
      method: "GET",
      headers: {
        RequestVerificationToken: $(
          'input[name="__RequestVerificationToken"]',
        ).val(),
      },
      success: function (response) {
        if (response.statusCode !== 200) {
          alert(response.message);
          return;
        }
        const data = response.data.newsItem;
        const form = $("#newsForm");
        form.find("#newsId").val(data.newsId);
        form.find("#title").val(data.title);
        form.find("#subtitle").val(data.subtitle);
        form.find("#content").val(data.content);
        form.find("#referenceUrl").val(data.referenceUrl);
        form.find("#videoReferenceUrl").val(data.videoReferenceUrl);

        // Manually update counters for specific elements without events
        document
          .querySelectorAll(".char-counter-input")
          .forEach((el) => updateCharCounter(el));
        const publishedDate = new Date(data.publishedDate);
        const expiryDate = new Date(data.expiryDate);
        form
          .find("#publishedDate")
          .val(publishedDate.toISOString().split("T")[0]);
        form.find("#expiryDate").val(expiryDate.toISOString().split("T")[0]);

        $("#isFeatured").prop("checked", data.isFeatured);
        $("#isFeaturedHidden").val(data.isFeatured ? "true" : "false");

        $("#isActive").prop("checked", data.isActive);
        $("#isActiveHidden").val(data.isActive ? "true" : "false");

        $("#isVideo").prop("checked", data.isVideo);
        $("#isVideoHidden").val(data.isVideo ? "true" : "false");

        $("#isQuizzes").prop("checked", data.isQuizzes);
        $("#isQuizzesHidden").val(data.isQuizzes ? "true" : "false");

        $("#isRelatedContent").prop("checked", data.isRelatedContent);
        $("#isRelatedContentHidden").val(
          data.isRelatedContent ? "true" : "false",
        );

        $("#isPremierLeagueGame").prop("checked", data.isPremierLeagueGame);
        $("#isPremierLeagueGameHidden").val(
          data.isPremierLeagueGame ? "true" : "false",
        );

        window.selectNewsTagInst.setValue(data.newsTagId);
        window.selectMatchInst.setValue(data.matchId);
        window.selectClubInst.setValue(data.clubId);
        window.selectNewsCategoryInst.setValue(data.newsCategoryId);

        form.find("#imageUrl").val(data.imageUrl);
        if (data.imageUrl) {
          const photoPath = "/upload/news/" + data.imageUrl;
          $("#filePreview").attr("src", photoPath);
          $("#fileName").text(data.imageUrl);
          $("#previewArea").removeClass("hidden");
          $("#fileInput").attr("required", false);
        } else {
          $("#previewArea").addClass("hidden");
          $("#filePreview").attr("src", "");
          $("#fileName").text("");
          $("#fileInput").attr("required", true);
        }

        if (data.clubIds?.length)
          data.clubIds.forEach((id) => addClubSelect(id));
        else addClubSelect();

        if (data.playerIds?.length)
          data.playerIds.forEach((id) => addPlayerSelect(id));
        else addPlayerSelect();

        form.attr("action", NEWS_ENDPOINT.UPDATE_NEWS_ENDPOINT + "/" + newsId);
        openModal("modal-8xl", true);
      },
      error: function (xhr, status, error) {
        console.error(error);
        alert(JSON.stringify(error));
      },
    });
  } catch (error) {
    console.error(error);
    alert(JSON.stringify(error));
  }
}

$("#isFeatured").on("change", function () {
  $("#isFeaturedHidden").val(this.checked ? "true" : "false");
});

$("#isActive").on("change", function () {
  $("#isActiveHidden").val(this.checked ? "true" : "false");
});

$("#isVideo").on("change", function () {
  $("#isVideoHidden").val(this.checked ? "true" : "false");
});

$("#isQuizzes").on("change", function () {
  $("#isQuizzesHidden").val(this.checked ? "true" : "false");
});

$("#isRelatedContent").on("change", function () {
  $("#isRelatedContentHidden").val(this.checked ? "true" : "false");
});

$("#isPremierLeagueGame").on("change", function () {
  $("#isPremierLeagueGameHidden").val(this.checked ? "true" : "false");
});
