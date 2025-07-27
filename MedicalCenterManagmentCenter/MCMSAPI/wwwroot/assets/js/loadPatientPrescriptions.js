function loadPrescriptionsForPatient(patientId) {
   
    const url = `https://localhost:7119/api/Patients/prescriptions/${patientId}`;

   $.ajax({
  url: url,
  method: "GET",
  success: function(data) {
    renderPrescriptions(data); // عرض البيانات باستخدام نفس الدالة
  },
  error: function(xhr) {
    
    showErrorMessage("  Could not load prescriptions or there are no records.");
  }
});

}

function renderPrescriptions(prescriptions) {
    const tbody = document.getElementById("prescriptions-body");
    tbody.innerHTML = "";

    prescriptions.forEach(pres => {
        const row = document.createElement("tr");

        const doctorCell = `
            <td class="p-3">
                <div class="d-flex align-items-center">
                    <img   src="${pres.doctorImage}" alt="Doctor" class="avatar avatar-md-sm rounded-circle shadow me-2" style="width: 40px; height: 40px;">
                    <span>${pres.doctorName}</span>
                </div>
            </td>
        `;

         

        const rowHTML = `
            ${doctorCell}
            <td class="p-3">${pres.medicationName}</td>
            <td class="p-3">${pres.refills}</td>
            <td class="p-3">${pres.instructions}</td>
            <td class="p-3 text-muted">N/A</td>
            <td class="p-3">${pres.prescriptionDate}</td>
        `;

        row.innerHTML = rowHTML;
        tbody.appendChild(row);
    });
}



// عند تحميل الصفحة – استبدل ID
document.addEventListener("DOMContentLoaded", function () {
      patientId = localStorage.getItem("entityId");
    loadPrescriptionsForPatient(patientId);
});






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
