$(document).ready(function () {
    $.ajax({
        url: "https://localhost:7119/api/Pharmacists/pharmacy-stats",
        method: "GET",
        success: function (data) {
            $('#totalMedications').text(data.totalMedications);
            $('#totalPrescriptions').text(data.totalPrescriptions);
            $('#inventoryStock').text(data.inventoryStock);
            $('#lowStockCount').text(data.lowStockCount);
        },
        error: function (xhr, status, error) {
            console.error("Failed to load pharmacy dashboard stats:", error);
        }
    });
});
