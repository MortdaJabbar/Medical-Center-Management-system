
 function fetchPatients(pageNumber = 1 , pageSize=10) {
    $.ajax({
        url: `https://localhost:7119/api/Patients/all?page=${pageNumber}&size=${pageSize}`, // Replace with actual API URL
        method: "GET",
        success: function (data) {
            const tbody = $("#patientsBody");
            tbody.empty(); // Clear any existing rows

            data.forEach(function (patient, index) {
                const fullName = `${patient.person.firstName} ${patient.person.secondName} ${patient.person.thirdName ?? ''}`;
                const dob = patient.person.dateOfBirth;
                const gender = patient.person.gender ? "Male" : "Female";
                const phone = patient.person.phone;
                const email = patient.person.email;
                const weight = patient.weight;
                const height = patient.height;
                const image = patient.person.imageLocation;
                const patientId = patient.patientId;
                const personId  = patient.personId;

                const row = `
                    <tr>
                        <input type="hidden" class="patient-id" value="${patientId}">
                        <td class="p-3"><img src="${image}" class="avatar avatar-md-sm rounded-circle shadow" alt=""></td>
                        <td class="p-3">${fullName}</td>
                        <td class="p-3">${dob}</td>
                        <td class="p-3">${gender}</td>
                        <td class="p-3">${email}</td>
                        <td class="p-3">${phone}</td>
                        <td class="p-3">${weight} kg</td>
                        <td class="p-3">${height} cm</td>
                        <td class="text-end p-3">
                          <a   href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-primary"                  onclick="viewPatient('${patientId}')"> <i class="uil uil-eye"></i></a>
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-success"                 onclick="showUpdatePatientModal('${patientId}')"><i class="uil uil-pen"></i></a>
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-danger"                  onclick="openDeleteModal('${patientId}','${personId}','${fullName}','${image}')"><i class="uil uil-trash"></i></a>
                        </td>
                    </tr>
                `;
                tbody.append(row);
            });
            

PagationDataTable("#myTable",[8],5);
        },
        error: function (err) {
           

            // Show error message in the UI instead of just alerting
            const tbody = $("#patientsBody");
            tbody.empty(); // clear existing rows

            tbody.append(`
                <tr>
                    <td colspan="9" class="text-center text-danger">
                        🚨 There was an issue loading the patients Internal server error . Please try again later.
                    </td>
                </tr>
            `);
        }
    });
}


$(document).ready(function () {
    fetchPatients();
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

    // Auto close after 3 seconds
    setTimeout(() => { 
        $('#successAlert').alert('close'); 
    }, 5000);
}


