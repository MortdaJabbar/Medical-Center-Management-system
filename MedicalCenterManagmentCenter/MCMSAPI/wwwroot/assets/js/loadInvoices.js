function loadAllInvoices() {
    $.ajax({
        url: 'https://localhost:7119/api/Invoices/detailed',
        method: 'GET',
        success: function (data) {
            const tableBody = $('#invoices-body');
            tableBody.empty();
              
            data.forEach((invoice) => {
                const row = `
                    <tr>
                       
                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${invoice.patientImage}" class="avatar avatar-sm rounded-circle me-2" width="40" height="40" alt="">
                                <span>${invoice.patientName}</span>
                            </div>
                        </td>
                        <td>${invoice.serviceType}</td>
                        <td>${invoice.serviceDescription}</td>
                        <td>$${invoice.totalAmount}</td>
                        <td>${invoice.paymentMethod}</td>
                        <td>${invoice.paymentStatus}</td>
                        <td>${invoice.notes || ''}</td>
                        <td class="text-end">
                            <button class="btn btn-sm btn-warning edit-invoice-btn me-1"
                                    data-id="${invoice.invoiceID}"
                                    data-total="${invoice.totalAmount}"
                                    data-status="${invoice.paymentStatus}"
                                    data-notes="${invoice.notes}"
                                     >
                                Edit
                            </button>
                            <button class="btn btn-sm btn-danger delete-invoice-btn"
                                    data-id="${invoice.invoiceID}">
                                Delete
                            </button>
                        </td>
                    </tr>`;
                tableBody.append(row);
            });
        },
        error: function () {
            alert("Failed to load invoices.");
        }
    });
}

$(document).ready(function () {
    loadAllInvoices();
});
