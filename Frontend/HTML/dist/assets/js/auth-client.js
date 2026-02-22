// Centralized auth client to handle cookie-based auth, refresh rotation and retries
(function (global) {
    const API_BASE = 'https://localhost:7119';

    async function refresh() {
        try {
            const res = await fetch(`${API_BASE}/api/Auth/refresh`, {
                method: 'POST',
                credentials: 'include'
            });
            return res.ok;
        } catch (e) {
            return false;
        }
    }

    async function request(path, opts = {}, retry = true) {
        const url = path.startsWith('http') ? path : API_BASE + path;
        const fetchOpts = Object.assign({
            credentials: 'include',
            headers: { 'Accept': 'application/json' }
        }, opts);

        try {
            const res = await fetch(url, fetchOpts);

            if (res.status === 401 && retry) {
                const ok = await refresh();
                if (ok) {
                    // retry once
                    return request(path, opts, false);
                }
                // failed to refresh
                throw new Error('Unauthorized');
            }

            const text = await res.text();
            try { return JSON.parse(text); } catch { return text; }
        } catch (err) {
            throw err;
        }
    }

    async function getCurrentUser() {
        return request('/api/Auth/me', { method: 'GET' });
    }

    global.AuthClient = {
        apiBase: API_BASE,
        request,
        refresh,
        getCurrentUser
    };
})(window);
