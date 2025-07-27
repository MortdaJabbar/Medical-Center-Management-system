function fetchPharmacists() {
    $.ajax({
        url: `https://localhost:7119/api/Pharmacists/all`,
        method: "GET",
        success: function (data) {
            const tbody = $("#pharmacistsBody");
            tbody.empty();

            data.forEach(function (pharmacist, index) {
                const fullName = `${pharmacist.person.firstName} ${pharmacist.person.secondName} ${pharmacist.person.thirdName ?? ''}`;
                const dob = pharmacist.person.dateOfBirth;
                const gender = pharmacist.person.gender ? "Male" : "Female";
                const phone = pharmacist.person.phone;
                const email = pharmacist.person.email;
                const image = pharmacist.person.imageLocation;
                const hireDate = pharmacist.hireDate;
                const experience = pharmacist.expereinceYears ?? "N/A";
                const license = pharmacist.licenseNumber;
                const pharmacistId = pharmacist.pharmacistId;
                const personId = pharmacist.person.personId;
                
                const row = `
                    <tr>
                         <input type="hidden" class="pharmacist-id" value="${pharmacistId}">
                        <td class="p-3"><img src="${image}" class="avatar avatar-md-sm rounded-circle shadow" alt=""></td>
                        <td class="p-3">${fullName}</td>
                        <td class="p-3">${dob}</td>
                        <td class="p-3">${gender}</td>
                        <td class="p-3">${email}</td>
                        <td class="p-3">${phone}</td>
                        <td class="p-3">${hireDate}</td>
                        <td class="p-3">${experience}</td>
                        <td class="p-3">${license}</td>
                        <td class="text-end p-3">
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-primary" onclick="viewPharmacist('${pharmacistId}')"><i class="uil uil-eye"></i></a>
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-success" onclick="showUpdatePharmacistModal('${pharmacistId}')"><i class="uil uil-pen"></i></a>
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-danger" onclick="confirmDeletePharmacistModal('${pharmacistId}','${fullName}','${image}')"><i class="uil uil-trash"></i></a>
                        </td>
                    </tr>
                `;
                tbody.append(row);

            });
            

PagationDataTable("#myTable",[9],5);
        },
        error: function () {
            switch (err.status) {
                case 404:
                    showErrorMessage("The requested data could not be found.");
                    break;
                case 500:
                    showErrorMessage("Something went wrong on our server. Please try again later.");
                case 0 : showErrorMessage("Cannot connect to server. Please check your internet connection or if the server is running.");
                    break;
                default:
                    showErrorMessage("Something went wrong.  Please Check Your internet connection or try again later.");
               break;
            }  
        }
    });
}

$(document).ready(function () {
   
    fetchPharmacists();
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


