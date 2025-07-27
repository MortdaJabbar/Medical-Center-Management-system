function loadAppointmentsForPatient(patientId) {
    
    const url = `https://localhost:7119/api/Patients/appointments/${patientId}`;

    $.ajax({
  url: url, // نفس المتغير url
  method: "GET",
  success: function(data) {
    renderAppointments(data); // نفس الدالة الأصلية
  },
  error: function(xhr) {
    console.error(xhr.responseText);
    showErrorMessage("❌ Could not load appointments.");
  }
});

}

function renderAppointments(appointments) {
    const tbody = document.getElementById("appointments-body");
    tbody.innerHTML = "";

    appointments.forEach(app => {
        const row = document.createElement("tr");

        const doctorCell = `
            <td class="p-3">
                <div class="d-flex align-items-center">
                    <img src="${app.doctorImagePath}" alt="Doctor" class="avatar avatar-md-sm rounded-circle shadow me-2" style="width: 40px; height: 40px;">
                    <span>${app.doctorFullName}</span>
                </div>
            </td>
        `;

        const statusBadge = getStatusBadge(app.status);
        const paymentBadge = app.paid 
            ? '<span class="badge bg-success">Paid</span>' 
            : '<span class="badge bg-warning text-dark">Unpaid</span>';

        const rowHTML = `
            ${doctorCell}
            <td class="p-3">${app.date}</td>
            <td class="p-3">${app.time}</td>
            <td class="p-3">${app.reason}</td>
            <td class="p-3">${statusBadge}</td>
            <td class="p-3">${paymentBadge}</td>
            <td class="p-3">${app.notes ?? '<span class="text-muted">N/A</span>'}</td>
        `;

        row.innerHTML = rowHTML;
        tbody.appendChild(row);
    });

    PagationDataTable("#myTable",[],10);
}

function getStatusBadge(status) {
    switch (status) {
        case 0: return '<span class="badge bg-warning text-dark">Pending</span>';
        case 1: return '<span class="badge bg-success">Confirmed</span>';
        case 2: return '<span class="badge bg-danger">Canceled</span>';
        case 3: return '<span class="badge bg-secondary">Completed</span>';
        default: return '<span class="badge bg-dark">Unknown</span>';
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


document.addEventListener("DOMContentLoaded", function () {
     patientId = localStorage.getItem("entityId");
    loadAppointmentsForPatient(patientId);
});
