(function () {

    const currentPage = window.location.pathname.split('/').pop().toLowerCase();
    const authPages = ['login.html', 'request-password-reset.html', 'resetpassword.html', 'email-confirmation.html', 'twofactorauthentication.html'];
    if (authPages.includes(currentPage)) {
        return;
    }

    function clearUiSession() {
        // Clear only UI/session hints; backend auth is cookie-based.
        localStorage.removeItem('token');
        localStorage.removeItem('roleId');
        localStorage.removeItem('userId');
        localStorage.removeItem('entityId');
        localStorage.removeItem('role');
    }

    function redirectToLogin() {
        clearUiSession();
        window.location.href = 'login.html';
    }

    // Always send cookies
    $.ajaxSetup({
        xhrFields: { withCredentials: true }
    });

    function validateSession(retriedRefresh) {
        $.ajax({
            url: 'https://localhost:7119/api/auth/me',
            method: 'GET',
            success: function (user) {

                // Save UI data only (not tokens)
                localStorage.setItem('role', user.role);
                localStorage.setItem('roleId', user.roleId);
                localStorage.setItem('userId', user.userId);
                localStorage.setItem('entityId', user.personId);

                validateRoleAccess(user.role);
            },
            error: function (xhr, status, error) {
                if (xhr && xhr.status === 401) {
                    // Try refresh once to avoid bouncing users when the access token expires.
                    if (!retriedRefresh) {
                        $.ajax({
                            url: 'https://localhost:7119/api/auth/refresh',
                            method: 'POST',
                            xhrFields: { withCredentials: true },
                            success: function () { validateSession(true); },
                            error: function () { redirectToLogin(); }
                        });
                        return;
                    }

                    redirectToLogin();
                    return;
                }

                // Network errors or CORS issues often have status 0; warn instead of redirecting
                console.warn('check-auth failed', { status: xhr && xhr.status, statusText: xhr && xhr.statusText, error });
            }
        });
    }

    validateSession(false);

    function validateRoleAccess(role) {

        const meta = document.querySelector('meta[name="allowed-roles"]');

        if (!meta) return;

        const allowedRoles = meta.content
            .split(',')
            .map(r => r.trim());

        if (!allowedRoles.includes(role)) {
            window.location.href = "403.html";
        }
    }

})();