 

function loadUnpaidPrescriptions() {
    $.ajax({
        url: 'https://localhost:7119/api/Invoices/unpaid-prescriptions',
        method: 'GET',
        success: function (data) {
            const tbody = $('#unpaid-prescriptions-body');
            tbody.empty();

            data.forEach(item => {
                const row = `
                    <tr>
                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${item.patientImage}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                <span class="ms-2">${item.patientName}</span>
                            </div>
                        </td>
                        <td>${item.serviceName}</td>
                        <td>$${item.totalAmount.toFixed(2)}</td>
                        <td>${new Date(item.createdAt).toLocaleDateString()}</td>
                        <td>
                             <button class="btn btn-sm btn-success add-invoice-btn" data-patient-id="${item.patientID}" data-service-id="${item.serviceID}" data-amount="${item.totalAmount}">
  Add Invoice
</button>
                        </td>
                    </tr>
                `;
                tbody.append(row);
            });
        },
        error: function () {
            alert('❌ Failed to load unpaid prescriptions.');
        }
    });
}

$(document).ready(function () {
    loadUnpaidPrescriptions();
});
