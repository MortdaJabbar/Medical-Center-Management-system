(function () {
    const token = localStorage.getItem("token") || sessionStorage.getItem("token");
    const role = localStorage.getItem("role") || sessionStorage.getItem("role");

    // تحقق من وجود التوكن
    if (!token ) {
        
        window.location.href = "login.html";
        return;
    }
if (isTokenExpired(token)) {
     
        localStorage.removeItem("token");
        localStorage.removeItem("role");
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("role");

       
        window.location.href = "login.html";
        return;
    }

    // إعداد الهيدر لكل طلب AJAX
     

    // التعامل مع أخطاء AJAX
    $(document).ajaxError(function (event, jqxhr) {
        if (jqxhr.status === 401 || jqxhr.status === 403) {
            localStorage.clear();
            sessionStorage.clear();
            window.location.href = "login.html";
        } else if (jqxhr.status === 404) {
            window.location.href = "404.html";
        }
        else if (jqxhr.status === 500) {
            localStorage.clear();
            sessionStorage.clear();
            window.location.href = "500.html";
        }
    });

    // تحقق من صلاحية الدور فقط إذا الصفحة تطلب ذلك
    const meta = document.querySelector('meta[name="allowed-roles"]');
    if (meta) {
        const allowedRoles = meta.content.split(',').map(r => r.trim());
        if (!allowedRoles.includes(role)) {
          
        
            localStorage.clear();
            sessionStorage.clear();
             window.location.href = "403.html";
            return;
        }
    }
})();


 function isTokenExpired(token) {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const exp = payload.exp;
            const now = Math.floor(Date.now() / 1000); // الآن بالثواني

            return exp < now;
        } catch (e) {
            console.error("Failed to parse token", e);
            return true; // احتياطاً اعتبره منتهي إذا صار خطأ
        }
    }