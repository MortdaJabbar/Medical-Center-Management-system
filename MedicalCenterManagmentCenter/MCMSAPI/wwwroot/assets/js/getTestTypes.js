$(document).ready(function () {
    getTestTypes();
});

function getTestTypes() {
    $.ajax({
        url: 'https://localhost:7119/api/TestTypes/all',  // عدل حسب مسارك الفعلي
        method: 'GET',
        success: function (data) {
            const tableBody = $('#testType-body');
            tableBody.empty();

            data.forEach(testType => {
                const row = `
                    <tr>
                        <td>${testType.name}</td>
                        <td>${testType.description}</td>
                        <td>${testType.cost}</td>
                        <td class="text-end">
                            <button class="btn btn-sm btn-primary edit-btn" data-id="${testType.testTypeId}"><i class="ri-edit-line"></i></button>
                            <button class="btn btn-sm btn-danger delete-btn" data-id="${testType.testTypeId}"><i class="ri-delete-bin-line"></i></button>
                        </td>
                    </tr>
                `;
                tableBody.append(row);
                 PagationDataTable("#testtypes",[7,8],10);
            });
        },
        error: function (xhr) {
            console.error("Error fetching test types:", xhr.responseText);
        }
    });
}
