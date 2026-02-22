$(document).ready(function () {
    $.ajax({
        url: "https://localhost:7119/api/Staff/admin-stats",
        method: "GET",
        xhrFields: { withCredentials: true },
        success: function (data) {
            $('#totalUsers').text(data.totalUsers);
            $('#totalPatients').text(data.totalPatients);
            $('#totalPharmacists').text(data.totalPharmacists);
            $('#totalStaff').text(data.totalStaff);
            $('#totalAccounts').text(data.totalAccounts);
        },
        error: function (xhr, status, error) {
            if (xhr && xhr.status === 401) {
                // session expired -> redirect to login
                window.location.href = "login.html";
            }
        }
    });
});
