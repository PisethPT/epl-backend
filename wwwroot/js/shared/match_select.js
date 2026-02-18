export const MatchSelect = (function () {
  const defaults = {
    placeholder: "Select Match",
    imgSize: "h-12 w-auto",
    maxHeight: 64,
  };

  function escapeHtml(s = "") {
    return s.replace(
      /[&<>"']/g,
      (m) =>
        ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        })[m],
    );
  }

  function optFromEl(opt) {
    return {
      value: opt.value,
      label: opt.textContent.trim(),
      subtitle: opt.dataset.sub || "",
      homeImg: opt.dataset.homeImg || null,
      awayImg: opt.dataset.awayImg || null,
      homeTheme: opt.dataset.homeTheme || "#222",
      awayTheme: opt.dataset.awayTheme || "#222",
    };
  }

  function createNode(select, cfg) {
    select.classList.add("hidden");

    // const wrapper = document.createElement("label");
    const wrapper = document.createElement("div");
    wrapper.className = "relative flex flex-col gap-2";

    const btn = document.createElement("button");
    btn.type = "button";
    btn.className =
      "w-full px-3 py-2 rounded-lg bg-[#55005a] border border-[#430044] text-white flex items-center justify-between gap-3 focus:outline-none focus:ring-2 focus:ring-[#8a3fbf] overflow-hidden";
    btn.setAttribute("aria-expanded", "false");

    const content = document.createElement("div");
    content.className =
      "flex items-center justify-between gap-3 min-w-0 w-full";

    const homeBox = document.createElement("div");
    homeBox.className =
      "flex justify-center items-center w-10 h-10 rounded-xl hidden";
    const homeImg = document.createElement("img");
    homeImg.className = `${cfg.imgSize} object-contain`;
    homeBox.appendChild(homeImg);

    const label = document.createElement("div");
    label.className = "flex-1 text-center truncate text-white";
    label.textContent = cfg.placeholder;

    const awayBox = document.createElement("div");
    awayBox.className =
      "flex justify-center items-center w-10 h-10 rounded-xl hidden";

    const awayImg = document.createElement("img");
    awayImg.className = `${cfg.imgSize} object-contain`;
    awayBox.appendChild(awayImg);

    content.append(homeBox, label, awayBox);

    const arrow = document.createElement("span");
    arrow.textContent = "▼";
    arrow.className = "text-[8px]";

    btn.append(content, arrow);

    const panel = document.createElement("div");
    panel.className =
      "hidden absolute z-50 mt-[46px] w-full bg-[#55005a] border border-[#430044] rounded-lg shadow-lg";

    const ul = document.createElement("ul");
    ul.className = `max-h-${cfg.maxHeight} scrollbar-none overflow-auto divide-y divide-[#8a3fbf]`;

    panel.appendChild(ul);
    wrapper.append(btn, panel);
    select.after(wrapper);

    return {
      wrapper,
      btn,
      panel,
      ul,
      label,
      homeBox,
      homeImg,
      awayBox,
      awayImg,
    };
  }

  function renderList(ul, items, cfg, selectedValue) {
    ul.innerHTML = items
      .map(
        (it, i) => `
<li data-idx="${i}" data-value="${escapeHtml(it.value)}"
    class="px-3 py-2 cursor-pointer hover:bg-[#3a003c]
    ${selectedValue === it.value ? "bg-[#55005a]" : ""}">

  <div class="flex items-center justify-between gap-3">
    <div class="flex justify-center items-center w-10 h-10 rounded-xl" style="background:${
      it.homeTheme
    }">
      <img src="${it.homeImg}" class="${cfg.imgSize} object-contain"  />
    </div>
    <div class="flex-1 text-center truncate text-white">
      ${escapeHtml(it.label)}
    </div>
    <div class="flex justify-center items-center w-10 h-10 rounded-xl" style="background:${
      it.awayTheme
    }">
    <img src="${it.awayImg}" class="${cfg.imgSize} object-contain" />
    </div>
    
  </div>

  ${
    it.subtitle
      ? `<div class="text-xs text-gray-400 mt-1 text-center">${escapeHtml(
          it.subtitle,
        )}</div>`
      : ""
  }
</li>`,
      )
      .join("");
  }

  function init(selectEl, options = {}) {
    const cfg = { ...defaults, ...options };
    const nodes = createNode(selectEl, cfg);
    const { btn, panel, ul, label, homeImg, awayImg, homeBox, awayBox } = nodes;

    let items = Array.from(selectEl.options).map(optFromEl);
    let selected = null;
    let open = false;

    renderList(ul, items, cfg, null);

    function openPop() {
      open = true;
      panel.classList.remove("hidden");
      btn.setAttribute("aria-expanded", "true");
    }

    function closePop() {
      open = false;
      panel.classList.add("hidden");
      btn.setAttribute("aria-expanded", "false");
    }

    btn.addEventListener("click", () => (open ? closePop() : openPop()));

    ul.addEventListener("click", (e) => {
      e.preventDefault();
      e.stopPropagation();
      const li = e.target.closest("li");
      if (!li) return;

      const idx = Number(li.dataset.idx);
      const it = items[idx];
      if (!it) return;

      selected = it;

      label.textContent = it.label;
      homeBox.style.backgroundColor = it.homeTheme;
      awayBox.style.backgroundColor = it.awayTheme;

      homeImg.src = it.homeImg;
      awayImg.src = it.awayImg;

      homeBox.classList.remove("hidden");
      awayBox.classList.remove("hidden");

      selectEl.value = it.value;
      selectEl.dispatchEvent(new Event("change", { bubbles: true }));

      panel.classList.remove("mt-[46px]");
      panel.classList.add("mt-[60px]");

      renderList(ul, items, cfg, it.value);

      closePop();
    });

    document.addEventListener("pointerdown", (e) => {
      if (!btn.contains(e.target) && !panel.contains(e.target)) closePop();
    });

    return {
      setValue(value) {
        const it = items.find((x) => x.value == value);
        if (!it) return false;
        selected = it;
        label.textContent = it.label;
        homeImg.src = it.homeImg;
        awayImg.src = it.awayImg;
        homeImg.classList.remove("hidden");
        awayImg.classList.remove("hidden");
        selectEl.value = value;
        return true;
      },
    };
  }

  function initAll(selector = ".js-match-select", options = {}) {
    return [...document.querySelectorAll(selector)].map((s) =>
      init(s, options),
    );
  }

  return { init, initAll };
})();
