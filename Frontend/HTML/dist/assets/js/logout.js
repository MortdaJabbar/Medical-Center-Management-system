document.getElementById('logoutBtn').addEventListener('click', function (e) {
    e.preventDefault();

    $.ajax({
        url: "https://localhost:7119/api/auth/logout",
        method: "POST",
        xhrFields: { withCredentials: true },
        success: function () {

            // Clear only UI data
            localStorage.clear();
            sessionStorage.clear();

            window.location.href = "login.html";
        },
        error: function () {

            // Even if API fails, clear UI and redirect
            localStorage.clear();
            sessionStorage.clear();

            window.location.href = "login.html";
        }
    });
});