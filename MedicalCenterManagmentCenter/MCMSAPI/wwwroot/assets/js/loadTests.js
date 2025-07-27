$(document).ready(function () {
   
   

    loadTests();
    
});


 function loadTests() {
        $.ajax({
            url: 'https://localhost:7119/api/Tests/detailed', // ← عدّل حسب مسارك الفعلي
            method: 'GET',
            success: function (data) {
                const tableBody = $('#test-body');
                tableBody.empty();

                data.forEach(test => {
                    const row = `
                        <tr>
                            <td>
                                <div class="d-flex align-items-center">
                                    <img src="${test.patientImage}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                    <span>${test.patientFullName}</span>
                                </div>
                            </td>
                            <td>
                                <div class="d-flex align-items-center">
                                    <img src="${test.doctorImage}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                    <span>${test.doctorFullName}</span>
                                </div>
                            </td>
                            <td>${test.testName}</td>
                            <td>${test.createdAt}</td>
                            <td><span class="badge bg-soft-${getStatusColor(test.status)}">${test.status}</span></td>
                            <td>${test.notes || ''}</td>
                            <td>${test.cost.toFixed(2)} $</td>
                            <td>
                                ${
                                    test.testResult
                                    ? `<a href="${test.testResult}" target="_blank" download class="btn btn-sm btn-light">Download</a>`
                                    : `<span class="text-muted">N/A</span>`
                                }
                            </td>
                            <td class="text-end">
                                <button class="btn btn-sm btn-soft-primary me-1 edit-btn"  data-toggle="modal" data-target="#exampleModalCenter"  data-id="${test.testID}" >
                                    <i class="uil uil-pen"></i>
                                </button>
                                <button class="btn btn-sm btn-soft-danger delete-btn"    data-toggle="modal" data-target="#exampleModalCenter"  data-id="${test.testID}">
                                    <i class="uil uil-trash"></i>
                                </button>
                            </td>
                        </tr>
                    `;
                    tableBody.append(row);
                });
            },
            error: function () {
                alert('Failed to load test data.');
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
