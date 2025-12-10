/// <reference types="jquery" />
const USER_BASE_CONTROLLER = "/auth";
const USER_ENDPOINT = {
  CREATE_USER_ENDPOINT: USER_BASE_CONTROLLER + "/create",
  UPDATE_USER_ENDPOINT: USER_BASE_CONTROLLER + "/update",
  GET_USER_BY_USER_ID_ENDPOINT: USER_BASE_CONTROLLER + "/get-user",
};

const selectGender = document.getElementById("gender");
const genderIcon = document.getElementById("gender-icon");

(function () {
  $("#removePhoto").on("click", function () {
    resetFile();
  });

  $("#resetBtn").on("click", function () {
    const form = $("#userForm");
    form[0].reset();
    resetFile();

    form.find("input").each(function () {
      if (this.name !== "__RequestVerificationToken") {
        this.value = "";
      }
    });
  });

  $("#userForm").on("submit", function (e) {
    const domForm = e.target;

    if (!domForm.checkValidity()) {
      domForm.reportValidity();
      e.preventDefault();
      return;
    }

    const userId = $("#userId").val();
    const fileInput = $("#fileInput")[0];
    const fileError = $("#fileError");

    fileError.addClass("hidden").text("");

    const isCreate = !userId;
    const hasNewFile = fileInput && fileInput.files.length > 0;
    if (isCreate && !hasNewFile) {
      fileError.removeClass("hidden").text("Please upload a user photo");
      e.preventDefault();
      return;
    }
  });
})();

$("#btnAddNewUser").on("click", function () {
  const form = $("#userForm");
  $("#modalTitle").text("Create New User");
  form[0].reset();
  resetFile();

  form.find("input").each(function () {
    if (this.name !== "__RequestVerificationToken") {
      this.value = "";
    }
  });

  form.attr("action", USER_ENDPOINT.CREATE_USER_ENDPOINT);
});

const attachUserByUserIdForUpdate = (userId) => {
  $.ajax({
    url: USER_ENDPOINT.GET_USER_BY_USER_ID_ENDPOINT + "/" + userId,
    method: "POST",
    headers: {
      RequestVerificationToken: $(
        'input[name="__RequestVerificationToken"]'
      ).val(),
    },
    success: function (data) {
      $("#userId").val(data.userId);
      $("#photo").val(data.photo ?? "");
      $("#firstName").val(data.firstName ?? "");
      $("#lastName").val(data.lastName ?? "");
      $("#gender").val(data.gender ?? "");
      $("#email").val(data.email ?? "");
      $("#phoneNumber").val(data.phoneNumber ?? "");

      if (data.photo) {
        const photoPath = "/upload/users/" + data.photo;
        $("#filePreview").attr("src", photoPath);
        $("#fileName").text(data.photo);
        $("#previewArea").removeClass("hidden");
        $("#fileInput").attr("required", false);
      } else {
        $("#previewArea").addClass("hidden");
        $("#filePreview").attr("src", "");
        $("#fileName").text("");
        $("#fileInput").attr("required", true);
      }

      $("#modalTitle").text("Update User");
      $("#userForm").attr(
        "action",
        USER_ENDPOINT.UPDATE_USER_ENDPOINT + "/" + data.userId
      );

      // Open modal after fill
      openModal("modal-8xl", true);
    },
    error: function (err) {
      alert(JSON.stringify(err));
      console.error(err);
    },
  });
};

(function () {
  const select = document.getElementById("gender");
  const btn = document.getElementById("fakeSelect");
  const label = document.getElementById("genderLabel");
  const icon = document.getElementById("genderIcon");
  const list = document.getElementById("dropdownList");

  // SVG paths
  const icons = {
    Male: "M12 2a5 5 0 100 10 5 5 0 000-10zm0 12c-5 0-9 4-9 9h18c0-5-4-9-9-9z",
    Female:
      "M12 2a5 5 0 100 10 5 5 0 000-10zm-1 12h2v6h-2v-6zm-3 0h2v6H8v-6zm6 0h2v6h-2v-6z",
  };

  // initial
  function setValue(value) {
    select.value = value;
    label.textContent = value;
    icon.innerHTML = icons[value] || "";
  }
  setValue(select.value || "Male");

  // toggle dropdown
  btn.addEventListener("click", () => {
    list.classList.toggle("hidden");
  });

  // select option
  list.querySelectorAll("li").forEach((li) => {
    li.addEventListener("click", () => {
      const value = li.dataset.value;
      setValue(value);
      list.classList.add("hidden");
    });
  });

  // click outside closes dropdown
  document.addEventListener("click", (e) => {
    if (!btn.contains(e.target) && !list.contains(e.target)) {
      list.classList.add("hidden");
    }
  });
})();
