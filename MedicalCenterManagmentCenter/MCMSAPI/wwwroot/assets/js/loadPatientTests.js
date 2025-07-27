function loadPatientTestsFromApi(patientId) {

    
    const url = `https://localhost:7119/api/Patients/tests/${patientId}`;
    
    $.ajax({
  url: url,
  method: "GET",
  success: function(data) {
    renderTests(data); // عرض البيانات مثل ما كان
  },
  error: function(xhr) {
     
    showErrorMessage("Could not load test data.");
  }
});

}

function renderTests(tests) {
    const tbody = document.getElementById("tests-body");
    tbody.innerHTML = "";

    tests.forEach(test => {
        const row = document.createElement("tr");

        const doctorCell = `
            <td class="p-3">
                <div class="d-flex align-items-center">
                    <img src="${test.doctorImage}" alt="Doctor" class="avatar avatar-md-sm rounded-circle shadow me-2" style="width: 40px; height: 40px;">
                    <span>${test.doctorName}</span>
                </div>
            </td>
        `;

        const testResult = test.testResult
            ? `<a href="${test.testResult}" class="btn btn-sm btn-outline-primary" download>Download</a>`
            : `<span class="text-muted">N/A</span>`;

        const rowHTML = `
            ${doctorCell}
            <td class="p-3">${test.testTypeName}</td>
            <td class="p-3">${test.createdAt}</td>
            <td class="p-3"><span class="badge bg-${getStatusColor(test.status)}">${test.status}</span></td>
            <td class="p-3">$${test.cost}</td>
            <td class="p-3">${test.notes}</td>
            <td class="p-3">${testResult}</td>
        `;

        row.innerHTML = rowHTML;
        tbody.appendChild(row);
    });
}

function getStatusColor(status) {
    switch (status) {
        case "Pending": return "warning";
        case "Canceled": return "danger";
        case "Completed": return "success";
        default: return "secondary";
    }
}



// 📥 استدعاء المثال - استبدل ID بالقيمة الفعلية
document.addEventListener("DOMContentLoaded", function () {
     const   patientId = localStorage.getItem("entityId");// استبدل هذه بالقيمة الحقيقية أو اجلبها من كود آخر
    loadPatientTestsFromApi(patientId);
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
