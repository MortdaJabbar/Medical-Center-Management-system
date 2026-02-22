Project: Medical Center Management - Frontend (dist)

Summary
- Scanned: ~52 HTML files, ~36 CSS files, ~213 JS files (assets and libs) under Frontend/HTML/dist
- Representative files read: Pages/login.html, Pages/admin-dashboard.html, assets/css/style.css, assets/js/app.js, assets/js/plugins.init.js, assets/js/loadAdminDashboard.js, assets/js/sessionCheck.js, assets/js/check-auth.js, assets/js/login.js, assets/js/logout.js

Key libraries & frameworks observed
- Bootstrap 5 (CSS + JS)
- jQuery (3.6.0) used for many AJAX calls and DOM helpers
- SweetAlert2 for alerts
- Feather icons, Remixicon, Unicons
- Tiny-slider, Tobii, FullCalendar, ApexCharts, Simplebar (third-party libs present)

Authentication & backend
- Backend API base: https://localhost:7119 (used throughout JS) — endpoints discovered:
  - POST /api/auth/login (login.js)
  - GET /api/auth/me (check-auth.js)
  - POST /api/auth/logout (logout.js)
  - GET /api/Staff/admin-stats (loadAdminDashboard.js)
- AJAX calls set xhrFields.withCredentials and $.ajaxSetup({xhrFields:{withCredentials:true}}) — backend expects cookies for session auth.
- Auth client flow:
  - login.js posts credentials, stores UI info in localStorage (roleId, userId, entityId, role).
  - check-auth.js fetches /api/auth/me and writes role/roleId/userId/entityId to localStorage; validates role against meta[name="allowed-roles"].
  - sessionCheck.js reads localStorage.roleId and forwards user to the appropriate dashboard page.

Security & correctness notes
- Absolute local file paths appear in HTML: admin-dashboard.html uses C:/Images/00.jpg for profile image — this will fail for remote clients and leaks local file references.
- Backend base is localhost; deployment will require changing API base to a public host or proxy.
- Credentials are sent with cookies (withCredentials) — ensure backend sets proper SameSite, Secure, and CSRF protections.
- Many JS files use jQuery AJAX without centralized error handling; some error handlers are empty (silent failures).
- Some files include third-party libs (minified) — verify license compatibility if modifying.

Code & UX observations (representative)
- assets/css/style.css: Theme-driven stylesheet from Doctris template. Contains RTL/dark-theme variants, many utility classes; ready-made responsive rules.
- assets/js/app.js: Main UI glue (menu activation, sidebar toggling, back-to-top, tooltip/popover init). Uses `feather.replace()` and Gumshoe (try/catch). Clean, template-based.
- assets/js/plugins.init.js: Initializes tiny-slider, Tobii lightbox, datepicker, CKEditor; sets theme switching by swapping CSS hrefs.
- assets/js/loadAdminDashboard.js: Uses jQuery AJAX to GET admin stats and fill dashboard counters; no visible auth header besides cookies.
- assets/js/login.js: Posts to /api/auth/login, handles 2FA message by redirecting to TwoFactorAuthentication.html and storing tempUserId; stores role & ids in localStorage on success.
- assets/js/logout.js: POST to /api/auth/logout and clears localStorage/sessionStorage.
- assets/js/check-auth.js: Validates session; redirects to login on error and to 403 when role not allowed.

Problems to fix / recommended next actions
- Replace absolute local image paths (C:/Images/00.jpg) with user-provided or remote URLs.
- Centralize API base URL in a small config (e.g., assets/js/config.js) to ease switching from localhost.
- Add robust error handling and user-friendly messages for failed API calls.
- Audit auth flows for token vs cookie usage and ensure secure cookie attributes + CSRF protection.
- Consider removing unused/duplicated library files and verify their versions/licenses.
- Optionally produce a full per-file analysis (function list, potential XSS sinks, exact line-level notes). This repo contains ~300 JS/CSS/HTML files; generating a line-by-line audit will take longer.

Saved analysis file
- Location: Frontend/HTML/dist/analysis.md (this document)

If you want: I can now
- Produce a full per-file detailed report (one file per HTML/CSS/JS) and store them under Frontend/HTML/dist/analysis/ (will create many files), or
- Implement a small `config.js` that centralizes the API base and a quick patch to fix absolute image paths on main pages.

End of analysis.
