(function () {

    // Always send cookies
    $.ajaxSetup({
        xhrFields: { withCredentials: true }
    });

    // Call backend to validate session
    $.ajax({
        url: "https://localhost:7119/api/auth/me",
        method: "GET",
        success: function (user) {

            // Save UI data only (not tokens)
            localStorage.setItem("role", user.role);
            localStorage.setItem("roleId", user.roleId);
            localStorage.setItem("userId", user.userId);
            localStorage.setItem("entityId", user.personId);

            validateRoleAccess(user.role);
        },
        error: function (xhr, status, error) {
                // If unauthorized, force login. For network/CORS/server errors, avoid redirect loop
                if (xhr && xhr.status === 401) {
                    window.location.href = "login.html";
                    return;
                }

                // Network errors or CORS issues often have status 0; warn instead of redirecting
                console.warn('check-auth failed', { status: xhr && xhr.status, statusText: xhr && xhr.statusText, error });
            }
    });

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