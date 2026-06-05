// ============================================
// HiSUP — Site JavaScript
// HITEC University Student Portal
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    initSidebar();
    highlightActiveNav();
    initScrollAnimations();
    initToasts();
    initCounters();
    initEnrollButtons();
});

// ============ SIDEBAR TOGGLE (Mobile) ============

function initSidebar() {
    var toggle = document.getElementById('sidebarToggle');
    var sidebar = document.getElementById('sidebar');
    var overlay = document.getElementById('sidebarOverlay');

    if (toggle && sidebar) {
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('open');
            if (overlay) overlay.classList.toggle('active');
        });
    }

    if (overlay) {
        overlay.addEventListener('click', function () {
            if (sidebar) sidebar.classList.remove('open');
            overlay.classList.remove('active');
        });
    }
}

// ============ ACTIVE NAV LINK ============

function highlightActiveNav() {
    var path = window.location.pathname.toLowerCase().replace(/\/+$/, '');
    var links = document.querySelectorAll('.sidebar-link[data-page]');

    links.forEach(function (link) {
        var page = link.getAttribute('data-page').toLowerCase();
        link.classList.remove('active');

        if (page === '/' && (path === '/' || path === '' || path === '/home' || path === '/home/index')) {
            link.classList.add('active');
        } else if (page !== '/' && path.startsWith(page)) {
            link.classList.add('active');
        }
    });
}

// ============ SCROLL FADE-IN ANIMATIONS ============

function initScrollAnimations() {
    var elements = document.querySelectorAll('.fade-in');
    if (!elements.length) return;

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -40px 0px'
    });

    elements.forEach(function (el) {
        observer.observe(el);
    });
}

// ============ TOAST AUTO-DISMISS ============

function initToasts() {
    var toasts = document.querySelectorAll('.toast-notification');
    toasts.forEach(function (toast) {
        setTimeout(function () {
            toast.style.animation = 'slideOut 0.4s ease forwards';
            setTimeout(function () {
                if (toast.parentNode) {
                    toast.remove();
                }
            }, 400);
        }, 5000);
    });
}

// ============ ANIMATED COUNTERS ============

function initCounters() {
    var counters = document.querySelectorAll('.counter');
    if (!counters.length) return;

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.5 });

    counters.forEach(function (counter) {
        observer.observe(counter);
    });
}

function animateCounter(el) {
    var target = parseInt(el.getAttribute('data-target'), 10);
    if (isNaN(target)) return;

    var duration = 1500;
    var start = performance.now();

    function update(currentTime) {
        var elapsed = currentTime - start;
        var progress = Math.min(elapsed / duration, 1);
        // Ease-out cubic
        var eased = 1 - Math.pow(1 - progress, 3);
        var current = Math.floor(eased * target);
        el.textContent = current.toLocaleString();

        if (progress < 1) {
            requestAnimationFrame(update);
        } else {
            el.textContent = target.toLocaleString();
        }
    }

    requestAnimationFrame(update);
}

// ============ ENROLL BUTTON LOADING ============

function initEnrollButtons() {
    var forms = document.querySelectorAll('.enroll-form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function () {
            var btn = this.querySelector('.enroll-btn');
            if (btn && !btn.disabled) {
                btn.classList.add('loading');
                var span = btn.querySelector('span');
                if (span) span.textContent = 'Enrolling...';
                var icon = btn.querySelector('i');
                if (icon) icon.style.display = 'none';
            }
        });
    });
}
