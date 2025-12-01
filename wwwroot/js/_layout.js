(function () {
  const sidebar = document.getElementById("sidebar");
  const overlay = document.getElementById("overlay");
  const toggleBtn = document.getElementById("sidebarToggle");
  const page = document.getElementById("page");
  const mainContent = document.getElementById("mainContent");

  const menuItemsSelector = ".menu-item";
  const submenuToggleSelector = ".submenu-toggle";

  const isMobile = () => window.innerWidth < 1024;

  function setCollapsedState(collapsed) {
    if (collapsed) {
      sidebar.classList.remove("w-64");
      sidebar.classList.add("w-[3.55rem]");
      page.classList.remove("lg:ml-64");
      page.classList.add("lg:ml-12");

      document.querySelectorAll('a[aria-current="page"]').forEach((mi) => {
        mi.classList.remove("py-2");
        mi.classList.add("py-[0.75rem]");
      });
      document.querySelectorAll(menuItemsSelector).forEach((item) => {
        item.classList.remove("justify-start");
        item.classList.add("justify-center");
        const label = item.querySelector(".menu-label");
        const adminPanel = document.getElementById("adminPanel");
        if (label) {
          label.classList.add("hidden");
          adminPanel.classList.add("hidden");
        }

        item.classList.add("mb-[0.7rem]");
      });
      document.querySelectorAll(submenuToggleSelector).forEach((btn) => {
        btn.querySelector("i.fas.fa-chevron-right")?.classList.add("hidden");
      });
      sidebar.dataset.collapsed = "true";
    } else {
      sidebar.classList.remove("w-[3.55rem]");
      sidebar.classList.add("w-64");
      page.classList.remove("lg:ml-12");
      page.classList.add("lg:ml-64");

      document.querySelectorAll('a[aria-current="page"]').forEach((mi) => {
        mi.classList.remove("py-[0.75rem]");
        mi.classList.add("py-2");
      });

      document.querySelectorAll(menuItemsSelector).forEach((item) => {
        item.classList.remove("justify-center");
        item.classList.add("justify-start");
        const adminPanel = document.getElementById("adminPanel");
        const label = item.querySelector(".menu-label");
        if (label) {
          label.classList.remove("hidden");
        }
        adminPanel.classList.remove("hidden");
        item.classList.remove("mb-[0.7rem]");
      });
      document.querySelectorAll(submenuToggleSelector).forEach((btn) => {
        btn.querySelector("i.fas.fa-chevron-right")?.classList.remove("hidden");
      });
      delete sidebar.dataset.collapsed;
    }
  }

  function setInitialState() {
    if (isMobile()) {
      sidebar.classList.add("-translate-x-full");
      sidebar.classList.remove("translate-x-0");
      overlay.classList.add("hidden");
      overlay.classList.remove("block");
      toggleBtn.setAttribute("aria-expanded", "false");
      setCollapsedState(false);
    } else {
      sidebar.classList.remove("-translate-x-full");
      sidebar.classList.add("translate-x-0");
      overlay.classList.add("hidden");
      overlay.classList.remove("block");
      toggleBtn.setAttribute("aria-expanded", "true");
      setCollapsedState(sidebar.dataset.collapsed === "true");
    }
  }
  function showMobileSidebar() {
    sidebar.classList.remove("-translate-x-full");
    overlay.classList.remove("hidden");
    overlay.classList.add("block");
    toggleBtn.setAttribute("aria-expanded", "true");
    document.documentElement.classList.add("overflow-hidden");
  }
  function hideMobileSidebar() {
    sidebar.classList.add("-translate-x-full");
    overlay.classList.add("hidden");
    overlay.classList.remove("block");
    toggleBtn.setAttribute("aria-expanded", "false");
    document.documentElement.classList.remove("overflow-hidden");
  }
  function toggleMobileSidebar() {
    const isOpen = !sidebar.classList.contains("-translate-x-full");
    if (isOpen) hideMobileSidebar();
    else showMobileSidebar();
  }

  function toggleDesktopCollapse() {
    if (sidebar.dataset.collapsed === "true") {
      setCollapsedState(false);
    } else {
      setCollapsedState(true);
    }
  }
  function onToggleClick(e) {
    if (isMobile()) {
      toggleMobileSidebar();
    } else {
      toggleDesktopCollapse();
    }
  }

  function onMenuItemClick(e) {
    const el = e.currentTarget;
    document.querySelectorAll(menuItemsSelector).forEach((mi) => {
      mi.classList.remove("bg-[#410544]", "text-white");
      mi.removeAttribute("aria-current");
    });
    el.classList.add("bg-[#410544]", "text-white");
    el.setAttribute("aria-current", "page");

    if (isMobile()) {
      hideMobileSidebar();
    }
  }

  function setupSubmenus() {
    document.querySelectorAll(submenuToggleSelector).forEach((button) => {
      button.addEventListener("click", function (e) {
        if (!isMobile() && sidebar.dataset.collapsed === "true") {
          setCollapsedState(false);
        }

        e.preventDefault();
        const parent = button.closest(".has-submenu");
        const submenu = parent.querySelector(".submenu");
        const expanded = button.getAttribute("aria-expanded") === "true";
        button.setAttribute("aria-expanded", String(!expanded));
        if (!expanded) {
          submenu.style.maxHeight = submenu.scrollHeight + "px";
          submenu.setAttribute("aria-hidden", "false");
        } else {
          submenu.style.maxHeight = 0;
          submenu.setAttribute("aria-hidden", "true");
        }
        parent.classList.toggle("open");
      });
    });
  }

  function initActiveFromUrl() {
    const path = location.pathname;
    let found = false;
    document.querySelectorAll("a.menu-item").forEach((a) => {
      try {
        const href = a.getAttribute("href");
        if (!href) return;
        if (path === href || path.startsWith(href)) {
          document.querySelectorAll(menuItemsSelector).forEach((mi) => {
            mi.classList.remove("bg-[#410544]", "text-white");
            mi.removeAttribute("aria-current");
          });
          a.classList.add("bg-[#410544]", "text-white");
          a.setAttribute("aria-current", "page");
          found = true;
        }
      } catch (err) {
        /* ignore */
      }
    });
  }

  function attachMenuHandlers() {
    document.querySelectorAll(menuItemsSelector).forEach((item) => {
      if (!item.dataset.listenerAttached) {
        item.addEventListener("click", onMenuItemClick);
        item.dataset.listenerAttached = "1";
      }
    });
  }

  document.addEventListener(
    "click",
    function (e) {
      if (isMobile()) {
        const target = e.target;
        if (
          !sidebar.contains(target) &&
          !toggleBtn.contains(target) &&
          !overlay.contains(target)
        ) {
          if (!sidebar.classList.contains("-translate-x-full")) {
            hideMobileSidebar();
          }
        }
      }
    },
    true
  );

  overlay.addEventListener("click", function (e) {
    hideMobileSidebar();
  });

  mainContent.addEventListener("click", function (e) {
    if (isMobile() && !sidebar.classList.contains("-translate-x-full")) {
      hideMobileSidebar();
    }
  });

  toggleBtn.addEventListener("click", onToggleClick);

  window.addEventListener("resize", function () {
    setInitialState();
  });

  window.addEventListener("DOMContentLoaded", function () {
    setInitialState();
    setupSubmenus();
    attachMenuHandlers();
    initActiveFromUrl();
  });
})();

function closeAlertPanel(alertPanel) {
  const alert = document.getElementById(alertPanel);
  if (!alert.classList.contains("hidden")) alert.classList.add("hidden");
}
