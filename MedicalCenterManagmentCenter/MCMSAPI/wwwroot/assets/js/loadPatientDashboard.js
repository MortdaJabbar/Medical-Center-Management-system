document.addEventListener("DOMContentLoaded", function () {
    const patientId = localStorage.getItem("entityId"); // ← استبدله بالـ ID الحقيقي

   $.ajax({
  url: `https://localhost:7119/api/Patients/dashboard/${patientId}`,
  method: "GET",
  success: function (data) {
    document.getElementById("totalTests").textContent = data.totalTests;
    document.getElementById("totalPrescriptions").textContent = data.totalPrescriptions;
    document.getElementById("upcomingAppointments").textContent = data.upcomingAppointments;
    document.getElementById("lastTestDate").textContent = data.lastTestDate ?? "—";

    const statusElement = document.getElementById("lastTestStatus");
    const status = data.lastTestStatus ?? "—";
    statusElement.textContent = status;

    // تغيير لون البادج حسب الحالة
    statusElement.className = "badge " + getStatusBadgeColor(status);
  },
  error: function (err) {
    console.log(err);
    showErrorMessage("Failed to load dashboard.");
  }
});

});

// ✅ بسيطة لتلوين الحالة
function getStatusBadgeColor(status) {
    switch (status) {
        case "Completed": return "bg-success";
        case "Pending": return "bg-warning text-dark";
        case "Canceled": return "bg-danger";
        default: return "bg-secondary";
    }
}






function showErrorMessage(message) {
    // Remove any existing alert first
    $('#errorAlert').remove();

    // Create and append the alert
    $('body').append(`
        <div id="errorAlert" class="alert alert-danger alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 300px;">
            <i class="uil uil-exclamation-circle"></i> <strong>Error:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);

    window.scrollTo({ top: 0, behavior: 'smooth' });

    // Auto close after 5 seconds
    setTimeout(() => { 
        $('#errorAlert').alert('close'); 
    }, 5000);
}
function showSuccessMessage(message) {
    // Remove any existing alert first
    $('#successAlert').remove();

    // Create and append the alert
    $('body').append(`
        <div id="successAlert" class="alert alert-success alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 600px;">
            <i class="uil uil-check-circle"></i> <strong>Success:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    window.scrollTo({ top: 0, behavior: 'smooth' });


    // Auto close after 3 seconds
    setTimeout(() => { 
        $('#successAlert').alert('close'); 
    }, 5000);
}
