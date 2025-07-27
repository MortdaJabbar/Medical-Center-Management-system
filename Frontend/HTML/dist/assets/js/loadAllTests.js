$(document).ready(function () {
    loadTests();
    

});

function loadTests() {
    $.ajax({
        url: 'https://localhost:7119/api/Tests/detailed',
        method: 'GET',
        success: function (data) {
            const tableBody = $('#test-body');
            tableBody.empty();

            data.forEach(test => {
                const row = `
                    <tr>
                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${convertPath(test.patientImage)}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                <span class="ms-2">${test.patientFullName}</span>
                            </div>
                        </td>

                        <td>
                            <div class="d-flex align-items-center">
                                <img src="${convertPath(test.doctorImage)}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                <span class="ms-2">${test.doctorFullName}</span>
                            </div>
                        </td>

                        <td>${test.testName}</td>
                        <td>${test.createdAt}</td>
                        <td><span class="badge bg-soft-${getStatusColor(test.status)}">${test.status}</span></td>
                        <td>${test.notes ?? ''}</td>
                        <td>${test.cost.toFixed(2)} $</td>
                        <td>
                            ${test.testResult
                                ? `<a href="${convertPath(test.testResult)}" target="_blank" class="btn btn-light btn-sm">Download</a>`
                                : `<span class="text-muted">N/A</span>`}
                        </td>

                        <td class="text-end">
                            <button 
                                class="btn btn-sm btn-soft-primary me-1 edit-btn" 
                                data-id="${test.testID}" 
                                data-bs-toggle="modal" 
                                data-bs-target="#editTestModal">
                                <i class="uil uil-pen"></i>
                            </button>

                            <button 
                                class="btn btn-sm btn-soft-danger delete-btn" 
                                data-id="${test.testID}" 
                                data-bs-toggle="modal" 
                                data-bs-target="#deleteTestModal">
                                <i class="uil uil-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;

                tableBody.append(row);
            });


           PagationDataTable("#testTable",[7,8],10)



        },
        error: function () {
            alert('Failed to load tests');
        }


        
    });
}

function getStatusColor(status) {
    switch (status.toLowerCase()) {
        case 'pending': return 'warning';
        case 'canceled': return 'danger';
        case 'completed': return 'success';
        default: return 'secondary';
    }
}

function convertPath(path) {
    // يعالج المسار المحلي ويحوله لمسار مناسب للعرض
    if (!path) return '';
    return path.replace(/\\\\|\\/g, '/').replace('C:/Users/User/Desktop/Doctris_v1.4.0/HTML/dist', '..');
}
