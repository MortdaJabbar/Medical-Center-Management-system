    $(document).ready(function () {
        loadMedications();
    });

    function loadMedications() {
        $.ajax({
            url: 'https://localhost:7119/api/Medications/all', // ← غيّره حسب رابط API مالتك
            method: 'GET',
            success: function (medications) {
                const tbody = $('#Medication-body');
                tbody.empty();

                medications.forEach(med => {
                const row = `
    <tr>
        <td class="p-3">${med.name}</td>
        <td class="p-3">${med.description ?? ''}</td>
        <td class="p-3">${med.strength}</td>
        <td class="p-3">${med.dosageForm}</td>
        <td class="p-3 text-end">
            <button class="btn btn-sm btn-primary me-2 edit-btn" data-id="${med.medicationID}">Edit</button>
            <button class="btn btn-sm btn-danger delete-btn" data-id="${med.medicationID}">Delete</button>
        </td>
    </tr>
`;
                    tbody.append(row);
                });
                PagationDataTable("#myTable",[4],10);
            },
            error: function () {
                alert('Failed to load medications.');
            }
        });
    }