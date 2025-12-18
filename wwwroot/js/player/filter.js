$(function () {
  $("#filterBtnId").on("click", function () {
    $("#filterDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#showSessionDrawer").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");

    $("#sessionDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#showClubDrawer").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");

    $("#clubDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#showPositionDrawer").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");

    $("#positionDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#closeFilterDrawer").on("click", function () {
    $("#filterDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");
  });
});

$(function () {
  $("#sessionBtnId").on("click", function () {
    $("#sessionDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#closeSessionDrawer").on("click", function () {
    $("#sessionDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");
  });

  $("#backSessionDrawer").on("click", function () {
    $("#sessionDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });
});

$(function () {
  $("#positionBtnId, #showPositionDrawer").on("click", function () {
    $("#positionDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#closePositionDrawer").on("click", function () {
    $("#positionDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");
  });

  $("#backPositionDrawer").on("click", function () {
    $("#positionDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  // $('#positionForm').on('submit', function (e) {
  //     e.preventDefault();

  //     const positions = $('input[name="positions[]"]:checked')
  //         .map(function () {
  //             return $(this).val();
  //         })
  //         .get();

  //     $.ajax({
  //         url: '/players',
  //         type: 'POST',
  //         data: {
  //             positions: positions
  //         },
  //         success: function (res) {
  //             console.log('Saved', res);
  //         }
  //     });
  // });
});

$(function () {
  $("#clubBtnId, #showClubDrawer").on("click", function () {
    $("#clubDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });

  $("#closeClubDrawer").on("click", function () {
    $("#clubDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");
  });

  $("#backClubDrawer").on("click", function () {
    $("#clubDrawer").addClass("translate-x-full");
    $("#drawerBackdrop").addClass("hidden");

    $("#filterDrawer").removeClass("translate-x-full");
    $("#drawerBackdrop").removeClass("hidden");
  });
});

// $(function (){
//   $("#btnResetFilter").on("click", function () {
//     $("#clubDrawer").addClass("translate-x-full");
//     $("#drawerBackdrop").addClass("hidden");
//   });
// });
