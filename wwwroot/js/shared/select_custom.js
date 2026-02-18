export const CustomSelect = (function () {
  // helper: build option object from <option> element
  const optFromEl = (optEl) => ({
    value: optEl.value,
    label: optEl.textContent.trim(),
    img: optEl.dataset.img || null,
    subtitle: optEl.dataset.sub || optEl.dataset.subtitle || "",
  });

  const defaults = {
    placeholder: "Select",
    showImage: false,
    maxHeight: 56,
    imgSize: "w-auto h-7",
    noneScroll: "scrollbar-none",
  };

  function escapeHtml(s) {
    if (!s) return "";
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

  function createNode(select, cfg) {
    const wrapper = document.createElement("label");
    wrapper.className = "flex flex-col gap-2 relative";

    // hide original select
    select.classList.add("hidden");

    // button
    const btn = document.createElement("button");
    btn.type = "button";
    btn.className =
      "w-full bg-[#55005a] text-white px-3 py-2 rounded-lg flex items-center gap-3 justify-between border border-[#430044] focus:outline-none focus:ring-2 focus:ring-[#8a3fbf] overflow-hidden";
    btn.setAttribute("aria-haspopup", "listbox");
    btn.setAttribute("aria-expanded", "false");

    const left = document.createElement("div");
    left.className = "flex items-center gap-3 min-w-0";

    // button image (uses cfg.imgSize!)
    const img = document.createElement("img");
    img.className = `${cfg.imgSize} object-cover flex-shrink-0 hidden`;
    img.alt = "";

    const labelSpan = document.createElement("span");
    labelSpan.className = "truncate block";
    labelSpan.textContent = cfg.placeholder;

    left.appendChild(img);
    left.appendChild(labelSpan);

    const arrow = document.createElement("span");
    arrow.className = "text-[8px] text-white ml-2";
    arrow.textContent = "▼";

    btn.appendChild(left);
    btn.appendChild(arrow);

    // panel
    const panel = document.createElement("div");
    panel.className =
      "hidden absolute !mt-[45px] w-full bg-[#55005a] border border-[#430044] rounded-lg shadow-lg z-50 overflow-hidden";
    panel.style.minWidth = "100%";

    const searchWrap = document.createElement("div");
    searchWrap.className = "p-2";
    const input = document.createElement("input");
    input.type = "text";
    input.placeholder = "Search...";
    input.autocomplete = "off";
    input.className =
      "w-full px-3 py-2 rounded-md bg-[#2a002c] placeholder:text-gray-400 text-white focus:outline-none overflow-hidden";
    searchWrap.appendChild(input);

    const ul = document.createElement("ul");
    ul.setAttribute("role", "listbox");
    ul.tabIndex = 0;
    ul.className = `max-h-${cfg.maxHeight} ${cfg.noneScroll} overflow-auto divide-y divide-[#8a3fbf]`;

    panel.appendChild(searchWrap);
    panel.appendChild(ul);

    wrapper.appendChild(btn);
    wrapper.appendChild(panel);
    select.parentNode.insertBefore(wrapper, select.nextSibling);

    return {
      wrapper,
      btn,
      panel,
      input,
      ul,
      img,
      labelSpan,
      hiddenSelect: select,
    };
  }

  function renderList(ul, items, cfg, selectedValue) {
    ul.innerHTML = items
      .map((it, idx) => {
        const imageHtml =
          cfg.showImage && it.img
            ? `<img src="${it.img}" class="${cfg.imgSize} object-cover mr-2" alt="">`
            : "";
        const subtitle = it.subtitle
          ? `<div class="text-xs text-gray-400 truncate">${escapeHtml(
              it.subtitle,
            )}</div>`
          : "";
        return `<li role="option" data-idx="${idx}" data-value="${escapeHtml(
          it.value,
        )}"
                 class="flex items-center gap-3 px-3 py-2 cursor-pointer hover:bg-[#3a003c] ${
                   selectedValue == it.value ? "bg-[#55005a]" : ""
                 }">
                ${imageHtml}
                <div class="flex-1 min-w-0">
                  <div class="text-white truncate">${escapeHtml(it.label)}</div>
                  ${subtitle}
                </div>
              </li>`;
      })
      .join("");
  }

  function Instance(selectEl, options = {}) {
    const cfg = Object.assign({}, defaults, options);

    // override from data attributes if present
    if (selectEl.dataset.placeholder)
      cfg.placeholder = selectEl.dataset.placeholder;
    if (selectEl.dataset.showImage !== undefined)
      cfg.showImage = selectEl.dataset.showImage === "true";
    if (selectEl.dataset.imgSize) cfg.imgSize = selectEl.dataset.imgSize;

    const nodes = createNode(selectEl, cfg);
    const { btn, panel, input, ul, img, labelSpan, hiddenSelect } = nodes;

    // initial items
    let items = Array.from(hiddenSelect.options).map(optFromEl);
    let filtered = items.slice();
    let focused = -1;
    let open = false;

    let selected =
      hiddenSelect.selectedIndex >= 0
        ? items[hiddenSelect.selectedIndex]
        : null;
    if (selected) {
      labelSpan.textContent = selected.label;
      if (cfg.showImage && selected.img) {
        img.src = selected.img;
        img.classList.remove("hidden");
      }
    }

    renderList(ul, filtered, cfg, selected ? selected.value : null);

    function openPop() {
      open = true;
      panel.classList.remove("hidden");
      btn.setAttribute("aria-expanded", "true");
      btn.classList.toggle("ring-2", open);
      btn.classList.toggle("ring-[#8a3fbf]", open);
      input.focus();
      focused = -1;
      highlight();
    }
    function closePop() {
      open = false;
      panel.classList.add("hidden");
      btn.setAttribute("aria-expanded", "false");
      btn.classList.toggle("ring-2", open);
      btn.classList.toggle("ring-[#8a3fbf]", open);
      input.value = "";
      filtered = items.slice();
      renderList(ul, filtered, cfg, selected ? selected.value : null);
      focused = -1;
    }
    function filter(q) {
      const Q = q.trim().toLowerCase();
      filtered = items.filter(
        (it) =>
          it.label.toLowerCase().includes(Q) ||
          (it.subtitle || "").toLowerCase().includes(Q),
      );
      renderList(ul, filtered, cfg, selected ? selected.value : null);
    }
    function highlight() {
      const nodes = ul.querySelectorAll("li");
      nodes.forEach((n, i) => {
        const active = i === focused;
        n.classList.toggle("ring-2", active);
        n.classList.toggle("ring-purple-500/40", active);
      });
      if (focused >= 0 && nodes[focused])
        nodes[focused].scrollIntoView({ block: "nearest" });
    }

    btn.addEventListener("click", (e) => {
      e.preventDefault();
      open ? closePop() : openPop();
    });

    input.addEventListener("input", (e) => {
      filter(e.target.value);
      focused = -1;
      highlight();
    });

    ul.addEventListener("click", (e) => {
      e.preventDefault();
      e.stopPropagation();
      const li = e.target.closest("li");
      if (!li) return;
      const idx = Number(li.dataset.idx);
      const it = filtered[idx];
      if (!it) return;
      selected = it;
      labelSpan.textContent = it.label;
      if (cfg.showImage && it.img) {
        img.src = it.img;
        img.classList.remove("hidden");
      } else img.classList.add("hidden");
      hiddenSelect.value = it.value;
      hiddenSelect.dispatchEvent(new Event("change", { bubbles: true }));
      renderList(ul, filtered, cfg, selected.value);
      closePop();
    });

    input.addEventListener("keydown", (e) => {
      const nodes = ul.querySelectorAll("li");
      if (e.key === "ArrowDown") {
        focused = Math.min(focused + 1, nodes.length - 1);
        e.preventDefault();
        highlight();
      } else if (e.key === "ArrowUp") {
        focused = Math.max(focused - 1, 0);
        e.preventDefault();
        highlight();
      } else if (e.key === "Enter") {
        if (focused >= 0 && nodes[focused]) nodes[focused].click();
        e.preventDefault();
      } else if (e.key === "Escape") {
        closePop();
        btn.focus();
      }
    });

    function onDocPointerDown(e) {
      if (!btn.contains(e.target) && !panel.contains(e.target)) closePop();
    }
    function onFocusIn(e) {
      if (!btn.contains(e.target) && !panel.contains(e.target)) closePop();
    }

    document.addEventListener("pointerdown", onDocPointerDown, true);
    document.addEventListener("focusin", onFocusIn);

    return {
      setValue(value) {
        const it = items.find((x) => x.value == value);
        if (!it) return false;
        selected = it;
        hiddenSelect.value = it.value;
        hiddenSelect.dispatchEvent(new Event("change", { bubbles: true }));
        labelSpan.textContent = it.label;
        if (cfg.showImage && it.img) {
          img.src = it.img;
          img.classList.remove("hidden");
        } else img.classList.add("hidden");
        renderList(ul, filtered, cfg, selected.value);
        return true;
      },
      updateOptions(newOptions) {
        items = newOptions.slice();
        hiddenSelect.innerHTML = items
          .map((it) => {
            const o = document.createElement("option");
            o.value = it.value;
            o.textContent = it.label;
            if (it.img) o.dataset.img = it.img;
            if (it.subtitle) o.dataset.sub = it.subtitle;
            return o.outerHTML;
          })
          .join("");
        filtered = items.slice();
        selected = items.find((x) => x.value === hiddenSelect.value) || null;
        renderList(ul, filtered, cfg, selected ? selected.value : null);
        if (!selected) {
          labelSpan.textContent = cfg.placeholder;
          img.classList.add("hidden");
        }
      },
      destroy() {
        document.removeEventListener("pointerdown", onDocPointerDown, true);
        document.removeEventListener("focusin", onFocusIn);
        hiddenSelect.classList.remove("hidden");
        nodes.wrapper.remove();
      },
      _nodes: nodes,
    };
  }

  function initAll(selector = ".js-custom-select", options = {}) {
    const instances = [];
    document.querySelectorAll(selector).forEach((sel) => {
      instances.push(init(sel, options));
    });
    return instances;
  }

  function init(selectEl, options = {}) {
    if (!selectEl) throw new Error("selectElement is required");
    if (selectEl._customSelectInstance) return selectEl._customSelectInstance;
    const inst = Instance(selectEl, options);
    selectEl._customSelectInstance = inst;
    return inst;
  }

  return { initAll, init };
})();
