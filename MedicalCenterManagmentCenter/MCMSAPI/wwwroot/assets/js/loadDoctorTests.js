$(document).ready(function () {
   
    
    const doctorId = localStorage.getItem("entityId");
    
     // ممكن تجيبه من localStorage

    $.ajax({
        url: `https://localhost:7119/api/Doctors/tests/${doctorId}`,
        method: "GET",
         
        success: function (tests) {
            $("#tests-body").empty();

            tests.forEach(t => {
                const row = `
                    <tr>
                        <td class="p-3 d-flex align-items-center">
                            <img src="${t.patientImage}" alt="avatar" class="avatar avatar-md-sm rounded-circle shadow me-2" style="object-fit: cover; width: 40px; height: 40px;">
                            <span>${t.patientName}</span>
                        </td>
                        <td class="p-3">${t.testTypeName}</td>
                        <td class="p-3">${t.createdAt}</td>
                        <td class="p-3">${t.status}</td>
                        <td class="p-3">$${t.cost}</td>
                        <td class="p-3">${t.notes}</td>
                        <td class="p-3">${t.testResult ? `<a href="${t.testResult}" target="_blank">Download</a>` : "Not Available"}</td>
                    </tr>
                `;
                $("#tests-body").append(row);
            });
        },
        error: function (xhr) {
            console.error("Failed to load tests:", xhr.responseText);
        }
    });
});
