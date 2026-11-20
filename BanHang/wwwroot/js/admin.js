(function () {
    'use strict';

    // ----- 1. Ripple effect on menu items -----
    document.querySelectorAll('.ripple').forEach(function (el) {
        el.addEventListener('click', function (e) {
            var rect = el.getBoundingClientRect();
            var size = Math.max(rect.width, rect.height);
            var ink = document.createElement('span');
            ink.className = 'ripple-ink';
            ink.style.width = ink.style.height = size + 'px';
            ink.style.left = (e.clientX - rect.left - size / 2) + 'px';
            ink.style.top = (e.clientY - rect.top - size / 2) + 'px';
            el.appendChild(ink);
            setTimeout(function () { ink.remove(); }, 600);
        });
    });

    // ----- 2. Sidebar collapse (250px <-> 72px) / mobile slide-in -----
    var sb = document.getElementById('sidebar');
    var toggle = document.getElementById('sidebarToggle');
    if (toggle && sb) {
        toggle.addEventListener('click', function () {
            if (window.innerWidth <= 575) {
                sb.classList.toggle('mobile-open');
                sb.classList.remove('collapsed');
            } else {
                sb.classList.toggle('collapsed');
            }
        });
    }

    // ----- 3. Submenu toggle (slideDown + arrow rotate) -----
    document.querySelectorAll('.nav-group.has-sub > .nav-toggle').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var group = btn.parentElement;
            group.classList.toggle('open');
        });
    });

    // ----- 4. Profile dropdown (fade + scale) -----
    var profileWrap = document.getElementById('profileWrap');
    var profileToggle = document.getElementById('profileToggle');
    if (profileWrap && profileToggle) {
        profileToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            profileWrap.classList.toggle('open');
        });
        document.addEventListener('click', function (e) {
            if (profileWrap.classList.contains('open') && !profileWrap.contains(e.target)) {
                profileWrap.classList.remove('open');
            }
        });
        profileWrap.querySelectorAll('a, form, button').forEach(function (el) {
            el.addEventListener('click', function () {
                profileWrap.classList.remove('open');
            });
        });
    }

    // ----- 5. Scroll reveal + number counting (premium entrance) -----
    function formatViet(v) {
        return v.toLocaleString('vi-VN');
    }

    function animateCount(el) {
        var html = el.innerHTML.trim();
        var m = html.match(/^([\d.,]+)([\s\S]*)$/);
        if (!m) return;
        if (!/^\d[\d.,]*$/.test(m[1])) return;
        var target = parseInt(m[1].replace(/[.,]/g, ''), 10);
        if (isNaN(target)) return;
        var suffix = m[2];
        var duration = 900;
        var start = null;

        function step(ts) {
            if (!start) start = ts;
            var p = Math.min((ts - start) / duration, 1);
            var eased = 1 - Math.pow(1 - p, 3);
            el.innerHTML = formatViet(Math.round(target * eased)) + suffix;
            if (p < 1) requestAnimationFrame(step);
        }
        requestAnimationFrame(step);
    }

    var toReveal = document.querySelectorAll(
        '.stat-card, .card-fold, .toolbar, .page-head'
    );

    if ('IntersectionObserver' in window && toReveal.length) {
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                var el = entry.target;
                if (!el.classList.contains('reveal')) {
                    el.classList.add('reveal');
                    // need a frame so the starting (hidden) styles apply before 'in'
                    requestAnimationFrame(function () {
                        el.classList.add('in');
                    });
                }
                // number counting when its parent first appears
                el.querySelectorAll('.stat-value').forEach(function (sv) {
                    if (!sv.dataset.counted) {
                        sv.dataset.counted = '1';
                        animateCount(sv);
                    }
                });
                io.unobserve(el);
            });
        }, { threshold: 0.08 });
        toReveal.forEach(function (el) { io.observe(el); });
    } else {
        // fallback: show everything immediately and count
        toReveal.forEach(function (el) {
            el.querySelectorAll('.stat-value').forEach(function (sv) {
                if (!sv.dataset.counted) { sv.dataset.counted = '1'; animateCount(sv); }
            });
        });
    }
})();