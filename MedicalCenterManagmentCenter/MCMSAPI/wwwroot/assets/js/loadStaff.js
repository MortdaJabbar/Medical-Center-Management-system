function fetchStaff() {
    $.ajax({
        url: `https://localhost:7119/api/Staff/all`,
        method: "GET",
        success: function (data) {
            const tbody = $("#staff-body");
            tbody.empty();

            data.forEach(function (staff, index) {
                const p = staff.person;
                const fullName = `${p.firstName} ${p.secondName} ${p.thirdName ?? ''}`;
                const dob = p.dateOfBirth;
                const gender = p.gender ? "Male" : "Female";
                const phone = p.phone;
                const email = p.email;
                const image = p.imageLocation;
                const hireDate = staff.hireDate;
                const isAdmin = staff.isAdmin ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>';
                const staffId = staff.staffId;

                const row = `
                    <tr>
                        <td class="p-3">
                            <div class="d-flex align-items-center">
                                <img src="${image}" class="avatar avatar-md-sm rounded-circle shadow me-2" style="width: 40px; height: 40px;" alt="">
                                <span>${fullName}</span>
                            </div>
                        </td>
                        <td class="p-3">${dob}</td>
                        <td class="p-3">${gender}</td>
                        <td class="p-3">${email}</td>
                        <td class="p-3">${phone}</td>
                        <td class="p-3">${hireDate}</td>
                        <td class="p-3">${isAdmin}</td>
                        <td class="text-end p-3">
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-success" onclick="showUpdateStaffModal('${staffId}')">
                                <i class="uil uil-pen"></i>
                            </a>
                            <a href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-danger" onclick="confirmDeleteStaffModal('${staffId}', '${fullName}', '${image}')">
                                <i class="uil uil-trash"></i>
                            </a>
                        </td>
                    </tr>
                `;
                tbody.append(row);
            });
        },
        error: function (err) {
            switch (err.status) {
                case 404:
                    showErrorMessage("The requested staff data could not be found.");
                    break;
                case 500:
                    showErrorMessage("Something went wrong on the server.");
                    break;
                case 0:
                    showErrorMessage("Cannot connect to the server. Please check your internet connection.");
                    break;
                default:
                    showErrorMessage("An unexpected error occurred.");
                    break;
            }
        }
    });
}

$(document).ready(function () {
    fetchStaff();
});


function showErrorMessage(message) {
    $('#errorAlert').remove();
    $('body').append(`
        <div id="errorAlert" class="alert alert-danger alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 300px;">
            <i class="uil uil-exclamation-circle"></i> <strong>Error:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    setTimeout(() => { $('#errorAlert').alert('close'); }, 5000);
}

function showSuccessMessage(message) {
    $('#successAlert').remove();
    $('body').append(`
        <div id="successAlert" class="alert alert-success alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 600px;">
            <i class="uil uil-check-circle"></i> <strong>Success:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `);
    setTimeout(() => { $('#successAlert').alert('close'); }, 5000);
}

