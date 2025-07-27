
            const patientId = 'ea9d259c-6bda-ef11-8002-9c2a701ec388'; // ← عيّن الـ ID الديناميكي حسب الجلسة
        
            $(document).ready(function () {
                $.ajax({
                    url: `https://localhost:7119/api/Patients/appointments/ea9d259c-6bda-ef11-8002-9c2a701ec388`,
                    type: 'GET',
                    success: function (appointments) {
                        let rows = '';
        
                        appointments.forEach(appt => {
                            const statusText = getStatusText(appt.status);
                            const paymentText = appt.paid ? `<span class="badge bg-success">Paid</span>` : `<span class="badge bg-danger">Unpaid</span>`;
        
                            rows += `
                                <tr>
                                    <td class="p-3">
                                        <div class="d-flex align-items-center">
                                            <img src="${appt.doctorImagePath.replace('C:\\\\Users\\\\User\\\\Desktop\\\\Doctris_v1.4.0\\\\HTML\\\\dist', '..')}" alt="Doctor" class="avatar avatar-md-sm rounded-circle shadow" />
                                            <span class="ms-2">${appt.doctorFullName}</span>
                                        </div>
                                    </td>
                                    <td class="p-3">${appt.date}</td>
                                    <td class="p-3">${appt.time.substring(0, 5)}</td>
                                    <td class="p-3">${appt.reason ?? ''}</td>
                                    <td class="p-3">${statusText}</td>
                                    <td class="p-3">${paymentText}</td>
                                    <td class="p-3">${appt.notes ?? ''}</td>
                                </tr>
                            `;
                        });
        
                        $('#appointments-body').html(rows);
                    },
                    error: function () {
                        $('#appointments-body').html('<tr><td colspan="7" class="text-center text-danger">Failed to load appointments.</td></tr>');
                    }
                });
        
                function getStatusText(status) {
                    switch (status) {
                        case 0: return 'Pending';
                        case 1: return 'Cancelled';
                        case 2: return 'Completed';
                        default: return 'Unknown';
                    }
                }
            });
 
        