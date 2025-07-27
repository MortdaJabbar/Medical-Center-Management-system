$(document).ready(function () {
    
    const doctorId = localStorage.getItem("entityId");
  

    if (!doctorId) {
        console.error("Doctor ID not found in localStorage");
        return;
    }

    $.ajax({
        url: `https://localhost:7119/api/Doctors/dashboard/${doctorId}`,
        method: "GET",
         
        success: function (data) {
            $("#doctorPatients").text(data.totalPatients);
            $("#doctorAppointments").text(data.upcomingAppointments);
            $("#doctorPrescriptions").text(data.totalPrescriptions);
            $("#doctorTests").text(data.totalTests);
        },
        error: function (xhr) {
            console.error("Failed to load dashboard data:", xhr.responseText);
        }
    });
});
