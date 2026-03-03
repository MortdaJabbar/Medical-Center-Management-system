const currentPage = window.location.pathname.split('/').pop().toLowerCase();
const authPages = ['login.html', 'request-password-reset.html', 'resetpassword.html', 'email-confirmation.html', 'twofactorauthentication.html'];

function clearUiSession() {
    // Clear only UI/session hints; backend auth is cookie-based.
    localStorage.removeItem('token');
    localStorage.removeItem('roleId');
    localStorage.removeItem('userId');
    localStorage.removeItem('entityId');
    localStorage.removeItem('role');
}

function forwardToDashboard(roleId) {
    const parsed = parseInt(roleId, 10);
    if (isNaN(parsed) || parsed <= 0) return;
    FrowardToDashboardPage(parsed);
}

// Only run auto-forward logic on auth pages (login/reset/etc).
// Protected pages should use check-auth.js (cookie-based) instead.
if (!authPages.includes(currentPage)) {
    // Keep legacy behavior: if someone opens a protected page directly without UI state,
    // it will likely be handled by check-auth.js.
} else {
    const roleIdRaw = localStorage.getItem('roleId');
    if (roleIdRaw === null || roleIdRaw === undefined) {
        // Not logged in (or UI state not present) -> stay on auth page.
    } else {
        // IMPORTANT: roleId in localStorage may be stale. Verify cookie session first.
        const url = 'https://localhost:7119/api/auth/me';

        const onValid = function (user) {
            if (user && user.roleId) {
                // Keep UI values in sync
                localStorage.setItem('roleId', user.roleId);
                localStorage.setItem('userId', user.userId);
                localStorage.setItem('entityId', user.personId);
                localStorage.setItem('role', user.role);
                forwardToDashboard(user.roleId);
                return;
            }
            // Unexpected response shape -> treat as not logged in
            clearUiSession();
        };

        const onInvalid = function () {
            clearUiSession();
        };

        if (window.jQuery && window.$ && $.ajax) {
            $.ajax({
                url: url,
                method: 'GET',
                xhrFields: { withCredentials: true },
                success: onValid,
                error: function (xhr) {
                    if (xhr && xhr.status === 401) {
                        onInvalid();
                        return;
                    }
                    // For network/CORS issues, don't force navigation.
                    console.warn('sessionCheck: unable to validate session', xhr);
                }
            });
        } else {
            fetch(url, { method: 'GET', credentials: 'include' })
                .then(function (res) {
                    if (!res.ok) throw new Error('unauthorized');
                    return res.json();
                })
                .then(onValid)
                .catch(onInvalid);
        }
    }
}


function FrowardToDashboardPage(roleId) {
    switch (roleId) {
        case 1:
            if (currentPage !== 'admin-dashboard.html') {
                window.location.href = 'admin-dashboard.html';
            }
            break;
        case 2:
            if (currentPage !== 'doctor-dashboard.html') {
                window.location.href = 'doctor-dashboard.html';
            }
            break;
        case 3:
            if (currentPage !== 'patient-dashboard.html') {
                window.location.href = 'patient-dashboard.html';
            }
            break;
        case 4:
            if (currentPage !== 'pharmacist-dashboard.html') {
                window.location.href = 'pharmacist-dashboard.html';
            }
            break;
        case 5:
            if (currentPage !== 'staff-dashboard.html') {
                window.location.href = 'staff-dashboard.html';
            }
            break;
        default:
            if (!authPages.includes(currentPage)) {
                window.location.href = 'login.html';
            }
            break;
    }
}
  

