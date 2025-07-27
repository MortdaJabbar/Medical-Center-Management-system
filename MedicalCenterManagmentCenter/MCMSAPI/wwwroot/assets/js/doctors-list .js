
function fetchDoctors() {
    $.ajax({
        url: "https://localhost:7119/api/Doctors/all?page=1&size=10",
        method: "GET",
        success: function (data) {
            
             const tbody = $("#doctorsBody");
            tbody.empty();

            data.forEach(function (doctor, index) {
              
                const fullName = `${doctor.person.firstName} ${doctor.person.secondName} ${doctor.person.thirdName ?? ''}`;
                const dob = doctor.person.dateOfBirth;
                const gender = doctor.person.gender ? "Male" : "Female";
                const phone = doctor.person.phone;
                const email = doctor.person.email;
                const specialization = doctor.specialization;
                const available = doctor.available ? "OnDuty" : "Not Available";
                const image = doctor.person.imageLocation;
                const Years = doctor.experienceyears;
                const DoctorId = doctor.doctorId;
                const PersonId = doctor.person.personId;
              
                const row = `
                    <tr>
                       <input type="hidden" class="patient-id" value="${DoctorId}">
                        <td class="p-3"><img src="${image}" class="avatar avatar-md-sm rounded-circle shadow" alt=""></td>
                        <td class="p-3">${fullName}</td>
                        <td class="p-3">${dob}</td>
                        <td class="p-3">${gender}</td>
                        <td class="p-3">${email}</td>
                        <td class="p-3">${phone}</td>
                        <td class="p-3">${specialization}</td>
                        <td class="p-3">${Years}</td>
                        <td class="p-3">${available}</td>
                        <td class="text-end p-3">
                            <a  href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-primary"      onclick="viewDoctor('${DoctorId}')"       ><i class="uil uil-eye"></i></a>
                            <a  href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-success"      onclick="showUpdateDoctorModal('${DoctorId}')"     ><i class="uil uil-pen"></i></a>
                            <a  href="javascript:void(0)" class="btn btn-icon btn-pills btn-soft-danger"       onclick="confirmDeleteDoctorModal('${DoctorId}','${PersonId}','${fullName}','${image}')"><i class="uil uil-trash"></i></a>
                        </td>
                    </tr>`;
                tbody.append(row);
            });
        },
        error: function (err) {
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
  fetchDoctors();

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


