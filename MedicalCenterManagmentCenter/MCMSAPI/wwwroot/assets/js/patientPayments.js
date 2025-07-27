
    const patientId = 'ea9d259c-6bda-ef11-8002-9c2a701ec388'; // عيّنه حسب الجلسة

    $(document).ready(function () {
        $.ajax({
            url: `https://localhost:7119/api/Patients/payments/${patientId}`,
            type: 'GET',
            success: function (payments) {
                let rows = '';

                payments.forEach(payment => {
                    const badge = getStatusBadge(payment.paymentStatus);

                    rows += `
                        <tr>
                            <td class="p-3">${payment.serviceType}</td>
                            <td class="p-3">${payment.paymentMethod}</td>
                            <td class="p-3">$${payment.amount.toFixed(2)}</td>
                            <td class="p-3">${badge}</td>
                            <td class="p-3">${new Date(payment.paymentDate).toLocaleString()}</td>
                            <td class="p-3">${payment.notes ?? ''}</td>
                        </tr>
                    `;
                });

                $('#payments-body').html(rows);
            },
            error: function () {
                $('#payments-body').html('<tr><td colspan="7" class="text-center text-danger">Failed to load payments.</td></tr>');
            }
        });

        function getStatusBadge(status) {
            switch (status.toLowerCase()) {
                case 'completed': return `<span class="badge bg-success">Completed</span>`;
                case 'pending': return `<span class="badge bg-warning text-dark">Pending</span>`;
                case 'failed': return `<span class="badge bg-danger">Failed</span>`;
                default: return `<span class="badge bg-secondary">Unknown</span>`;
            }
        }
    });

