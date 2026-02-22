const roleIdRaw = localStorage.getItem('roleId');

const currentPage = window.location.pathname.split('/').pop().toLowerCase();
const authPages = ['login.html', 'request-password-reset.html', 'resetpassword.html', 'email-confirmation.html'];

if (roleIdRaw !== null && roleIdRaw !== undefined) {
    const roleId = parseInt(roleIdRaw, 10);
    if (!isNaN(roleId) && roleId > 0) {
        FrowardToDashboardPage(roleId);
    } else {
        // If user has no valid role and is not already on an auth page, redirect to login
        if (!authPages.includes(currentPage)) {
            window.location.href = 'login.html';
        }
    }
} else {
    if (!authPages.includes(currentPage)) {
        window.location.href = 'login.html';
    }
}


function FrowardToDashboardPage(roleId) {
    switch (roleId) {
        case 1:
            if (currentPage !== 'admin-dashboard.html') {
                window.location.href = 'admin-dashboard.html';
            }
            break;
        case 2:
            if (currentPage !== 'doctor-dashboard.html') {
                window.location.href = 'doctor-dashboard.html';
            }
            break;
        case 3:
            if (currentPage !== 'patient-dashboard.html') {
                window.location.href = 'patient-dashboard.html';
            }
            break;
        case 4:
            if (currentPage !== 'pharmacist-dashboard.html') {
                window.location.href = 'pharmacist-dashboard.html';
            }
            break;
        case 5:
            if (currentPage !== 'staff-dashboard.html') {
                window.location.href = 'staff-dashboard.html';
            }
            break;
        default:
            if (!authPages.includes(currentPage)) {
                window.location.href = 'login.html';
            }
            break;
    }
}
  

