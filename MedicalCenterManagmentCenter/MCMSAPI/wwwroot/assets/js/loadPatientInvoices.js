

function loadInvoicesForPatient(patientId) {
 
    const url = `https://localhost:7119/api/Patients/Invoices/${patientId}`;

    $.ajax({
  url: url,
  method: "GET",
  success: function(data) {
    renderInvoices(data); // نفس الدالة الأصلية لعرض الفواتير
  },
  error: function(xhr) {
    
    showErrorMessage("Could not load invoices.");
  }
});

}

function renderInvoices(invoices) {
    const tbody = document.getElementById("payments-body");
    tbody.innerHTML = "";

    invoices.forEach(inv => {
        const statusBadge = inv.paymentStatus === "Paid"
            ? '<span class="badge bg-success">Paid</span>'
            : '<span class="badge bg-warning text-dark">Unpaid</span>';

        const viewBtn = `
            <button class="btn btn-sm btn-outline-primary" onclick="viewService('${inv.serviceType}', ${inv.serviceID})">
                View
            </button>
        `;

        const row = `
            <tr>
                <td class="p-3">${inv.serviceType}</td>
                <td class="p-3">${inv.paymentMethod}</td>
                <td class="p-3">$${inv.totalAmount}</td>
                <td class="p-3">${statusBadge}</td>
                <td class="p-3">${formatDate(inv.createdAt)}</td>
                <td class="p-3">${inv.notes ?? '<span class="text-muted">N/A</span>'}</td>
                
            </tr>
        `;
        tbody.innerHTML += row;
    });
}

function formatDate(dateStr) {
    const d = new Date(dateStr);
    return d.toLocaleDateString() + ' ' + d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function viewService(serviceType, serviceID) {
    alert(`🔍 You clicked to view ${serviceType} with ID: ${serviceID}`);
    // لاحقاً تربط هذا بمودال أو تنقله لصفحة التفاصيل
}

function showErrorMessage(message) {
    alert(message);
}

document.addEventListener("DOMContentLoaded", function () {
        patientId = localStorage.getItem("entityId");
    loadInvoicesForPatient(patientId);
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
