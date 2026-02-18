// Filter Drawer
$(function () {
  $("#filterBtnId").on("click", function () {
    $("#filterDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#closeFilterDrawer").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");
  });

  $("#showCompetitionDrawer , #competitionBoxId").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#competitionDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#showSeasonDrawer , #seasonBoxId").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#seasonDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#showMatchweekDrawer , #matchweekBoxId").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#matchweekDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#showClubDrawer").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#clubDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });
});

// Season Drawer
$(function () {
  $("#seasonBtnId").on("click", function () {
    $("#seasonDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#closeSeasonDrawer").on("click", function () {
    $("#seasonDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");
  });

  $("#backSeasonDrawer").on("click", function () {
    $("#seasonDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });
});

// Competition Drawer
$(function () {
  $("#competitionBtnId").on("click", function () {
    $("#competitionDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#closeCompetitionDrawer").on("click", function () {
    $("#competitionDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");
  });

  $("#backCompetitionDrawer").on("click", function () {
    $("#competitionDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });
});

// Matcheweeks Drawer
$(function () {
  $("#matchweekBtnId").on("click", function () {
    $("#matchweekDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#closeMatchweekDrawer").on("click", function () {
    $("#matchweekDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");
  });

  $("#backMatchweekDrawer").on("click", function () {
    $("#matchweekDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });
});

// Club Drawer
$(function () {
  $("#clubBtnId, #showClubDrawer").on("click", function () {
    $("#clubDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });

  $("#closeClubDrawer").on("click", function () {
    $("#clubDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");
  });

  $("#backClubDrawer").on("click", function () {
    $("#clubDrawer").addClass("translate-x-full");
    $("#modalBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#modalBackdrop").removeClass("hidden");
  });
});
