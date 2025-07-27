$(document).ready(function () {
   const doctorId = localStorage.getItem("entityId"); // ممكن تجيبه من localStorage

    $.ajax({
        url: `https://localhost:7119/api/Doctors/Prescriptions/${doctorId}`,
        method: "GET",
        success: function (prescriptions) {
            $("#prescriptions-body").empty();

            prescriptions.forEach(p => {
                const row = `
                    <tr>
                        <td class="p-3 d-flex align-items-center">
                            <img src="${p.patientImage}" alt="avatar" class="avatar avatar-md-sm rounded-circle shadow me-2" style="object-fit: cover; width: 40px; height: 40px;">
                            <span>${p.patientFullName}</span>
                        </td>
                         
                        <td class="p-3">${p.medicationName}</td>
                        <td class="p-3">${p.refills}</td>
                        <td class="p-3">${p.instructions}</td>
                        
                        <td class="p-3">${p.prescriptionDate}</td>
                    </tr>
                `;
                $("#prescriptions-body").append(row);
            });
            
PagationDataTable("#myTable",[],10);
            

        },
        error: function (xhr) {
            console.error("Failed to load prescriptions:", xhr.responseText);
        }
    });
});
