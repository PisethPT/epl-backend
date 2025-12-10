(function () {
  const sidebar = document.getElementById("sidebar");
  const overlay = document.getElementById("overlay");
  const toggleBtn = document.getElementById("sidebarToggle");
  const page = document.getElementById("page");
  const mainContent = document.getElementById("mainContent");

  if (!sidebar || !overlay || !toggleBtn || !page || !mainContent) {
    console.warn("Sidebar script: missing required DOM elements.");
    return;
  }

  const menuItemsSelector = ".menu-item";
  const submenuToggleSelector = ".submenu-toggle";

  const isMobile = () => window.innerWidth < 1024;

  document.querySelectorAll(".has-submenu").forEach((parent, idx) => {
    if (!parent.dataset.menuId) parent.dataset.menuId = "submenu-" + (idx + 1);
  });

  function setCollapsedState(collapsed) {
    if (collapsed) {
      sidebar.classList.remove("w-64");
      sidebar.classList.add("w-[3.55rem]");
      page.classList.remove("lg:ml-64");
      page.classList.add("lg:ml-12");

      document.querySelectorAll(menuItemsSelector).forEach((item) => {
        item.classList.remove("justify-start");
        item.classList.add("justify-center");
        const label = item.querySelector(".menu-label");
        const adminPanel = document.getElementById("adminPanel");
        if (label) label.classList.add("hidden");
        if (adminPanel) adminPanel.classList.add("hidden");
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

      document.querySelectorAll(menuItemsSelector).forEach((item) => {
        item.classList.remove("justify-center");
        item.classList.add("justify-start");
        const label = item.querySelector(".menu-label");
        const adminPanel = document.getElementById("adminPanel");
        if (label) label.classList.remove("hidden");
        if (adminPanel) adminPanel.classList.remove("hidden");
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
      overlay.classList.add("hidden");
      toggleBtn.setAttribute("aria-expanded", "false");
      setCollapsedState(false);
    } else {
      sidebar.classList.remove("-translate-x-full");
      overlay.classList.add("hidden");
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
    if (sidebar.classList.contains("-translate-x-full")) showMobileSidebar();
    else hideMobileSidebar();
  }

  function toggleDesktopCollapse() {
    const willCollapse = sidebar.dataset.collapsed !== "true";

    if (willCollapse) {
      let rememberedId = null;

      const openParent = document.querySelector(".has-submenu.open");
      if (openParent) rememberedId = openParent.dataset.menuId;

      if (!rememberedId) {
        const activeSub = document.querySelector(
          ".submenu .menu-item[aria-current='page']"
        );
        if (activeSub) {
          const parent = activeSub.closest(".has-submenu");
          if (parent) rememberedId = parent.dataset.menuId;
        }
      }

      if (!rememberedId) {
        const activeTop = document.querySelector(
          ".menu-item[aria-current='page']"
        );
        if (activeTop) {
          const parent = activeTop.closest(".has-submenu");
          if (parent) rememberedId = parent.dataset.menuId;
        }
      }

      if (rememberedId) sidebar.dataset.openParent = rememberedId;
      else delete sidebar.dataset.openParent;

      // Close all submenus
      document.querySelectorAll(".has-submenu").forEach((parent) => {
        const submenu = parent.querySelector(".submenu");
        if (submenu) {
          submenu.style.maxHeight = 0;
          submenu.setAttribute("aria-hidden", "true");
        }
        parent.classList.remove("open");
        const tbtn = parent.querySelector(submenuToggleSelector);
        if (tbtn) tbtn.setAttribute("aria-expanded", "false");
      });

      setCollapsedState(true);
    } else {
      setCollapsedState(false);

      const remembered = sidebar.dataset.openParent;
      let restored = false;

      if (remembered) {
        const parent = document.querySelector(
          `.has-submenu[data-menu-id="${remembered}"]`
        );
        if (parent) {
          const submenu = parent.querySelector(".submenu");
          if (submenu) {
            submenu.style.maxHeight = submenu.scrollHeight + "px";
            submenu.setAttribute("aria-hidden", "false");
          }
          parent.classList.add("open");
          const tbtn = parent.querySelector(submenuToggleSelector);
          if (tbtn) tbtn.setAttribute("aria-expanded", "true");
          restored = true;
        }
      }

      if (!restored) {
        const activeSub = document.querySelector(
          ".submenu .menu-item[aria-current='page']"
        );
        if (activeSub) {
          const parent = activeSub.closest(".has-submenu");
          if (parent) {
            const submenu = parent.querySelector(".submenu");
            if (submenu) {
              submenu.style.maxHeight = submenu.scrollHeight + "px";
              submenu.setAttribute("aria-hidden", "false");
            }
            parent.classList.add("open");
            const tbtn = parent.querySelector(submenuToggleSelector);
            if (tbtn) tbtn.setAttribute("aria-expanded", "true");
            restored = true;
          }
        }
      }

      delete sidebar.dataset.openParent;
    }
  }

  function onToggleClick() {
    if (isMobile()) toggleMobileSidebar();
    else toggleDesktopCollapse();
  }

  function onMenuItemClick(e) {
    const el = e.currentTarget;

    document.querySelectorAll(menuItemsSelector).forEach((item) => {
      item.classList.remove("bg-[#410544]", "text-white");
      item.removeAttribute("aria-current");
    });

    // Set current
    el.classList.add("bg-[#410544]", "text-white");
    el.setAttribute("aria-current", "page");

    const parentSubmenu = el.closest(".has-submenu");
    if (parentSubmenu) {
      const toggleBtn = parentSubmenu.querySelector(submenuToggleSelector);
      if (toggleBtn) toggleBtn.setAttribute("aria-expanded", "true");
      const submenu = parentSubmenu.querySelector(".submenu");
      if (submenu) submenu.style.maxHeight = submenu.scrollHeight + "px";
      parentSubmenu.classList.add("open");
    }

    if (isMobile()) hideMobileSidebar();
  }

  // setup submenu toggle buttons
  function setupSubmenus() {
    document.querySelectorAll(submenuToggleSelector).forEach((button) => {
      // safe-attach
      if (button.dataset.attached) return;
      button.dataset.attached = "1";

      button.addEventListener("click", function (e) {
        // if we are collapsed on desktop, expand first
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
    const path = location.pathname.replace(/\/+$/, ""); // strip trailing slash
    let bestMatch = null;
    let bestLength = 0;

    document.querySelectorAll("a.menu-item").forEach((a) => {
      const href = a.getAttribute("href");
      if (!href) return;
      const candidate = href.replace(/\/+$/, "");
      if (
        path === candidate ||
        path.startsWith(candidate + "/") ||
        (candidate !== "/" && path.startsWith(candidate))
      ) {
        if (candidate.length > bestLength) {
          bestMatch = a;
          bestLength = candidate.length;
        }
      }
    });

    if (bestMatch) {
      bestMatch.classList.add("bg-[#410544]", "text-white");
      bestMatch.setAttribute("aria-current", "page");

      const parentSubmenu = bestMatch.closest(".has-submenu");
      if (parentSubmenu) {
        const toggleBtn = parentSubmenu.querySelector(submenuToggleSelector);
        if (toggleBtn) toggleBtn.setAttribute("aria-expanded", "true");
        const submenu = parentSubmenu.querySelector(".submenu");
        if (submenu) submenu.style.maxHeight = submenu.scrollHeight + "px";
        parentSubmenu.classList.add("open");
      }
    }
  }

  function attachMenuHandlers() {
    document.querySelectorAll(menuItemsSelector).forEach((item) => {
      if (item.dataset.listenerAttached) return;
      item.addEventListener("click", onMenuItemClick);
      item.dataset.listenerAttached = "1";
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
          if (!sidebar.classList.contains("-translate-x-full"))
            hideMobileSidebar();
        }
      }
    },
    true
  );

  overlay.addEventListener("click", hideMobileSidebar);

  mainContent.addEventListener("click", function () {
    if (isMobile() && !sidebar.classList.contains("-translate-x-full"))
      hideMobileSidebar();
  });

  toggleBtn.addEventListener("click", onToggleClick);
  window.addEventListener("resize", setInitialState);

  document.addEventListener("DOMContentLoaded", function () {
    setInitialState();
    setupSubmenus();
    attachMenuHandlers();
    initActiveFromUrl();
  });
})();
