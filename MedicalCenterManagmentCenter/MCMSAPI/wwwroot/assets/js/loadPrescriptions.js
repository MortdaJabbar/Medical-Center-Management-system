
$(document).ready(function () {
    loadPrescriptions();
});

function loadPrescriptions() {
    $.ajax({
        url: 'https://localhost:7119/api/Prescriptions/detailed',
        method: 'GET',
        success: function (data) {
            const tableBody = $('#Prescriptions-table');
            tableBody.empty();

            data.forEach(p => {
                const row = `
                    <tr>
                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${p.patientImage}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                <span class="ms-2">${p.patientFullName}</span>
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${p.doctorImage}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                <span class="ms-2">${p.doctorFullName}</span>
                            </div>
                        </td>
                        <td>${p.medicationName}</td>
                        <td>${formatDate(p.prescriptionDate)}</td>
                        <td>${p.refills ?? '-'}</td>
                        <td>${p.instructions ?? '-'}</td>
                        <td>
                            <button class="btn btn-sm btn-primary me-1" data-id="${p.prescriptionID}" onclick="editPrescription(this)">Edit</button>
                            <button class="btn btn-sm btn-danger" data-id="${p.prescriptionID}" onclick="confirmDeletePrescription(this)">Delete</button>
                        </td>
                    </tr>
                `;
                tableBody.append(row);
            });
        },
        error: function () {
            alert("Failed to load prescriptions.");
        }
    });
}

function formatDate(dateStr) {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-GB'); // You can change to 'ar-IQ' if needed
}

