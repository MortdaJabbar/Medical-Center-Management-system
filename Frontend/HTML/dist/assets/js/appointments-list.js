    $(document).ready(function () {
        loadAppointments();
    });

    function loadAppointments() {
        $.ajax({
            url: "https://localhost:7119/api/Appointemnts/WithDetails",  
            method: "GET",
            success: function (appointments) {
                const tbody = $("#appointments-body");
                tbody.empty();

                appointments.forEach(app => {
                    const row = `
                        <tr>
                       <input type="hidden" name="appointmentId" class="appointment-id" value="${app.appointmentId}">

                            <td class="p-3">
                                <div class="d-flex align-items-center">
                                    <img loading="lazy" src="${app.patientImagePath ?? '../assets/images/default-user.jpg'}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                    <span class="ms-2">${app.patientFullName}</span>
                                </div>
                            </td>
                            <td class="p-3">
                                <div class="d-flex align-items-center">
                                    <img loading="lazy" src="${app.doctorImagePath ?? '../assets/images/default-user.jpg'}" class="avatar avatar-md-sm rounded-circle shadow" alt="">
                                    <span class="ms-2">${app.doctorFullName}</span>
                                </div>
                            </td>
                            <td class="p-3">${app.date}</td>
                            <td class="p-3">${app.time}</td>
                            <td class="p-3">${app.reason ?? '-'}</td>
                            <td class="p-3">${getStatusText(app.status)}</td>
                            <td class="p-3">${app.paid ? 'Paid' : 'Pending'}</td>
                             <td class="p-3">${app.notes}</td>
                            <td class="p-3 text-end">
                               <button type="button" class="btn btn-sm btn-primary edit-appointment-btn" data-id="${app.appointmentId}" data-bs-toggle="modal" data-bs-target="#updateAppointmentModal"> Edit </button>
                               <button class="btn btn-sm btn-danger delete-appointment-btn" data-id="1">Delete</button>

                            </td>
                        </tr>
                    `;
                    tbody.append(row);
                  
                 PagationDataTable("#AppointementsTable",[8],10);
                  
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
                        
                }  
                
            }
        });
    }

    function getStatusText(status) {
        switch (status) {
            case 0: return "Scheduled";
            case 1: return "Completed";
            case 2: return "Cancelled";
            default: return "Unknown";
        }
    }

   
    