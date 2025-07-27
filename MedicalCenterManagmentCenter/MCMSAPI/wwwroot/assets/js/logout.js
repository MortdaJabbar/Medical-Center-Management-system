
  document.getElementById('logoutBtn').addEventListener('click', function (e) {
    e.preventDefault();

    // إزالة البيانات من التخزين
    localStorage.clear();
    sessionStorage.clear();

    // توجيه لصفحة تسجيل الدخول
    window.location.href = 'login.html';
  });

