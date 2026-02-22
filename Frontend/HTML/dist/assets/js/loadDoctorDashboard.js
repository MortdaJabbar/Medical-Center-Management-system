$(document).ready(function () {
    const doctorId = localStorage.getItem("entityId");

    if (!doctorId) {
        console.error("Doctor ID not found in localStorage");
        return;
    }

    function loadDoctorDashboard(retried = false) {
        $.ajax({
            url: `https://localhost:7119/api/Doctors/dashboard/${doctorId}`,
            method: "GET",
            xhrFields: { withCredentials: true },
            success: function (data) {
                $("#doctorPatients").text(data.totalPatients);
                $("#doctorAppointments").text(data.upcomingAppointments);
                $("#doctorPrescriptions").text(data.totalPrescriptions);
                $("#doctorTests").text(data.totalTests);
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    if (!retried && typeof AuthClient !== 'undefined' && AuthClient.refresh) {
                        AuthClient.refresh()
                            .then(function () { loadDoctorDashboard(true); })
                            .catch(function () { window.location.href = 'login.html'; });
                        return;
                    }
                    window.location.href = 'login.html';
                    return;
                }
                console.error("Failed to load dashboard data:", xhr.responseText);
            }
        });
    }

    loadDoctorDashboard();
});
