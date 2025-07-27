$(document).ready(function () {
    const doctorId = "065ba226-6a9c-4925-8e89-f50618480450"; // أو خليه من localStorage

    $.ajax({
        url: `https://localhost:7119/api/Doctors/appointments/${doctorId}`,
        method: "GET",
        success: function (appointments) {
            $("#appointments-body").empty(); // تفريغ الجدول أولاً

            appointments.forEach(app => {
                const row = `
                    <tr>
                        <td class="p-3 d-flex align-items-center">
                            <img src="${app.patientImage}" alt="avatar" class="avatar avatar-md-sm rounded-circle shadow me-2" style="object-fit: cover; width: 40px; height: 40px;">
                            <span>${app.patientFullName}</span>
                        </td>
                        <td class="p-3">${app.appointmentDate}</td>
                        <td class="p-3">${app.appointmentTime}</td>
                        <td class="p-3">${app.notes}</td>
                        <td class="p-3">${app.status}</td>
                        <td class="p-3">$${app.cost}</td>
                        <td class="p-3">${app.notes}</td>
                    </tr>
                `;
                $("#appointments-body").append(row);
            });
        },
        error: function (xhr) {
            console.error("Failed to load appointments:", xhr.responseText);
        }
    });
});
